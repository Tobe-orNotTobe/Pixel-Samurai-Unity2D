using UnityEngine;

public class Boss2 : Entity
{
	public B2_MoveState moveState { get; private set; }
	public B2_IdleState idleState { get; private set; }
	public B2_PlayerDetectedState playerDetectedState { get; private set; }
	public B2_LookForPlayerState lookForPlayerState { get; private set; }
	public B2_StunState stunState { get; private set; }
	public B2_DeadState deadState { get; private set; }
	public B2_DodgeState dodgeState { get; private set; }
	public B2_MeleeAttackState meleeAttackState { get; private set; }
	public B2_RangedAttackState rangedAttackState { get; private set; }

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

		moveState = new B2_MoveState(this, stateMachine, "move", moveStateData, this);
		idleState = new B2_IdleState(this, stateMachine, "idle", idleStateData, this);
		playerDetectedState = new B2_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
		lookForPlayerState = new B2_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
		stunState = new B2_StunState(this, stateMachine, "stun", stunStateData, this);
		deadState = new B2_DeadState(this, stateMachine, "dead", deadStateData, this);
		dodgeState = new B2_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
		meleeAttackState = new B2_MeleeAttackState(this, stateMachine, "meleeAttack", rangedAttackPosition, meleeAttackStateData, this);
		rangedAttackState = new B2_RangedAttackState(this, stateMachine, "rangedAttack", rangedAttackPosition, rangedAttackStateData, this);

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
