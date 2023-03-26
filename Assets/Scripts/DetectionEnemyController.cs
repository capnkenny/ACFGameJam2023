using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectionEnemyController : EnemyController
{
    [Header("Detection-specific Traits")]
    public CircleCollider2D collider;
    //FoundSoundEffect
    

    //Private members
    private GameObject _playerToDetect;
    private Collider2D _playerCollider;

    void Start()
    {
        _playerToDetect = GameObject.FindWithTag("Player");
        _playerCollider = _playerToDetect.GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col == _playerCollider)
        {
            AttackBehavior.PerformBehavior();
        }
    }
}
