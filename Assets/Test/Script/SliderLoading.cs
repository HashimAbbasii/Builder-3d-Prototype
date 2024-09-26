using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderLoading : MonoBehaviour
{
    private Slider loadingSlider;
    public float targetValue = 100f; // The value you want the slider to reach
    public float speed = 1f; // Speed at which the slider will move

    // Start is called before the first frame update
    void Start()
    {
        loadingSlider = GetComponent<Slider>();
        loadingSlider.minValue = 0;
        loadingSlider.maxValue = targetValue; // Set the max value to the target value
        loadingSlider.value = 0; // Start at 0 or any desired initial value
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly increase the slider value towards the target value
        if (loadingSlider.value < targetValue)
        {
            loadingSlider.value += speed * Time.deltaTime; // Increment slider value by speed
            if (loadingSlider.value > targetValue) // Prevent overshooting
            {
                loadingSlider.value = targetValue;
            }
        }
    }
}
