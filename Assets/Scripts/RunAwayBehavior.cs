using UnityEngine;

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
    private Animator _secondaryAnimator;
    [SerializeField]
    private bool _found;
    [SerializeField]
    private string _runningAnimName;

    private int _runningAnimHash;
    private bool _runningAway;
    private float _runAwayDistance;

    private bool _running;
    private bool _flipChar;

    private bool _isDead;

    void Awake()
    {
        _runningAnimHash = Animator.StringToHash(_runningAnimName);
        _player = GameObject.FindWithTag("Player");
        _runningAway = _running = false;
        _flipChar = false;
        _runAwayDistance = _collider.radius;
        _isDead = false;
    }

    private void Start()
    {
        _rigidbody.bodyType = RigidbodyType2D.Static;
    }

    void Update()
    {
      // Check if dead
      _isDead = controller.Health == 0;
      //Debug.Log($"Health: {controller.Health}, Found: {_found}, Running Away: {_runningAway}, Dead: {_isDead}");
        //Set animation stuff
      _animator.SetBool("WasFound", _found);
      _animator.SetBool("RunningAway", _runningAway);
      _animator.SetBool("IsDead", _isDead);
      _secondaryAnimator.SetBool("IsRunning", _runningAway);

        if (_flipChar)
        {
            //Debug.Log($"Transform: {transform.rotation.y}");
            float yAxis = 180;
            _flipChar = false;
            if (transform.rotation.y >= 180)
            {
                yAxis = -yAxis;
            }
            transform.Rotate(0, yAxis, 0);
        }

        //Handle movement and reaction

        float distance = Vector2.Distance((Vector2)_enemyTransform.position, (Vector2)_player.transform.position);
        var direction = _player.transform.position - _enemyTransform.position;

        if (!_found)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (_collider.bounds.Contains(p.transform.position))
            {
                _found = true;
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                _collider.enabled = false;
                return;
            }
        }

        if (_found && !_runningAway)
        {
          Debug.LogFormat("distance - {0}, radius: {1}", distance, _collider.radius);
            if (distance <= _collider.radius)
            {
                _runningAway = true;
                return;
            }
        }

        if (_runningAway && !_running)
        {
          Debug.Log("Running away!");
            _rigidbody.velocity = (Vector2)((-direction) * controller.MovementSpeed);

            _running = true;
            return;
        }

        if(_isDead)
        {
          Debug.Log("dying :(");
          var state = _animator.GetCurrentAnimatorStateInfo(0);
          if(state.IsName("Base Layer.Death") && state.normalizedTime > 1.0f)
          {
                var mgr = GameObject.FindGameObjectWithTag("LvlMgr");
                var levelManager = mgr == null ? null : mgr.GetComponent<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.SignalEnemyDied();
                }
                GameObject.Destroy(this.gameObject);
          }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.otherCollider.tag.Contains("Detection"))
            Debug.LogFormat("MIKEY - Collided with {0}", col.gameObject.tag);

        if (col.gameObject.tag == "Collision_B" && _running)
        {
            if (_rigidbody.velocity.x < 0.00f)
            {
                _rigidbody.velocity = _rigidbody.velocity + new Vector2(1, 0);
            }
        }
        if (col.gameObject.tag == "Collision_L" && _running)
        {
            if (transform.rotation.y != 0)
                _flipChar = true;

            if (_rigidbody.velocity.y < 0.00f)
            {
                _rigidbody.velocity = _rigidbody.velocity + new Vector2(0, 1);
            }
        }
        if (col.gameObject.tag == "Collision_R" && _running)
        {
            if (transform.rotation.y == 0)
                _flipChar = true;

            if (_rigidbody.velocity.y < 0.00f)
            {
                _rigidbody.velocity = _rigidbody.velocity + new Vector2(0, -1);
            }
        }
        if (col.gameObject.tag == "Collision_T" && _running)
        {

            if (_rigidbody.velocity.x < 0.00f)
            {
                _rigidbody.velocity = _rigidbody.velocity + new Vector2(-1, 0);
            }
        }
        if (col.gameObject.tag == "Player")
        {
            if (col.otherCollider.tag == "Enemy" && _running)
            {
                controller.Health -= controller.Health;
                _runningAway = false;
                _found = false;
                _rigidbody.velocity = Vector2.zero;
            }
        }
    }

    public override void PerformBehavior()
    {
        _found = true;
    }

}
