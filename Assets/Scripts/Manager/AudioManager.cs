using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	[SerializeField] private AudioSource backgroundMusicSource;
	[SerializeField] private AudioSource bossMusicSource;

	[SerializeField] private AudioClip mainMenuMusic;
	[SerializeField] private AudioClip scene1Music;
	[SerializeField] private AudioClip scene2Music;
	[SerializeField] private AudioClip boss1Music;
	[SerializeField] private AudioClip boss2Music;

	private bool isBossMusicPlaying = false;
	private float fadeSpeed = 1.0f;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// Tạo AudioSource nếu chưa có
			if (backgroundMusicSource == null)
				backgroundMusicSource = gameObject.AddComponent<AudioSource>();

			if (bossMusicSource == null)
				bossMusicSource = gameObject.AddComponent<AudioSource>();

			// Đặt loop cho cả hai nguồn âm thanh
			backgroundMusicSource.loop = true;
			bossMusicSource.loop = true;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// Đăng ký sự kiện khi chuyển scene
		SceneManager.sceneLoaded += OnSceneLoaded;

		// Phát nhạc dựa vào scene hiện tại
		PlayMusicForCurrentScene();
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		PlayMusicForCurrentScene();
	}

	private void PlayMusicForCurrentScene()
	{
		// Dừng nhạc boss nếu đang phát
		if (isBossMusicPlaying)
		{
			StopBossMusic(true);
		}

		// Phát nhạc dựa vào tên scene
		string sceneName = SceneManager.GetActiveScene().name;

		if (sceneName == "Main Menu")
		{
			PlayBackgroundMusic(mainMenuMusic);
		}
		else if (sceneName == "Scene 1")
		{
			PlayBackgroundMusic(scene1Music);
		}
		else if (sceneName == "Scene 2")
		{
			PlayBackgroundMusic(scene2Music);
		}
	}

	public void PlayBackgroundMusic(AudioClip music)
	{
		if (music == null) return;

		// Nếu đang phát đúng clip này rồi thì không cần phát lại
		if (backgroundMusicSource.clip == music && backgroundMusicSource.isPlaying)
			return;

		// Dừng nhạc đang phát (nếu có)
		backgroundMusicSource.Stop();

		// Đặt clip mới và phát
		backgroundMusicSource.clip = music;
		backgroundMusicSource.Play();
	}

	public void PlayBossMusic(bool isBoss1)
	{
		AudioClip bossMusic = isBoss1 ? boss1Music : boss2Music;

		if (bossMusic == null) return;

		// Tạm dừng nhạc nền
		backgroundMusicSource.Pause();

		// Phát nhạc boss
		bossMusicSource.clip = bossMusic;
		bossMusicSource.Play();

		isBossMusicPlaying = true;
	}

	public void StopBossMusic(bool immediate = false)
	{
		// Dừng nhạc boss
		bossMusicSource.Stop();

		// Tiếp tục nhạc nền
		if (backgroundMusicSource.clip != null)
			backgroundMusicSource.Play();

		isBossMusicPlaying = false;
	}

	public void PauseAllMusic(bool pause)
	{
		if (pause)
		{
			// Tạm dừng tất cả nhạc
			if (backgroundMusicSource.isPlaying)
				backgroundMusicSource.Pause();

			if (bossMusicSource.isPlaying)
				bossMusicSource.Pause();
		}
		else
		{
			// Tiếp tục phát nhạc tùy thuộc vào trạng thái
			if (isBossMusicPlaying && bossMusicSource.clip != null)
				bossMusicSource.UnPause();
			else if (backgroundMusicSource.clip != null)
				backgroundMusicSource.UnPause();
		}
	}
}