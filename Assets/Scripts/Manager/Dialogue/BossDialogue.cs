using UnityEngine;

[CreateAssetMenu(fileName = "BossDialogue", menuName = "Dialogue/Boss Dialogue")]
public class BossDialogue : ScriptableObject
{
	[TextArea(3, 10)]
	public string[] playerDialogueLines;

	[TextArea(3, 10)]
	public string[] bossDialogueLines;

	public string playerName = "Jiro";
	public string bossName = "";

	public DialogueData GetDialogueData()
	{
		int totalLines = playerDialogueLines.Length + bossDialogueLines.Length;
		DialogueData dialogueData = new DialogueData();
		dialogueData.messages = new DialogueMessage[totalLines];

		bool isPlayerTurn = true; 
		int playerIndex = 0;
		int bossIndex = 0;

		for (int i = 0; i < totalLines; i++)
		{
			DialogueMessage message = new DialogueMessage();

			if (isPlayerTurn)
			{
				if (playerIndex < playerDialogueLines.Length)
				{
					message.speakerName = playerName;
					message.text = playerDialogueLines[playerIndex];
					message.isPlayer = true;
					playerIndex++;
				}
			}
			else
			{
				if (bossIndex < bossDialogueLines.Length)
				{
					message.speakerName = bossName;
					message.text = bossDialogueLines[bossIndex];
					message.isPlayer = false;
					bossIndex++;
				}
			}

			dialogueData.messages[i] = message;
			isPlayerTurn = !isPlayerTurn; 
		}

		return dialogueData;
	}
}