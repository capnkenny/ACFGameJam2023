using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyController controller;
    public bool Enabled;

    void Awake()
    {
    }

    void FixedUpdate()
    {
        if(Enabled)
        {
            PerformBehavior();
        }
    }

    public virtual void PerformBehavior()
    {}
}
