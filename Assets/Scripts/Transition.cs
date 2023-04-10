using UnityEngine;

public class Transition : MonoBehaviour
{

    public LevelLoad loader;

    public int NextLevel;

    public void OnClick()
    {
        Debug.Log("Clicked!");
        StartCoroutine(loader.LoadLevel(NextLevel));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
