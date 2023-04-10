using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelLoad loader;

    private bool submittedDisable = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!submittedDisable)
        { 
            if (loader.GetAnimationFinished())
            {
                var go = loader.gameObject.transform.Find("Canvas");
                if (go != null) { go.gameObject.SetActive(false); }
            }
        }
    }
}
