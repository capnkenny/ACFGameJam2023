using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{
    public Animator transition;
    public GameObject Koi;

    public Sprite Koi1;
	public Sprite Koi2;
	public Sprite Koi3;
	public Sprite Koi4;

	public float transitionTime = 3.0f;
    
    private int _index = 1;
    private float degreesRotated = 0;
    public bool callAction = false;

    private Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();

    public Action LoadEndFunction;

	private void Start()
	{
        sprites.Add(1, Koi1);
        sprites.Add(2, Koi2);
        sprites.Add(3, Koi3);
        sprites.Add(4, Koi4);
	}


	private void Update()
	{
        if (callAction)
        {
            LoadEndFunction();
            callAction = false;
        }

        if (transition.GetCurrentAnimatorStateInfo(0).IsName("LoadLevel"))
        {
            var rotate = -90 * Time.deltaTime;
            Koi.transform.Rotate(0, 0, rotate);
            degreesRotated += rotate;

            if (degreesRotated < -90)
            {
                _index++;
                degreesRotated = 0;
            }

            if (_index > 4)
                _index = 1;

            if (sprites[_index].name != Koi.GetComponent<UnityEngine.UI.Image>().name)
            {
                Koi.GetComponent<UnityEngine.UI.Image>().sprite = sprites[_index];
            }
        }
	}

    public void SetLoading()
    {
        transition.SetTrigger("Loading");
        Debug.LogWarning("Setting loading anim");
    }

    public void SetLoadingIfNot()
    {
        if (transition.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LoadEnd"))
            SetLoading();
    }

	public void SetDoneLoadingIfNot()
	{
		if (transition.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LoadLevel"))
			SetDoneLoading();
	}

	public void SetDoneLoading()
    {
		transition.SetTrigger("DoneLoad");
		Debug.LogWarning("Setting loading anim to finish");
    }

	public bool GetAnimationFinished()
    {
        return transition.GetCurrentAnimatorStateInfo(0).length >= 1.0f;
    }

    public IEnumerator LoadLevel(int index)
    {
        Debug.LogFormat("Scene requested - {0}", index);
		transition.SetTrigger("Loading");
		
        yield return new WaitForSeconds(transitionTime);
        
        SceneManager.LoadScene(index);
    }

    public void Load(int index)
    {
        LoadLevel(index);
    }


}
