using UnityEngine;

public class TitleHandler : MonoBehaviour
{
	// Start is called before the first frame update

	public MusicPlayer player;

	public LevelLoad loader;

	public Animator animator;

	public GameObject title;
	public GameObject start;

	private float yStop = -0.65f;


	private bool requested = false;
	private void Start()
	{
		loader.LoadEndFunction = () => player.PlayMusic();
	}


	private void Update()
	{
		var anim = animator.GetCurrentAnimatorStateInfo(0);

		if (anim.IsName("LoadEnd"))
		{
			if (anim.normalizedTime > 0.75f && !requested)
			{
				loader.callAction = true;
				requested = true;
			}
		}

		if (requested)
		{
			if (title.transform.position.y > yStop)
			{
				var pos = title.transform.position;
				pos.y -= 2.0f * Time.deltaTime;
				title.transform.SetPositionAndRotation(pos, Quaternion.identity);
				pos = start.transform.position;
				pos.y -= 2.0f * Time.deltaTime;
				start.transform.SetPositionAndRotation(pos, Quaternion.identity);
			}
		}
		
	}

	public void OnClick()
	{
		Debug.Log("Clicked!");
		player.StopMusic();
		StartCoroutine(loader.LoadLevel(2));
	}


}
