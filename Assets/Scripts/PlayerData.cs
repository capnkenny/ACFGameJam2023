using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int Level;
    //public List<Item> Inventory;
    public int Score;
    public int Koiency;

    // For now
    // 1 = Normal
    // 2 = Hard
    public int Difficulty;
    public float MaxSensory;
    public float MaxHealth;
    public float MaxShield;
    public bool ViewedIntro;

    public float CurrentHealth;
    public float CurrentSensory;

    public float Taste;
    public float Smell;
    public float Sight;
    public float Hearing;
    public float Touch;

    public int SoakerLevel;

    public PlayerData()
    {
        Level = 1;
        //Inventory = new();
        Score = 0;
        Koiency = 0;
        Difficulty = 1;
        MaxSensory = 100;
        MaxHealth = 100;
        MaxShield = 100;
        ViewedIntro = false;

        CurrentHealth = 100;
        CurrentSensory = 0;

        Taste = 0;
        Smell = 0;
        Sight = 0;
        Hearing = 0;
        Touch = 0;

        SoakerLevel = 1;
    }
}