using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    [SerializeField]
    private LevelLoad _loader;

	private void Awake()
	{
		_loader.transition.SetTrigger("Loading");
	}

	void Start()
    {
		
	}

}
