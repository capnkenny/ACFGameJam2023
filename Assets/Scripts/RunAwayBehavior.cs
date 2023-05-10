using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    private Collider2D _secondaryCollider;
    [SerializeField]
    private bool _found;
    [SerializeField]
    private string _runningAnimName;
    [SerializeField]
    private Light2D spotlight;

    [SerializeField]
    private AudioSource mic;

    private int _runningAnimHash;
    private bool _runningAway;
    private float _runAwayDistance;

    private bool _running;
    private bool _flipChar;

    private bool _isDead;
    private float sensoryTimer;
    private bool playerDead = false;

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
        spotlight.enabled = false;
        _secondaryCollider.enabled = false;
    }

    void Update()
    {
        if (_player != null)
        {
            playerDead = _player.GetComponent<PlayerController>().Dead;
        }

        if (!playerDead)
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
                    _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    _rigidbody.velocity = Vector2.zero;
                    _collider.enabled = false;
                    spotlight.enabled = true;
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
                //Debug.Log("Running away!");
                _rigidbody.velocity = (Vector2)((-direction) * controller.MovementSpeed);
                _running = true;
                _secondaryCollider.enabled = true;
                mic.Play();

                return;
            }
            else if (_runningAway && _running)
            {
                float cap = 5.0f;
                //player stuff
                var pc = _player.GetComponent<PlayerController>();
                if (pc._sensoryOverload)
                {
                    if (mic.pitch != 0.5f)
                        mic.pitch = 0.5f;
                    cap = 2.5f;
                }
                else
                {
                    if (mic.pitch != 1.0f)
                        mic.pitch = 1.0f;
                }

                Debug.LogFormat("Velocity: {0}", _rigidbody.velocity);
                var velocity = _rigidbody.velocity;
                if (velocity.x > cap)
                    velocity.x = cap;
                else if (velocity.x < -cap) 
                    velocity.x = -cap;
                if (velocity.y > cap)
                    velocity.y = cap;
                else if (velocity.y < -cap)
                    velocity.y = -cap;
                _rigidbody.velocity = velocity;

                if (_secondaryCollider.enabled && _secondaryCollider.bounds.Contains(_player.transform.position))
                {
                    pc.ProvideSensoryEffect(controller.TasteFactor, controller.SmellFactor, controller.SightFactor, controller.HearingFactor * 1.05f, controller.TouchFactor);
                }
                sensoryTimer += Time.deltaTime;
                if (sensoryTimer >= 3.0f)
                {
                    sensoryTimer = 0;
                    pc.ProvideSensoryEffect(controller.TasteFactor, controller.SmellFactor, controller.SightFactor, controller.HearingFactor, controller.TouchFactor);
                }
            }

            if (_isDead)
            {
                mic.Stop();
                _rigidbody.velocity = Vector2.zero;
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("Base Layer.Death") && state.normalizedTime > 1.0f)
                {
                    //Debug.LogWarning("Mikey is going away");
                    var mgr = GameObject.FindGameObjectWithTag("LvlMgr");
                    var levelManager = mgr == null ? null : mgr.GetComponent<LevelManager>();
                    if (levelManager != null)
                    {
                        levelManager.SignalEnemyDied();
                    }
                    _player.GetComponent<PlayerController>().ReduceDirectStimulation(12.5f);
                    DestroyImmediate(this.gameObject);
                }
            }
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        //if(col.otherCollider.tag.Contains("Detection"))
        //    Debug.LogFormat("MIKEY - Collided with {0}", col.gameObject.tag);

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
            Debug.LogFormat("Mikey collision - {0} and {1}", col.collider.tag, this.tag);
            if (_running)
            {
                col.gameObject.GetComponent<PlayerController>().HurtPlayer((int)controller.AttackDamage, true, controller.TasteFactor, controller.SmellFactor, controller.SightFactor, controller.HearingFactor, controller.TouchFactor);
                controller.Health -= controller.Health;
                _isDead = true;
                _runningAway = false;
                _found = false;
                
                //Destroy(this);
            }
        }
    }

    public override void PerformBehavior()
    {
        _found = true;
    }

}
