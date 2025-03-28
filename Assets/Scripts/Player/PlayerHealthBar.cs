using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
	public static PlayerHealthBar Instance { get; private set; }

	[Header("Health Bar References")]
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Image fillImage;
	[SerializeField] private Gradient healthGradient;
	[SerializeField] private float updateSpeed = 5f;
	[SerializeField] private bool animateHealthChange = true;
	[SerializeField] private float damageFlashDuration = 0.2f;
	[SerializeField] private Color damageFlashColor = Color.red;

	private PlayerStats playerStats;
	private Color originalFillColor;
	private Coroutine flashCoroutine;
	private Coroutine smoothCoroutine;

	private void Start()
	{
		playerStats = FindObjectOfType<PlayerStats>();
		if (playerStats == null)
		{
			Debug.LogError("PlayerHealthBar: Could not find PlayerStats component!");
			return;
		}

		playerStats.OnHealthChanged += UpdateHealthBar;
		originalFillColor = fillImage.color;
		InitializeHealthBar();
	}

	private void OnDestroy()
	{
		if (playerStats != null)
			playerStats.OnHealthChanged -= UpdateHealthBar;
	}

	private void InitializeHealthBar()
	{
		if (healthSlider == null) return;

		healthSlider.maxValue = 1f; 
		healthSlider.value = playerStats.GetCurrentHealth() / playerStats.GetMaxHealth();
		UpdateFillColor();
	}

	public void UpdateHealthBar(float currentHealth, float maxHealth)
	{
		if (healthSlider == null) return;

		float targetValue = currentHealth / maxHealth; // Hiển thị phần trăm máu

		if (animateHealthChange)
		{
			if (smoothCoroutine != null)
				StopCoroutine(smoothCoroutine);
			smoothCoroutine = StartCoroutine(SmoothHealthChange(targetValue));
		}
		else
		{
			healthSlider.value = targetValue;
		}

		UpdateFillColor();
		FlashDamage();
	}

	private IEnumerator SmoothHealthChange(float targetValue)
	{
		while (Mathf.Abs(healthSlider.value - targetValue) > 0.01f)
		{
			healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, updateSpeed * Time.deltaTime);
			yield return null;
		}
		healthSlider.value = targetValue; 
	}

	private void UpdateFillColor()
	{
		if (fillImage != null && healthGradient != null)
			fillImage.color = healthGradient.Evaluate(healthSlider.value);
	}

	private void FlashDamage()
	{
		if (flashCoroutine != null) StopCoroutine(flashCoroutine);
		flashCoroutine = StartCoroutine(DamageFlashEffect());
	}

	private IEnumerator DamageFlashEffect()
	{
		fillImage.color = damageFlashColor;
		yield return new WaitForSeconds(damageFlashDuration);
		UpdateFillColor();
	}
}
