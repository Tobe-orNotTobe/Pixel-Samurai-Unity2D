using UnityEngine;

public class B2_IdleState : IdleState
{
	private Boss2 boss;

	public B2_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Boss2 boss) : base(etity, stateMachine, animBoolName, stateData)
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

		entity.Flip();
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
		else if (isIdleTimeOver)
		{
			stateMachine.ChangeState(boss.moveState);
		}
	}

	public override void PhysicsUpdate()
	{
		base.PhysicsUpdate();
	}
}
