using UnityEngine;

public class HubManager : MonoBehaviour
{
    [SerializeField]
    private Travelpoint _levelDoor;

    [SerializeField]
    private Door _shopKeepDoor;

    [SerializeField]
    private LevelLoad _levelLoader;

    [SerializeField]
    private GameObject _gameManagerPrefab;

    [SerializeField]
    private MusicPlayer _music;

    private GameManager _mgr;

    private bool loaded = false;
    private bool sceneRequested = false;
    private bool musicRequested = false;

    private void Awake()
    {
        //Get the game manager
        var mg = GameObject.FindGameObjectWithTag("GameManager");
        if (mg == null)
        {
            _mgr = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();
            _mgr.Loader = _levelLoader;

        }
        else
        {
            _mgr = mg.GetComponent<GameManager>();
        }
    }

    private void Update()
    {
        if (_mgr.Loaded && !loaded)
        {
            Debug.Log("GameManager loaded!");

            int level = _mgr.playerData.Level;
            //Debug.LogFormat("Player Level: {0}", level);
            _levelDoor.SetDelegate(LoadToLevel, level);


            loaded = true;
            _levelLoader.transition.SetTrigger("DoneLoad");
            _mgr.state = GameState.HUB;
        }

        if (loaded && !_music.IsPlaying && !musicRequested)
        {
            if (_levelLoader.transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                musicRequested = true;
                _music.PlayMusic();
            }

        }

    }

    void LoadToLevel(int level)
    {
        if (!sceneRequested)
        {
            _levelLoader.transition.SetTrigger("Loading");
            _mgr.TransitionToLevel(level);
            sceneRequested = true;
        }
        
    }

}
