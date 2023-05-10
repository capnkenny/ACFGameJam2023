using UnityEngine;

public class BoxyController : EnemyController
{
    public Animator animator;
    public bool Throw = false;
    public bool Hurt = false;
    private bool Dead = false;
    public float SecondsToWait;
    public float CoolDownTime;
    public float HurtCoolDownTime;

    private float timer = 0;
    private float hurtTimer = 0;
    private float coolDown = 0;
    private bool cooldownMode = false;

    private GameManager mgr;
    
    private void Awake()
    {
        var mg = GameObject.FindGameObjectWithTag("GameManager");
        if (mg != null) mgr = mg.GetComponent<GameManager>();
    }

    private void Update()
    {        
        if (!Dead && mgr != null && !mgr.GetPlayer().Dead)
        {
            animator.SetBool("Spraying", ((ThrowBehavior)AttackBehavior).throwing);

            if(!cooldownMode)
                timer += Time.deltaTime;

            if (timer >= SecondsToWait) 
            {
                Throw = true;
                timer = 0;
            }
            if (Throw)
            {
                AttackBehavior.PerformBehavior();
                Throw = false;
                cooldownMode = true;
                return;
            }

            if (cooldownMode)
            {
                coolDown += Time.deltaTime;
                if (coolDown >= CoolDownTime)
                {
                    cooldownMode = false;
                    coolDown = 0;
                }
            }

            if (Hurt)
            {
                hurtTimer = 0.0f;
                animator.SetTrigger("Hurt");
                
                if (((ThrowBehavior)AttackBehavior).throwing)
                    ((ThrowBehavior)AttackBehavior).EndEarly();
                Throw = false;
                Hurt = false;
            }
            else if (hurtTimer > 0.0f)
            {
                hurtTimer += Time.deltaTime;
                if (hurtTimer >= HurtCoolDownTime)
                {
                    Hurt = false;
                    hurtTimer = 0.0f;
                }
            }



            if (Health <= 0)
            {
                Dead = true;
                animator.SetBool("IsDead", Dead);
                return;
            }
        }
        else
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Base Layer.Dead") && state.normalizedTime >= 1.0f)
            {
                var mg = GameObject.FindGameObjectWithTag("LvlMgr");
                var levelManager = mg == null ? null : mg.GetComponent<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.SignalEnemyDied();
                }
                var p = mgr.GetPlayer();
                if (p != null)
                {
                    p.ReduceDirectStimulation(12.5f);
                }
                DestroyImmediate(this.gameObject);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Soaker"))
        {
            Debug.Log("Collision");
            base.Health -= 5;
            Hurt = true;
        }
    }
}
