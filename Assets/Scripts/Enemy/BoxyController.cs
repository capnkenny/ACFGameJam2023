using UnityEngine;

public class BoxyController : EnemyController
{
    public Animator animator;
    public bool Throw = false;
    public bool Hurt = false;
    private bool Dead = false;
    public float SecondsToWait;
    public float CoolDownTime;

    private float timer = 0;
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
                animator.SetTrigger("Hurt");
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
            if (state.IsName("Base Layer.Dead") && state.normalizedTime > 1.0f)
            {
                var mgr = GameObject.FindGameObjectWithTag("LvlMgr");
                var levelManager = mgr == null ? null : mgr.GetComponent<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.SignalEnemyDied();
                }
                GameObject.DestroyImmediate(this.gameObject);
            }
        }

    }


}
