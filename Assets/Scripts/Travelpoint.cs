using System;
using UnityEngine;

public class Travelpoint : MonoBehaviour
{
    public delegate void TravelDelegate(int lvl);

    private TravelDelegate del;

    private int parameter = -1;
    public void SetDelegate(TravelDelegate function, int param)
    {
        del = (p) => function(p);
        parameter = param;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (del != null && collision.tag.Contains("Player"))
        {
            del(parameter);
        }
    }
}
