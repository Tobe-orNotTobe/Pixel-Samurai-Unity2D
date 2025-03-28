using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_MoveState : MoveState
{
	private Boss2 boss;

	public B2_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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
		else if (isDetectingWall || !isDetectingLedge)
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
