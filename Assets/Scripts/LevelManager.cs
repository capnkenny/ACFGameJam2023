using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<EnemyIndex> AllowedEnemies;

    [SerializeField]
    private List<GameObject> EnemyPrefabs;

    [SerializeField]
    private LevelLoad _levelLoader;

    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private GameObject _gameManagerPrefab;

    [SerializeField]
    private LevelGenerator _levelGen;

    private GameManager _mgr;

    private bool instantiatedGM = false;
    private bool loaded = false;

    public int CurrentRoomNumber;
    public int PreviousRoomNumber;

    private int _currentEnemyCount;
    private int _currentRoomEnemiesDead = 0;
    private bool _endRoomSetup = false;

    private void Awake()
    {
        _levelLoader.transition.SetTrigger("Loading");
        //Get the game manager
        var mg = GameObject.FindGameObjectWithTag("GameManager");
        if (mg == null)
        {
            _mgr = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();
            _mgr.Loader = _levelLoader;
            instantiatedGM = true;

        }
        else
        {
            _mgr = mg.GetComponent<GameManager>();
        }
        CurrentRoomNumber = 0;
        PreviousRoomNumber = 0;
    }

    private void Start()
    {
        //foreach (var r in _levelGen.ValidPath)
        //{
        //    var room = GameObject.Find($"Room{r.RoomNumber}");
        //    room.GetComponent<Room>().LockAllDoors(false);
        //}
    }

    private void Update()
    {
        if (_mgr.state == GameState.PLAYING)
        {
            // Make sure GameManager is loaded too
            if (_mgr.Loaded && !loaded)
            {
                Debug.Log("Game Manager loaded for level!");


                SpawnPlayer();


                loaded = true;

                if (instantiatedGM)
                    _levelLoader.transition.SetTrigger("DoneLoad");
            }

            // We switched rooms so spawn enemies here
            // and lock the doors
            if (CurrentRoomNumber != PreviousRoomNumber)
            {
                PreviousRoomNumber = CurrentRoomNumber;
                _currentRoomEnemiesDead = 0;
                var room = GameObject.Find($"Room{CurrentRoomNumber}");
                if (room != null)
                {
                    var roomObject = room.GetComponent<Room>();
                    Debug.Log($"Switched to room {CurrentRoomNumber} - {roomObject.EnemyCount} enemies");
                    if (roomObject.EndRoom && !_endRoomSetup)
                    {
                        List<Door> disabledDoors = new List<Door>();
                        //find the exit door
                        if (!roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>().DoorEnabled)
                            disabledDoors.Add(roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>());
                        if (!roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>().DoorEnabled)
                            disabledDoors.Add(roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>());
                        if (!roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>().DoorEnabled)
                            disabledDoors.Add(roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>());
                        if (!roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>().DoorEnabled)
                            disabledDoors.Add(roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>());

                        int doorIdx = UnityEngine.Random.Range(0, disabledDoors.Count);
                        var p = GameObject.FindGameObjectWithTag("Player");
                        if (p != null)
                        {
                            var pl = p.GetComponent<PlayerData>();
                            disabledDoors[doorIdx].DoorEnabled = true;
                            disabledDoors[doorIdx].DoorOpened = true;
                            disabledDoors[doorIdx].DoorPoint.SetDelegate(_levelGen.CompleteLevel, pl);
                        }
                        _endRoomSetup = true;
                    }

                    if (roomObject.EnemyCount > 0 && roomObject.PlayerInside)
                    {
                        Debug.Log($"Enemy Count is {roomObject.EnemyCount} - locking room {CurrentRoomNumber}'s doors");
                        _currentEnemyCount = roomObject.EnemyCount;
                        roomObject.LockAllDoors(true);
                        SpawnEnemies(roomObject);
                    }
                }

            }
            else
            {
                var roomObject = GameObject.Find($"Room{CurrentRoomNumber}");
                Room room = null;
                if (roomObject != null) room = roomObject.GetComponent<Room>();
                if (room != null && (_currentEnemyCount == 0 || room.EnemyCount == 0) && room.SpawnRoom)
                {
                    room.LockAllDoors(false);
                }
                else
                {
                    if (room != null)
                    {
                        var rc = room.GetComponent<Room>();
                        if (room.SpawnRoom)
                        {
                            room.LockAllDoors(false);
                        }
                        if (rc.EnemyCount > 0 && rc.PlayerInside)
                        {
                            //Debug.Log($"Enemy Count is {rc.EnemyCount} - locking room {CurrentRoomNumber}'s doors");
                            _currentEnemyCount = rc.EnemyCount;
                            rc.LockAllDoors(true);
                            SpawnEnemies(rc);
                        }
                        if (_currentRoomEnemiesDead == _currentEnemyCount)
                        {
                            rc.LockAllDoors(false);
                        }
                    }
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab);
        var pos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        pos.z = 0;
        pos.y -= 2.5f;
        player.transform.position = pos;
    }

    private void SpawnEnemies(Room room)
    {
        if (room.CurrentlySpawnedEnemies == 0)
        {
            for (int i = 0; i < room.EnemyCount; i++)
            {
                //bool spawned = false;
                int y = Random.Range(1, 5);
                int x = Random.Range(1, 4);
                //while (!spawned)
                //{
                    EnemySpawnRow esr = null;
                    switch (y)
                    {
                        case 1:
                            {
                                esr = room.EnemyRow1;
                                break;
                            }
                        case 2:
                            {
                                esr = room.EnemyRow2;
                                break;
                            }
                        case 3:
                            {
                                esr = room.EnemyRow3;
                                break;
                            }
                        case 4:
                            {
                                esr = room.EnemyRow4;
                                break;
                            }
                    }

                    EnemySpawner es = null;

                    if (esr != null)
                    {
                        switch (x)
                        {
                            case 1:
                                {
                                    es = esr.SpawnPoint1;
                                    break;
                                }
                            case 2:
                                {
                                    es = esr.SpawnPoint2;
                                    break;
                                }
                            case 3:
                                {
                                    es = esr.SpawnPoint3;
                                    break;
                                }
                        }

                        if (es != null && !es.EnemySpawned)
                        {
                            int e = Random.Range(0, AllowedEnemies.Count);
                            es.SpawnEnemy(room, EnemyPrefabs[e]);
                            //spawned = true;
                        }
                    }
                //}
            }
        }
    }

    public void SignalEnemyDied() => _currentRoomEnemiesDead++;
}
