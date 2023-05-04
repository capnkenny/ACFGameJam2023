using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("GameObject References")]
    public PlayerInput PlayerInputComponent;
    public Item BodyEquipment;
    public Item RangedWeapon;
    public Item MeleeWeapon;
    public Sprite PlayerSprite;
    public SpriteRenderer renderer;
    public Rigidbody2D rb2d;

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
    private float _tasteFactor;
    private float _smellFactor;
    private float _sightFactor;
    private float _hearingFactor;
    private float _touchFactor;
    private bool _sensoryOverload;
    private int _soakerLevel;

    
    //Private members
    private Vector2 _movement;

    void Awake()
    {
        renderer.sprite = PlayerSprite;
        _movement = new Vector2(0,0);
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Visible)
        {
                rb2d.velocity = _movement;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>() * MovementSpeed;
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
        }

        return newData;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogFormat("Player collision - {0} and {1}", collision.collider.tag, this.tag);
        if (collision.gameObject.CompareTag("Untagged"))
            Debug.Log(collision.gameObject.name);
        //if (collision.gameObject.CompareTag("RoomDetection"))
        //{
        //    //Debug.Log("ITSSSSSSSSSSSS SHOWTIME :^)");
        //    var room = collision.gameObject.GetComponentInParent<Room>();
        //    room.SignalPlayerInside();
        //}
        //if (collision.gameObject.CompareTag("Travel"))
        //{
        //    var tp = collision.gameObject.GetComponent<Travelpoint>();
        //    tp.InvokeDelegate();
        //}
    }

}
