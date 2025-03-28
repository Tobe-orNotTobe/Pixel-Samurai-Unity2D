using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_DodgeState : DodgeState
{
	private Boss2 boss;

	public B2_DodgeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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
			if (isPlayerInMaxAgroRange && performCloseRangeAction)
			{
				stateMachine.ChangeState(boss.meleeAttackState);
			}
			else if (isPlayerInMaxAgroRange && !performCloseRangeAction)
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
