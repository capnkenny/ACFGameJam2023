using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int RoomNumber;
    public int X;
    public int Y;
    
    public int Cost;
    public int Distance;
    public int CostDistance => Cost = Distance;

    public Room Parent;
    public GameObject NorthDoor;
    public GameObject EastDoor;
    public GameObject SouthDoor;
    public GameObject WestDoor;

    public List<GameObject> enemies;
    public EnemySpawnRow EnemyRow1;
    public EnemySpawnRow EnemyRow2;
    public EnemySpawnRow EnemyRow3;
    public EnemySpawnRow EnemyRow4;

    public Collider2D RoomDetectionCollider;

    public int EnemyCount;
    public int CurrentlySpawnedEnemies = 0;
    public bool SpawnRoom;
    public bool EndRoom;

    private bool _allEnemiesDefeated = false;
    
    public bool PlayerInside { get { return RoomDetectionCollider.bounds.Contains(GameObject.FindGameObjectWithTag("Player").transform.position); } }

    public GameObject RoomPrefab;

    public void LockAllDoors(bool locked)
    {
        if (!SpawnRoom)
        {
            var nd = NorthDoor.GetComponent<Door>();
            var sd = SouthDoor.GetComponent<Door>();
            var ed = EastDoor.GetComponent<Door>();
            var wd = WestDoor.GetComponent<Door>();

            if (nd != null && nd.DoorEnabled && nd.DoorOpened != !locked)
            {
                //Debug.Log($"Call to check room {RoomNumber} - north door");
                nd.DoorOpened = !locked;
            }
            if (sd != null && sd.DoorEnabled && sd.DoorOpened != !locked)
            {
                //Debug.Log($"Call to check room {RoomNumber} - soutch door");
                sd.DoorOpened = !locked;
            }
            if (ed != null && ed.DoorEnabled && ed.DoorOpened != !locked)
            {
                //Debug.Log($"Call to check room {RoomNumber} - east door");
                ed.DoorOpened = !locked;
            }
            if (wd != null && wd.DoorEnabled && wd.DoorOpened != !locked)
            {
                //Debug.Log($"Call to check room {RoomNumber} - west door");
                wd.DoorOpened = !locked;
            }
        }
    }

    public bool EnemiesDefeated() => _allEnemiesDefeated;

    public void SignalEnemiesDefeated() => _allEnemiesDefeated = true;

}
