using UnityEngine;

public class Projectile : MonoBehaviour
{
	public int Damage;
	public float TimeToLive;
    public float ImpactForce;

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
            var p = collision.rigidbody.velocity;
            var v = collision.otherRigidbody.velocity;
            if (p == null)
            {
                Debug.LogError("Didn't get velocity!");
            }
            else
            {
                //Knockback
                var force = v * new Vector2(ImpactForce, ImpactForce);
                collision.rigidbody.AddForce(force);
                collision.otherRigidbody.velocity = -v;
            }
            
        }
    }


}
