using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RunAwayBehavior : EnemyBehavior
{
   // [Header("Stats")]
   // private float _distanceAwayFromPlayer;
    private GameObject _player;
    
    [Header("Other fields")]
    [SerializeField]
    private CircleCollider2D _collider;
    [SerializeField]
    private Transform _enemyTransform;
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private bool _found;
    [SerializeField]
    private string _runningAnimName;
    [SerializeField]
    private float _cooldownTime;
    
    private int _runningAnimHash;
    private bool _runningAway;
    private float  _runAwayDistance;
    private bool _cooldownMode;
    private float _deltaTime;

	  private bool _running;
    private bool _flipChar;
    
    void Awake()
    {
      _runningAnimHash = Animator.StringToHash(_runningAnimName);
      _player = GameObject.FindWithTag("Player");
      _runningAway = _running = false;
      _flipChar = false;
      _runAwayDistance = _collider.radius;
      _deltaTime = 0.0f;
    }

    void Update()
    {   
	    //Set animation stuff
	    _animator.SetBool("WasFound", _found);
	    _animator.SetBool("RunningAway", _runningAway);
      
      if(_flipChar)
      {
        float yAxis = 180;
        _flipChar = false;
        if(transform.rotation.y >= 180)
        {
          yAxis = -yAxis;
        }
        transform.Rotate(0,yAxis,0);
      }
      
      //Handle movement and reaction
      
      float distance = Vector2.Distance((Vector2)_enemyTransform.position, (Vector2)_player.transform.position);
      var direction = _player.transform.position - _enemyTransform.position;
        
      if(_found && !_runningAway)
      {
        if(distance <= _collider.radius)
        {
          _runningAway = true;
          return;
        }	
      }

      if(_runningAway && !_running)
      {
        _rigidbody.velocity = (Vector2)((-direction)*controller.MovementSpeed);
        
        _running = true;
      }

      if(_runningAway)
      {
        Debug.Log($"Velocity: {_rigidbody.velocity}");
        if(_rigidbody.velocity.x < 0 && transform.rotation.y == 0)
          _flipChar = true;
      }
    

    }

    void OnCollisionEnter2D(Collision2D col)
    {
      if(col.gameObject.tag == "Collision_L")
      {
        if(transform.rotation.y != 0)
          _flipChar = true;
        
       if(_rigidbody.velocity.y < 0.00f)
       {
          _rigidbody.velocity = _rigidbody.velocity + new Vector2(0, 1);
       }

      }
      if(col.gameObject.tag == "Collision_R")
      {
        if(transform.rotation.y == 0)
          _flipChar = true;
        
       if(_rigidbody.velocity.y < 0.00f)
       {
          _rigidbody.velocity = _rigidbody.velocity + new Vector2(0, -1);
       }

      }

      if(col.gameObject.tag == "Collision_T")
      {
        
       if(_rigidbody.velocity.x < 0.00f)
       {
          _rigidbody.velocity = _rigidbody.velocity + new Vector2(-1, 0);
       }

      }

      if(col.gameObject.tag == "Collision_B")
      {
        
       if(_rigidbody.velocity.x < 0.00f)
       {
          _rigidbody.velocity = _rigidbody.velocity + new Vector2(1, 0);
       }

      }
    }


    public override void PerformBehavior()
    {
	    _found = true;
    }
    
}
