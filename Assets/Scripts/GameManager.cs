using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LevelLoad Loader;

    //[SerializeField]
    //private ItemDatabase itemDb;

    public PlayerData playerData;
    public GameObject playerPrefab;
    public GameObject GameUI;
    public GameObject GameOverUI;
    public Canvas UICanvas;

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
    public bool GoToHub = false;
    public bool Quit = false;

    private float intensityOrig;
    private PlayerController playerController;

    void Awake()
    {
        //Debug.Log("Setting to not destroy on transition");
        DontDestroyOnLoad(this.gameObject);
        Loaded = false;
    }

    void Start()
    {
        Loader.transition.SetTrigger("Loading");
        switch (state)
        {
            case GameState.INITIALLOAD:
            case GameState.LOADING:
                {
                    HandleLoading();
                    if (!InitialLoad)

                        state = GameState.PLAYING;
                    break;
                }
            case GameState.INTRO:
            case GameState.HUB:
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

    private void Update()
    {
        switch (state)
        {
            case GameState.INITIALLOAD:
            case GameState.LOADING:
                {
                    HandleLoading();
                    //if (InitialLoad)
                    //    state = GameState.INITIALLOAD;
                    //else
                    if(!InitialLoad)
                        state = GameState.PLAYING;
                    break;
                }
            case GameState.INTRO:
            case GameState.HUB:
            case GameState.PLAYING:
                {
                    break;
                }
            case GameState.GAMEOVER:
                {
                    if (!GoToHub && !Quit)
                        break;
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
        Debug.Log("Loading player data...");
		playerData = Save.LoadPlayer();
		if (playerData == null)
		{
			playerData = Save.SaveNewPlayer();
		}

		Loaded = true;

        if (state == GameState.INITIALLOAD)
        {
            InitialLoad = true;
            Debug.Log("Initial load detected");
            if (playerData.ViewedIntro)
            {

                Debug.Log("Skipping intro");
                TransitionToLevel(-1);
            }
            else
            {
                state = GameState.INTRO;
                Debug.Log("Going to intro");
                TransitionToLevel(-2);
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == HomeHubIndex)
            { }
            else
            {
                state = GameState.PLAYING;
            }
        }
    }

    public void TransitionToLevel(int level)
    {
        if(state != GameState.INTRO)
            state = GameState.LOADING;
        int index = 0;

        switch (level)
        {
            case -2: index = IntroIndex; break;
            case 1: index = Level1dot1Index; break;
            case 2: index = Level1dot2Index; break;
            //case 3: index = Level1dot3Index; break;
            //case 4: index = Level1dot4Index; break;
            case 3:
            case 4:
            case 5: index = AJIndex; break;
            case -1:
            default: index = HomeHubIndex; break;
        }

        Debug.LogFormat("Transition called - navigating to scene {0}", index);

        StartCoroutine(Loader.LoadLevel(index));
    }

    public PlayerController SpawnPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab);
            var player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerStats(playerData);
            playerController = player;
            return player;
        }
        else
        {
            var player = playerObject.GetComponent<PlayerController>();
            player.SetPlayerStats(playerData);
            playerController = player;
            return player;
        }
    }

    private void HandleExit()
    {
        Save.SavePlayer(playerData);
        Application.Quit();
    }

    public void EnableGameUI()
    {
        UICanvas.worldCamera = Camera.main;
        GameUI.SetActive(true);
    }

    public void DisableGameUI()
    {
        GameUI.SetActive(false);
    }

    public PlayerController? GetPlayer()
    {
        if (playerController != null)
            return playerController;
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) { return null; }
            PlayerController pc = playerObject.GetComponent<PlayerController>();
            if (pc == null) return null;
            else
            {
                playerController = pc;
                return playerController;
            }
        }
    }

    public void SensoryOverload(bool activated = false)
    {
        var l2d = Camera.main.GetComponentInChildren<Light2D>();
        if (activated)
        {
            if (l2d != null)
            {
                intensityOrig = l2d.intensity;
                l2d.intensity = 0.2f;
                //l2d.color = new Color()
            }
        }
        else
        {
            //deactivated
            if (l2d != null)
            {
                l2d.intensity = intensityOrig;
                //l2d.color = new Color()
            }
        }
    }

    public void EnableGameOverMenu(bool active = true)
    {
        GameOverUI.SetActive(active);
    }

    public void SignalPlayerDeath()
    {
        state = GameState.GAMEOVER;
        DisableGameUI();
        EnableGameOverMenu();
    }
}
