using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour {

    public Image healthSlider;
    public Image healthSliderRed;

    public void Init()
    {
        healthSlider.fillAmount = 1;
        healthSliderRed.fillAmount = 1;
    }

    public void PrintDamage(int amount)
    {
        healthSlider.fillAmount -= amount / 150f;
        StartCoroutine(PrintDamageRed(amount / 150f));
    }

    IEnumerator PrintDamageRed(float amount)
    {
        float duration = 1f;
        float increment = 0.01f;
        float totalTime = 0f;

        while (totalTime < duration)
        {
            totalTime += increment;
            healthSliderRed.fillAmount -= amount / (duration / increment);
            yield return new WaitForSeconds(increment);
        }
    }
}
