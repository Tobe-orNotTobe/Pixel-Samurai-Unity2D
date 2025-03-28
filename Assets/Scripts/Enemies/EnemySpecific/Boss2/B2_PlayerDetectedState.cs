using UnityEngine;

public class B2_PlayerDetectedState : PlayerDetectedState
{
	private Boss2 boss;

	public B2_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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
			if (Time.time >= boss.dodgeState.startTime + boss.dodgeStateData.dodgeCooldown)
			{
				stateMachine.ChangeState(boss.dodgeState);
			}
			else
			{
				stateMachine.ChangeState(boss.meleeAttackState);
			}
		}
		else if (performLongRangeAction)
		{
			stateMachine.ChangeState(boss.moveState);
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
