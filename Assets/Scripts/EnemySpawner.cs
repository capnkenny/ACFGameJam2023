using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool EnemySpawned = false;

    public void SpawnEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            Instantiate(enemy, transform);
            EnemySpawned = true;
        }
    }

}
