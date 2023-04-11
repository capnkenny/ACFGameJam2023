using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public TextWriter writer;

    public LevelLoad loader;

    public int NextLevelIndex;

    // Update is called once per frame
    void Update()
    {
        if (writer.Done)
        {
            Debug.Log("Time to go!");
            var level = GameObject.FindGameObjectWithTag("GameManager");
            if(level != null) 
            {
                var gm = level.GetComponent<GameManager>();
				StartCoroutine(loader.LoadLevel(gm.HomeHubIndex));
			}
            
        }
    }
}
