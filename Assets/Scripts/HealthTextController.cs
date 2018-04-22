using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthTextController : MonoBehaviour {

    public Text healthText;

    public void ShowText(string text)
    {
        healthText.text = text;
    }
}
