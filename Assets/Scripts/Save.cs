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
        try
		{
			string json = JsonUtility.ToJson(data);
			string b64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
			string path = Application.persistentDataPath + "/player.koi";
			using (var stream = File.Open(path, FileMode.Create))
			{
				using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
				{
					writer.Write(b64);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogWarningFormat("Error when saving new player data - {0}", e.Message);
		}
		
        return data;
    }

	public static void SavePlayer(PlayerData data)
	{
		string json = JsonUtility.ToJson(data);
		string b64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
		string path = Application.persistentDataPath + "/player.koi";
		using (var stream = File.Open(path, FileMode.Create))
		{
			using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
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
				using (var stream = File.Open(path, FileMode.Create))
				{
					using (var writer = new BinaryReader(stream, Encoding.UTF8, false))
					{
						var b64bytes = Convert.FromBase64String(writer.ReadString());
						var json = System.Text.Encoding.UTF8.GetString(b64bytes);
						data = JsonUtility.FromJson<PlayerData>(json);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}
        else
        {
            Debug.LogWarning("Save file not found: " + path);
        }

		return data;
    }

}
