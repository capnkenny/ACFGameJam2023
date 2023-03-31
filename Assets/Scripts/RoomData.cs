using System.Collections.Generic;

public class RoomData
{
    public int X {get; set;}
    public int Y {get; set;}

    public int RoomNumber {get; set;}

    public bool Enabled {get; set;}

    public RoomData Parent {get; set;}

    public int StartCost {get; set;}

    public int EndCost {get; set;}
    public int FullCost { get { return StartCost + EndCost; } }

    public List<RoomData> Neighbors = new List<RoomData>();
}
