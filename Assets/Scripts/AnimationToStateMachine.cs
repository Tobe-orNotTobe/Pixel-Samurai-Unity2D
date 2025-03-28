using System.Xml;
using UnityEngine;

public class AnimationToStateMachine : MonoBehaviour
{
	public AttackState attackState;
	public DeadState deadState;

	private void TriggerAttack()
	{
		attackState.TriggerAttack();
	}

	private void FinishAttack()
	{
		attackState.FinishAttack();
	}
	private void FinishDeath()
	{
		if (deadState != null)
			deadState.FinishDeath();
	}
}
