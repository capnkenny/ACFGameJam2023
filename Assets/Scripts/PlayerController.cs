using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("GameObject References")]
    public PlayerInput PlayerInputComponent;
    public Item BodyEquipment;
    public Item RangedWeapon;
    public Item MeleeWeapon;
    public Sprite PlayerSprite;
    public Animator animator;
    public new SpriteRenderer renderer;
    public Rigidbody2D rb2d;
    private Light2D l2d;

    [Header("Base Player Stats")]
    public float Health;
    public float SensoryMeter;
    public float YippeeMeter;
    public int Koiency;
    public List<Item> Inventory;

    [Header("Predefined Controls")]
    public bool Visible;
    public float MovementSpeed;
    public float YippeeMeterFactor;
    public float MaxYippeeMeter;
    public float MaxHealth;
    public float MaxSensoryMeter;
    public float MaxShield;

    //Randomized / Internal Player Stats
    public float _tasteFactor;
    public float _smellFactor;
    public float _sightFactor;
    public float _hearingFactor;
    public float _touchFactor;
    private bool _sensoryOverload;
    private int _soakerLevel;
    private Color originalColor;
    private float originalIntensity;
    private int currentLevel = 1;

    public bool Hurt = false;
    public bool Dead = false;
    private float hurtTimer = 3.0f;
    private Direction dir;
    
    //Private members
    public Vector2 movement;
    private GameManager mgr;

    void Awake()
    {
        //renderer.sprite = PlayerSprite;
        originalColor = renderer.color;
        l2d = GetComponentInChildren<Light2D>();
        originalIntensity = l2d.intensity;
        movement = new Vector2(0,0);
        _sensoryOverload = false;
    }

    // Start is called before the first frame update
    void Start()
    {
	//If inventory not provided already, we should clear it
	if(Inventory == null)
	    Inventory = new List<Item>();
	
	//Randomize senses to give uniqueness to character/playthrough
        _tasteFactor = Random.Range(0.0f, 1.5f);
        _smellFactor = Random.Range(0.0f, 1.5f);
        _sightFactor = Random.Range(0.0f, 1.5f);
        _hearingFactor = Random.Range(0.0f, 1.5f);
	    _touchFactor = Random.Range(0.0f, 1.5f);

        mgr = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mgr.state == GameState.PLAYING || mgr.state == GameState.HUB)
        {
            //animator.SetBool("Hurt", Hurt);
            animator.SetBool("Dead", Dead);
            animator.SetInteger("VelocityX", (int)movement.x);
            animator.SetInteger("VelocityY", (int)movement.y);
            animator.SetInteger("Direction", ((int)dir));
            


            if (!_sensoryOverload)
            {
                if (SensoryMeter >= MaxSensoryMeter)
                {
                    SensoryMeter = MaxSensoryMeter;
                    _sensoryOverload = true;
                }
            }

            if (!Dead)
            {
                if (_sensoryOverload)
                {
                    rb2d.velocity = (movement / 2);
                    if (SensoryMeter <= 0)
                    {
                        _sensoryOverload = false;
                        SensoryMeter = 0;
                    }
                }
                else
                    rb2d.velocity = movement;
            }

            if (Hurt && !Dead && mgr.state == GameState.PLAYING)
            {
                renderer.color = new Color(Mathf.Cos(hurtTimer / 3.0f)+1, 0, 0);
                l2d.intensity = 1 * (hurtTimer / 3.0f);
                hurtTimer -= Time.deltaTime;

                if (hurtTimer <= 0)
                {
                    l2d.intensity = originalIntensity;
                    renderer.color = originalColor;
                    hurtTimer = 3.0f;
                    Hurt = false;
                }
            }
            
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!Dead)
        {
            movement = context.ReadValue<Vector2>() * MovementSpeed;
            if (movement.x > 0) // right
                dir = Direction.EAST;
            else if (movement.x < 0)
                dir = Direction.WEST;
            else if (movement.y > 0)
                dir = Direction.NORTH;
            else if (movement.y < 0)
                dir = Direction.SOUTH;
        }
    }

    public void AddSensoryInput(float taste, float smell, float sight, float hearing, float touch)
    {
        _tasteFactor += taste;
        _smellFactor += smell;
        _sightFactor += sight;
        _hearingFactor += hearing;
        _touchFactor += touch;
    }

    public void RemoveSensoryInput(float taste, float smell, float sight, float hearing, float touch)
    {
        _tasteFactor -= taste;
        _smellFactor -= smell;
        _sightFactor -= sight;
        _hearingFactor -= hearing;
        _touchFactor -= touch;

	 if(_tasteFactor < -1.0f)
		_tasteFactor = -1.0f;
    }

    public void SetPlayerStats(PlayerData data)
    {
        MaxHealth = data.MaxHealth;
        MaxSensoryMeter = data.MaxSensory;
        MaxShield = data.MaxShield;
        SensoryMeter = data.CurrentSensory;
        Health = data.CurrentHealth;

        _tasteFactor = data.Taste;
        _smellFactor = data.Smell;
        _sightFactor = data.Sight;
        _hearingFactor = data.Hearing;
        _touchFactor = data.Touch;
        _soakerLevel = data.SoakerLevel;
        currentLevel = data.Level;
    }

    public void HurtPlayer(int dmg, bool sensoryModifier, float tf = 0, float smf = 0, float sif = 0, float hf = 0, float tof = 0)
    {
        float sensoryMod = 0;
        if (!Hurt)
        {
            int damage = dmg;
            
            if (sensoryModifier)
            {
                sensoryMod = ProvideSensoryEffect(tf, smf, sif, hf, tof);
                damage = (int)(dmg * 1.25) + (int)(sensoryMod);
            }
            Health -= damage;
            Hurt = true;
            if (Health <= 0)
            {
                Hurt = false;
                Dead = true;
                mgr.SignalPlayerDeath();
            }
        }

    }

    public float ProvideSensoryEffect(float tf = 0, float smf = 0, float sif = 0, float hf = 0, float tof = 0)
    {
        float effect = (_tasteFactor * tf) + (_smellFactor * smf) + (_sightFactor * sif) + (_hearingFactor * hf) + (_touchFactor * tof);
        if(SensoryMeter < MaxSensoryMeter && !_sensoryOverload)
            SensoryMeter += effect;
        return effect;
    }

    public void ReduceDirectSensoryEffect(float tf = 0, float smf = 0, float sif = 0, float hf = 0, float tof = 0)
    {
        float effect = (_tasteFactor * tf) + (_smellFactor * smf) + (_sightFactor * sif) + (_hearingFactor * hf) + (_touchFactor * tof);
        SensoryMeter -= effect;
    }

    public PlayerData GetPlayerData()
    {
        PlayerData p = new PlayerData
        {
            Taste = _tasteFactor,
            Touch = _touchFactor,
            Smell = _smellFactor,
            Sight = _sightFactor,
            Hearing = _hearingFactor,
            MaxHealth = MaxHealth,
            MaxSensory = MaxSensoryMeter,
            MaxShield = MaxShield,
            CurrentHealth = Health,
            Koiency = Koiency,
            CurrentSensory = SensoryMeter,
            Level = currentLevel
        };
        return p;
    }

    public PlayerData UpdatePlayerData(PlayerData reference, int level = -1)
    {
        PlayerData newData = reference;

        newData.MaxHealth = MaxHealth;
        newData.MaxSensory = MaxSensoryMeter;
        newData.MaxShield = MaxShield;
        newData.CurrentHealth = Health;
        newData.SoakerLevel = _soakerLevel;
        newData.Koiency = Koiency;
        newData.CurrentSensory = SensoryMeter;

        if (level != -1 && level != reference.Level)
        {
            newData.Level = level;
            currentLevel = level;
        }

        return newData;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Dead)
        {
            if (collision.collider.CompareTag("Projectile"))
            {
                var force = (collision.rigidbody.velocity * 2).normalized;
                rb2d.AddRelativeForce(force);
            }
            if (collision.collider.CompareTag("Enemy"))
            {

                Vector2 force;
                if (movement == Vector2.zero)
                    force = -(collision.rigidbody.velocity * 2);
                else
                    force = -(movement * 100).normalized;
                Debug.LogFormat("Collided with {0} - Applied Force: {1}", collision.gameObject.name, force);
                rb2d.AddRelativeForce(force);
            }
        }

    }

}
