using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("Player")]
	[SerializeField] private GameObject playerPrefab;

	[Header("Game States")]
	private bool isGamePaused = false;
	private bool isGameOver = false;

	[Header("Game Over Settings")]
	[SerializeField] private float delayBeforeShowingButtons = 2f;

	private const int MAIN_MENU_SCENE = 0;
	private const int SCENE_1 = 1;
	private const int SCENE_2 = 2;

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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
		{
			if (isGamePaused)
				ResumeGame();
			else
				PauseGame();
		}
	}

	#region Gameplay Management

	public void StartNewGame()
	{
		isGameOver = false;
		UIManager.Instance.HideAllUI();

		// Set the flag to reset health
		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.ShouldResetHealth = true;
		}

		SceneManager.LoadSceneAsync(SCENE_1);
	}

	public void Respawn()
	{
		PlayerDied();
	}

	public void PlayerDied()
	{
		isGameOver = true;

		// Tạm dừng nhạc khi game over
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PauseAllMusic(true);
		}

		UIManager.Instance.ShowGameOver();
		StartCoroutine(ShowGameOverButtonsDelayed());
	}

	private IEnumerator ShowGameOverButtonsDelayed()
	{
		yield return new WaitForSeconds(delayBeforeShowingButtons);
		UIManager.Instance.ShowGameOverButtons();
	}
	#endregion

	#region Menu Management

	public void PauseGame()
	{
		isGamePaused = true;
		Time.timeScale = 0f;

		// Tạm dừng nhạc khi pause game
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PauseAllMusic(true);
		}

		UIManager.Instance.ShowPauseMenu(true);
	}

	public void ResumeGame()
	{
		isGamePaused = false;
		Time.timeScale = 1f;

		// Tiếp tục nhạc khi resume game
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PauseAllMusic(false);
		}

		UIManager.Instance.ShowPauseMenu(false);
	}

	public void LoadMainMenu()
	{
		Time.timeScale = 1f;
		UIManager.Instance.HideAllUI();
		isGameOver = false;
		isGamePaused = false;

		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.ShouldResetHealth = true;
		}

		SceneManager.LoadSceneAsync(MAIN_MENU_SCENE);
	}

	public void LoadScene(int sceneIndex, bool skipCutscene = false)
	{
		Time.timeScale = 1f;
		UIManager.Instance.HideAllUI();
		isGameOver = false;
		isGamePaused = false;

		// Nếu là scene 1 và yêu cầu bỏ qua cutscene
		if (sceneIndex == 1 && skipCutscene)
		{
			// Load scene gameplay (scene 1) trực tiếp
			SceneManager.LoadSceneAsync(SCENE_1);
			Debug.Log("Đang load thẳng đến scene gameplay, bỏ qua cutscene");
		}
		else
		{
			// Flow bình thường, có thể qua cutscene trước
			SceneManager.LoadSceneAsync(sceneIndex);
		}
	}

	public void QuitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	#endregion
}