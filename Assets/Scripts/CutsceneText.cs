using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneText : MonoBehaviour
{
	public TextMeshProUGUI cutsceneText; // Text hiển thị nội dung
	public float typeSpeed = 0.05f; // Tốc độ gõ chữ
	public float scrollSpeed = 30f; // Tốc độ trôi lên
	public float waitAfterTyping = 2f; // Thời gian chờ trước khi text trôi lên

	private RectTransform rectTransform;
	private bool isSkipping = false;
	private bool isScrolling = false;

	private string[] storyLines = new string[]
	{
		"During the chaotic Sengoku era, the old samurai Jiro seeks to lay down his sword...",
		"But fate has other plans. He is tasked with escorting the young lord Yoshihiro.",
		"Along the way, they are ambushed. Yoshihiro is kidnapped...",
		"Jiro pursues the enemy and uncovers a forbidden secret...",
		"Hansho, his former comrade, plans to sacrifice Yoshihiro to resurrect his lost love...",
		"At Iwakura Shrine, the fateful battle begins...",
		"The destiny of Japan will be decided... by Jiro’s blade."
	};

	void Start()
	{
		rectTransform = cutsceneText.GetComponent<RectTransform>();
		cutsceneText.text = ""; 
		StartCoroutine(PlayCutscene());
	}

	IEnumerator PlayCutscene()
	{
		foreach (string line in storyLines)
		{
			if (isSkipping) break;

			yield return StartCoroutine(TypeText(line)); 
			yield return new WaitForSeconds(waitAfterTyping); 
		}

		if (!isSkipping)
		{
			StartCoroutine(ScrollTextUp()); 
		}
	}

	IEnumerator TypeText(string line)
	{
		cutsceneText.text = "";
		foreach (char letter in line)
		{
			cutsceneText.text += letter;
			yield return new WaitForSeconds(typeSpeed);
		}
	}

	IEnumerator ScrollTextUp()
	{
		isScrolling = true;
		while (rectTransform.anchoredPosition.y < 500f)
		{
			rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
			yield return null;
		}

		SceneManager.LoadScene("Scene 1"); 
	}

	public void SkipCutscene()
	{
		isSkipping = true;
		StopAllCoroutines();
		SceneManager.LoadScene("Scene 1"); 
	}
}
