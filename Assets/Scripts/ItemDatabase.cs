using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{

    [SerializeField]
    private int numberOfColumns;
    [SerializeField]
    private TextAsset itemsFile;

    private List<Item> _items = new List<Item>();
    private char _delimiter = ',';
    private char _separator = '\n';

    public void Awake()
    {
        Debug.Log("Attempting to read item asset");

        Debug.Log("Separating lines...");
        string[] lines = itemsFile.text.Split(_separator);
        //Assume 0 represents headers
        for(int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Replace('\r', ' ').TrimEnd();
            string[] columns = line.Split(_delimiter);
            if(columns.Length != numberOfColumns)
                continue;

            Item it = ScriptableObject.CreateInstance<Item>();

            it.Id = int.Parse(columns[0]);
            it.Name = columns[1];
            it.Description = columns[2];
            it.DamageModifier = float.Parse(columns[4]);
            it.DefenseModifier = float.Parse(columns[5]);
            it.YippeeModifier = float.Parse(columns[6]);

            //Set icon
            try
            {
                it.Icon = Resources.Load<Sprite>($"Sprites/Items/{it.Name}");
            }
            catch(Exception e)
            {
                Debug.Log($"{e.Message}\n{e.StackTrace}");
                it.Icon = Resources.Load<Sprite>("Sprites/Items/Missingno");
            }

            //Set isEquipable
            if(!bool.TryParse(columns[3], out bool res))
            {
                it.IsEquipable = false;
            }
            else
            {
                it.IsEquipable = res;
            }

            _items.Add(it);
            Debug.Log($"Added item: {it.Name}");
        }
    }

    public Item GetEmptyItem()
    {
        return _items[0];
    }
    
}
