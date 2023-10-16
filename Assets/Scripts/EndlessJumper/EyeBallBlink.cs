using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBallBlink : MonoBehaviour
{
    Vector3 startScale;
    Vector3 closeScale;

    private float time = 0f;

    private void Awake()
    {
        startScale = this.transform.localScale;
        closeScale = Vector3.zero;
        closeScale.y = 0f;
    }

    private void OnEnable()
    {
        time = Random.Range(4f, 15f);
    }

    private void Update()
    {
        if (time > 0) {
            time -= Time.deltaTime;
            if (time <= 0) {
                StartCoroutine(Blink());
            }
        }
    }

    IEnumerator Blink()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .1f) {
            this.transform.localScale = Vector3.Lerp(startScale, closeScale, elapsedTime / .1f);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(.1f);
        while (elapsedTime > .1f) {
            this.transform.localScale = Vector3.Lerp(closeScale, startScale, elapsedTime / .1f);
            elapsedTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.transform.localScale = startScale;
        time = Random.Range(4f, 15f);
    }
}
