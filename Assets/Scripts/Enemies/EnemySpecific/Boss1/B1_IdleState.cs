using UnityEngine;

public class B1_IdleState : IdleState
{
	private Boss1 boss;

	public B1_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Boss1 boss) : base(etity, stateMachine, animBoolName, stateData)
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
