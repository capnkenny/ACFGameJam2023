
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	public TextMeshProUGUI NameText;
	public TextMeshProUGUI DialogueText;
	//public TextMeshProUGUI ContinueText;
	public GameObject DialogueBox;

	private Queue<string> sentences;
	public bool DialogueStarted;

	private void Start()
	{
		sentences = new();
		//ContinueText.enabled = false;
		DialogueStarted = false;
	}

	public void StartDialogue(Dialogue d)
	{
		DialogueBox.SetActive(true);
		NameText.text = d.Name;

		sentences.Clear();
		foreach(string s in d.Sentences)
			sentences.Enqueue(s);
		DialogueStarted = true;
		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if(sentences.Count == 0) 
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();

		DialogueText.text = sentence;
	}

	public void EndDialogue() 
	{
		DialogueBox.SetActive(false);
		NameText.text = "";
		DialogueStarted = false;
		Debug.Log("End Dialogue");
	}

}

