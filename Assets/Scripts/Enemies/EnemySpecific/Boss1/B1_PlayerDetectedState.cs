using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class B1_PlayerDetectedState : PlayerDetectedState
{
    private Boss1 boss;

    public B1_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, Boss1 boss) : base(etity, stateMachine, animBoolName, stateData)
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

        if (performCloseRangeAction)
        {
            if(Time.time >= boss.dodgeState.startTime + boss.dodgeStateData.dodgeCooldown)
            {
                stateMachine.ChangeState(boss.dodgeState);
            }
            else
            {
                stateMachine.ChangeState(boss.rangedAttackState);
            }            
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(boss.rangedAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(boss.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
