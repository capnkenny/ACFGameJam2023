using UnityEngine;

public class Item : ScriptableObject
{
    public int Id;
    
    public string Name;
    
    public string Description;
    
    public Sprite Icon;

    public bool IsEquipable;
    
    public float DamageModifier;

    public float DefenseModifier;

    public float YippeeModifier;
}
