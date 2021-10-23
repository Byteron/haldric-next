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
        public DamageEvent DamageEvent;
        public bool IsRanged;

        public AttackData(EcsEntity attackerEntity, EcsEntity defenderEntity, DamageEvent damageEvent, bool isRanged)
        {
            AttackerEntity = attackerEntity;
            DefenderEntity = defenderEntity;
            DamageEvent = damageEvent;
            IsRanged = isRanged;
        }
    }

    public EcsEntity AttackerEntity;
    public EcsEntity DefenderEntity;
    public EcsEntity AttackerAttackEntity;
    public EcsEntity DefenderAttackEntity;

    private Queue<AttackData> _attackDataQueue = new Queue<AttackData>();
    private AttackData _attackData;

    private Tween _tween;

    public CombatCommand(EcsEntity attackerEntity, EcsEntity attackerAttackEntity, EcsEntity defenderEntity, EcsEntity defenderAttackEntity)
    {
        AttackerEntity = attackerEntity;
        AttackerAttackEntity = attackerAttackEntity;
        DefenderEntity = defenderEntity;
        DefenderAttackEntity = defenderAttackEntity;
    }

    public override void Execute()
    {
        var position = AttackerEntity.Get<Coords>().World + Vector3.Up * 5f;
        var text = AttackerAttackEntity.Get<Id>().Value;
        var color = new Color(1f, 1f, 1f);
        var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

        Main.Instance.World.Spawn().Add(spawnLabelEvent);

        var attackerStrikes = AttackerAttackEntity.Get<Strikes>().Value;
        var attackerRange = AttackerAttackEntity.Get<Range>().Value;

        var defenderStrikes = 0;
        if (DefenderAttackEntity.IsAlive())
        {
            defenderStrikes = DefenderAttackEntity.Get<Strikes>().Value;

            position = DefenderEntity.Get<Coords>().World + Vector3.Up * 5f;
            text = DefenderAttackEntity.Get<Id>().Value;
            color = new Color(1f, 1f, 1f);
            spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

            Main.Instance.World.Spawn().Add(spawnLabelEvent);
        }

        for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
        {
            if (i < attackerStrikes)
            {
                var damageEvent = new DamageEvent(AttackerAttackEntity, DefenderEntity);
                _attackDataQueue.Enqueue(new AttackData(AttackerEntity, DefenderEntity, damageEvent, attackerRange > 1));
            }

            if (i < defenderStrikes)
            {
                var damageEvent = new DamageEvent(DefenderAttackEntity, AttackerEntity);
                _attackDataQueue.Enqueue(new AttackData(DefenderEntity, AttackerEntity, damageEvent, attackerRange > 1));
            }
        }
        
        ref var unitActions = ref AttackerEntity.Get<Attribute<Actions>>();
        unitActions.Decrease(AttackerAttackEntity.Get<Costs>().Value);

        if (_attackDataQueue.Count > 0)
        {
            _attackData = _attackDataQueue.Dequeue();
            Attack();
        }
    }

    public void Attack()
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
        Main.Instance.World.Spawn().Add(_attackData.DamageEvent);

        var position = _attackData.DefenderEntity.Get<Coords>().World + Vector3.Up * 5f;
        var text = _attackData.DamageEvent.DamagerEntity.Get<Damage>().Value.ToString();
        var color = new Color(1f, 0f, 0f);
        var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

        Main.Instance.World.Spawn().Add(spawnLabelEvent);
    }

    private void OnStrikeFinished()
    {
        if (_attackData.DefenderEntity.Get<Attribute<Health>>().IsEmpty())
        {
            Main.Instance.World.Spawn().Add(new DeathEvent(DefenderEntity));
            IsDone = true;
            return;
        }
        else if (_attackDataQueue.Count > 0)
        {
            _attackData = _attackDataQueue.Dequeue();
            Attack();
        }
    }

    public override void Revert()
    {
        throw new System.NotImplementedException();
    }
}