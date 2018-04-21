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

    public void ApplyDamage(int amount)
    {
        healthSlider.fillAmount -= amount / 100f;
        StartCoroutine(ApplyDamageRed(amount / 100f));
    }

    IEnumerator ApplyDamageRed(float amount)
    {
        float currentTime = 0;
        do
        {
            currentTime += Time.deltaTime / 10f;

            healthSliderRed.fillAmount = 1 - currentTime;

            yield return null;

        } while (currentTime < amount);
    }

}
