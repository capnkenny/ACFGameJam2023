
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public Dialogue dialogue;

	public void TriggerDialogue()
	{
		var dm = FindObjectOfType<DialogueManager>();
		if (dm != null)
		{
			if (dm.DialogueStarted)
			{
				dm.DisplayNextSentence();
			}
			else
			{
				dm.StartDialogue(dialogue);
			}
		}
	}
}

