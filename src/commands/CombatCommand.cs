using System.Collections.Generic;
using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public partial class CombatCommand : Command
{
    private struct AttackData
    {
        public EcsEntity AttackerEntity { get; set; }
        public EcsEntity DefenderEntity { get; set; }
        public TerrainTypes TerrainTypes { get; set; }
        public DamageEvent DamageEvent { get; set; }
        public int DefenderLevel { get; set; }
        public PackedScene Projectile { get; set; }
        public bool IsRanged { get; set; }

        public AttackData(
            EcsEntity attackerEntity,
            EcsEntity defenderEntity,
            TerrainTypes terrainTypes,
            DamageEvent damageEvent,
            PackedScene projectile,
            bool isRanged)
        {
            AttackerEntity = attackerEntity;
            DefenderEntity = defenderEntity;
            TerrainTypes = terrainTypes;
            DamageEvent = damageEvent;
            DefenderLevel = DefenderEntity.Get<Level>().Value;
            Projectile = projectile;
            IsRanged = isRanged;
        }
    }

    private EcsEntity _attackerLocEntity;
    private EcsEntity _defenderLocEntity;
    private EcsEntity _attackerAttackEntity;
    private EcsEntity _defenderAttackEntity;

    private int _attackDistance;
    private ulong _seed;

    private EcsEntity _attackerEntity;
    private EcsEntity _defenderEntity;

    private Queue<AttackData> _attackDataQueue = new Queue<AttackData>();
    private AttackData _attackData;

    private Tween _tween;

    public CombatCommand(ulong seed, EcsEntity attackerLocEntity, EcsEntity attackerAttackEntity, EcsEntity defenderLocEntity, EcsEntity defenderAttackEntity, int attackDistance)
    {
        _seed = seed;
        _attackerLocEntity = attackerLocEntity;
        _attackerAttackEntity = attackerAttackEntity;
        _defenderLocEntity = defenderLocEntity;
        _defenderAttackEntity = defenderAttackEntity;
        _attackDistance = attackDistance;
        _attackerEntity = _attackerLocEntity.Get<HasUnit>().Entity;
        _defenderEntity = _defenderLocEntity.Get<HasUnit>().Entity;
    }

    public override void Execute()
    {
        GD.Seed(_seed);

        var attackerStrikes = _attackerAttackEntity.Get<Strikes>().Value;
        var attackerRange = _attackerAttackEntity.Get<Range>().Value;

        var defenderStrikes = 0;
        if (_defenderAttackEntity.IsAlive())
        {
            defenderStrikes = _defenderAttackEntity.Get<Strikes>().Value;
        }

        for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
        {
            if (i < attackerStrikes)
            {
                var damageEvent = new DamageEvent(_attackerAttackEntity, _defenderEntity, _attackerEntity.Get<Aligned>().Value);
                _attackDataQueue.Enqueue(new AttackData(
                    _attackerEntity,
                    _defenderEntity,
                    TerrainTypes.FromLocEntity(_defenderLocEntity),
                    damageEvent,
                    _attackerAttackEntity.Get<AssetHandle<PackedScene>>().Asset,
                    attackerRange > 1
                ));
            }

            if (i < defenderStrikes)
            {
                var damageEvent = new DamageEvent(_defenderAttackEntity, _attackerEntity, _defenderEntity.Get<Aligned>().Value);
                _attackDataQueue.Enqueue(new AttackData(
                    _defenderEntity,
                    _attackerEntity,
                    TerrainTypes.FromLocEntity(_attackerLocEntity),
                    damageEvent,
                    _attackerAttackEntity.Get<AssetHandle<PackedScene>>().Asset,
                    attackerRange > 1
                ));
            }
        }

        ref var moves = ref _attackerEntity.Get<Attribute<Moves>>();
        ref var actions = ref _attackerEntity.Get<Attribute<Actions>>();

        moves.Empty();
        actions.Decrease(1);

        if (_attackDataQueue.Count > 0)
        {
            _attackData = _attackDataQueue.Dequeue();
            Attack();
        }
    }

    private void SpawnFloatingLabelEvent(Coords coords, string text, Color color)
    {
        var position = coords.World() + Vector3.Up * 5f;
        var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

        Main.Instance.World.Spawn().Add(spawnLabelEvent);
    }

    private void Attack()
    {
        _tween = Main.Instance.CreateTween();

        var attackerView = _attackData.AttackerEntity.Get<NodeHandle<UnitView>>().Node;
        var defenderView = _attackData.DefenderEntity.Get<NodeHandle<UnitView>>().Node;

        attackerView.LookAt(defenderView.Position, Vector3.Up);
        defenderView.LookAt(attackerView.Position, Vector3.Up);

        attackerView.Rotation = new Vector3(0f, attackerView.Rotation.y, 0f);
        defenderView.Rotation = new Vector3(0f, defenderView.Rotation.y, 0f);

        Vector3 attackPos = attackerView.Position;

        if (_attackData.IsRanged)
        {
            attackerView.Play("Attack");
            _tween.TweenCallback(new Callable(this, nameof(SpawnProjectile))).SetDelay(0.5f);
            _tween.TweenProperty(attackerView, "position", attackPos, 0.5f);
        }
        else
        {
            attackPos = (attackerView.Position + defenderView.Position) / 2;
            _tween.TweenProperty(attackerView, "position", attackPos, 0.2f);
            _tween.TweenCallback(new Callable(this, nameof(OnStrike)));
        }

        _tween.TweenProperty(attackerView, "position", attackerView.Position, 0.2f);
        _tween.TweenCallback(new Callable(this, nameof(OnStrikeFinished)));

        _tween.Play();
    }

    private void SpawnProjectile()
    {
        var attackerView = _attackData.AttackerEntity.Get<NodeHandle<UnitView>>().Node;
        var defenderView = _attackData.DefenderEntity.Get<NodeHandle<UnitView>>().Node;

        var tween = Main.Instance.CreateTween();

        var projectile = _attackData.Projectile.Instantiate<Projectile>();
        Main.Instance.AddChild(projectile);

        projectile.Position = attackerView.Position + Vector3.Up * 5f;
        projectile.LookAt(defenderView.Position + Vector3.Up * 5f);

        tween.TweenProperty(projectile, "position", defenderView.Position + Vector3.Up * 5f, 0.2f);

        tween.TweenCallback(new Callable(projectile, "queue_free"));
        tween.TweenCallback(new Callable(this, nameof(OnStrike)));
    }

    private void OnStrike()
    {
        var defense = _attackData.TerrainTypes.GetDefense();

        GD.Print($"OnStrike! Types: {_attackData.TerrainTypes.ToString()}, Defense: {defense}");

        if (defense < GD.Randf())
        {
            Main.Instance.World.Spawn().Add(_attackData.DamageEvent);
        }
        else
        {
            Main.Instance.World.Spawn().Add(new MissEvent(_attackData.DefenderEntity));
        }
    }

    private void OnStrikeFinished()
    {
        if (!_attackData.DefenderEntity.IsAlive())
        {
            Main.Instance.World.Spawn().Add(new GainExperienceEvent(_attackData.AttackerEntity, _attackData.DefenderLevel * 8));
            IsDone = true;
            return;
        }
        else if (_attackDataQueue.Count > 0)
        {
            _attackData = _attackDataQueue.Dequeue();
            Attack();
        }
        else
        {
            Main.Instance.World.Spawn().Add(new GainExperienceEvent(_attackData.AttackerEntity, _attackData.DefenderEntity.Get<Level>().Value));
            Main.Instance.World.Spawn().Add(new GainExperienceEvent(_attackData.DefenderEntity, _attackData.AttackerEntity.Get<Level>().Value));
            IsDone = true;
        }
    }

    public override void Revert()
    {
        throw new System.NotImplementedException();
    }
}