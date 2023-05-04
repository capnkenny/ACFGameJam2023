using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool EnemySpawned = false;

    public void SpawnEnemy(Room r, GameObject enemy)
    {
        if (enemy != null)
        {
            var localPos = transform.position;
            localPos.z = 0;
            
            Instantiate(enemy, localPos, Quaternion.identity, GetComponentInParent<Transform>());
            r.CurrentlySpawnedEnemies++;
            EnemySpawned = true;
        }
    }

}
