using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSize : MonoBehaviour
{
    [SerializeField]
    private RectTransform target;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void FixedUpdate()
    {
        // match size and position of target
        rectTransform.sizeDelta = target.sizeDelta;
        rectTransform.position = target.position;
    }
}
