using System.Collections.Generic;
using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public partial class CombatCommand : Command
{
    private struct AttackData
    {
        public EcsEntity AttackerEntity;
        public EcsEntity DefenderEntity;
        public TerrainTypes TerrainTypes;
        public DamageEvent DamageEvent;
        public bool IsRanged;

        public AttackData(
            EcsEntity attackerEntity, 
            EcsEntity defenderEntity, 
            TerrainTypes terrainTypes,
            DamageEvent damageEvent, 
            bool isRanged)
        {
            AttackerEntity = attackerEntity;
            DefenderEntity = defenderEntity;
            TerrainTypes = terrainTypes;
            DamageEvent = damageEvent;
            IsRanged = isRanged;
        }
    }

    public EcsEntity AttackerLocEntity;
    public EcsEntity DefenderLocEntity;
    public EcsEntity AttackerAttackEntity;
    public EcsEntity DefenderAttackEntity;

    private EcsEntity _attackerEntity;
    private EcsEntity _defenderEntity;

    private Queue<AttackData> _attackDataQueue = new Queue<AttackData>();
    private AttackData _attackData;

    private Tween _tween;

    public CombatCommand(EcsEntity attackerLocEntity, EcsEntity attackerAttackEntity, EcsEntity defenderLocEntity, EcsEntity defenderAttackEntity)
    {
        AttackerLocEntity = attackerLocEntity;
        AttackerAttackEntity = attackerAttackEntity;
        DefenderLocEntity = defenderLocEntity;
        DefenderAttackEntity = defenderAttackEntity;
        _attackerEntity = AttackerLocEntity.Get<HasUnit>().Entity;
        _defenderEntity = DefenderLocEntity.Get<HasUnit>().Entity;
    }

    public override void Execute()
    {
        SpawnFloatingLabelEvent(_attackerEntity.Get<Coords>(), AttackerAttackEntity.Get<Id>().Value, new Color(1f, 1f, 1f));

        var attackerStrikes = AttackerAttackEntity.Get<Strikes>().Value;
        var attackerRange = AttackerAttackEntity.Get<Range>().Value;

        var defenderStrikes = 0;
        if (DefenderAttackEntity.IsAlive())
        {
            defenderStrikes = DefenderAttackEntity.Get<Strikes>().Value;
            SpawnFloatingLabelEvent(_defenderEntity.Get<Coords>(), DefenderAttackEntity.Get<Id>().Value, new Color(1f, 1f, 1f));
        }

        for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
        {
            if (i < attackerStrikes)
            {
                var damageEvent = new DamageEvent(AttackerAttackEntity, _defenderEntity);
                _attackDataQueue.Enqueue(new AttackData(_attackerEntity, _defenderEntity, TerrainTypes.FromLocEntity(AttackerLocEntity), damageEvent, attackerRange > 1));
            }

            if (i < defenderStrikes)
            {
                var damageEvent = new DamageEvent(DefenderAttackEntity, _attackerEntity);
                _attackDataQueue.Enqueue(new AttackData(_defenderEntity, _attackerEntity, TerrainTypes.FromLocEntity(DefenderLocEntity), damageEvent, attackerRange > 1));
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

        Main.Instance.World.Spawn().Add(new UnitDeselectedEvent());
    }

    private void SpawnFloatingLabelEvent(Coords coords, string text, Color color)
    {
        var position = coords.World + Vector3.Up * 5f;
        var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

        Main.Instance.World.Spawn().Add(spawnLabelEvent);
    }

    private void Attack()
    {
        _tween = Main.Instance.CreateTween();

        var attackerView = _attackData.AttackerEntity.Get<NodeHandle<UnitView>>().Node;
        var defenderView = _attackData.DefenderEntity.Get<NodeHandle<UnitView>>().Node;

        Vector3 attackPos = attackerView.Position;

        if (!_attackData.IsRanged)
        {
            attackPos = (attackerView.Position + defenderView.Position) / 2;
        }

        _tween.TweenProperty(attackerView, "position", attackPos, 0.2f);
        _tween.TweenCallback(new Callable(this, "OnStrike"));
        _tween.TweenProperty(attackerView, "position", attackerView.Position, 0.2f);
        _tween.TweenCallback(new Callable(this, "OnStrikeFinished"));

        _tween.Play();
    }

    private void OnStrike()
    {
        if (_attackData.TerrainTypes.GetDefense() < GD.Randf())
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
            IsDone = true;
        }
    }

    public override void Revert()
    {
        throw new System.NotImplementedException();
    }
}