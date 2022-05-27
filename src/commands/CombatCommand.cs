using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public partial class CombatCommand : Resource, ICommandSystem
{
    struct AttackData
    {
        public Entity AttackerEntity { get; set; }
        public Entity DefenderEntity { get; set; }
        public TerrainTypes TerrainTypes { get; set; }
        public DamageTrigger DamageTrigger { get; set; }
        public int DefenderLevel { get; set; }
        public PackedScene Projectile { get; set; }
        public bool IsRanged { get; set; }

        public AttackData(
            Entity attackerEntity,
            Entity defenderEntity,
            TerrainTypes terrainTypes,
            DamageTrigger damageTrigger,
            PackedScene projectile,
            bool isRanged)
        {
            AttackerEntity = attackerEntity;
            DefenderEntity = defenderEntity;
            TerrainTypes = terrainTypes;
            DamageTrigger = damageTrigger;
            DefenderLevel = DefenderEntity.Get<Level>().Value;
            Projectile = projectile;
            IsRanged = isRanged;
        }
    }

    Entity _attackerLocEntity;
    Entity _defenderLocEntity;
    Entity _attackerAttackEntity;
    Entity _defenderAttackEntity;

    int _attackDistance;
    ulong _seed;

    Entity _attackerEntity;
    Entity _defenderEntity;

    Queue<AttackData> _attackDataQueue = new Queue<AttackData>();
    AttackData _attackData;

    Tween _tween;

    Commands _commands;

    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }

    public CombatCommand(ulong seed, Entity attackerLocEntity, Entity attackerAttackEntity, Entity defenderLocEntity, Entity defenderAttackEntity, int attackDistance)
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

    public void Run(Commands commands)
    {
        this._commands = commands;

        GD.Seed(_seed);

        var attackerStrikes = _attackerAttackEntity.Get<Strikes>().Value;
        var attackerRange = _attackerAttackEntity.Get<Range>().Value;

        var defenderStrikes = 0;
        if (_defenderAttackEntity.IsAlive)
        {
            defenderStrikes = _defenderAttackEntity.Get<Strikes>().Value;
        }

        for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
        {
            if (i < attackerStrikes)
            {
                var damageEvent = new DamageTrigger(_attackerAttackEntity, _defenderEntity, _attackerEntity.Get<Aligned>().Value);
                _attackDataQueue.Enqueue(new AttackData(
                    _attackerEntity,
                    _defenderEntity,
                    TerrainTypes.FromLocEntity(_defenderLocEntity),
                    damageEvent,
                    _attackerAttackEntity.Get<PackedScene>(),
                    attackerRange > 1
                ));
            }

            if (i < defenderStrikes)
            {
                var damageEvent = new DamageTrigger(_defenderAttackEntity, _attackerEntity, _defenderEntity.Get<Aligned>().Value);
                _attackDataQueue.Enqueue(new AttackData(
                    _defenderEntity,
                    _attackerEntity,
                    TerrainTypes.FromLocEntity(_attackerLocEntity),
                    damageEvent,
                    _attackerAttackEntity.Get<PackedScene>(),
                    attackerRange > 1
                ));
            }
        }

        var moves = _attackerEntity.Get<Attribute<Moves>>();
        var actions = _attackerEntity.Get<Attribute<Actions>>();

        moves.Empty();
        actions.Decrease(1);

        if (_attackDataQueue.Count > 0)
        {
            _attackData = _attackDataQueue.Dequeue();
            Attack();
        }
    }

    void SpawnFloatingLabelEvent(Coords coords, string text, Color color)
    {
        var position = coords.World() + Vector3.Up * 5f;
        var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

        _commands.Send(spawnLabelEvent);
    }

    void Attack()
    {
        _tween = _commands.GetElement<SceneTree>().CreateTween();

        var attackerView = _attackData.AttackerEntity.Get<UnitView>();
        var defenderView = _attackData.DefenderEntity.Get<UnitView>();

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

    void SpawnProjectile()
    {
        var attackerView = _attackData.AttackerEntity.Get<UnitView>();
        var defenderView = _attackData.DefenderEntity.Get<UnitView>();

        var tween = _commands.GetElement<SceneTree>().CreateTween();

        var projectile = _attackData.Projectile.Instantiate<Projectile>();
        _commands.GetElement<SceneTree>().CurrentScene.AddChild(projectile);

        projectile.Position = attackerView.Position + Vector3.Up * 5f;
        projectile.LookAt(defenderView.Position + Vector3.Up * 5f);

        tween.TweenProperty(projectile, "position", defenderView.Position + Vector3.Up * 5f, 0.2f);

        tween.TweenCallback(new Callable(projectile, "queue_free"));
        tween.TweenCallback(new Callable(this, nameof(OnStrike)));
    }

    void OnStrike()
    {
        var defense = _attackData.TerrainTypes.GetDefense();

        GD.Print($"OnStrike! Types: {_attackData.TerrainTypes.ToString()}, Defense: {defense}");

        if (defense < GD.Randf())
        {
            _commands.Send(_attackData.DamageTrigger);
        }
        else
        {
            _commands.Send(new MissTrigger(_attackData.DefenderEntity));
        }
    }

    void OnStrikeFinished()
    {
        if (!_attackData.DefenderEntity.IsAlive)
        {
            _commands.Send(new GainExperienceEvent(_attackData.AttackerEntity, _attackData.DefenderLevel * 8));
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
            _commands.Send(new GainExperienceEvent(_attackData.AttackerEntity, _attackData.DefenderEntity.Get<Level>().Value));
            _commands.Send(new GainExperienceEvent(_attackData.DefenderEntity, _attackData.AttackerEntity.Get<Level>().Value));
            IsDone = true;
        }
    }

    public void Revert()
    {
        throw new System.NotImplementedException();
    }
}