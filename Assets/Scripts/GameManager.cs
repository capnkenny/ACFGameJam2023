using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LevelLoad loader;

    //[SerializeField]
    //private ItemDatabase itemDb;

    public PlayerData playerData;

    public GameState state;

    public int IntroIndex;
    public int HomeHubIndex;
    public int ShopIndex;
    public int Level1Index;
    //public int Level2Index;
    public int AJIndex;

    public bool Loaded;

    void Awake()
    {
        Debug.Log("Setting to not destroy on transition");
        DontDestroyOnLoad(this.gameObject);
        Loaded = false;
        state = GameState.LOADING;
        loader.transition.SetTrigger("Loading");
    }

    void Start()
    {
        switch (state)
        {
            case GameState.LOADING:
                {
                    HandleLoading();
                    break;
                }
            case GameState.PLAYING:
                {
                    break;
                }
            case GameState.EXITING:
                {
                    HandleExit();
                    break;
                }
            default: break;
        }

    }

    public void SetCamera(Camera cam)
    {
        var canvas = gameObject.GetComponentInChildren<Canvas>();
        if(canvas != null) 
        {
            canvas.worldCamera = cam;
        }
    }

    private void HandleLoading()
    {
		playerData = Save.LoadPlayer();
		if (playerData == null)
		{
			playerData = Save.SaveNewPlayer();
		}

		Loaded = true;
        state = GameState.PLAYING;
		if (playerData.ViewedIntro)
		{
			StartCoroutine(loader.LoadLevel(HomeHubIndex));
		}
		else
			StartCoroutine(loader.LoadLevel(IntroIndex));
	}

    private void HandleExit()
    {
        Save.SavePlayer(playerData);
        Application.Quit();
    }
	
}
