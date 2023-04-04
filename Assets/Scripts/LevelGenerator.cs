using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomPrefab;

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

    [SerializeField]
    private float _xDistanceForPrefab;

    [SerializeField]
    private int _cardinalDirectionWeight;

    [SerializeField]
    private int _diagonalDirectionWeight;

    [SerializeField]
    private Vector2 _startPoint;

    private List<List<RoomData>> _roomMatrix;

    private int _numberOfRooms;
    private Queue<RoomData> _roomsToCheck;

    void Awake()
    {
        if(_gridHeight < 1)
            _gridHeight = 7;
        if(_gridWidth < 1)
            _gridWidth = 7;
        int roomNumber = 1;
        if(_levelNumber <= 0)
            throw new ArgumentException("The Level Number must be greater than or equal to 1!");

        //Generate number of rooms
        //Based on Florian Himsl's Binding of Isaac
        _numberOfRooms = (int)(UnityEngine.Random.Range(1,2) + 5 + _levelNumber * 2.6);

        _roomMatrix = new List<List<RoomData>>(_gridHeight);
        for(int h = 0; h < _gridHeight; h++)
        {
            _roomMatrix[h] = new List<RoomData>(_gridWidth);
            for(int w = 0; w < _gridWidth; w++)
            {
                _roomMatrix[h][w] = new RoomData 
                { 
                    X = w, 
                    Y = h,
                    RoomNumber = roomNumber, 
                    Enabled = false, 
                    Parent = null, 
                    StartCost = 0, 
                    EndCost = 0,
                };
                roomNumber++;
            }
        }

        //Set start point if defined
        if(_startPoint != null)
        {
            int x = (int)_startPoint.x;
            int y = (int)_startPoint.y;

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
                if(_roomMatrix[y][x] != null)
                {
                    _roomMatrix[y][x].Enabled = true;
                    Instantiate(_roomPrefab, new Vector2(x * _xDistanceForPrefab, y * _yDistanceForPrefab), Quaternion.identity);
                    _numberOfRooms--;
                    _roomsToCheck.Enqueue(_roomMatrix[y][x]);
                }
            }
            catch
            {
                int newX = UnityEngine.Random.Range(0, _gridWidth);
                int newY = UnityEngine.Random.Range(0, _gridHeight);
                _roomMatrix[newY][newX].Enabled = true;
                Instantiate(_roomPrefab, new Vector2(newX * _xDistanceForPrefab, newY * _yDistanceForPrefab), Quaternion.identity);
                _numberOfRooms--;
                _roomsToCheck.Enqueue(_roomMatrix[newY][newX]);
            }
        }

        //Check list of rooms for neighbors
        bool noMoreRooms = false;
        while(!noMoreRooms && _roomsToCheck.Count > 0)
        {
            var room = _roomsToCheck.Dequeue();
            if(room.Neighbors == null)
            {
                room.Neighbors = GetNeighbors(room.X, room.Y);
            }

        }

    }


    public List<RoomData> GetNeighbors(int x, int y)
    {
        List<RoomData> neighbors = new();

        if(y == 0)
        {
            if(x == 0)
            {
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y+1][x+1] != null && _roomMatrix[y+1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x+1]);
                if(_roomMatrix[y+1][x] != null && _roomMatrix[y+1][x].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y+1][x-1] != null && _roomMatrix[y+1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x-1]);
                if(_roomMatrix[y+1][x] != null && _roomMatrix[y+1][x].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
            else
            {
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y+1][x+1] != null && _roomMatrix[y+1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x+1]);
                if(_roomMatrix[y+1][x-1] != null && _roomMatrix[y+1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x-1]);
                if(_roomMatrix[y+1][x] != null && _roomMatrix[y+1][x].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x]);
            }
        }
        else if(y == _roomMatrix.Count - 1)
        {
            if(x == 0)
            {
                if(_roomMatrix[y-1][x] != null && _roomMatrix[y-1][x].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y-1][x+1] != null && _roomMatrix[y-1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x+1]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y-1][x] != null && _roomMatrix[y-1][x].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y-1][x-1] != null && _roomMatrix[y-1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x-1]);
            }
            else
            {
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y-1][x+1] != null && _roomMatrix[y-1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x+1]);
                if(_roomMatrix[y-1][x-1] != null && _roomMatrix[y-1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x-1]);
                if(_roomMatrix[y-1][x] != null && _roomMatrix[y-1][x].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x]);
            }
        }
        else
        {
            if(x == 0)
            {
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y+1][x+1] != null && _roomMatrix[y+1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x+1]);
                if(_roomMatrix[y-1][x] != null && _roomMatrix[y-1][x].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x]);
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y-1][x+1] != null && _roomMatrix[y-1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x+1]);
            }
            else if(x == _roomMatrix[y].Count - 1)
            {
                if(_roomMatrix[y+1][x] != null && _roomMatrix[y+1][x].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y+1][x-1] != null && _roomMatrix[y+1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x-1]);
                if(_roomMatrix[y-1][x-1] != null && _roomMatrix[y-1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x-1]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
            }
            else
            {
                if(_roomMatrix[y+1][x+1] != null && _roomMatrix[y+1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x+1]);
                if(_roomMatrix[y+1][x-1] != null && _roomMatrix[y+1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x-1]);
                if(_roomMatrix[y-1][x-1] != null && _roomMatrix[y-1][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x-1]);
                if(_roomMatrix[y-1][x+1] != null && _roomMatrix[y-1][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x+1]);
                if(_roomMatrix[y][x-1] != null && _roomMatrix[y][x-1].Enabled)
                    neighbors.Add(_roomMatrix[y][x-1]);
                if(_roomMatrix[y][x+1] != null && _roomMatrix[y][x+1].Enabled)
                    neighbors.Add(_roomMatrix[y][x+1]);
                if(_roomMatrix[y+1][x] != null && _roomMatrix[y+1][x].Enabled)
                    neighbors.Add(_roomMatrix[y+1][x]);
                if(_roomMatrix[y-1][x] != null && _roomMatrix[y-1][x].Enabled)
                    neighbors.Add(_roomMatrix[y-1][x]);
            }
        }
    
        return neighbors;
    }

    private void SetWeightsForMatrix(RoomData start, RoomData end)
    {
        foreach(var arr in _roomMatrix)
        {
            foreach(var room in arr)
            {
                room.StartCost = GetDistance(start, room);
                room.EndCost = GetDistance(room, end);
            }
        }
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

    private bool IsInList(List<RoomData> toSearch, RoomData room)
    {
        var result = toSearch.Where(w => w.RoomNumber == room.RoomNumber);
        return result.Count() > 0;
    }

    // private void FindPath(RoomData start, RoomData target)
    // {
    //     List<RoomData> open = new();
    //     List<RoomData> closed = new();
    //     open.Add(start);

    //     while (open.Count > 0)
    //     {
    //         RoomData current = open[0];
    //         for(int c = 1; c < open.Count; c++)
    //         {
    //             var room = open[c];
    //             if(room.FullCost < current.FullCost || 
    //                 room.FullCost == current.FullCost && 
    //                 room.EndCost < current.EndCost)
    //             {
    //                 current = room;
    //             }
    //         }

    //         open.Remove(current);
    //         closed.Add(current);
    //         if(current == target)
    //         {
    //             RetracePath(start, target);
    //             return;
    //         }

    //         foreach(var neighbor in current.Neighbors)
    //         {
    //             if(!neighbor.Enabled || closed.Contains(neighbor)) continue;
                
    //             int newCostToNeighbor = current.StartCost + GetDistance(current, neighbor);
    //             if(newCostToNeighbor < neighbor.StartCost || !open.Contains(neighbor))
    //             {
    //                 neighbor.StartCost = newCostToNeighbor;
    //                 neighbor.EndCost = GetDistance(neighbor, target);
    //                 neighbor.Parent = current;
    
    //                 if (!open.Contains(neighbor))
    //                     open.Add(neighbor);
    //             }
    //         }
    //     }
    // }

    // private List<RoomData> RetracePath(RoomData start, RoomData target)
    // {
    //     List<RoomData> path = new();
    //     RoomData current = target;
    //     Debug.Log("Retracing path...");
    //     while(current != start)
    //     {
    //         path.Add(current);
    //         if(current.Parent != null)
    //             current = current.Parent;
    //         else
    //             break;
    //     }

    //     path.Reverse();
    //     return path;
    // }


}
