using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 3.0f;

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
