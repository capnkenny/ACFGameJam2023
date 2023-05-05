using UnityEngine;

public class Projectile : MonoBehaviour
{
	public int Damage;
	public float TimeToLive;
    public float ImpactForce;
    public bool SensoryEffect;

	private bool disposable = false;
    private float timer = 0.0f;


    private void Awake()
    {
        if (TimeToLive > 0)
        {
            disposable = true;
        }
    }

    private void Update()
    {
        if (disposable)
        {
            timer += Time.deltaTime;
            if (timer > TimeToLive)
            {
                DestroyImmediate(this);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();
            var v = collision.otherRigidbody.velocity;
            var force = v * new Vector2(ImpactForce, ImpactForce);
            collision.rigidbody.AddForce(force);
            pc.HurtPlayer(Damage, SensoryEffect);
            Destroy(this);
        }
    }


}
