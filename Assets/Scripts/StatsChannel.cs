using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsChannel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text kills, saves;


    private void OnEnable()
    {
        kills.text = GameManager.Instance.KillCount.ToString();
        saves.text = GameManager.Instance.SaveCount.ToString();
    }
}
