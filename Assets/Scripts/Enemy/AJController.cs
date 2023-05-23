using UnityEngine;

public class AJController : EnemyController
{
    [Header("Music Objects")]
    public AudioSource BossMusic;
    public AudioSource ShockSound;
	public AudioSource MiniShockSound;
    public AudioSource DizzySound;
    public AudioSource PopSound;

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
    public float DelayBetweenWaves;
    public int WavesWaveCount;
    private float attack2Delay = 0.0f;
    private int waveClock = 0;

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

    [SerializeField]
    private MiniwaveManager _waveMgr;
	[SerializeField]
	private SensoryBar _healthBar;

    private float musicPitch;
    private float soundPitch;

	private void Awake()
    {
        _state = BossState.LOADING;
        
        musicPitch = BossMusic.pitch;
        soundPitch = ShockSound.pitch;
    }

    private void Update()
    {
        Init();

        if (mgr != null && mgr.state == GameState.PLAYING && !mgr.GetPlayer().Dead)
        {
            stepForward = true;
        }
        else
            stepForward = false;

        if (!Dead && stepForward)
        {
            _healthBar.SensoryValue = Health;
            switch (_state)
            {
                case BossState.ATTACKONE:
                    {
                        HandleAttackOne();
                        break;
                    }
                case BossState.ATTACKTWO:
                    {
                        HandleAttackTwo();
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
						else if (_animator.GetCurrentAnimatorStateInfo(0).length >= 1.0f)
						{
							DestroyImmediate(this.gameObject);
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


            if (_state == BossState.LOADING)
                _animator.SetInteger("State", (int)BossState.IDLE);
            else
            {
                _animator.SetInteger("State", (int)_state);
                if (mgr.PlayerInSensoryOverload)
                {
                    if(BossMusic.pitch != musicPitch / 2) BossMusic.pitch = musicPitch / 2;
                    if(ShockSound.pitch != soundPitch / 2) ShockSound.pitch = soundPitch / 2;
                    if (MiniShockSound.pitch != soundPitch / 2) MiniShockSound.pitch = soundPitch / 2;
                }
                else
                {
					if (BossMusic.pitch != musicPitch) BossMusic.pitch = musicPitch;
					if (ShockSound.pitch != soundPitch) ShockSound.pitch = soundPitch;
					if (MiniShockSound.pitch != soundPitch) MiniShockSound.pitch = soundPitch;
				}
            }


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
					_completionDoor.DoorOpened = false;
					_completionDoor.DoorEnabled = true;
					_completionTravelpoint.SetDelegate(mgr.CompleteBossBattle, mgr.playerData);
                    manager.EnableGameUI();
                    loaded = true;
                    _healthBar.UpdateSlider(Health, Health);
                    _state = BossState.IDLE;
                    BossMusic.Play();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.tag);

         if (collision.collider.CompareTag("Soaker"))
		{
            if (!Hurt)
            {
                Health -= 5;
                Hurt = true;
                _previousState = _state;
                _state = BossState.HURT;
            }
        }
    }

    private void HandleStun()
    {
        if (!DizzySound.isPlaying)
        {
            DizzySound.Play();
        }
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
        Debug.Log("Airhorn Jim is dead");
        mgr.DisableGameUI();
        BossMusic.Stop();
        ShockSound.Stop();
        MiniShockSound.Stop();
        _completionDoor.DoorOpened = true;
        _state = BossState.DYING;
        PopSound.Play();
		DestroyImmediate(this.gameObject);
	}

    private void HandleHurt()
    {
        _animator.SetTrigger("Hurt");

        if (Dead)
            _state = BossState.DYING;
        else
        {
            if (_hurtTimer >= HurtDurationInSeconds)
            {
                _state = _previousState;
                Hurt = false;
                _hurtTimer = 0;
            }
            else
            {
				_hurtTimer += Time.deltaTime;
			}
        }
    }

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
            ShockSound.Play();
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

	private void HandleAttackTwo()
	{
        if (attack2Delay == 0.0f)
        {
            _waveMgr.FireWave();
            MiniShockSound.Play();
            attack2Delay += 0.001f;
        }
        else
        {
			attack2Delay += Time.deltaTime;
		}
            
        if (attack2Delay >= DelayBetweenWaves)
        {
            attack2Delay = 0.0f;
            waveClock++;
        }

        if (waveClock >= WavesWaveCount)
        {
            waveClock = 0;
            _state = BossState.STUN;
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
