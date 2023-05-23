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
    [SerializeField]
    private InputActionMap actions;

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
    public bool _sensoryOverload;
    private int _soakerLevel;
    private Color originalColor;
    private float originalIntensity;
    private int currentLevel = 1;

    public bool Hurt = false;
    public bool Dead = false;
    public bool Attacking = false;
    public bool AtkTrigger = false;
    private float hurtTimer = 3.0f;
    private float AtkTimer = 0.5f;
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


        var mgrO = GameObject.FindGameObjectWithTag("GameManager");
        if (mgrO != null)
        {
            mgr = mgrO.GetComponent<GameManager>();
        }

        InputSystem.onActionChange += OnInput;
    }

    private void OnInput(object arg1, InputActionChange arg2)
    {
        if (!Dead)
        {
            if (arg1 is InputAction)
            {
                var action = (InputAction)arg1;
                //Debug.Log(action.name);
                if (arg2 == InputActionChange.ActionStarted && action.name.Contains("attack"))
                {
                    Attacking = true;
                }
            }
        }
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
                    rb2d.velocity = (movement * 0.75f);
                    if (SensoryMeter <= 0)
                    {
                        _sensoryOverload = false;
                        SensoryMeter = 0;
                    }
                }
                else
                    rb2d.velocity = movement;

                if (Attacking)
                {
                    if (!AtkTrigger)
                    {
                        AtkTrigger = true;
                        animator.SetTrigger("Attacking");
                    }
                    else
                    {
                        AtkTimer -= Time.deltaTime;
                        if (AtkTimer <= 0.0f)
                        {
                            Attacking = false;
                            AtkTrigger = false;
                            AtkTimer = 0.5f;
                        }
                    }
                }
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
            var vec = context.ReadValue<Vector2>();
            //if (vec.x > 0 || vec.x < 0)
            //{
            //    vec.y = 0;
            //}
            //else if (vec.y > 0 || vec.y < 0)
            //{
            //    vec.x = 0;
            //}

            movement = vec * MovementSpeed;
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

    public void HealPlayer(int dmg, float sens)
    {
        Health += dmg;
        ReduceDirectStimulation(sens);
        Debug.Log($"New Health: {Health}");
    }

    public float ProvideSensoryEffect(float tf = 0, float smf = 0, float sif = 0, float hf = 0, float tof = 0)
    {
        float effect = (_tasteFactor * tf) + (_smellFactor * smf) + (_sightFactor * sif) + (_hearingFactor * hf) + (_touchFactor * tof);
        if(SensoryMeter < MaxSensoryMeter && !_sensoryOverload)
            SensoryMeter += effect;
        Debug.LogWarning($"Sensory Effect added - {effect}");
        return effect;
    }

    public void ReduceDirectSensoryEffect(float tf = 0, float smf = 0, float sif = 0, float hf = 0, float tof = 0)
    {
        float effect = (_tasteFactor * tf) + (_smellFactor * smf) + (_sightFactor * sif) + (_hearingFactor * hf) + (_touchFactor * tof);
        SensoryMeter -= effect;
        if (SensoryMeter < 0)
        {
            SensoryMeter = 0;
            _sensoryOverload = false;
        }
    }

    public void ReduceDirectStimulation(float value)
    {
        SensoryMeter -= value;
        if (SensoryMeter < 0)
        {
            SensoryMeter = 0;
            _sensoryOverload = false;
        }
		Debug.Log($"New Sensory: {SensoryMeter}");
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
        }

    }

}
