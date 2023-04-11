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
    public int MaxSensory;
    public int MaxHealth;
    public int MaxShield;
    public bool ViewedIntro;

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
    }
}