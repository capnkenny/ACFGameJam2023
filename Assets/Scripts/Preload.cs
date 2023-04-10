using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    [SerializeField]
    private LevelLoad loader;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var scene in SceneManager.GetAllScenes())
        {
            Debug.Log(scene.name);
        }


        StartCoroutine(loader.LoadLevel(1));
    }

}
