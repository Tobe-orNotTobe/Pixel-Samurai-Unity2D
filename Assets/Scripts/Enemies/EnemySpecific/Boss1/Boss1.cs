using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Boss1 : Entity
{
	public B1_MoveState moveState { get; private set; }
	public B1_IdleState idleState { get; private set; }
	public B1_PlayerDetectedState playerDetectedState { get; private set; }
	public B1_LookForPlayerState lookForPlayerState { get; private set; }
	public B1_StunState stunState { get; private set; }
	public B1_DeadState deadState { get; private set; }
	public B1_DodgeState dodgeState { get; private set; }
	public B1_MeleeAttackState meleeAttackState { get; private set; }
	public B1_RangedAttackState rangedAttackState { get; private set; }

	[SerializeField]
	private D_MoveState moveStateData;
	[SerializeField]
	private D_IdleState idleStateData;
	[SerializeField]
	private D_PlayerDetected playerDetectedStateData;
	[SerializeField]
	private D_LookForPlayer lookForPlayerStateData;
	[SerializeField]
	private D_StunState stunStateData;
	[SerializeField]
	private D_DeadState deadStateData;
	[SerializeField]
	public D_DodgeState dodgeStateData;
	[SerializeField]
	private D_MeleeAttack meleeAttackStateData;
	[SerializeField]
	private D_RangedAttackState rangedAttackStateData;

	[SerializeField]
	private Transform meleeAttackPosition;
	[SerializeField]
	private Transform rangedAttackPosition;

	public bool IsDead
	{
		get { return base.isDead; } // Sử dụng trường isDead từ lớp cha Entity
	}

	public override void Start()
	{
		base.Start();

		moveState = new B1_MoveState(this, stateMachine, "move", moveStateData, this);
		idleState = new B1_IdleState(this, stateMachine, "idle", idleStateData, this);
		playerDetectedState = new B1_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
		lookForPlayerState = new B1_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
		stunState = new B1_StunState(this, stateMachine, "stun", stunStateData, this);
		deadState = new B1_DeadState(this, stateMachine, "dead", deadStateData, this);
		dodgeState = new B1_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
		meleeAttackState = new B1_MeleeAttackState(this, stateMachine, "meleeAttack", rangedAttackPosition, meleeAttackStateData, this);	  
		rangedAttackState = new B1_RangedAttackState(this, stateMachine, "rangedAttack", rangedAttackPosition, rangedAttackStateData, this);

		stateMachine.Initialize(idleState);
	}

	public override void Damage(AttackDetails attackDetails)
	{
		base.Damage(attackDetails);

		if (base.isDead)
		{
			stateMachine.ChangeState(deadState);
		}
		else if (isStunned && stateMachine.currentState != stunState)
		{
			stateMachine.ChangeState(stunState);
		}
		else if (CheckPlayerInMinAgroRange())
		{
			stateMachine.ChangeState(meleeAttackState);
		}
		else if (!CheckPlayerInMinAgroRange())
		{
			lookForPlayerState.SetTurnImmediately(true);
			stateMachine.ChangeState(lookForPlayerState);
		}
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();

		Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);

	}
}
