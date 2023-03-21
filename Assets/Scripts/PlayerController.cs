using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //public GameObject PrefabForGameManager;
    public PlayerInput PlayerInputComponent;
    
    public float ControlRateForYippeeMeter;
    public float ControlRateForHealth;

    public float YippeeMeter;
    public float SensoryMeter;
    public float Health;
    public float MovementScale;

    public bool IsAllowedToMove;

    public Sprite playerSprite;

    public List<Item> Inventory = new();
    private Item BodyEquipment;
    private Item WeaponA;
    private Item WeaponB;
    private int DabloonCount = 0;

    public float speed;
    public Rigidbody2D rb2d;
    //private Transform transform;
    private Vector2 movement;


    // Start is called before the first frame update
    void Start()
    {
        IsAllowedToMove = false;
        GetComponent<SpriteRenderer>().sprite = playerSprite;
        //transform = GetComponent<Transform>();
        movement = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += (Vector3)movement;
    }

    private void FixedUpdate()
    {
        rb2d.velocity = movement;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>() * speed;
        Debug.Log($"{movement.x},{movement.y}");
    }

}
