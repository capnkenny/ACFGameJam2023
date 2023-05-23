using TMPro;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [SerializeField]
    private Travelpoint _levelDoor;

    [SerializeField]
    private Door _shopKeepDoor;

    [SerializeField]
    private Travelpoint _debugDoor;

    [SerializeField]
    private LevelLoad _levelLoader;

    [SerializeField]
    private GameObject _gameManagerPrefab;

    [SerializeField]
    private MusicPlayer _music;

    private GameManager _mgr;

    public TextPanel levelText;

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
            _debugDoor.SetDelegate(LoadToLevel, 2);

            loaded = true;
            //_levelLoader.SetDoneLoading();
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

        var pos = _mgr.GetPlayer().transform.position;
        if(pos != null) 
        {
            pos.z = levelText.collider.transform.position.z;
            if (levelText.collider.bounds.Contains(pos))
            {
                if (_mgr.playerData.Level == 2)
                {
					levelText.text.SetText($"Proceed to Level: ->\nThe Revenge of Airhorn Jim");
				}
                else
                    levelText.text.SetText($"Proceed to Level {_mgr.playerData.Level} ->");
                levelText.text.gameObject.SetActive(true);
            }
            else if (levelText.text.gameObject.activeSelf)
            {
                levelText.text.gameObject.SetActive(false);
            }
        }

        

    }

    void LoadToLevel(int level)
    {
        if (!sceneRequested)
        {
			if (_levelLoader == null)
			{
				var ll = GameObject.FindGameObjectWithTag("LvlLoader");
				if (ll != null)
				{
					_levelLoader = ll.GetComponent<LevelLoad>();
				}
			}

			if (_levelLoader != null)
			{
				_levelLoader.SetLoadingIfNot();
			}
            //_levelLoader.SetLoading();
            _mgr.TransitionToLevel(level);
            sceneRequested = true;
        }
        
    }

}
