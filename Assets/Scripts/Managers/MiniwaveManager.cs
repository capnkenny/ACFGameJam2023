using System.Collections.Generic;
using UnityEngine;

public class MiniwaveManager : MonoBehaviour
{
	public static MiniwaveManager instance;

	private List<GameObject> waves;

	[SerializeField]
	private GameObject pooledMiniWave;
	[SerializeField]
	private int numberOfWaves;
	[SerializeField]
	private int waveCount;
	[SerializeField]
	private float angleStart;
	[SerializeField]
	private float angleEnd;
	[SerializeField]
	private float delay;
	[SerializeField]
	private GameObject _spawnLocation;
	

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		waves = new List<GameObject>();
	}

	public GameObject GetMiniwave()
	{
		//if (waves.Count > 0)
		//{
		//	foreach (var wave in waves)
		//	{
		//		if (wave != null && !wave.activeInHierarchy)
		//			return wave;
		//	}
		//}

		//if (waves.Count < numberOfWaves * waveCount)
		//{
			GameObject wave = Instantiate(pooledMiniWave);
			wave.SetActive(false);
			//waves.Add(wave);
			return wave;
		//}

		//return null;
	}

	public void FireWave()
	{
		float step = (angleEnd - angleStart) / numberOfWaves;
		float angle = angleEnd;

		for (int i = 0; i < numberOfWaves; i++)
		{
			float x = _spawnLocation.transform.position.x + Mathf.Cos((angle * Mathf.PI) / 180.0f);
			float y = _spawnLocation.transform.position.y + Mathf.Sin((angle * Mathf.PI) / 180.0f);
			Vector3 vector = new Vector3(x, y, 0f);
			Vector2 dir = (vector - _spawnLocation.transform.position).normalized;

			GameObject wave = instance.GetMiniwave();
			if (wave != null)
			{
				wave.transform.position = _spawnLocation.transform.position;
				wave.transform.rotation = _spawnLocation.transform.rotation;
				//var rot = transform.rotation;
				//rot.z = angle;
				wave.transform.Rotate(0, 0, angle+90);
				
				//wave.transform.rotation = rot;
				wave.SetActive(true);
				wave.GetComponent<MiniWave>().SetDirection(dir);
			}
			angle += step;
		}
	}

}
