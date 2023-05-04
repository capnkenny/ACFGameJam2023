using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Room constants
    
    #endregion Room constants
    
    #region Defaults

    private const int DEFAULT_GRID_SIZE = 8;
    private const int DEFAULT_CARDINAL_WEIGHT = 10;
    private const int DEFAULT_DIAGONAL_WEIGHT = 14;

    private const float DEFAULT_GENERATION_SEED = 2.6f;

    #endregion Defaults

    #region Serialized Fields

    [SerializeField]
    private GameObject _roomPrefab;

    [SerializeField]
    private GameObject _debugPrefab;

    [SerializeField]
    private int _levelNumber;

    //[SerializeField]
    private float _levelGenerationSeed;

    //[SerializeField]
    private int _gridWidth;

    //[SerializeField]
    private int _gridHeight;

    [SerializeField]
    private float _yDistanceForPrefab;
    //113.68

    [SerializeField]
    private float _xDistanceForPrefab;
    //135.41

    [SerializeField]
    private bool _debugEnabled;

    [SerializeField]
    private float _yDistanceForCam;
    //113.64

    [SerializeField]
    private float _xDistanceForCam;
    //135.32

    #endregion Serialized Fields

    #region Private members

    private int _numberOfRooms;

    private List<List<RoomData>> _grid;

    private GameObject debug;

    private GameObject RoomParent;

    private List<RoomData> _extraRooms;

    private bool disabledDebug = false;

    private int _numberOfEnemies;

    #endregion Private members

    public GameObject Camera;

    public Vector2Int StartingRoom;

    public Vector2Int EndRoom;

    public List<RoomData> ValidPath;

    public bool FinishedLoading = false;

    private void Update()
	{
        if (FinishedLoading)
        {
            if (!disabledDebug)
            {
                var debugObject = GameObject.Find("DebugLayer");
                if (debugObject != null)
                {
                    debugObject.SetActive(_debugEnabled);
                    disabledDebug = true;
                }
            }
            
            //DrawLine();
        }
            
	}

	void Start()
    {
        if(_levelNumber == 0)
            _levelNumber++;
        _levelGenerationSeed = UnityEngine.Random.Range(5.5f, 10.0f);

        _extraRooms = new List<RoomData>();

        debug = new GameObject("DebugLayer");
        RoomParent = new GameObject("Rooms");

		//Initialize Grid
		
		StringBuilder sb = new StringBuilder();

		sb.AppendLine("Initial Values");
		sb.AppendFormat("Level: {0}", _levelNumber).AppendLine();
		sb.AppendFormat("Seed: {0}", _levelGenerationSeed).AppendLine();

		float n = (float)_levelNumber / 2.0f;
        float u = _levelGenerationSeed * n;
        int m = (int)(u + 8);

		sb.AppendFormat("N: {0}, U: {1}, M: {2}", n,u,m).AppendLine();

        _gridHeight = _gridWidth = m;
        _numberOfRooms = (m - 2);
        _numberOfEnemies = (int)(_numberOfRooms * 1.5);
		//Debug Output
        
		sb.AppendFormat("Number of Rooms: {0}", _numberOfRooms).AppendLine();
		sb.AppendFormat("Grid Size: {0}, {1}", _gridWidth, _gridHeight).AppendLine();

		Debug.Log(sb.ToString());
		sb.Clear();
		InitializeGrid();

		sb.AppendFormat("Start Room: {0}, {1}", StartingRoom.x, StartingRoom.y).AppendLine();
		sb.AppendFormat("End Room: {0}, {1}", EndRoom.x, EndRoom.y).AppendLine();
		Debug.Log(sb.ToString());
        sb.Clear();

		
		DebugLayers();

    	//Determine Neighbors
	    var found = Search();

        //Find valid path
		ValidPath = FindPathToEnd();

        //Generate the mazzeeeeeeeee
		GenerateLevel();
        
		//SetupDoors();

		FinishedLoading = true;
	}

    private void InitializeGrid()
    {
        ValidPath = new List<RoomData>();
        _grid = new List<List<RoomData>>();
        int roomNumber = 1;
        int sX = 0;
        int sY = 0;
        for(int i = 0; i < _gridHeight; i++)
        {
            sX = 0;
            _grid.Add(new List<RoomData>());
            for(int j = 0; j < _gridWidth; j++)
            {
                _grid[i].Add(new RoomData
                    { 
                        X = sX, 
                        Y = sY, 
                        RoomNumber = roomNumber, 
                        Enabled = true, 
                        Spawn = false, 
                        End = false, 
                        Bonus = false,
                        Parent = null
                    }
                );
                roomNumber++;
                sX++;
            }
            sY++;
        }

        //Get starting room
        int x = UnityEngine.Random.Range(0, _gridWidth);
        int y = UnityEngine.Random.Range(0, _gridHeight);
		StartingRoom = new Vector2Int(x, y);
		_grid[y][x].Spawn = true;        

        //Get End room
        int eX = UnityEngine.Random.Range(0, _gridWidth);
        int eY = UnityEngine.Random.Range(0, _gridHeight);
        //Make sure its not the same as the starting room
        while(eX == x && eY == y || HeuristicCost(x,y,eX,eY) < (_numberOfRooms * DEFAULT_CARDINAL_WEIGHT))
        {
            eX = UnityEngine.Random.Range(0, _gridWidth);
            eY = UnityEngine.Random.Range(0, _gridHeight);
        }
        _grid[eY][eX].End = true;
        EndRoom = new Vector2Int(eX, eY);

        foreach (var arr in _grid)
            foreach (var room in arr)
            {
                room.GivenCost = DetermineGivenCost(_grid[room.Y][room.X]);
				room.HeuristicCost = DetermineHeuristicCost(_grid[room.Y][room.X]);
			}
                

		//Disable a random bunch of rooms
		int toDisable = UnityEngine.Random.Range(2, 8);
        for(int i = 0; i < toDisable; i++)
        {
            int dX = UnityEngine.Random.Range(0, _gridWidth);
            int dY = UnityEngine.Random.Range(0, _gridHeight);

            bool matchesEnd = (dX == eX) && (dY == eY);
            bool matchesSpawn = (dX == x) && (dY == y);
            while(matchesEnd || matchesSpawn)
            {
                dX = UnityEngine.Random.Range(0, _gridWidth);
                dY = UnityEngine.Random.Range(0, _gridHeight);
                matchesEnd = (dX == eX) && (dY == eY);
                matchesSpawn = (dX == x) && (dY == y);
            }

            _grid[dY][dX].Enabled = false;
            Debug.LogFormat("Disabled Room {0}", _grid[dY][dX].RoomNumber);
        }
    }

    private bool Search()
    {
        //Create an open list to check, and a closed list of rooms
        // we don't need to check anymore.
        var openList = new List<RoomData>();
        var closedList = new List<RoomData>();

        //Add the starting room to the list
        openList.Add(_grid[StartingRoom.y][StartingRoom.x]);

		//Recursion begins
		while (openList.Count > 0 && !openList.Any(r => r.RoomNumber == _grid[EndRoom.y][EndRoom.x].RoomNumber))
        {
            UpdateDebugLayers();
            //Get lowest F cost
            var room = openList.OrderBy(r => r.FullCost).First();
            //Debug.LogFormat("Room {0}, Cost: {1}", room.RoomNumber, room.FullCost);
            
            //Remove lowest cost and move to closed list
            openList.Remove(room);
            closedList.Add(room);

			//Check da naybuhs
			DetermineNeighbors(room, ref openList, ref closedList);
            
            if(closedList.Any(r => r.RoomNumber == _grid[EndRoom.y][EndRoom.x].RoomNumber))
            {
                Debug.Log("Completed search!");
                return true;
            }
        }

        return false;
    }
    
    private void DetermineNeighbors(RoomData room, ref List<RoomData> openList, ref List<RoomData> closed)
    {
        //Check if east neighbor is valid
        if(room.X + 1 < _gridWidth)
        {
            //Check if east neighbor is walkable
            if(_grid[room.Y][room.X + 1].Enabled && !closed.Any(r => r.RoomNumber == _grid[room.Y][room.X + 1].RoomNumber))
            {
                if(!openList.Any(r => r.RoomNumber == _grid[room.Y][room.X + 1].RoomNumber))
                {
                    _grid[room.Y][room.X + 1].Parent = _grid[room.Y][room.X];
                    _grid[room.Y][room.X + 1].GivenCost = DetermineGivenCost(_grid[room.Y][room.X + 1]);
                    _grid[room.Y][room.X + 1].HeuristicCost = DetermineHeuristicCost(_grid[room.Y][room.X + 1]);
                    openList.Add(_grid[room.Y][room.X + 1]);
                }
                else
                {
                    var foundRoom = openList.First(r => r.RoomNumber == _grid[room.Y][room.X + 1].RoomNumber);
                    int newCost = DetermineGivenCost(_grid[room.Y][room.X]) + 10;
                    if(newCost < foundRoom.GivenCost)
                    {
                        foundRoom.Parent = _grid[room.Y][room.X];
                        foundRoom.GivenCost = newCost;
                    }
                }
                
            }
        }   
        //North neighbor
        if(room.Y + 1 < _gridHeight)
        {
            if(_grid[room.Y + 1][room.X].Enabled && !closed.Any(r => r.RoomNumber == _grid[room.Y + 1][room.X].RoomNumber))
            {
                if(!openList.Any(r => r.RoomNumber == _grid[room.Y + 1][room.X].RoomNumber))
                {
                    _grid[room.Y + 1][room.X].Parent = _grid[room.Y][room.X];
                    _grid[room.Y + 1][room.X].GivenCost = DetermineGivenCost(_grid[room.Y + 1][room.X]);
                    _grid[room.Y + 1][room.X].HeuristicCost = DetermineHeuristicCost(_grid[room.Y + 1][room.X]);
                    openList.Add(_grid[room.Y + 1][room.X]);
				}
                else
                {
                    var foundRoom = openList.First(r => r.RoomNumber == _grid[room.Y + 1][room.X].RoomNumber);
                    int newCost = DetermineGivenCost(_grid[room.Y][room.X]) + 10;
                    if(newCost < foundRoom.GivenCost)
                    {
                        foundRoom.Parent = _grid[room.Y][room.X];
                        foundRoom.GivenCost = newCost;
                    }
                }
            }
        }
        //West neighbor
        if(room.X - 1 >= 0)
        {
            if(_grid[room.Y][room.X - 1].Enabled && !closed.Any(r => r.RoomNumber == _grid[room.Y][room.X - 1].RoomNumber))
            {
                if(!openList.Any(r => r.RoomNumber == _grid[room.Y][room.X - 1].RoomNumber))
                {
                    _grid[room.Y][room.X - 1].Parent = _grid[room.Y][room.X];
                    _grid[room.Y][room.X - 1].GivenCost = DetermineGivenCost(_grid[room.Y][room.X - 1]);
                    _grid[room.Y][room.X - 1].HeuristicCost = DetermineHeuristicCost(_grid[room.Y][room.X - 1]);
                    openList.Add(_grid[room.Y][room.X - 1]);
				}
                else
                {
                    var foundRoom = openList.First(r => r.RoomNumber == _grid[room.Y][room.X - 1].RoomNumber);
                    int newCost = DetermineGivenCost(_grid[room.Y][room.X]) + 10;
                    if(newCost < foundRoom.GivenCost)
                    {
                        foundRoom.Parent = _grid[room.Y][room.X];
                        foundRoom.GivenCost = newCost;
                    }
                }
            }
        }
        //South Neighbor   
        if(room.Y - 1 >= 0)
        {
            if(_grid[room.Y - 1][room.X].Enabled && !closed.Any(r => r.RoomNumber == _grid[room.Y - 1][room.X].RoomNumber))
            {
                if(!openList.Any(r => r.RoomNumber == _grid[room.Y - 1][room.X].RoomNumber))
                {
                    _grid[room.Y - 1][room.X].Parent = _grid[room.Y][room.X];
                    _grid[room.Y - 1][room.X].GivenCost = DetermineGivenCost(_grid[room.Y - 1][room.X]);
                    _grid[room.Y - 1][room.X].HeuristicCost = DetermineHeuristicCost(_grid[room.Y - 1][room.X]);
                    openList.Add(_grid[room.Y - 1][room.X]);
				}
                else
                {
                    var foundRoom = openList.First(r => r.RoomNumber == _grid[room.Y - 1][room.X].RoomNumber);
                    int newCost = DetermineGivenCost(_grid[room.Y][room.X]) + 10;
                    if(newCost < foundRoom.GivenCost)
                    {
                        foundRoom.Parent = _grid[room.Y][room.X];
                        foundRoom.GivenCost = newCost;
                    }
                }
            }
        }
    }

    private int DetermineGivenCost(RoomData target)
    {
        int xDiff = Math.Abs(StartingRoom.x - target.X);
        int yDiff = Math.Abs(StartingRoom.y - target.Y);
        int diff = Math.Abs(xDiff - yDiff);

        if(diff == 0)
            return 0;
        if(xDiff == 0 || yDiff == 0)
            return ((xDiff + yDiff) * DEFAULT_CARDINAL_WEIGHT);
        
        //Diagonal
        if(xDiff == yDiff)
            return xDiff * DEFAULT_DIAGONAL_WEIGHT;
        else if(xDiff > yDiff)
            return (yDiff * DEFAULT_DIAGONAL_WEIGHT) + (diff * DEFAULT_CARDINAL_WEIGHT);
        else
            return (xDiff * DEFAULT_DIAGONAL_WEIGHT) + (diff * DEFAULT_CARDINAL_WEIGHT);
    }

    private int DetermineHeuristicCost(RoomData start)
    {
        //Manhattan-based estimation says to ignore all obstacles, and count
        // both number of vertical spaces plus horizontal spaces to get from
        // start to finish, like if you were counting city blocks (a la Manhattan, NY).
        int x = Math.Abs(start.X - EndRoom.x);
        int y = Math.Abs(start.Y - EndRoom.y);

        if (x == 0 && y == 0)
            return 0;
        
        return (x+y) * DEFAULT_CARDINAL_WEIGHT;
    }

    private int HeuristicCost(int x, int y, int ex, int ey)
    {
        int dx = Math.Abs(x - ex);
        int dy = Math.Abs(y - ey);

        return (dx + dy) * DEFAULT_CARDINAL_WEIGHT;
    }

    private List<RoomData> FindPathToEnd()
    {
        List<RoomData> path = new List<RoomData>();
        RoomData currentRoom = _grid[EndRoom.y][EndRoom.x];
        while(currentRoom != null)
        {
            path.Add(_grid[currentRoom.Y][currentRoom.X]);
            currentRoom = currentRoom.Parent;
        }
        
        return path;
    }

    private void GenerateLevel()
    {
        foreach (var column in _grid)
        {
            foreach (var room in column)
            {
                if (!ValidPath.Any(a => a.RoomNumber == room.RoomNumber) && !_extraRooms.Any(a => a.RoomNumber == room.RoomNumber))
                    _grid[room.Y][room.X].Enabled = false;
                else
                {
                    CreateRoomPrefab(room);
                }
            }
        }
        
        SetDoors();
        AssignEnemyCounts();
        SnapCamera(_grid[StartingRoom.y][StartingRoom.x]);
    }

    private void CreateRoomPrefab(RoomData data)
    {
        foreach (var n in data.Neighbors)
            Debug.Log($"{data.RoomNumber} neighbor: {n.RoomNumber}");

        var gameObject = Instantiate(_roomPrefab, new Vector2(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab), Quaternion.identity, RoomParent.transform);
        gameObject.name = $"Room{data.RoomNumber}";
        var comp = gameObject.GetComponent<Room>();
        if(comp == null)
            throw new NullReferenceException("An error occurred when creating the prefab - could not retrieve component!");
        comp.RoomNumber = data.RoomNumber;
        comp.X = data.X;
        comp.Y = data.Y;
    }

    private List<RoomData> DetermineNeighborsWithoutPathfinding(RoomData room, bool enabled = true)
    {
        List<RoomData> neighbors = new List<RoomData>();
        var center = _grid[room.Y][room.X];
        //Debug.Log($"Checking neighbors for room {center.RoomNumber} at {center.X}, {center.Y}");
        if (room.Y == 0)
        {
            //Check north
            if (_grid[room.Y + 1][room.X].Enabled == enabled)
                neighbors.Add(_grid[room.Y + 1][room.X]);

            if (room.X == 0)
            {
                //Never check west when x = 0
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
            else if (room.X == _gridWidth-1)
            {
                //Never check east if x == grid width
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
            }
            else
            {
                //Check east and west
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
        }
        else if (room.Y == _gridHeight-1)
        {
            //Check south
            if (_grid[room.Y - 1][room.X].Enabled == enabled)
                neighbors.Add(_grid[room.Y - 1][room.X]);

            if (room.X == 0)
            {
                //Never check west when x = 0
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
            else if (room.X == _gridWidth-1)
            {
                //Never check east if x == grid width
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
            }
            else
            {
                //Check east and west
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
        }
        else
        {
            //Check north
            if (_grid[room.Y + 1][room.X].Enabled == enabled)
                neighbors.Add(_grid[room.Y + 1][room.X]);

            //Check south
            if (_grid[room.Y - 1][room.X].Enabled == enabled)
                neighbors.Add(_grid[room.Y - 1][room.X]);

            if (room.X == 0)
            {
                //Never check west when x = 0
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
            else if (room.X == _gridWidth-1)
            {
                //Never check east if x == grid width
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
            }
            else
            {
                //Check east and west
                if (_grid[room.Y][room.X - 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X - 1]);
                if (_grid[room.Y][room.X + 1].Enabled == enabled)
                    neighbors.Add(_grid[room.Y][room.X + 1]);
            }
        }

        return neighbors;
    }


    private void SetDoors()
    {
        for (int i = 0; i < ValidPath.Count; i++)
        {
            var room = ValidPath[i];
            room.Neighbors = DetermineNeighborsWithoutPathfinding(room);
            var roomObject = GameObject.Find($"Room{room.RoomNumber}");
            foreach (var neighbor in room.Neighbors)
            {
                int x = room.X - neighbor.X;
                int y = room.Y - neighbor.Y;
                if (y == 0)
                {
                    if (x > 0)
                    {
                        //Debug.Log($"Unlocking room {room.RoomNumber} - west door");
                        roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>().DoorEnabled = true;
                        roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>().DoorOpened = true;
                        roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>().DoorPoint.SetDelegate(SnapCamera, neighbor, true, Direction.WEST);
                    }
                    else
                    {
                        //Debug.Log($"Unlocking room {room.RoomNumber} - east door");
                        roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>().DoorEnabled = true;
                        roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>().DoorOpened = true;
						roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>().DoorPoint.SetDelegate(SnapCamera, neighbor, true, Direction.EAST);
					}
                }
                else
                {
                    if (y < 0)
                    {
                        //Debug.Log($"Unlocking room {room.RoomNumber} - north door");
                        roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>().DoorEnabled = true;
                        roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>().DoorOpened = true;
						roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>().DoorPoint.SetDelegate(SnapCamera, neighbor, true, Direction.NORTH);
					}
                    else
                    {
                        //Debug.Log($"Unlocking room {room.RoomNumber} - south door");
                        roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>().DoorEnabled = true;
                        roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>().DoorOpened = true;
						roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>().DoorPoint.SetDelegate(SnapCamera, neighbor, true, Direction.SOUTH);
					}
                }

            }

            if (room.Spawn)
            {
                roomObject.GetComponent<Room>().EnemyCount = 0;
                roomObject.GetComponent<Room>().SpawnRoom = true;
            }

            if (room.End)
                roomObject.GetComponent<Room>().EndRoom = true;
        }
        
    }

    private void UpdateDebugLayers()
    {
        foreach (var arr in _grid)
        {
            foreach (var data in arr)
            {
                var parent = transform.Find("DebugLayer");
                if (parent != null)
                {
                    var gameObject = parent.Find($"Debug{data.RoomNumber}");
                    gameObject.transform.Find("RoomNumber").GetComponent<TextMeshPro>().text = $"Room {data.RoomNumber}";
                    gameObject.transform.Find("G").GetComponent<TextMeshPro>().text = $"{data.GivenCost}";
                    gameObject.transform.Find("H").GetComponent<TextMeshPro>().text = $"{data.HeuristicCost}";
                    gameObject.transform.Find("F").GetComponent<TextMeshPro>().text = $"{data.FullCost}";
                }
            }
        }
        
	}

    private void DebugLayers()
    {
        debug.tag = "DebugLayer";
        foreach (var arr in _grid)
        {
            foreach (var data in arr)
            {
				var gameObject = Instantiate(_debugPrefab, new Vector2(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab), Quaternion.identity, debug.transform);
				gameObject.name = $"Debug{data.RoomNumber}";
                gameObject.transform.Find("RoomNumber").GetComponent<TextMeshPro>().text = $"Room {data.RoomNumber}";
				gameObject.transform.Find("G").GetComponent<TextMeshPro>().text = $"{data.GivenCost}";
				gameObject.transform.Find("H").GetComponent<TextMeshPro>().text = $"{data.HeuristicCost}";
				gameObject.transform.Find("F").GetComponent<TextMeshPro>().text = $"{data.FullCost}";
				if (data.Spawn)
				{
					var floor = gameObject.transform.Find("DebugSquare");
					var render = floor.GetComponent<SpriteRenderer>();
					render.color = Color.green;
				}
				if (data.End)
				{
					var floor = gameObject.transform.Find("DebugSquare");
					var render = floor.GetComponent<SpriteRenderer>();
					render.color = Color.red;
				}
				if (data.Bonus)
				{
					var floor = gameObject.transform.Find("DebugSquare");
					var render = floor.GetComponent<SpriteRenderer>();
					render.color = Color.yellow;
				}
				if (!data.Enabled)
				{
					var floor = gameObject.transform.Find("DebugSquare");
					var render = floor.GetComponent<SpriteRenderer>();
					render.color = Color.black;
				}
			}    
        }
		
	}

    private void DrawLine()
    {
        if (_debugEnabled)
        {
            var gameObject = GameObject.FindGameObjectWithTag("DebugLayer");
            foreach (var arr in _grid)
            {
                foreach (var data in arr)
                {
                    if (gameObject != null)
                    {
                        var go = gameObject.transform.Find($"Debug{data.RoomNumber}");
                        if (go != null && !ValidPath.Any(v => v.RoomNumber == data.RoomNumber))
                            go.gameObject.SetActive(false);
                    }

                    if (data.Parent != null && ValidPath.Any(r => r.RoomNumber == data.RoomNumber))
                        Debug.DrawLine(new Vector3(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab, 0), new Vector3(data.Parent.X * _xDistanceForPrefab, data.Parent.Y * _yDistanceForPrefab, -3), Color.blue);

                }
            }

            foreach (var room in ValidPath)
            {
                var go = gameObject.transform.Find($"Debug{room.RoomNumber}");
                if (go != null)
                    go.gameObject.SetActive(true);
            }
        }
	}

    private void SnapCamera(RoomData room, bool moveCharacter = false, Direction dir = Direction.NORTH)
    {
        //Debug.Log($"Snapping to room {room.RoomNumber}");
        Camera.transform.position = new Vector3(room.X * _xDistanceForCam, room.Y * _yDistanceForCam, Camera.transform.position.z);

        if (moveCharacter)
        {
			var roomObject = GameObject.Find($"Room{room.RoomNumber}");
            if(roomObject != null)
            {
                Vector3 pos;

                switch (dir)
                {
                    case Direction.WEST:
                        {
                            pos = roomObject.GetComponent<Room>().EastDoor.GetComponent<Door>().SnapPoint.transform.position;
							break;
                        }
					case Direction.SOUTH:
						{
							pos = roomObject.GetComponent<Room>().NorthDoor.GetComponent<Door>().SnapPoint.transform.position;
							break;
						}
					case Direction.EAST:
						{
							pos = roomObject.GetComponent<Room>().WestDoor.GetComponent<Door>().SnapPoint.transform.position;
							break;
						}
					case Direction.NORTH:
                    default:
						{
							pos = roomObject.GetComponent<Room>().SouthDoor.GetComponent<Door>().SnapPoint.transform.position;
							break;
						}
				}
               
                //Debug.Log($"Door position: {pos}");
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.SetPositionAndRotation(pos, Quaternion.identity);
                }
            }
		}
        var mgr = GameObject.FindGameObjectWithTag("LvlMgr");
        if (mgr != null)
        {
            mgr.GetComponent<LevelManager>().CurrentRoomNumber = room.RoomNumber;
        }
    }

    private void AssignEnemyCounts()
    {
        int counter = _numberOfEnemies;
        while (counter > 0)
        {
            foreach (var room in ValidPath)
            {
                if (counter <= 0)
                {
                    return;
                }
                var rc = GameObject.Find($"Room{room.RoomNumber}").GetComponent<Room>();
                if (room.Spawn)
                    continue;

                if (rc.EnemyCount == 0)
                {
                    rc.EnemyCount++;
                    counter--;
                }
                else
                {
                    int r = UnityEngine.Random.Range(1, 6);
                    if (r % 2 == 0)
                    {
                        rc.EnemyCount++;
                        counter--;
                    }
                }
            }
        }
    }

    public void CompleteLevel(PlayerData data)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var gmo = GameObject.FindGameObjectWithTag("GameManager");
        if (gmo != null)
        {
            var gm = gmo.GetComponent<GameManager>();
            var pc = player.GetComponent<PlayerController>();
            gm.playerData = pc.UpdatePlayerData(data);
        }

        LoadToLevel(4); //fix later maybe ¯\_(ツ)_/¯
    }

    private void LoadToLevel(int level)
    {
        var lvlload = GameObject.Find("LevelLoader").GetComponent<LevelLoad>();
        lvlload.transition.SetTrigger("Loading");
        lvlload.LoadLevel(level); 
    }
}