using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_DeadState : DeadState
{
	private Boss2 boss;

	public B2_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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

