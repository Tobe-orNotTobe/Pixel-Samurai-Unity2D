using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class BossFightManager : MonoBehaviour
{
	[Header("Boss Settings")]
	[SerializeField] private Entity boss;
	[SerializeField] private bool isBoss1; // True for Boss1, False for Boss2

	[Header("Camera Settings")]
	[SerializeField] private CinemachineCamera bossCamera;
	[SerializeField] private CinemachineCamera playerCamera;

	[Header("Boundaries")]
	[SerializeField] private GameObject boundaryColliders;

	[Header("UI Settings")]
	[SerializeField] private BossHealthBar bossHealthBar;

	[Header("Dialogue Settings")]
	[SerializeField] private BossDialogue boss1Dialogue;
	[SerializeField] private BossDialogue boss2Dialogue;
	[SerializeField] private Transform playerDialoguePosition;
	[SerializeField] private Transform bossDialoguePosition;

	[Header("Trigger Settings")]
	[SerializeField] private bool activateOnTriggerEnter = true;
	[SerializeField] private float delayAfterBossDefeat = 3f; // Time before scene change
	[SerializeField] private float delayBeforeFight = 1f; // Time after dialogue before starting the fight

	private bool isBossFightActive = false;
	private bool isBossDefeated = false;
	private bool isDialogueCompleted = false;
	private GameObject player;
	private PlayerController playerController;

	private void Start()
	{
		// Ensure boss reference is set correctly
		if (boss == null)
		{
			Debug.LogError("Boss reference not set in BossFightManager!");
			return;
		}

		// Hide boundaries initially
		if (boundaryColliders != null)
			boundaryColliders.SetActive(false);

		// Turn off boss camera initially
		if (bossCamera != null)
			bossCamera.enabled = false;

		// Subscribe to dialogue events
		if (DialogueManager.Instance != null)
		{
			DialogueManager.Instance.OnDialogueStarted += OnDialogueStarted;
			DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
		}
	}

	private void OnDestroy()
	{
		// Unsubscribe from dialogue events
		if (DialogueManager.Instance != null)
		{
			DialogueManager.Instance.OnDialogueStarted -= OnDialogueStarted;
			DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
		}
	}

	private void Update()
	{
		if (isBossFightActive && !isBossDefeated)
		{
			CheckBossStatus();
		}
	}

	private void CheckBossStatus()
	{
		if (boss == null) return;

		bool isDead = false;

		// Check boss type and death status
		if (isBoss1 && boss is Boss1)
		{
			isDead = ((Boss1)boss).IsDead;
		}
		else if (!isBoss1 && boss is Boss2)
		{
			isDead = ((Boss2)boss).IsDead;
		}

		if (isDead && !isBossDefeated)
		{
			isBossDefeated = true;

			// Ẩn thanh máu khi boss bị đánh bại
			if (bossHealthBar != null)
				bossHealthBar.HideHealthBar();

			// Dừng nhạc boss và quay trở lại nhạc nền
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.StopBossMusic();
			}

			StartCoroutine(HandleBossDefeat());
		}
	}

	private IEnumerator HandleBossDefeat()
	{
		// Wait for player to see boss defeat
		yield return new WaitForSeconds(delayAfterBossDefeat);

		// Unlock camera and boundaries
		EndBossFight();

		// Actions after boss defeat
		if (isBoss1)
		{
			// Move to Scene 2 after Boss1 defeat
			SceneManager.LoadScene("Scene 2"); // Scene 2
		}
		else
		{
			// Return to main menu after Boss2 defeat
			SceneManager.LoadScene("Main Menu"); // Main Menu
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (activateOnTriggerEnter && collision.CompareTag("Player") && !isBossFightActive && !isBossDefeated)
		{
			player = collision.gameObject;
			playerController = player.GetComponent<PlayerController>();
			StartBossCutscene();
		}
	}

	private void StartBossCutscene()
	{
		// Enable boss fight boundaries
		if (boundaryColliders != null)
			boundaryColliders.SetActive(true);

		// Switch to boss camera
		if (bossCamera != null && playerCamera != null)
		{
			playerCamera.enabled = false;
			bossCamera.enabled = true;
		}

		// Force player and boss into idle state
		if (playerController != null)
		{
			playerController.SetIsHealing(true); // Reusing healing state to prevent movement
		}

		// Ensure the boss is in idle state too
		if (boss != null)
		{
			if (isBoss1 && boss is Boss1)
			{
				Boss1 boss1 = (Boss1)boss;
				boss1.stateMachine.ChangeState(boss1.idleState);
			}
			else if (!isBoss1 && boss is Boss2)
			{
				Boss2 boss2 = (Boss2)boss;
				boss2.stateMachine.ChangeState(boss2.idleState);
			}
		}

		// Start dialogue
		StartCoroutine(StartDialogueAfterDelay(0.5f));
	}

	private IEnumerator StartDialogueAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		// Start appropriate dialogue
		BossDialogue dialogueToUse = isBoss1 ? boss1Dialogue : boss2Dialogue;
		if (dialogueToUse != null && DialogueManager.Instance != null)
		{
			DialogueData dialogueData = dialogueToUse.GetDialogueData();
			DialogueManager.Instance.StartDialogue(dialogueData);
		}
		else
		{
			// If no dialogue, skip to fight
			OnDialogueEnded();
		}
	}

	private void OnDialogueStarted()
	{
		isDialogueCompleted = false;
		Debug.Log("Dialogue started");
	}

	private void OnDialogueEnded()
	{
		isDialogueCompleted = true;
		Debug.Log("Dialogue ended");

		// Start the actual boss fight after dialogue
		StartCoroutine(StartFightAfterDelay());
	}

	private IEnumerator StartFightAfterDelay()
	{
		yield return new WaitForSeconds(delayBeforeFight);
		StartBossFight();
	}

	public void StartBossFight()
	{
		if (isBossFightActive || isBossDefeated) return;

		isBossFightActive = true;

		// Chuyển từ nhạc nền sang nhạc boss
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayBossMusic(isBoss1);
		}

		// Release player from forced idle state
		if (playerController != null)
		{
			playerController.SetIsHealing(false);
		}

		if (bossHealthBar != null)
		{
			bossHealthBar.ShowHealthBar();
			bossHealthBar.ManualUpdate();
		}

		Debug.Log("Boss fight started with " + (isBoss1 ? "Boss1" : "Boss2"));
	}

	public void EndBossFight()
	{
		isBossFightActive = false;

		// Disable boundaries
		if (boundaryColliders != null)
			boundaryColliders.SetActive(false);

		// Switch back to player camera
		if (bossCamera != null && playerCamera != null)
		{
			bossCamera.enabled = false;
			playerCamera.enabled = true;
		}

		// Ẩn thanh máu của boss khi kết thúc trận chiến
		if (bossHealthBar != null)
			bossHealthBar.HideHealthBar();
	}
}