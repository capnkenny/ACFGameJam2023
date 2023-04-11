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

    private Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();

	private void Start()
	{
        sprites.Add(1, Koi1);
        sprites.Add(2, Koi2);
        sprites.Add(3, Koi3);
        sprites.Add(4, Koi4);
	}


	private void Update()
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

	public bool GetAnimationFinished()
    {
        return transition.GetCurrentAnimatorStateInfo(0).length > 1;
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
