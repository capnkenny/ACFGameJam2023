using System.Collections;
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
    public GameUIManager gameUiManager;

    public GameState state;

    public int IntroIndex;
    public int HomeHubIndex;
    public int ShopIndex;
    public int Level1dot1Index;
    public int Level1dot2Index;
    //public int Level1dot3Index;
    //public int Level1dot4Index;
    //public int Level2Index;
    public int AJIndex;
    public int TFPIndex;

    public bool Loaded;
    public bool InitialLoad = false;
    public bool GoToHub = false;
    public bool Quit = false;

    [Header("Sensory Overload")]
    public float R;
    public float G;
    public float B;
    public float A;
    public Color Level1Color;
    public bool PlayerInSensoryOverload;

    private float intensityOrig;
    private PlayerController playerController;
    private Light2D playerLight;

    void Awake()
    {
        //Debug.Log("Setting to not destroy on transition");
        DontDestroyOnLoad(this.gameObject);
        Loaded = false;
    }

    void Start()
    {
        Loader.SetLoading();
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
                    if(!InitialLoad)
                    { 
                        if (SceneManager.GetActiveScene().buildIndex == HomeHubIndex)
                            state = GameState.HUB;
                        else
                            state = GameState.PLAYING;
                    }
					var ll = GameObject.FindGameObjectWithTag("LvlLoader");
					if (ll != null)
						Loader = ll.GetComponent<LevelLoad>();
                    if (InitialLoad && state == GameState.LOADING) InitialLoad = false;

					break;
                }
            
            case GameState.HUB:
                {
                    DisableGameUI();
                    break;
                }
			case GameState.INTRO:
                {
                    break;
                }
			case GameState.PLAYING:
                {
                    if (playerController == null) playerController = GetPlayer();
                    if (playerController != null)
                    {
                        PlayerInSensoryOverload = playerController._sensoryOverload;
                    }
                    SensoryOverload(PlayerInSensoryOverload);
                    //if (gameUiManager.gameObject.activeSelf)
                    //{
                    //    gameUiManager.Set
                    //}
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
        if (playerData == null)
            playerData = new PlayerData();//LoadPlayerData();

		Loaded = true;

        if (state == GameState.INITIALLOAD)
        {
            InitialLoad = true;
            Debug.Log("Initial load detected");
            //LoadPlayerData();
            //if (playerData.ViewedIntro)
            //{

            //    Debug.Log("Skipping intro");
            //    TransitionToLevel(-1);
            //}
            //else
            //{
                state = GameState.INTRO;
                Debug.Log("Going to intro");
                TransitionToLevel(-2);
            //}
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

    //public void LoadPlayerData()
    //{
    //    Debug.Log("Loading player data...");
    //    playerData = Save.LoadPlayer();
    //    if (playerData == null)
    //    {
    //        playerData = Save.SaveNewPlayer();
    //    }
    //}

    public void TransitionToLevel(int level)
    {
        if (Loader == null)
        {
			var ll = GameObject.FindGameObjectWithTag("LvlLoader");
            if (ll != null)
            {
                Loader = ll.GetComponent<LevelLoad>();
            }
		}

        if (Loader != null)
        {
            Loader.SetLoading();
        }
		
		if (state != GameState.INTRO)
            state = GameState.LOADING;
        int index = 0;

        switch (level)
        {
            case -10: index = TFPIndex; break;
            case -2: index = IntroIndex; break;
            case 1: index = Level1dot1Index; break;
            //case 2: index = Level1dot2Index; break;
            //case 3: index = Level1dot3Index; break;
            //case 4: index = Level1dot4Index; break;
            case 2:
            case 3:
            case 4:
            case 5: index = AJIndex; break;
            case -1:
            default: index = HomeHubIndex; break;
        }

        Debug.LogFormat("Transition called - navigating to scene {0}", index);
		if (index == HomeHubIndex)
			state = GameState.HUB;
		SceneManager.LoadScene(index);
	}

    private IEnumerator LoadLevel(int index)
    {
        Debug.LogFormat("GM - Scene requested - {0}", index);

        if (index == HomeHubIndex)
            state = GameState.HUB;

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(index);
    }

    public void CompleteBossBattle(PlayerData _)
    {
        TransitionToLevel(-10);
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

    public void HandleExit()
    {
        Application.Quit();
    }

    public void EnableGameUI()
    {
		//Debug.LogWarning("Enabling in-game ui");
		UICanvas.worldCamera = Camera.main;
        GameUI.SetActive(true);
    }

    public void DisableGameUI()
    {
        //Debug.LogWarning("Disabling in-game ui");
        if (GameUI.activeSelf)
            GameUI.SetActive(false);
    }

    public float GetPlayerHealth()
    {
        if (playerController != null)
        {
            return playerController.Health;
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) { return 0; }
            PlayerController pc = playerObject.GetComponent<PlayerController>();
            if (pc == null) return 0;
            else
            {
                playerController = pc;
                return pc.Health;
            }
        }
    }

    public float GetPlayerSensoryMeter()
    {
        if (playerController != null)
        {
            return playerController.SensoryMeter;
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) { return 0; }
            PlayerController pc = playerObject.GetComponent<PlayerController>();
            if (pc == null) return 0;
            else
            {
                playerController = pc;
                return pc.SensoryMeter;
            }
        }
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

    public PlayerController? GetRefreshedPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) { return null; }
        PlayerController pc = playerObject.GetComponent<PlayerController>();
        if (pc == null) return null;
        else
        {
            playerController = pc;
            pc.Health = 100;
            return playerController;
        }
    }

    //public void SaveCurrentPlayerData()
    //{
    //    var pl = GetPlayer();
    //    if (pl == null)
    //        Debug.LogError("Player was null, could not save data!");
    //    else
    //        Save.SavePlayer(pl.GetPlayerData());
    //}

    public void SensoryOverload(bool activated = false)
    {
        var l2d = Camera.main.GetComponentInChildren<Light2D>();
        if (activated)
        {
            if (l2d != null)
            {
                //intensityOrig = l2d.intensity;
                l2d.intensity = 0.2f;
                l2d.color = new Color(R, G, B, A);
                //l2d.color = new Color()
            }
        }
        else
        {
            //deactivated
            if (l2d != null)
            {
                l2d.intensity = 0.55f;
                l2d.color = Level1Color;
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
