using UnityEngine;

public class AJController : EnemyController
{
    [Header("Durations (s)")]
    public float StunDurationInSeconds;
    public float HurtDurationInSeconds;
    public float Attack1Duration;

    private GameManager mgr;
    private bool stepForward;
    private bool deathTriggered;
    private float _stunTimer = 0;
    public BossState _state;
    private BossState _previousState = BossState.IDLE;
    private float _hurtTimer = 0;
    private bool largeShockSpawn;
    private float _cooldown = 0;
    private float _actionTimer = 0;
    private bool loaded = false;

    [Header("Attack Specific Info")]
    public Transform ShockwaveSpawnTransform;
    public GameObject ShockwavePrefab;
    public float DurationToDetermineAttack;

    [Header("Attack 1")]
    public float Attack1CooldownTime;
    public float Attack1Speed;

    [Header("Attack 2")]
    public float Attack2CooldownTime;

    [Header("Attack 3")]
    public float Attack3CooldownTime;
    public Vector2 StopPositionForAttack3;
    public float Attack3Magnitude;  

    [Header("State and other stuff")]
    public bool Hurt;
    public bool Dead { get { return Health <= 0; } }

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Door _completionDoor;

    [SerializeField]
    private Travelpoint _completionTravelpoint;

    private void Awake()
    {
        _state = BossState.LOADING;
        _completionDoor.DoorOpened = false;
        _completionDoor.DoorEnabled = true;
    }

    private void Update()
    {
        Init();

        if (mgr != null && mgr.state == GameState.PLAYING && !mgr.GetPlayer().Dead)
            stepForward = true;
        else
            stepForward = false;

        if (!Dead && stepForward)
        {
            switch (_state)
            {
                case BossState.ATTACKONE:
                    {
                        HandleAttackOne();
                        break;
                    }
                case BossState.STUN:
                    {
                        HandleStun();
                        break;
                    }
                case BossState.HURT:
                    {
                        HandleHurt();
                        break;
                    }
                case BossState.DYING:
                    {
                        if (!deathTriggered)
                        {
                            deathTriggered = true;
                            _animator.SetTrigger("Dying");
                        }
                        break;
                    }
                case BossState.IDLE:
                    {
                        HandleIdle();
                        break;
                    }
                //case BossState.DEBUG:
                //    {
                //        DebugAttackOne();
                //        break;
                //    }
                default:
                    {
                        _state = BossState.IDLE;
                        break;
                    }
            }


            if(_state == BossState.LOADING)
                _animator.SetInteger("State", (int)BossState.IDLE);
            else
                _animator.SetInteger("State", (int)_state);
        }

        if (Dead)
            HandleDeath();
    }

    private void Init()
    {
        if (!loaded)
        {
            var mg = GameObject.FindGameObjectWithTag("GameManager");
            if (mg != null)
            {
                var manager = mg.GetComponent<GameManager>();
                if (manager != null)
                {
                    mgr = manager;
                    _completionTravelpoint.SetDelegate(CompleteBossBattle, mgr.playerData);
                    loaded = true;
                    _state = BossState.IDLE;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Soaker") && !Hurt)
        {
            Debug.Log("Collision");
            base.Health -= 5;
            Hurt = true;
            _previousState = _state;
            _state = BossState.HURT;
        }
    }

    private void HandleStun()
    {
        _stunTimer += Time.deltaTime;
        if (_stunTimer >= StunDurationInSeconds)
        {
            _stunTimer = 0;
            _state = BossState.IDLE;
            return;
        }

    }

    private void HandleDeath()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dead") && _animator.GetCurrentAnimatorStateInfo(0).length >= 1.0f)
        {
            _completionDoor.DoorOpened = true;
        }
    }

    private void HandleHurt()
    {
        _animator.SetTrigger("Hurt");


        Hurt = false;
        if (Dead)
            _state = BossState.DYING;
        else
            _state = _previousState;
    }

    private void CompleteBossBattle(PlayerData data)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var pc = player.GetComponent<PlayerController>();
        var pd = pc.UpdatePlayerData(data, data.Level + 1);
        mgr.playerData = pd;
        mgr.DisableGameUI();

        mgr.TransitionToLevel(-10);
    }

    //private void DebugAttackOne()
    //{
    //    if (_cooldown == 0.0f)
    //    {
    //        var pos = new Vector3(ShockwaveSpawnTransform.position.x, ShockwaveSpawnTransform.position.y, 0);
    //        var sw = Instantiate(ShockwavePrefab, pos, Quaternion.identity, ShockwaveSpawnTransform);
    //        var playerTransform = mgr.GetPlayer().transform;
    //        Vector3 direction = playerTransform.position - sw.transform.position;
    //        var rot = Quaternion.LookRotation(Vector3.forward, -direction);
    //        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, 1.6f, 1.0f);
    //        //sw.transform.Rotate(newDirection);
    //        sw.transform.rotation = rot;
    //        sw.GetComponent<Shockwave>().Velocity = (direction * Attack1Speed * Time.deltaTime);
    //        _cooldown += 0.001f;
    //    }
    //    else
    //    {
    //        _cooldown += Time.deltaTime;
    //        if (_cooldown >= Attack1CooldownTime)
    //        {
    //            _cooldown = 0.0f;
    //            _state = BossState.IDLE;
    //        }
    //    }
    //}

    private void HandleAttackOne()
    {
        if (_cooldown == 0.0f)
        {
			var pos = new Vector3(ShockwaveSpawnTransform.position.x, ShockwaveSpawnTransform.position.y, 0);
			var sw = Instantiate(ShockwavePrefab, pos, Quaternion.identity, ShockwaveSpawnTransform);
			var playerTransform = mgr.GetPlayer().transform;
			Vector3 direction = playerTransform.position - sw.transform.position;
			var rot = Quaternion.LookRotation(Vector3.forward, -direction);
			sw.transform.rotation = rot;
			sw.GetComponent<Shockwave>().Velocity = (direction * Attack1Speed);
            sw.GetComponent<Shockwave>().TimeToLive = 7;
			_cooldown += 0.001f;
		}
        else
        {
            _cooldown += Time.deltaTime;
            if (_cooldown >= Attack1CooldownTime)
            {
                _cooldown = 0.0f;
                _state = BossState.IDLE;
            }
        }    
    }

    private void HandleIdle()
    {
        _actionTimer += Time.deltaTime;

        if (_actionTimer >= DurationToDetermineAttack)
        {
            int value = Random.Range(1, 5);
            if (value == 4)
                _state = BossState.CHARGING;
            else
                _state = (BossState)value;

            _actionTimer = 0.0f;
        }
    }

}
