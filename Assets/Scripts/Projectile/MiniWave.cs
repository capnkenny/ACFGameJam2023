using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiniWave : MonoBehaviour
{
    [Header("MiniWave-specific items")]
    public float RedLightChangeInSeconds;
    public float YellowLightChangeInSeconds;
    public float GreenLightChangeInSeconds;
    public float LightOffChangeInSeconds;
    public Collider2D Collider;
    public Light2D Red;
    public Light2D Yellow;
    public Light2D Green;
    public float MoveSpeed;
    public Vector2 Direction;
	public int Damage;
	public float TimeToLive;
	public float ImpactForce;
	public bool SensoryEffect;
	public float SensoryEffectValue;
    public Rigidbody2D rb2d;

	private float timer = 0.0f;
    private float healthTimer = 0.0f;


    private void Awake()
    {
        Red.intensity = 0;
        Yellow.intensity = 0;
        Green.intensity = 0;
    }

    private void Start()
    {
    }


    private void Update()
    {
        timer += Time.deltaTime;
        healthTimer += Time.deltaTime;

        if (timer >= YellowLightChangeInSeconds)
        {
            Yellow.intensity = 1.0f;
        }
        if (timer >= GreenLightChangeInSeconds)
        {
            Yellow.intensity = 1.0f;
        }
        if (timer >= RedLightChangeInSeconds)
        {
            Red.intensity = 1.0f;
        }
        if (timer >= LightOffChangeInSeconds)
        {
            timer = 0.0f;
            Red.intensity = 0;
            Yellow.intensity = 0;
            Green.intensity = 0;
        }

        //transform.Translate(Direction * MoveSpeed * Time.deltaTime);
        if (healthTimer >= TimeToLive)
            DestroyImmediate(this.gameObject);

    }

    public void SetDirection(Vector2 direction)
    {
        rb2d.velocity = (direction * MoveSpeed);
        Direction = direction;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();
            if (Damage <= 0)
                pc.HealPlayer(Damage, SensoryEffectValue);
            else
                pc.HurtPlayer(Damage, SensoryEffect, 0, 0, 0, 0, SensoryEffectValue);
            Destroy(this.gameObject);
        }
    }

}
