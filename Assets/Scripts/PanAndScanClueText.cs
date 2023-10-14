using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanAndScanClueText : MonoBehaviour
{
    public TMP_Text numberText;

    private void OnEnable()
    {
        numberText.text = Random.Range(0, 10).ToString();
    }
}
