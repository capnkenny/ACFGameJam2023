using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameManager parentManager;

    public HeartBar healthbar;
    public SensoryBar sensory;


    private void Update()
    {
        healthbar.HealthValue = parentManager.GetPlayerHealth();
        healthbar.MaxHealthValue = parentManager.playerData.MaxHealth;
        sensory.UpdateSlider(parentManager.GetPlayerSensoryMeter());
    }
}
