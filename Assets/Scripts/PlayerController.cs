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
    public int DabloonCount;
    public List<Item> Inventory;

    [Header("Predefined Controls")]
    public bool Visible;
    public float MovementSpeed;
    public float YippeeMeterFactor;
    public float MaxYippeeMeter;
    public float MaxHealth;
    public float MaxSensoryMeter;

    //Randomized / Internal Player Stats
    private float _tasteFactor;
    private float _smellFactor;
    private float _sightFactor;
    private float _hearingFactor;
    private float _touchFactor;
    private bool _sensoryOverload;
    
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

}
