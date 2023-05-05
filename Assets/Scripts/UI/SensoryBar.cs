using UnityEngine;
using UnityEngine.UI;

public class SensoryBar : MonoBehaviour
{
    public float MaxSensoryValue;
    public float SensoryValue;

    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        if (MaxSensoryValue > 0)
            slider.value = (SensoryValue / MaxSensoryValue);
        else
            slider.value = 0;
    }
}
