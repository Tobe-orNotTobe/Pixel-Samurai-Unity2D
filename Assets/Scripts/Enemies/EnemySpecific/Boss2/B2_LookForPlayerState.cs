using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_LookForPlayerState : LookForPlayerState
{
	private Boss2 boss;
	public B2_LookForPlayerState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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
		else if (isAllTurnsTimeDone)
		{
			stateMachine.ChangeState(boss.moveState);
		}
	}

	public override void PhysicsUpdate()
	{
		base.PhysicsUpdate();
	}
}
