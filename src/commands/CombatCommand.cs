using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public partial class CombatCommand : Command
{
    private struct AttackData
    {
        public EcsEntity AttackerEntity;
        public EcsEntity DefenderEntity;
        public DamageEvent DamageEvent;

        public AttackData(EcsEntity attackerEntity, EcsEntity defenderEntity, DamageEvent damageEvent)
        {
            AttackerEntity = attackerEntity;
            DefenderEntity = defenderEntity;
            DamageEvent = damageEvent;
        }
    }

    public EcsEntity AttackerEntity;
    public EcsEntity DefenderEntity;

    private Queue<AttackData> _attackDataQueue = new Queue<AttackData>();
    private AttackData _attackData;

    private Tween _tween;

    public CombatCommand(EcsEntity attackerEntity, EcsEntity defenderEntity)
    {
        AttackerEntity = attackerEntity;
        DefenderEntity = defenderEntity;
    }

    public override void Execute()
    {
        _tween = Main.Instance.CreateTween();

        var attackerAttackEntity = AttackerEntity.Get<Attacks>().GetFirst();
        var defenderAttackEntity = DefenderEntity.Get<Attacks>().GetFirst();

        var attackerStrikes = attackerAttackEntity.Get<Strikes>().Value;
        var defenderStrikes = defenderAttackEntity.Get<Strikes>().Value;

        for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
        {
            if (i < attackerStrikes)
            {
                var damageEvent = new DamageEvent(attackerAttackEntity, DefenderEntity);
                _attackDataQueue.Enqueue(new AttackData(AttackerEntity, DefenderEntity, damageEvent));
            }

            if (i < defenderStrikes)
            {
                var damageEvent = new DamageEvent(defenderAttackEntity, AttackerEntity);
                _attackDataQueue.Enqueue(new AttackData(DefenderEntity, AttackerEntity, damageEvent));
            }
        }
        
        ref var unitActions = ref AttackerEntity.Get<Attribute<Actions>>();
        unitActions.Decrease(attackerAttackEntity.Get<Costs>().Value);

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

        _tween.TweenProperty(attackerView, "position", (attackerView.Position + defenderView.Position) / 2, 0.25f);
        _tween.TweenCallback(new Callable(this, "OnStrike"));
        _tween.TweenProperty(attackerView, "position", attackerView.Position, 0.25f);
        _tween.TweenCallback(new Callable(this, "OnStrikeFinished"));

        _tween.Play();
    }

    private void OnStrike()
    {
        Main.Instance.World.Spawn().Add(_attackData.DamageEvent);
    }

    private void OnStrikeFinished()
    {
        if (_attackData.DefenderEntity.Get<Attribute<Health>>().IsEmpty())
        {
            Main.Instance.World.Spawn().Add(new DeathEvent(DefenderEntity));
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