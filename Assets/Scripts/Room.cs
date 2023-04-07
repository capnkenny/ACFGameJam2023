using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    public int RoomNumber;
    public int X;
    public int Y;
    
    public int Cost;
    public int Distance;
    public int CostDistance => Cost = Distance;

    public Room Parent;
    
    
    public int EnemyCount;
    public bool SpawnRoom;
    public bool EndRoom;


    public GameObject RoomPrefab;

    void Awake()
    {
       
    }

    void Start()
    {
    }

    void Update()
    {
    }

}
