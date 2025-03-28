using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance { get; private set; }

	[SerializeField] private DialogueUI dialogueUI;
	[SerializeField] private GameObject dialogueUIPrefab;

	[Header("Dialogue Settings")]
	[SerializeField] private float typingSpeed = 0.05f;
	[SerializeField] private float delayBetweenMessages = 0.5f;

	private Queue<DialogueMessage> currentMessages;
	private bool isDialogueActive = false;
	private Coroutine dialogueCoroutine;

	public event Action OnDialogueStarted;
	public event Action OnDialogueEnded;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Find or create DialogueUI in the new scene
		FindOrCreateDialogueUI();
	}

	private void Start()
	{
		FindOrCreateDialogueUI();
		CloseDialogueUI();
	}

	private void FindOrCreateDialogueUI()
	{
		// Try to find DialogueUI in the scene
		dialogueUI = FindObjectOfType<DialogueUI>();

		// If not found and we have a prefab, instantiate it
		if (dialogueUI == null && dialogueUIPrefab != null)
		{
			GameObject uiObject = Instantiate(dialogueUIPrefab);
			dialogueUI = uiObject.GetComponent<DialogueUI>();

			// Make sure it's not destroyed when loading a new scene
			DontDestroyOnLoad(uiObject);
		}

		// Set up the event handler
		if (dialogueUI != null)
		{
			// Remove any existing handlers to prevent duplicates
			dialogueUI.OnSkipDialogue -= SkipEntireDialogue;
			dialogueUI.OnSkipDialogue += SkipEntireDialogue;
		}
		else
		{
			Debug.LogError("DialogueManager: Could not find or create DialogueUI!");
		}
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;

		if (dialogueUI != null)
		{
			dialogueUI.OnSkipDialogue -= SkipEntireDialogue;
		}
	}

	public bool IsDialogueActive()
	{
		return isDialogueActive;
	}

	public void StartDialogue(DialogueData dialogue)
	{
		if (isDialogueActive) return;

		// Ensure we have a valid DialogueUI
		if (dialogueUI == null)
		{
			FindOrCreateDialogueUI();
			if (dialogueUI == null)
			{
				Debug.LogError("Cannot start dialogue - DialogueUI is missing!");
				return;
			}
		}

		isDialogueActive = true;
		OnDialogueStarted?.Invoke();

		// Queue up all messages
		currentMessages = new Queue<DialogueMessage>();
		foreach (DialogueMessage message in dialogue.messages)
		{
			currentMessages.Enqueue(message);
		}

		// Show UI
		dialogueUI.gameObject.SetActive(true);

		// Start displaying messages
		DisplayNextMessage();
	}

	public void DisplayNextMessage()
	{
		if (currentMessages.Count == 0)
		{
			EndDialogue();
			return;
		}

		DialogueMessage message = currentMessages.Dequeue();
		if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);
		}

		dialogueCoroutine = StartCoroutine(TypeMessage(message));
	}

	public void SkipEntireDialogue()
	{
		if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);
		}

		EndDialogue();
	}

	public void SkipCurrentMessage()
	{
		if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);

			// Get the current message
			DialogueMessage currentMessage = null;
			if (currentMessages.Count > 0)
			{
				currentMessage = currentMessages.Peek();
			}

			// Show the full message immediately
			if (currentMessage != null)
			{
				dialogueUI.SetText(currentMessage.text);
				dialogueUI.ShowContinuePrompt(currentMessages.Count == 1);
			}
		}
	}

	private IEnumerator TypeMessage(DialogueMessage message)
	{
		dialogueUI.SetSpeakerName(message.speakerName);
		dialogueUI.ClearText();

		foreach (char letter in message.text.ToCharArray())
		{
			dialogueUI.AppendText(letter.ToString());

			// Check for Space key to skip typing
			if (Input.GetKeyDown(KeyCode.Space))
			{
				// Show the full message immediately
				dialogueUI.SetText(message.text);
				break;
			}

			yield return new WaitForSeconds(typingSpeed);
		}

		// If no more messages, show the "Continue" prompt
		dialogueUI.ShowContinuePrompt(currentMessages.Count == 0);

		// Wait for player input to continue
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
		yield return new WaitForSeconds(delayBetweenMessages);

		DisplayNextMessage();
	}

	private void EndDialogue()
	{
		isDialogueActive = false;
		CloseDialogueUI();
		OnDialogueEnded?.Invoke();
	}

	private void CloseDialogueUI()
	{
		if (dialogueUI != null)
		{
			dialogueUI.gameObject.SetActive(false);
		}
	}
}

[System.Serializable]
public class DialogueData
{
	public DialogueMessage[] messages;
}

[System.Serializable]
public class DialogueMessage
{
	public string speakerName;
	public string text;
	public bool isPlayer = false;
}