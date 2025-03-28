using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	[SerializeField]
	private float maxHealth;

	[SerializeField]
	private GameObject
		deathChunkParticle,
		deathBloodParticle;

	[SerializeField]
	private float deathAnimationDuration = 2.0f;

	[Header("Healing Settings")]
	[SerializeField] private int maxHealingCharges = 3;
	[SerializeField] private float healAmount = 30f;
	[SerializeField] private float healingDuration = 1.5f;

	private int currentHealingCharges;
	private bool isHealing = false;

	public bool IsHealing => isHealing;
	public int CurrentHealingCharges => currentHealingCharges;
	public int MaxHealingCharges => maxHealingCharges;

	public event Action<float, float> OnHealthChanged;
	public event Action<int, int> OnHealingChargesChanged;

	private float currentHealth;
	private GameManager GM;
	private Animator anim;
	private PlayerController playerController;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();
	}

	private void Start()
	{
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();

		// Initialize health based on the data manager
		if (PlayerDataManager.Instance != null)
		{
			// First time starting or after a game over
			if (PlayerDataManager.Instance.ShouldResetHealth)
			{
				currentHealth = maxHealth;
				currentHealingCharges = maxHealingCharges;
				PlayerDataManager.Instance.MaxHealth = maxHealth;
				PlayerDataManager.Instance.CurrentHealth = currentHealth;
				PlayerDataManager.Instance.ShouldResetHealth = false;
			}
			else
			{
				// Use persisted health from previous scene
				maxHealth = PlayerDataManager.Instance.MaxHealth;
				currentHealth = PlayerDataManager.Instance.CurrentHealth;
				currentHealingCharges = PlayerDataManager.Instance.CurrentHealingCharges;
			}
		}
		else
		{
			// Fallback if data manager not found
			currentHealth = maxHealth;
			currentHealingCharges = maxHealingCharges;
		}

		OnHealthChanged?.Invoke(currentHealth, maxHealth);
		OnHealingChargesChanged?.Invoke(currentHealingCharges, maxHealingCharges);
	}

	private void Update()
	{
		// Healing input check
		if (Input.GetKeyDown(KeyCode.Q) && !isHealing && currentHealingCharges > 0 && currentHealth < maxHealth)
		{
			StartHealing();
		}
	}

	private void StartHealing()
	{
		if (isHealing || currentHealingCharges <= 0 || currentHealth >= maxHealth)
			return;

		isHealing = true;
		// Trigger healing animation
		anim.SetBool("isHealing", true);

		StartCoroutine(HealingProcess());
	}

	private IEnumerator HealingProcess()
	{
		// Notify player controller about healing state
		if (playerController != null)
		{
			playerController.SetIsHealing(true);
		}

		// Wait for the healing duration
		yield return new WaitForSeconds(healingDuration);

		// Apply the healing
		currentHealingCharges--;
		IncreaseHealth(healAmount);

		// Update data manager
		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.CurrentHealingCharges = currentHealingCharges;
		}

		// Notify listeners about healing charges change
		OnHealingChargesChanged?.Invoke(currentHealingCharges, maxHealingCharges);

		// End healing state
		isHealing = false;
		anim.SetBool("isHealing", false);

		// Notify player controller
		if (playerController != null)
		{
			playerController.SetIsHealing(false);
		}
	}

	public void DecreaseHealth(float amount)
	{
		currentHealth -= amount;
		currentHealth = Mathf.Max(0, currentHealth);

		// Update data manager
		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.CurrentHealth = currentHealth;
		}

		OnHealthChanged?.Invoke(currentHealth, maxHealth);

		if (currentHealth <= 0.0f)
		{
			Die();
		}
	}

	public void IncreaseHealth(float amount)
	{
		currentHealth += amount;
		currentHealth = Mathf.Min(currentHealth, maxHealth);

		// Update data manager
		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.CurrentHealth = currentHealth;
		}

		OnHealthChanged?.Invoke(currentHealth, maxHealth);
	}

	public void RestoreHealingCharge()
	{
		if (currentHealingCharges < maxHealingCharges)
		{
			currentHealingCharges++;

			// Update data manager
			if (PlayerDataManager.Instance != null)
			{
				PlayerDataManager.Instance.CurrentHealingCharges = currentHealingCharges;
			}

			OnHealingChargesChanged?.Invoke(currentHealingCharges, maxHealingCharges);
		}
	}

	public float GetCurrentHealth() => currentHealth;
	public float GetMaxHealth() => maxHealth;
	public float GetHealthPercentage() => currentHealth / maxHealth;

	private void Die()
	{
		anim.SetBool("dead", true);

		Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
		Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
		DisablePlayerControls();
		StartCoroutine(DelayedGameOver());
	}

	private void DisablePlayerControls()
	{
		PlayerController playerController = GetComponent<PlayerController>();
		if (playerController != null)
		{
			playerController.SetIsDead(true);
		}

		PlayerCombatController combatController = GetComponent<PlayerCombatController>();
		if (combatController != null)
		{
			combatController.enabled = false;
		}

		if (anim != null)
		{
			anim.SetBool("isWalking", false);
			anim.SetBool("isGrounded", true);
			anim.SetBool("isWallSliding", false);
			anim.SetBool("isAttacking", false);
			anim.SetBool("canClimbLedge", false);
			anim.SetFloat("yVelocity", 0f);
		}

		gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
	}

	private IEnumerator DelayedGameOver()
	{
		yield return new WaitForSeconds(deathAnimationDuration);

		if (PlayerDataManager.Instance != null)
		{
			// Reset health on game over
			PlayerDataManager.Instance.ShouldResetHealth = true;
		}

		GM.PlayerDied();

		yield return new WaitForSeconds(1.0f);
	}
}