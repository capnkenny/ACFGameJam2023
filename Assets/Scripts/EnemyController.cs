using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{

    [Header("GameObjects")]
    public SpriteRenderer SpriteRenderer;
    public EnemyBehavior AttackBehavior;

    [Header("Enemy Specific Stats - Senses")]
    public float TasteFactor;
    public float SmellFactor;
    public float SightFactor;
    public float HearingFactor;
    public float TouchFactor;

    public bool IsIncremental;


    [Header("Enemy Specific Stats - Regular")]
    public float MovementSpeed;

    public float AttackDamage;

    public float Health;


    
}
