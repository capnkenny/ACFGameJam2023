using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Save
{

    public static PlayerData SaveNewPlayer()
    {
		Debug.Log("Creating new player data");
        PlayerData data = new PlayerData();
		SavePlayer(data);
		
        return data;
    }

	public static void SavePlayer(PlayerData data)
	{
		string json = JsonUtility.ToJson(data);
		string b64 = System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(json));
		string path = Application.persistentDataPath + "/player.koi";
		using (var stream = File.Open(path, FileMode.Create))
		{
			using (var writer = new StreamWriter(stream, Encoding.Default))
			{
				writer.Write(b64);
			}
		}

	}

	public static PlayerData? LoadPlayer()
    {
		PlayerData data = null;
        string path = Application.persistentDataPath + "/player.koi";
        if(File.Exists(path))
        {
			try
			{
				using (var stream = File.Open(path, FileMode.Open))
				{
					using (var reader = new StreamReader(stream))
					{
						string str = reader.ReadToEnd();
						var b64bytes = Convert.FromBase64String(str);
                        var json = Encoding.Default.GetString(b64bytes);
                        data = JsonUtility.FromJson<PlayerData>(json);
                    }
                }
			}
			catch (Exception e)
			{
				Debug.LogWarning("Could not load save file properly: " + e.Message);
			}
		}
        else
        {
            Debug.LogWarning("Save file not found: " + path);
        }

		return data;
    }

}
