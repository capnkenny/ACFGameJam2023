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
    private int _gridWidth;

    [SerializeField]
    private int _gridHeight;
    
    [SerializeField]
    private float _ySnapDistanceForCamera;

    [SerializeField]
    private float _xSnapDistanceForCamera;

    [SerializeField]
    private int _cardinalDirectionWeight;

    [SerializeField]
    private int _diagonalDirectionWeight;

    private List<List<RoomData>> _roomMatrix;

    void Awake()
    {
        if(_gridHeight == null || _gridHeight < 1)
            _gridHeight = 5;
        if(_gridWidth == null || _gridWidth < 1)
            _gridWidth = 5;
        int roomNumber = 1;

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
                    EndCost = 0 
                };
                roomNumber++;
            }
        }
    }

    void Start()
    {


    }

    void Update()
    {

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

    private void FindPath(RoomData start, RoomData target)
    {
        List<RoomData> open = new();
        List<RoomData> closed = new();
        open.Add(start);

        while (open.Count > 0)
        {
            RoomData current = open[0];
            for(int c = 1; c < open.Count; c++)
            {
                var room = open[c];
                if(room.FullCost < current.FullCost || 
                    room.FullCost == current.FullCost && 
                    room.EndCost < current.EndCost)
                {
                    current = room;
                }
            }

            open.Remove(current);
            closed.Add(current);
            if(current == target)
            {
                RetracePath(start, target);
                return;
            }

            foreach(var neighbor in current.Neighbors)
            {
                if(!neighbor.Enabled || closed.Contains(neighbor)) continue;
                
                int newCostToNeighbor = current.StartCost + GetDistance(current, neighbor);
                if(newCostToNeighbor < neighbor.StartCost || !open.Contains(neighbor))
                {
                    neighbor.StartCost = newCostToNeighbor;
                    neighbor.EndCost = GetDistance(neighbor, target);
                    neighbor.Parent = current;
    
                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }
    }

    private List<RoomData> RetracePath(RoomData start, RoomData target)
    {
        List<RoomData> path = new();
        RoomData current = target;
        Debug.Log("Retracing path...");
        while(current != start)
        {
            path.Add(current);
            if(current.Parent != null)
                current = current.Parent;
            else
                break;
        }

        path.Reverse();
        return path;
    }
}