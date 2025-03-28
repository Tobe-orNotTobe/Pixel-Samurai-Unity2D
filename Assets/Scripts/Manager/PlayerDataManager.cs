using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
	public static PlayerDataManager Instance { get; private set; }

	// Health data
	public float MaxHealth { get; set; }
	public float CurrentHealth { get; set; }
	public bool ShouldResetHealth { get; set; } = true;

	// Healing data
	public int MaxHealingCharges { get; set; } = 3;
	public int CurrentHealingCharges { get; set; } = 3;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}