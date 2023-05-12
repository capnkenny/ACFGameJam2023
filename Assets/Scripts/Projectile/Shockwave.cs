using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shockwave : Projectile
{
    [Header("Shockwave-specific items")]
    public float RedLightChangeInSeconds;
    public float YellowLightChangeInSeconds;
    public float GreenLightChangeInSeconds;
    public float LightOffChangeInSeconds;
    public Collider2D Collider;
    public Rigidbody2D rb2d;
    public Light2D Red;
    public Light2D Yellow;
    public Light2D Green;
    public Vector2 Velocity;

    private float timer = 0.0f;


    private void Awake()
    {
        Red.intensity = 0;
        Yellow.intensity = 0;
        Green.intensity = 0;
    }

    private void Start()
    {
        rb2d.velocity = Velocity;
    }


    private void Update()
    {
        timer += Time.deltaTime;

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

        var camcol = Camera.main.GetComponentInChildren<BoxCollider2D>();
        if (camcol != null)
        {   
            if (!camcol.bounds.Contains(transform.position))
                DestroyImmediate(this.gameObject);
        }

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
