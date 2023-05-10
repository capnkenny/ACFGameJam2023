using UnityEngine;

public class Projectile : MonoBehaviour
{
	public int Damage;
	public float TimeToLive;
    public float ImpactForce;
    public bool SensoryEffect;
    public float SensoryEffectValue;

	private bool disposable = true;
    private bool destroy = false;
    //private float timer = 0.0f;


    private void Start()
    {
        //GameObject.Destroy(this, TimeToLive);
    }

    private void Update()
    {
        if(destroy)
            GameObject.DestroyImmediate(this.gameObject);
        //if (disposable)
        //{
        //    TimeToLive -= Time.deltaTime;
        //    if (TimeToLive <= 0)
        //    {
        //        Debug.LogWarning("Destroying projectile");
        //        DestroyImmediate(this);
        //    }
        //}
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
            destroy = true;
        }
    }


}
