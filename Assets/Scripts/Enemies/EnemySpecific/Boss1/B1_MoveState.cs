using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MoveState : MoveState
{
    private Boss1 boss;

    public B1_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Boss1 boss) : base(etity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(boss.playerDetectedState);
        }
        else if(isDetectingWall || !isDetectingLedge)
        {
            boss.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(boss.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
