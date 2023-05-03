using System;
using UnityEngine;

public class Travelpoint : MonoBehaviour
{
    public delegate void TravelDelegate(int lvl);
    public delegate void TravelDelegateRoom(RoomData r, bool booleanResult, Direction dir);

    private TravelDelegate del;
    private TravelDelegateRoom roomDel;

    private bool isLevelSwitcher = false;

    private int parameter = -1;
    private RoomData roomParameter = null;
    private bool booleanParameter = false;
    private Direction directionParameter;

    public void SetDelegate(TravelDelegate function, int param)
    {
        del = (p) => function(p);
        parameter = param;
        isLevelSwitcher = true;
    }

    public void SetDelegate(TravelDelegateRoom function, RoomData param, bool result, Direction d)
    {
        roomDel = (p1, p2, p3) => function(p1, p2, p3);
        roomParameter = param;
        booleanParameter = result;
        directionParameter = d;
        isLevelSwitcher = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            if (isLevelSwitcher && del != null)
                del(parameter);
            else if (!isLevelSwitcher && roomDel != null)
                roomDel(roomParameter, booleanParameter, directionParameter);
        }
    }
}
