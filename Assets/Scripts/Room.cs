using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    public int RoomNumber;

    public int X;

    public int Y;

    public int EnemyCount;

    public TextMeshPro Text;

    public bool SpawnRoom;

    public bool EndRoom;

    void Awake()
    {
       
    }

    void Start()
    {
        Text.text = $"Room # {RoomNumber}";
    }

    void Update()
    {

    }

}
