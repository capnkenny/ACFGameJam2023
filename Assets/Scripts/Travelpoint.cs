using System;
using UnityEngine;

public class Travelpoint : MonoBehaviour
{
    public Collider2D collider;
    public delegate void TravelDelegate(int lvl);
    public delegate void TravelDelegateRoom(RoomData r, bool booleanResult, Direction dir);
    public delegate void CompleteRoomDelegate(PlayerData data);

    private TravelDelegate del;
    private TravelDelegateRoom roomDel;
    private CompleteRoomDelegate completeDel;

    private bool isLevelSwitcher = false;
    //private bool levelComplete = false;

    private int parameter = -1;
    private RoomData roomParameter = null;
    private bool booleanParameter = false;
    private Direction directionParameter;
    private PlayerData dataParameter = null;

    private void Update()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (collider != null && p != null && collider.bounds.Contains(p.transform.position))
            InvokeDelegate();
    }

    public void SetDelegate(TravelDelegate function, int param)
    {
        del = (p) => function(p);
        parameter = param;
        isLevelSwitcher = true;
    }
    public void SetDelegate(CompleteRoomDelegate function, PlayerData param)
    {
        completeDel = (p) => function(p);
        dataParameter = param;
        isLevelSwitcher = false;
    }

    public void SetDelegate(TravelDelegateRoom function, RoomData param, bool result, Direction d)
    {
        roomDel = (p1, p2, p3) => function(p1, p2, p3);
        roomParameter = param;
        booleanParameter = result;
        directionParameter = d;
        isLevelSwitcher = false;
    }

    public void InvokeDelegate()
    {
        //Debug.Log("YOLO time to go");
        if (isLevelSwitcher && del != null)
            del(parameter);
        else if (!isLevelSwitcher && roomDel != null)
            roomDel(roomParameter, booleanParameter, directionParameter);
        else if (!isLevelSwitcher && completeDel != null)
            completeDel(dataParameter);
    }
}
