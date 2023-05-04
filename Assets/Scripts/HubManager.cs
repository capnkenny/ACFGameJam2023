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

    private GameManager _mgr;

    private bool loaded = false;

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
            _levelDoor.SetDelegate(LoadToLevel, level);


            loaded = true;
            _levelLoader.transition.SetTrigger("DoneLoad");
        }

    }

    void LoadToLevel(int level)
    {
        _levelLoader.transition.SetTrigger("Loading");
        _mgr.TransitionToLevel(level);
    }

}
