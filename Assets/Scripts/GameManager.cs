using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelLoad Loader;

    //[SerializeField]
    //private ItemDatabase itemDb;

    public PlayerData playerData;

    public GameState state;

    public int IntroIndex;
    public int HomeHubIndex;
    public int ShopIndex;
    public int Level1dot1Index;
    public int Level1dot2Index;
    public int Level1dot3Index;
    public int Level1dot4Index;
    //public int Level2Index;
    public int AJIndex;
    public int TFPIndex;

    public bool Loaded;
    public bool InitialLoad = false;

    void Awake()
    {
        Debug.Log("Setting to not destroy on transition");
        DontDestroyOnLoad(this.gameObject);
        Loaded = false;
        state = GameState.LOADING;
        
    }

    void Start()
    {
        Loader.transition.SetTrigger("Loading");
        switch (state)
        {
            case GameState.LOADING:
                {
                    HandleLoading();
                    if (InitialLoad)
                        state = GameState.INITIALLOAD;
                    else
                        state = GameState.PLAYING;
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

        if (state == GameState.INITIALLOAD)
        {
            if (playerData.ViewedIntro)
            {
                StartCoroutine(Loader.LoadLevel(HomeHubIndex));
            }
            else
                StartCoroutine(Loader.LoadLevel(IntroIndex));
        }
    }

    public void TransitionToLevel(int level)
    {
        int index = 0;

        switch (level)
        {
            case 1: index = Level1dot1Index; break;
            case 2: index = Level1dot2Index; break;
            case 3: index = Level1dot3Index; break;
            case 4: index = Level1dot4Index; break;
            case 5: index = AJIndex; break;
            default: index = HomeHubIndex; break;
        }

        StartCoroutine(Loader.LoadLevel(index));
    }

    private void HandleExit()
    {
        Save.SavePlayer(playerData);
        Application.Quit();
    }
	
}
