using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

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
    //13.68

    [SerializeField]
    private float _xDistanceForPrefab;
    //35.41
    
    #endregion Serialized Fields

    #region Private members
    
    private int _numberOfRooms;

    private List<List<RoomData>> _grid;

    private Vector2Int _startingRoom;

    private Vector2Int _endRoom;

    private List<RoomData> _validPath;

    private GameObject debug;

    private GameObject RoomParent;

    private bool finished = false;

    private List<RoomData> _extraRooms;

	#endregion Private members

	private void Update()
	{
        if(finished)
            DrawLine();
	}

	void Awake()
    {
        //Validation
        //if(_gridHeight == 0)
        //    _gridHeight = DEFAULT_GRID_SIZE;
        //if(_gridWidth == 0)
        //    _gridWidth = DEFAULT_GRID_SIZE;
        if(_levelNumber == 0)
            _levelNumber++;
        //if(_levelGenerationSeed == 0.0f || _levelGenerationSeed > 10.0f)
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
		//Debug Output
		
        
		sb.AppendFormat("Number of Rooms: {0}", _numberOfRooms).AppendLine();
		sb.AppendFormat("Grid Size: {0}, {1}", _gridWidth, _gridHeight).AppendLine();

		Debug.Log(sb.ToString());
		sb.Clear();
		InitializeGrid();

		sb.AppendFormat("Start Room: {0}, {1}", _startingRoom.x, _startingRoom.y).AppendLine();
		sb.AppendFormat("End Room: {0}, {1}", _endRoom.x, _endRoom.y).AppendLine();
		Debug.Log(sb.ToString());
        sb.Clear();

		
		DebugLayers();

    	//Determine Neighbors
	    var found = Search();

        
		_validPath = FindPathToEnd();

        AdjustGrid();

		GenerateLevel();
        finished = true;
		//AdjustGrid();


	}

    private void InitializeGrid()
    {
        _validPath = new List<RoomData>();
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
		_startingRoom = new Vector2Int(x, y);
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
        _endRoom = new Vector2Int(eX, eY);

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
        openList.Add(_grid[_startingRoom.y][_startingRoom.x]);

		//Recursion begins
		while (openList.Count > 0 && !openList.Any(r => r.RoomNumber == _grid[_endRoom.y][_endRoom.x].RoomNumber))
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
            
            if(closedList.Any(r => r.RoomNumber == _grid[_endRoom.y][_endRoom.x].RoomNumber))
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
        int xDiff = Math.Abs(_startingRoom.x - target.X);
        int yDiff = Math.Abs(_startingRoom.y - target.Y);
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
        int x = Math.Abs(start.X - _endRoom.x);
        int y = Math.Abs(start.Y - _endRoom.y);

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
        RoomData currentRoom = _grid[_endRoom.y][_endRoom.x];
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
                if (!_validPath.Any(a => a.RoomNumber == room.RoomNumber) && !_extraRooms.Any(a => a.RoomNumber == room.RoomNumber))
                    _grid[room.Y][room.X].Enabled = false;
                else
                {
                    CreateRoomPrefab(room);
                }
            }
        }

        //StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendLine("Grid:");
        //foreach (var arr in _grid)
        //{
        //    stringBuilder.Append("|");
        //    foreach (var room in arr)
        //    {
        //        if (room.Bonus)
        //            stringBuilder.Append('B');
        //        else if (room.Spawn)
        //            stringBuilder.Append('S');
        //        else if (room.End)
        //            stringBuilder.Append('E');
        //        else if (room.Enabled)
        //            stringBuilder.Append('X');
        //        else
        //            stringBuilder.Append('-');
        //    }
        //    stringBuilder.AppendLine("|");
        //}
        //Debug.Log(stringBuilder.ToString());

        //foreach(var room in _validPath)
        //{
        //    //CreateRoomPrefab(room);
        //}
    }

    private void CreateRoomPrefab(RoomData data)
    {
        var gameObject = Instantiate(_roomPrefab, new Vector2(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab), Quaternion.identity, RoomParent.transform);
        gameObject.name = $"Room{data.RoomNumber}";
        var comp = gameObject.GetComponent<Room>();
        if(comp == null)
            throw new NullReferenceException("An error occurred when creating the prefab - could not retrieve component!");
        comp.RoomNumber = data.RoomNumber;
        comp.X = data.X;
        comp.Y = data.Y;
        if(data.Spawn)
        {
            var floor = gameObject.transform.Find("Floor");
            var render = floor.GetComponent<SpriteRenderer>();
            render.color = Color.green;
        }
        if(data.End)
        {
            var floor = gameObject.transform.Find("Floor");
            var render = floor.GetComponent<SpriteRenderer>();
            render.color = Color.red;
        }
        if(data.Bonus)
        {
            var floor = gameObject.transform.Find("Floor");
            var render = floor.GetComponent<SpriteRenderer>();
            render.color = Color.yellow;
        }
        if (!data.Enabled)
        {
			var floor = gameObject.transform.Find("Floor");
			var render = floor.GetComponent<SpriteRenderer>();
			render.color = Color.black;
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
        var gameObject = GameObject.FindGameObjectWithTag("DebugLayer");
		foreach (var arr in _grid)
		{
			foreach (var data in arr)
			{
                if (gameObject != null)
                {
                    var go = gameObject.transform.Find($"Debug{data.RoomNumber}");
                    if (go != null && !_validPath.Any(v => v.RoomNumber == data.RoomNumber))
                        go.gameObject.SetActive(false);
				}

				if (data.Parent != null && _validPath.Any(r => r.RoomNumber == data.RoomNumber))
					Debug.DrawLine(new Vector3(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab, 0), new Vector3(data.Parent.X * _xDistanceForPrefab, data.Parent.Y * _yDistanceForPrefab, -3), Color.blue);

			}
		}

		foreach (var room in _validPath)
        {
			var go = gameObject.transform.Find($"Debug{room.RoomNumber}");
			if (go != null)
				go.gameObject.SetActive(true);
		}
	}
    
    private void AdjustGrid()
    {
  //      int count = _validPath.Count();
  //      int roomsLeft = _numberOfRooms - count;

  //      var open = new List<RoomData>();
  //      var closed = new List<RoomData>();

  //      if (roomsLeft <= 0)
  //          return;

  //      int diff = 0;
  //      int previousCount = 0;

  //      while (roomsLeft > 0)
  //      {
  //          Debug.Log($"{roomsLeft} rooms left");
  //          previousCount = closed.Count;
            
  //          int mid = _validPath.Count / 2;

  //          mid += UnityEngine.Random.Range(0, _validPath.Count / 4);

  //          DetermineNeighbors(_validPath[mid], ref open, ref closed);
			
  //          diff = Math.Abs(closed.Count - previousCount);
  //          roomsLeft -= diff;
		//}

  //      foreach (var room in closed)
  //      {
  //          _extraRooms.Add(_grid[room.Y][room.X]);
  //      }
    }

}