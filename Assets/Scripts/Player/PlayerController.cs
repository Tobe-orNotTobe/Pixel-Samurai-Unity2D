﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private float movementInputDirection;
	private float jumpTimer;
	private float turnTimer;
	private float wallJumpTimer;
	private float dashTimeLeft;
	private float lastImageXpos;
	private float lastDash = -100f;
	private float knockbackStartTime;
	[SerializeField]
	private float knockbackDuration;

	private int amountOfJumpsLeft;
	private int facingDirection = 1;
	private int lastWallJumpDirection;

	private bool isFacingRight = true;
	private bool isWalking;
	private bool isGrounded;
	private bool isTouchingWall;
	private bool isWallSliding;
	private bool canNormalJump;
	private bool canWallJump;
	private bool isAttemptingToJump;
	private bool checkJumpMultiplier;
	private bool canMove;
	private bool canFlip;
	private bool hasWallJumped;
	private bool isTouchingLedge;
	private bool canClimbLedge = false;
	private bool ledgeDetected;
	private bool isDashing;
	private bool knockback;
	private bool isDead = false;
	private bool isHealing = false; // New variable for healing state

	[SerializeField]
	private Vector2 knockbackSpeed;

	private Vector2 ledgePosBot;
	private Vector2 ledgePos1;
	private Vector2 ledgePos2;

	private Rigidbody2D rb;
	private Animator anim;
	private PlayerStats playerStats;

	public int amountOfJumps = 1;

	public float movementSpeed = 10.0f;
	public float jumpForce = 16.0f;
	public float groundCheckRadius;
	public float wallCheckDistance;
	public float wallSlideSpeed;
	public float movementForceInAir;
	public float airDragMultiplier = 0.95f;
	public float variableJumpHeightMultiplier = 0.5f;
	public float wallHopForce;
	public float wallJumpForce;
	public float jumpTimerSet = 0.15f;
	public float turnTimerSet = 0.1f;
	public float wallJumpTimerSet = 0.5f;
	public float ledgeClimbXOffset1 = 0f;
	public float ledgeClimbYOffset1 = 0f;
	public float ledgeClimbXOffset2 = 0f;
	public float ledgeClimbYOffset2 = 0f;
	public float dashTime;
	public float dashSpeed;
	public float distanceBetweenImages;
	public float dashCoolDown;

	public Vector2 wallHopDirection;
	public Vector2 wallJumpDirection;

	public Transform groundCheck;
	public Transform wallCheck;
	public Transform ledgeCheck;

	public LayerMask whatIsGround;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		playerStats = GetComponent<PlayerStats>();
		amountOfJumpsLeft = amountOfJumps;
		wallHopDirection.Normalize();
		wallJumpDirection.Normalize();
	}

	void Update()
	{
		if (!isDead && !isHealing)
		{
			CheckInput();
		}

		CheckMovementDirection();
		UpdateAnimations();
		CheckIfCanJump();
		CheckIfWallSliding();

		if (!isDead && !isHealing)
		{
			CheckJump();
			CheckLedgeClimb();
			CheckDash();
		}

		CheckKnockback();
	}

	private void FixedUpdate()
	{
		if (!isHealing)
		{
			ApplyMovement();
		}
		else
		{
			rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		}
		CheckSurroundings();
	}

	public void SetIsDead(bool value)
	{
		isDead = value;

		if (isDead)
		{
			isWallSliding = false;
			isDashing = false;
			isAttemptingToJump = false;
			checkJumpMultiplier = false;
		}
	}

	public void SetIsHealing(bool value)
	{
		isHealing = value;

		if (isHealing)
		{
			// Stop all movement and actions when healing starts
			rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
			isDashing = false;
			isAttemptingToJump = false;
		}
	}

	public bool IsHealing()
	{
		return isHealing;
	}

	private void CheckIfWallSliding()
	{
		if (isTouchingWall && movementInputDirection == facingDirection && rb.linearVelocity.y < 0 && !canClimbLedge && !isHealing)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}
	}

	public bool GetDashStatus()
	{
		return isDashing;
	}

	public void Knockback(int direction)
	{
		knockback = true;
		knockbackStartTime = Time.time;
		rb.linearVelocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
	}

	private void CheckKnockback()
	{
		if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
		{
			knockback = false;
			rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
		}
	}

	private void CheckLedgeClimb()
	{
		if (ledgeDetected && !canClimbLedge && !isHealing)
		{
			canClimbLedge = true;

			if (isFacingRight)
			{
				ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}
			else
			{
				ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}

			canMove = false;
			canFlip = false;

			anim.SetBool("canClimbLedge", canClimbLedge);
		}

		if (canClimbLedge)
		{
			transform.position = ledgePos1;
		}
	}

	public void FinishLedgeClimb()
	{
		canClimbLedge = false;
		transform.position = ledgePos2;
		canMove = true;
		canFlip = true;
		ledgeDetected = false;
		anim.SetBool("canClimbLedge", canClimbLedge);
	}

	private void CheckSurroundings()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

		isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
		isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

		if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
		{
			ledgeDetected = true;
			ledgePosBot = wallCheck.position;
		}
	}

	private void CheckIfCanJump()
	{
		if (isGrounded && rb.linearVelocity.y <= 0.01f)
		{
			amountOfJumpsLeft = amountOfJumps;
		}

		if (isTouchingWall)
		{
			checkJumpMultiplier = false;
			canWallJump = true;
		}

		if (amountOfJumpsLeft <= 0)
		{
			canNormalJump = false;
		}
		else
		{
			canNormalJump = true;
		}
	}

	private void CheckMovementDirection()
	{
		if (isFacingRight && movementInputDirection < 0)
		{
			Flip();
		}
		else if (!isFacingRight && movementInputDirection > 0)
		{
			Flip();
		}

		if (Mathf.Abs(rb.linearVelocity.x) >= 0.01f)
		{
			isWalking = true;
		}
		else
		{
			isWalking = false;
		}
	}

	private void UpdateAnimations()
	{
		anim.SetBool("isWalking", isWalking);
		anim.SetBool("isGrounded", isGrounded);
		anim.SetFloat("yVelocity", rb.linearVelocity.y);
		anim.SetBool("isWallSliding", isWallSliding);
	}

	private void CheckInput()
	{
		movementInputDirection = Input.GetAxisRaw("Horizontal");

		if (Input.GetButtonDown("Jump"))
		{
			if (isGrounded || (amountOfJumpsLeft > 0 && !isTouchingWall))
			{
				NormalJump();
			}
			else
			{
				jumpTimer = jumpTimerSet;
				isAttemptingToJump = true;
			}
		}

		if (Input.GetButtonDown("Horizontal") && isTouchingWall)
		{
			if (!isGrounded && movementInputDirection != facingDirection)
			{
				canMove = false;
				canFlip = false;

				turnTimer = turnTimerSet;
			}
		}

		if (turnTimer >= 0)
		{
			turnTimer -= Time.deltaTime;

			if (turnTimer <= 0)
			{
				canMove = true;
				canFlip = true;
			}
		}

		if (checkJumpMultiplier && !Input.GetButton("Jump"))
		{
			checkJumpMultiplier = false;
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpHeightMultiplier);
		}

		if (Input.GetButtonDown("Dash"))
		{
			if (Time.time >= (lastDash + dashCoolDown))
				AttemptToDash();
		}
	}

	private void AttemptToDash()
	{
		isDashing = true;
		dashTimeLeft = dashTime;
		lastDash = Time.time;

		PlayerAfterImagePool.Instance.GetFromPool();
		lastImageXpos = transform.position.x;
	}

	public int GetFacingDirection()
	{
		return facingDirection;
	}

	private void CheckDash()
	{
		if (isDashing)
		{
			if (dashTimeLeft > 0)
			{
				canMove = false;
				canFlip = false;
				rb.linearVelocity = new Vector2(dashSpeed * facingDirection, 0.0f);
				dashTimeLeft -= Time.deltaTime;

				if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
				{
					PlayerAfterImagePool.Instance.GetFromPool();
					lastImageXpos = transform.position.x;
				}
			}

			if (dashTimeLeft <= 0 || isTouchingWall)
			{
				isDashing = false;
				canMove = true;
				canFlip = true;
			}
		}
	}

	private void CheckJump()
	{
		if (jumpTimer > 0)
		{
			//WallJump
			if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
			{
				WallJump();
			}
			else if (isGrounded)
			{
				NormalJump();
			}
		}

		if (isAttemptingToJump)
		{
			jumpTimer -= Time.deltaTime;
		}

		if (wallJumpTimer > 0)
		{
			if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
			{
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.0f);
				hasWallJumped = false;
			}
			else if (wallJumpTimer <= 0)
			{
				hasWallJumped = false;
			}
			else
			{
				wallJumpTimer -= Time.deltaTime;
			}
		}
	}

	private void NormalJump()
	{
		if (canNormalJump)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
			amountOfJumpsLeft--;
			jumpTimer = 0;
			isAttemptingToJump = false;
			checkJumpMultiplier = true;
		}
	}

	private void WallJump()
	{
		if (canWallJump)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.0f);
			isWallSliding = false;
			amountOfJumpsLeft = amountOfJumps;
			amountOfJumpsLeft--;
			Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
			rb.AddForce(forceToAdd, ForceMode2D.Impulse);
			jumpTimer = 0;
			isAttemptingToJump = false;
			checkJumpMultiplier = true;
			turnTimer = 0;
			canMove = true;
			canFlip = true;
			hasWallJumped = true;
			wallJumpTimer = wallJumpTimerSet;
			lastWallJumpDirection = -facingDirection;
		}
	}

	private void ApplyMovement()
	{
		if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x * airDragMultiplier, rb.linearVelocity.y);
		}
		else if (canMove && !knockback && !isHealing)
		{
			rb.linearVelocity = new Vector2(movementSpeed * movementInputDirection, rb.linearVelocity.y);
		}

		if (isWallSliding)
		{
			if (rb.linearVelocity.y < -wallSlideSpeed)
			{
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
			}
		}
	}

	public void DisableFlip()
	{
		canFlip = false;
	}

	public void EnableFlip()
	{
		canFlip = true;
	}

	private void Flip()
	{
		if (!isWallSliding && canFlip && !knockback && !isHealing)
		{
			facingDirection *= -1;
			isFacingRight = !isFacingRight;
			transform.Rotate(0.0f, 180.0f, 0.0f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
		Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgePos1.x, ledgePos1.y));
		Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgePos2.x, ledgePos2.y));
	}
}