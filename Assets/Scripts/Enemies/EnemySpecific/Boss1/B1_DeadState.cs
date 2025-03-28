using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_DeadState : DeadState
{
    private Boss1 boss;

    public B1_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, Boss1 boss) : base(etity, stateMachine, animBoolName, stateData)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

	public override void FinishDeath()
	{
		base.FinishDeath();
	}
}
    
