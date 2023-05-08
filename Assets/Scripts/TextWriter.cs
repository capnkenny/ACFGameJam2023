using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{
	public float deltaBetweenLetters = 0.1f;

	public TextMeshProUGUI textBox;
	public GameObject HiddenObject;

	public TextAsset textFile;

	public bool Done = false;
	private bool typing = false;

	private Queue<char> _textToWrite = new Queue<char>();

	private float timer = 0.0f;

	private char _previousChar = ' ';
	private char _currentChar = ' ';

	private void Awake()
	{
		var arr = textFile.text.Split('\r');
		foreach (var item in arr)
		{
			foreach (var c in item)
			{
				_textToWrite.Enqueue(c);
			}
		}

		
	}

	private IEnumerator ParseText()
	{
		if (!typing)
		{
			typing = true;
			_previousChar = _currentChar;
			_currentChar = _textToWrite.Dequeue();

			if (_currentChar == '/')
			{
				_previousChar = _currentChar;
				_currentChar = _textToWrite.Dequeue();
				if (_currentChar == 'w')
				{
					_textToWrite.Dequeue();
					bool r = int.TryParse(_textToWrite.Dequeue().ToString(), out int sec);
					if (r)
					{
						yield return new WaitForSeconds(sec);
						timer = 0.0f;
					}
				}
				else if (_currentChar == 'c')
				{
					textBox.SetText("");
				}
				else if (_currentChar == 'h')
				{
					HiddenObject.SetActive(true);
				}
				else
				{
					textBox.text += _currentChar;
				}
			}
			else
			{
				textBox.text += _currentChar;
			}

			typing = false;
			timer = 0.0f;
			if (_textToWrite.Count == 0)
			{
				Done = true;
			}
		}
	}

	private void Update()
	{
		if (!Done)
		{ 
			timer += Time.deltaTime;

			if (timer >= deltaBetweenLetters)
			{
				StartCoroutine(ParseText());
			}
		}
	}

}

