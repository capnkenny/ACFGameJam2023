using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField]
    private LevelLoad _loader;

	private void Awake()
	{
		_loader.SetLoading();
	}

	void Start()
    {
		
	}

}
