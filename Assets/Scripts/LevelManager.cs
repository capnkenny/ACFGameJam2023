using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private LevelLoad _levelLoader;

    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private GameObject _gameManagerPrefab;

    private GameManager _mgr;

    private bool instantiatedGM = false;
    private bool loaded = false;
    private void Awake()
    {
        _levelLoader.transition.SetTrigger("Loading");
        //Get the game manager
        var mg = GameObject.FindGameObjectWithTag("GameManager");
        if (mg == null)
        {
            _mgr = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();
            _mgr.Loader = _levelLoader;
            instantiatedGM = true;

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
            Debug.Log("Game Manager loaded for level!");


            SpawnPlayer();
            
            
            loaded = true;

            if (instantiatedGM)
                _levelLoader.transition.SetTrigger("DoneLoad");
        }
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab);
        var pos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        pos.z = 0;
        pos.y -= 2.5f;
        player.transform.position = pos;
    }

}
