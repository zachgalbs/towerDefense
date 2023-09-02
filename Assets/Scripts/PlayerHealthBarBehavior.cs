using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarBehavior : MonoBehaviour
{
    public Slider playerSlider;

    public void SetMaxHealth(float playerHealth)
    {
        playerSlider.maxValue = playerHealth;
        playerSlider.value = playerHealth;
    }

    public void SetHealth(float playerHealth)
    {
        playerSlider.value = playerHealth;
    }
}
