using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_StunState : StunState
{
	private Boss2 boss;

	public B2_StunState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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

		if (isStunTimeOver)
		{
			if (isPlayerInMinAgroRange)
			{
				stateMachine.ChangeState(boss.playerDetectedState);
			}
			else
			{
				stateMachine.ChangeState(boss.lookForPlayerState);
			}
		}
	}

	public override void PhysicsUpdate()
	{
		base.PhysicsUpdate();
	}
}
