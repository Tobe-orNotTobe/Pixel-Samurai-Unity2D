﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
	[SerializeField]
	private bool combatEnabled;
	[SerializeField]
	private float inputTimer, attack1Radius, attack1Damage;
	[SerializeField]
	private float stunDamageAmount = 1f;
	[SerializeField]
	private Transform attack1HitBoxPos;
	[SerializeField]
	private LayerMask whatIsDamageable;

	private bool gotInput, isAttacking, isFirstAttack;

	private float lastInputTime = Mathf.NegativeInfinity;

	private AttackDetails attackDetails;

	private Animator anim;

	private PlayerController PC;
	private PlayerStats PS;

	public AudioSource audioSource;  
	public AudioClip attackSound;
	private void Start()
	{
		anim = GetComponent<Animator>();
		anim.SetBool("canAttack", combatEnabled);
		PC = GetComponent<PlayerController>();
		PS = GetComponent<PlayerStats>();
	}

	private void Update()
	{
		if (!PC.IsHealing())
		{
			CheckCombatInput();
			CheckAttacks();
		}
	}

	private void CheckCombatInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (combatEnabled && !PC.IsHealing())
			{
				gotInput = true;
				lastInputTime = Time.time;
			}
		}
	}
	void PlayAttackSound()
	{
		if (audioSource != null && attackSound != null)
		{
			audioSource.PlayOneShot(attackSound);
		}
	}

	private void CheckAttacks()
	{
		if (gotInput)
		{
			if (!isAttacking && !PC.IsHealing())
			{
				gotInput = false;
				isAttacking = true;
				isFirstAttack = !isFirstAttack;
				anim.SetBool("attack1", true);
				anim.SetBool("firstAttack", isFirstAttack);
				anim.SetBool("isAttacking", isAttacking);
			}
		}

		if (Time.time >= lastInputTime + inputTimer)
		{
			gotInput = false;
		}
	}

	private void CheckAttackHitBox()
	{
		Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

		attackDetails.damageAmount = attack1Damage;
		attackDetails.position = transform.position;
		attackDetails.stunDamageAmount = stunDamageAmount;

		foreach (Collider2D collider in detectedObjects)
		{
			collider.transform.parent.SendMessage("Damage", attackDetails);
		}
	}

	private void FinishAttack1()
	{
		isAttacking = false;
		anim.SetBool("isAttacking", isAttacking);
		anim.SetBool("attack1", false);
	}

	private void Damage(AttackDetails attackDetails)
	{
		if (!PC.GetDashStatus())
		{
			int direction;

			PS.DecreaseHealth(attackDetails.damageAmount);

			if (attackDetails.position.x < transform.position.x)
			{
				direction = 1;
			}
			else
			{
				direction = -1;
			}

			PC.Knockback(direction);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
	}
}