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
    
    void Awake()
    {
	_runningAnimHash = Animator.StringToHash(_runningAnimName);
        _player = GameObject.FindWithTag("Player");
	_runningAway = false;
	_runAwayDistance = _collider.radius;
	_deltaTime = 0.0f;
    }

    void Update()
    {
	    //Debug.Log($"Found: {_found}, RunningAway: {_runningAway}");
	    
	    //Set animation stuff
	    _animator.SetBool("WasFound", _found);
	    _animator.SetBool("RunningAway", _runningAway);
	    if(_found)
	    {
		float distance = Vector2.Distance((Vector2)_enemyTransform.position, (Vector2)_player.transform.position);
		var direction = _player.transform.position - _enemyTransform.position;
		Debug.Log($"Direction: {direction}, Distance: {distance}, Runaway @: {_runAwayDistance}");
		
		if(_runningAway)
		{
		Debug.Log("Running away...");
		//var direction = _player.transform.position - _enemyTransform.position;
		//Debug.Log($"Direction: {direction}, Distance: {distance}, Runaway @: {_runAwayDistance}");
		Debug.DrawRay(_enemyTransform.position, -direction, Color.green);
		Debug.DrawRay(_enemyTransform.position, direction, Color.red);
     	        _rigidbody.velocity = (Vector2)((-direction)*controller.MovementSpeed);
		}
		
		else
		{
		    Debug.Log("Not Running away, but is found");
		    CheckCooldownStatus(Time.deltaTime, false, distance);
		}
	    }
    }

    private void CheckCooldownStatus(float delta, bool enteringCooldown = false, float distance = 0)
    {
	if(enteringCooldown)
	{
	    _runningAway = false;
	    _cooldownMode = true;
	    _rigidbody.velocity = Vector2.zero;
	    return;
	}
	else
	{
	   _deltaTime += delta;
	   if(distance <= _collider.radius)
	   {
		   //re-alert mikey to run away
	       _runningAway = true;
	       _cooldownMode = false;
	   }

	   if(_deltaTime >= _cooldownTime)
	   {
	      _found = false;
	      _cooldownMode = false;
	      _deltaTime = 0.0f;
	      return;
	   }
	}
    }

    public override void PerformBehavior()
    {
	    Debug.Log("RunningAway-PerformBehavior called");
	    _found = true;
    }
    
}
