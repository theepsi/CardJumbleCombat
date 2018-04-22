using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CombatTextController : MonoBehaviour {

    public Text combatText;

    public void ShowText(string text)
    {
        combatText.text = text;
        combatText.gameObject.SetActive(true);
        Vector3 offset = new Vector3(1.5f, 1.5f, 1f);
        combatText.transform.DOPunchPosition(offset, 2.5f).OnComplete(() => combatText.gameObject.SetActive(false));
    }
}
