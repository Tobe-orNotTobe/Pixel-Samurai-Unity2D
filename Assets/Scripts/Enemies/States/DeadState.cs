using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState stateData;
	protected bool isAnimationFinished;

	public DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        GameObject.Instantiate(stateData.deathBloodParticle, entity.aliveGO.transform.position, stateData.deathBloodParticle.transform.rotation);
        GameObject.Instantiate(stateData.deathChunkParticle, entity.aliveGO.transform.position, stateData.deathChunkParticle.transform.rotation);

		isAnimationFinished = false;

		entity.SetVelocity(0f);
	}

    public override void Exit()
    {
        base.Exit();

		entity.gameObject.SetActive(false);

	}

	public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
	}

	public virtual void FinishDeath()
	{
		isAnimationFinished = true;

		Object.Destroy(entity.gameObject, 2f);
	}
}
