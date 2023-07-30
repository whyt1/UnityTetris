using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_ScoreLogic : MonoBehaviour
{
    private TextMeshPro myText;
    private float score;
    private float combo;

    void OnEnable()
    {
        SC_CubeLogic.OnRowDone += OnRowDone;
    }
    void OnDisable()
    {
        SC_CubeLogic.OnRowDone -= OnRowDone;
    }
    void Awake()
    {
        myText = GetComponent<TextMeshPro>();
        score = 0;
        combo = 1;
    }
    void FixedUpdate()
    {
        myText.text = "SCORE\n" + (int)score;
    }

    void OnRowDone(float row)
    {
        combo++;
        StartCoroutine(AddScore());
    }
    IEnumerator AddScore()
    {
        yield return new WaitForFixedUpdate();
        score += combo/9;

        yield return new WaitForSeconds(0.5f);
        combo = 1;
    }
}
