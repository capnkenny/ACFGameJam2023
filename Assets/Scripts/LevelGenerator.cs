using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomPrefab;

    [SerializeField]
    private GameObject _debugPrefab;

    [SerializeField]
    private int _levelNumber;

    [SerializeField]
    private int _gridWidth;

    [SerializeField]
    private int _gridHeight;
    
    [SerializeField]
    private float _ySnapDistanceForCamera;

    [SerializeField]
    private float _xSnapDistanceForCamera;

    [SerializeField]
    private float _yDistanceForPrefab;
    //13.68

    [SerializeField]
    private float _xDistanceForPrefab;
    //35.41

    [SerializeField]
    private int _cardinalDirectionWeight;

    [SerializeField]
    private int _diagonalDirectionWeight;

    [SerializeField]
    private Vector2 _startPoint;

    [SerializeField]
    private bool _useStartPoint;

    [InspectorButton("OnRegenerate")]
    public bool RegenerateLevel;

    private List<List<RoomData>> _roomMatrix;
    private int _numberOfRooms;
    private List<RoomData> _enabledRooms;
    
    private List<RoomData> _path;

    void Start()
    {
        if(_gridHeight < 1)
            _gridHeight = 7;
        if(_gridWidth < 1)
            _gridWidth = 7;
        int roomNumber = 1;
        if(_levelNumber <= 0)
            throw new ArgumentException("The Level Number must be greater than or equal to 1!");

        if(_cardinalDirectionWeight <= 0)
            _cardinalDirectionWeight = UnityEngine.Random.Range(5,21);
        if(_diagonalDirectionWeight <= 0)
            _diagonalDirectionWeight = UnityEngine.Random.Range(5,21);

        //Generate number of rooms
        _numberOfRooms = (int)(UnityEngine.Random.Range(1,3) + _levelNumber + 5 * 2.6);
        _roomMatrix = new List<List<RoomData>>(_gridHeight);
        _path = new List<RoomData>();
        _enabledRooms = new List<RoomData>();

        for(int h = 0; h < _gridHeight; h++)
        {
            _roomMatrix.Add(new List<RoomData>(_gridWidth));
            //_roomMatrix[h] = new List<RoomData>(_gridWidth);
            for(int w = 0; w < _gridWidth; w++)
            {
                _roomMatrix[h].Add(new RoomData 
                { 
                    X = w, 
                    Y = h,
                    RoomNumber = roomNumber, 
                    Enabled = false, 
                    Parent = null, 
                    StartCost = 0, 
                    EndCost = 0,
                    Spawn = false,
                    End = false,
                    Bonus = false
                });
                // var go = Instantiate(_debugPrefab, new Vector2(w * _xDistanceForPrefab, h * _yDistanceForPrefab), Quaternion.identity, transform);
                // var goRoom = go.GetComponent<Room>();
                // goRoom.RoomNumber = roomNumber;
                // goRoom.X = w;
                // goRoom.Y = h;
                //Debug.LogFormat("Generated room # {0}", roomNumber);
                roomNumber++;
            }
        }

        //Set start point if defined
        int x = (int)_startPoint.x;
        int y = (int)_startPoint.y;
        RoomData startRoom = null;
        if(_useStartPoint)
        {
            if(x < 0 || x > _gridWidth)
            {
                throw new ArgumentOutOfRangeException("_startPoint.x");
            }
            if(y < 0 || y > _gridHeight)
            {
                throw new ArgumentOutOfRangeException("_startPoint.y");
            }
            try
            {
                Debug.LogFormat("Setting starting room to room # {0} @ {1}, {2}", _roomMatrix[y][x].RoomNumber, _roomMatrix[y][x].X, _roomMatrix[y][x].Y);
                _roomMatrix[y][x].Enabled = true;
                _roomMatrix[y][x].Spawn = true;
                startRoom = _roomMatrix[y][x];
                _numberOfRooms--;
            }
            catch
            {
                int newX = UnityEngine.Random.Range(0, _gridWidth);
                int newY = UnityEngine.Random.Range(0, _gridHeight);
                _startPoint = new Vector2(newX, newY);
                _roomMatrix[newY][newX].Enabled = true;
                _roomMatrix[newY][newX].Spawn = true;
                startRoom = _roomMatrix[newY][newX];
                Debug.LogFormat("Setting starting room to room # {0} @ {1}, {2}", _roomMatrix[newY][newX].RoomNumber, newX, newY);
                _numberOfRooms--;
            }
            _enabledRooms.Add(startRoom);
        }
        else
        {
            x = UnityEngine.Random.Range(0, _gridWidth);
            y = UnityEngine.Random.Range(0, _gridHeight);
            _startPoint = new Vector2(x, y);
            _roomMatrix[y][x].Enabled = true;
            _roomMatrix[y][x].Spawn = true;
            startRoom = _roomMatrix[y][x];
            Debug.LogFormat("Setting starting room to room # {0} @ {1}, {2}", _roomMatrix[y][x].RoomNumber, x, y);
            _numberOfRooms--;
        }

        GenerateRooms(ref startRoom);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Matrix: ");
        foreach(var row in _roomMatrix)
        {
            sb.Append("|");
            foreach(var col in row)
            {
                if(_path.Where(r => r.RoomNumber == col.RoomNumber).Count() > 0)
                {
                    sb.Append("x");
                    CreateRoomPrefab(col);
                }
                else
                    sb.Append("-");
            }
            sb.AppendLine("|");
        }
        Debug.Log(sb.ToString());

        var cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.position = new Vector3(startRoom.X * _xSnapDistanceForCamera , startRoom.Y * _ySnapDistanceForCamera, -10);
    }

    private void CreateRoomPrefab(RoomData data)
    {
        var gameObject = Instantiate(_roomPrefab, new Vector2(data.X * _xDistanceForPrefab, data.Y * _yDistanceForPrefab), Quaternion.identity, transform);
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
    }

    private void GenerateInitialLayout(ref RoomData start)
    {
        RoomData current = start;
        _path.Add(start);

        //Set the initial direction
        int direction = UnityEngine.Random.Range(1,5);

        // Generate the remaining rooms until
        // we have none left
        for(int n = 0; n < _numberOfRooms; n++)
        {
            Debug.LogFormat("{0} rooms remaining", (_numberOfRooms - n));
            bool okay = false;
            int x = current.X;
            int y = current.Y;
            while(!okay)
            {
                direction = UnityEngine.Random.Range(1,5);
                switch(direction)
                {
                    case 1: // North
                    {
                        y++;
                        break;
                    }
                    case 2: // East
                    {
                        x++;
                        break;
                    }
                    case 3: // South
                    {
                        y--;
                        break;
                    }
                    case 4: // West
                    {
                        x--;
                        break;
                    }
                }
                if(y >= 0 && y < _gridHeight)
                {
                    if(x >= 0 && x < _gridWidth)
                    {
                        if(!_roomMatrix[y][x].Enabled)
                        {
                            Debug.LogFormat("Enabling Room {0}", _roomMatrix[y][x].RoomNumber);
                            _roomMatrix[y][x].Enabled = true;
                            _path.Add(_roomMatrix[y][x]);
                            okay = true;
                        }
                    }
                }
            }
        }
        
        //Set the end room
        Vector2 endRoom = new Vector2(_path[_path.Count - 1].X, _path[_path.Count - 1].Y);
        _roomMatrix[(int)endRoom.y][(int)endRoom.x].End = true;
    }

    private void GenerateRooms(ref RoomData start)
    {
        GenerateInitialLayout(ref start);
    }
    public List<RoomData> GetNeighbors(int x, int y)
    {
        List<RoomData> neighbors = new();

        if(y == 0)
        {
            
            if(x == 0)
            {
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y+1][x] != null)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y+1][x] != null)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
            else
            {
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y+1][x] != null)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
        }
        else if(y == _roomMatrix.Count - 1)
        {
            if(x == 0)
            {
                if(_roomMatrix[y-1][x] != null)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y-1][x] != null)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
            }
            else
            {
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y-1][x] != null)
                    neighbors.Add(_roomMatrix[y-1][x]);
            }
        }
        else
        {
            if(x == 0)
            {
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y-1][x] != null)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y+1][x] != null)
                    neighbors.Add(_roomMatrix[y+1][x]);
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
            }
            else
            {
                if(_roomMatrix[y][x-1] != null)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y][x+1] != null)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y+1][x] != null)
                    neighbors.Add(_roomMatrix[y+1][x]);
                if(_roomMatrix[y-1][x] != null)
                    neighbors.Add(_roomMatrix[y-1][x]);
            }
        }
    
        return neighbors;
    }
    private int GetDistance(RoomData roomA, RoomData roomB)
    {
        if(roomA.RoomNumber == roomB.RoomNumber)
            return 0;

        int x = Math.Abs(roomA.X - roomB.X);
        int y = Math.Abs(roomA.Y - roomB.Y);

        int weight = 0;
        
        if(x > y)
            weight = (_diagonalDirectionWeight * y + _cardinalDirectionWeight * (x - y));
        else
            weight = (_diagonalDirectionWeight * x + _cardinalDirectionWeight * (y - x));
        
        return weight;
    }
    private void OnRegenerate()
    {
        _numberOfRooms = (int)(UnityEngine.Random.Range(1,3) + _levelNumber + 5 * 2.6);
        _enabledRooms = new List<RoomData>();
        int roomNumber = 0;
        //_roomMatrix.Clear();
        _roomMatrix = new List<List<RoomData>>(_gridHeight);

        int childrenCount = transform.childCount;
        for(int i = 0; i < childrenCount; i++)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        for(int h = 0; h < _gridHeight; h++)
        {
            _roomMatrix.Add(new List<RoomData>(_gridWidth));
            for(int w = 0; w < _gridWidth; w++)
            {
                _roomMatrix[h].Add(new RoomData 
                { 
                    X = w, 
                    Y = h,
                    RoomNumber = roomNumber, 
                    Enabled = false, 
                    Parent = null, 
                    StartCost = 0, 
                    EndCost = 0,
                    Spawn = false,
                    End = false,
                    Bonus = false
                });
                roomNumber++;
            }
        }

        int x = UnityEngine.Random.Range(0, _gridWidth);
        int y = UnityEngine.Random.Range(0, _gridHeight);
        RoomData startRoom = null;

        if(x < 0 || x > _gridWidth)
        {
            throw new ArgumentOutOfRangeException("_startPoint.x");
        }
        if(y < 0 || y > _gridHeight)
        {
            throw new ArgumentOutOfRangeException("_startPoint.y");
        }
        try
        {
            Debug.LogFormat("Setting starting room to room # {0} @ {1}, {2}", _roomMatrix[y][x].RoomNumber, _roomMatrix[y][x].X, _roomMatrix[y][x].Y);
            _roomMatrix[y][x].Enabled = true;
            _roomMatrix[y][x].Spawn = true;
            startRoom = _roomMatrix[y][x];
            _numberOfRooms--;
        }
        catch
        {
            int newX = UnityEngine.Random.Range(0, _gridWidth);
            int newY = UnityEngine.Random.Range(0, _gridHeight);
            _startPoint = new Vector2(newX, newY);
            _roomMatrix[newY][newX].Enabled = true;
            _roomMatrix[newY][newX].Spawn = true;
            startRoom = _roomMatrix[newY][newX];
            Debug.LogFormat("Setting starting room to room # {0} @ {1}, {2}", _roomMatrix[newY][newX].RoomNumber, newX, newY);
            //Instantiate(_roomPrefab, new Vector2(newX * _xDistanceForPrefab, newY * _yDistanceForPrefab), Quaternion.identity, transform);
            _numberOfRooms--;
        }
        _enabledRooms.Add(startRoom);

        //Create end point randomly
        bool endPointValid = false;
        RoomData endRoom = null;
        while(!endPointValid)
        {
            int endX = UnityEngine.Random.Range(0, _gridWidth);
            int endY = UnityEngine.Random.Range(0, _gridHeight);
            endRoom = _roomMatrix[endY][endX];
            endRoom.End = true;
            endRoom.Enabled = true;
            int distance = GetDistance(startRoom, endRoom);
            Debug.LogFormat("End Room Distance: {0}", distance);
            if(distance > 10)
            {
                endPointValid = true;
                _enabledRooms.Add(endRoom);
                _numberOfRooms--;
            }
            else
            {
                endRoom.End = endRoom.Enabled = false;
            }
        }

        GenerateRooms(ref startRoom);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Matrix: ");
        foreach(var row in _roomMatrix)
        {
            sb.Append("|");
            foreach(var col in row)
            {
                if(col.Enabled)
                {
                    sb.Append("x");
                    CreateRoomPrefab(col);
                }
                else
                    sb.Append("-");
            }
            sb.AppendLine("|");
        }
        Debug.Log(sb.ToString());

    }
}
