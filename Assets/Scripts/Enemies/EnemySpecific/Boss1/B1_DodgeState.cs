using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_DodgeState : DodgeState
{
    private Boss1 boss;

    public B1_DodgeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData, Boss1 boss) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.boss = boss;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isDodgeOver)
        {
            if(isPlayerInMaxAgroRange && performCloseRangeAction)
            {
                stateMachine.ChangeState(boss.rangedAttackState);
            }
            else if(isPlayerInMaxAgroRange && !performCloseRangeAction)
            {
                stateMachine.ChangeState(boss.rangedAttackState);
            }
            else if (!isPlayerInMaxAgroRange)
            {
                stateMachine.ChangeState(boss.lookForPlayerState);
            }

            //TODO: ranged attack state
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
