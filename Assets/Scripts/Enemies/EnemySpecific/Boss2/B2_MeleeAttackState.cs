using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_MeleeAttackState : MeleeAttackState
{
	private Boss2 boss;

	public B2_MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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

	public override void FinishAttack()
	{
		base.FinishAttack();
	}

	public override void LogicUpdate()
	{
		base.LogicUpdate();

		if (isAnimationFinished)
		{
			if (isPlayerInMinAgroRange)
			{
				stateMachine.ChangeState(boss.playerDetectedState);
			}
			else
			{
				stateMachine.ChangeState(boss.rangedAttackState);
			}
		}
	}

	public override void PhysicsUpdate()
	{
		base.PhysicsUpdate();
	}

	public override void TriggerAttack()
	{
		base.TriggerAttack();
	}
}
