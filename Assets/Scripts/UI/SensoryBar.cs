using UnityEngine;
using UnityEngine.UI;

public class SensoryBar : MonoBehaviour
{
    public float MaxSensoryValue;
    public float SensoryValue;

    public Slider slider;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateSlider(float value, float maxValue = 0.0f)
    {
        if (maxValue != 0.0f)
            MaxSensoryValue = maxValue;

        slider.value = value;
    }
}
