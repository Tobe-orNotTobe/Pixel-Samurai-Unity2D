using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueUI : MonoBehaviour
{
	public enum SpeakerType { Player, Boss }

	[Header("UI Elements")]
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TextMeshProUGUI speakerNameText;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private GameObject continuePrompt;
	[SerializeField] private Button skipButton;

	public event UnityAction OnSkipDialogue;

	private void Awake()
	{
		if (dialoguePanel == null)
			dialoguePanel = gameObject;

		if (skipButton != null)
		{
			skipButton.onClick.AddListener(SkipDialogue);
		}
		gameObject.SetActive(false);

	}

	private void OnDestroy()
	{
		if (skipButton != null)
		{
			skipButton.onClick.RemoveListener(SkipDialogue);
		}
	}

	private void SkipDialogue()
	{
		OnSkipDialogue?.Invoke();
	}

	public void SetSpeakerName(string name)
	{
		speakerNameText.text = name;
	}


	public void SetText(string text)
	{
		dialogueText.text = text;
	}

	public void AppendText(string text)
	{
		dialogueText.text += text;
	}

	public void ClearText()
	{
		dialogueText.text = "";
	}

	public void ShowContinuePrompt(bool isLastMessage)
	{
		if (continuePrompt != null)
			continuePrompt.SetActive(!isLastMessage);
	}

}