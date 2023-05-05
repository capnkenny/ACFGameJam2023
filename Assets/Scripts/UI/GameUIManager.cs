using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameManager parentManager;

    public HeartBar healthbar;
    public SensoryBar sensory;

    private PlayerController pc;

    public void SetPlayer(PlayerController p) => pc = p;

    private void Update()
    {
        if (pc == null)
        {
            pc = parentManager.GetPlayer();
        }
        if (pc != null)
        {
            healthbar.HealthValue = pc.Health;
            healthbar.MaxHealthValue = pc.MaxHealth;
            sensory.MaxSensoryValue = pc.MaxSensoryMeter;
            sensory.SensoryValue = pc.SensoryMeter;
        }

    }
}
