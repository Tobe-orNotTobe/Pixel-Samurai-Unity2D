using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Entity boss;
	[SerializeField] private GameObject healthBarContainer;

	private void Start()
	{
		if (boss == null)
		{
			Debug.LogError("BossHealthBar: Boss entity not assigned!");
			return;
		}

		// Khởi tạo thanh máu
		InitializeHealthBar();


		// Mặc định ẩn thanh máu lúc bắt đầu
		if (healthBarContainer != null)
			healthBarContainer.SetActive(false);
	}
	private void FixedUpdate()
	{
		boss.OnHealthChanged += UpdateHealthBar;
	}

	private void OnDestroy()
	{
		if (boss != null)
			boss.OnHealthChanged -= UpdateHealthBar;
	}

	private void InitializeHealthBar()
	{
		if (healthSlider == null) return;

		healthSlider.maxValue = boss.GetMaxHealth();
		healthSlider.value = boss.GetCurrentHealth();
	}

	private void UpdateHealthBar(float currentHealth, float maxHealth)
	{
		if (healthSlider == null) return;

		// Cập nhật giá trị thanh máu
		healthSlider.maxValue = maxHealth;
		healthSlider.value = currentHealth;
	}

	// Thêm phương thức mới để hiển thị thanh máu
	public void ShowHealthBar()
	{
		if (healthBarContainer != null)
		{
			// Cập nhật giá trị máu hiện tại trước khi hiển thị
			if (boss != null && healthSlider != null)
			{
				healthSlider.maxValue = boss.GetMaxHealth();
				healthSlider.value = boss.GetCurrentHealth();
			}

			// Hiển thị thanh máu
			healthBarContainer.SetActive(true);
		}
	}

	// Thêm phương thức để ẩn thanh máu
	public void HideHealthBar()
	{
		if (healthBarContainer != null)
			healthBarContainer.SetActive(false);
	}
	public void ManualUpdate()
	{
		if (boss != null && healthSlider != null)
		{
			healthSlider.maxValue = boss.GetMaxHealth();
			healthSlider.value = boss.GetCurrentHealth();
		}
	}
}