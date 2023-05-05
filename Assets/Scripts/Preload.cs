using UnityEngine;

public class Preload : MonoBehaviour
{
    [SerializeField]
    private LevelLoad loader;

    // Start is called before the first frame update
    void Start()
    {
        //int sceneCount = SceneManager.sceneCount;
        //for(int i = 0; i < sceneCount; i++)
        //{
        //    var scene = SceneManager.GetSceneAt(i);
        //    Debug.Log(scene.name);
        //}


        StartCoroutine(loader.LoadLevel(1));
    }

}
