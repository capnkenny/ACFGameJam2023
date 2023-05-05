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
                if (i == 3)
                {
                    int r = Random.Range(1, 10);
                    if (r % 7 != 0)
                        i = Random.Range(0, 3);
                }
                var position = transform.position;
                position.y += YPosition;
                var item = Instantiate(AllowedItemPrefabsToThrow[i], position, Quaternion.identity);
                int x = Random.Range(1, 3) % 2 == 0 ? -3 : 3;
                Vector2 force = (new Vector2(x, 10).normalized * Magnitude);
                Debug.Log(force);
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
        if (!throwing)
        {
            numberToThrow = Random.Range(1, AllowedItemPrefabsToThrow.Count);
            throwing = true;
        }
    }

}
