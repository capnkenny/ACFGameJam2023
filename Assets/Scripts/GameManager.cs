using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ItemDatabase itemDb;

    [SerializeField]
    private string firstSceneToLoad;

    [SerializeField]
    private int secondsToWaitBeforeLoad_DeleteBeforeLaunch;

    public bool DebuggingFlag = false;


    private IEnumerator Delay() {
        yield return new WaitForSeconds(secondsToWaitBeforeLoad_DeleteBeforeLaunch);
        Debug.Log("Switching scenes in Delay method");
        if (!DebuggingFlag) SceneManager.LoadScene(firstSceneToLoad);
    }

    void Awake()
    {
        Debug.Log("Setting to not destroy on transition");
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if(string.IsNullOrWhiteSpace(firstSceneToLoad))
            firstSceneToLoad = "SampleScene 1";

        StartCoroutine(Delay());
    }


}
