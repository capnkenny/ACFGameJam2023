using System.Collections.Generic;

public class RoomData
{
    public int X {get; set;}
    public int Y {get; set;}

    public int RoomNumber {get; set;}

    public bool Enabled {get; set;}

    public bool Spawn {get; set;}

    public bool End {get; set;}

    public bool Bonus {get; set;}

    public RoomData Parent {get; set;}

    public int GivenCost {get; set;}

    public int HeuristicCost {get; set;}
    public int FullCost { get { return GivenCost + HeuristicCost; } }

    public List<RoomData> Neighbors = new List<RoomData>();
}
