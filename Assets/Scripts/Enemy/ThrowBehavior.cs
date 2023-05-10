using System.Collections.Generic;
using UnityEngine;

public class ThrowBehavior : EnemyBehavior
{
    public List<GameObject> AllowedItemPrefabsToThrow;
    public int SecondsToThrow;
    public float ItemThrownThreshold;
    public float Magnitude;
    public float Torque;
    public float YPosition;
    public bool throwing;
    public bool hurt;
    public AudioSource source;
    public List<AudioClip> ThrowSounds;
    
    private int numberToThrow;
    private float timer;
    private float timer2;

    void Awake()
    {
        timer = 0;
        timer2 = 0;
    }

    private void Start()
    {
     
    }

    public void EndEarly()
    {
        timer2 = SecondsToThrow;
    }

    void Update()
    {
        if (throwing)
        {
            timer += Time.deltaTime;
            timer2 += Time.deltaTime;
            if (timer >= ItemThrownThreshold)
            {
                timer = 0.0f;
                int i = Random.Range(0, AllowedItemPrefabsToThrow.Count);
                int a = Random.Range(0, ThrowSounds.Count - 1);
                if (i == 3)
                {
                    int r = Random.Range(1, 10);
                    if (r % 7 != 0)
                        i = Random.Range(0, 3);
                    else
                        a = ThrowSounds.Count - 1;
                }
                var position = transform.position;
                position.y += YPosition;

                int x = Random.Range(1, 3) % 2 == 0 ? -3 : 3;
                Vector2 force = (new Vector2(x, 10).normalized * Magnitude);

                //get player
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null)
                {
                    var pc = p.GetComponent<PlayerController>();
                    if (pc != null && pc._sensoryOverload)
                    {
                        source.pitch = 0.5f;
                        source.volume = 0.25f;
                        force = force / 2;
                    }
                    else if (pc != null)
                    {
                        source.pitch = 1.0f;
                        source.volume = 0.5f;
                    }
                }

                source.PlayOneShot(ThrowSounds[a]);
                var item = Instantiate(AllowedItemPrefabsToThrow[i], position, Quaternion.identity);
                Destroy(item, 6.0f);
                
                item.GetComponent<Rigidbody2D>().AddForce(force);
                if (x < 0)
                    item.GetComponent<Rigidbody2D>().AddTorque(Torque);
                else
                    item.GetComponent<Rigidbody2D>().AddTorque(-Torque);
                //Debug.Log($"Throwing {item.name}");
            }

            if (timer2 >= SecondsToThrow)
            {
                throwing = false;
                timer2 = 0;
                timer = 0;
            }
        }
    }

    public override void PerformBehavior()
    {
        if (!throwing && !hurt)
        {
            numberToThrow = Random.Range(1, AllowedItemPrefabsToThrow.Count);
            throwing = true;
        }
    }

}
