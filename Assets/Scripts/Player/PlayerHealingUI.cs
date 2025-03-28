using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealingUI : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private Text healingCountText; // Text display for healing count

	private PlayerStats playerStats;

	private void Start()
	{
		playerStats = FindObjectOfType<PlayerStats>();
		if (playerStats == null)
		{
			Debug.LogError("PlayerHealingUI: Could not find PlayerStats component!");
			return;
		}

		playerStats.OnHealingChargesChanged += UpdateHealingUI;

		// Initialize UI
		UpdateHealingUI(playerStats.CurrentHealingCharges, playerStats.MaxHealingCharges);
	}

	private void OnDestroy()
	{
		if (playerStats != null)
			playerStats.OnHealingChargesChanged -= UpdateHealingUI;
	}

	private void UpdateHealingUI(int currentCharges, int maxCharges)
	{
		// Update text counter
		if (healingCountText != null)
		{
			healingCountText.text = currentCharges.ToString();
		}
	}
}