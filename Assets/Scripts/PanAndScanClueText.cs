using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanAndScanClueText : MonoBehaviour
{
    public TMP_Text numberText;
    public bool isClue = false;
    private void OnEnable()
    {
        if (!isClue)
            numberText.text = Random.Range(0, 10).ToString();
    }
}
