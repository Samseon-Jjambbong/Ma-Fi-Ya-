using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        StartTimer(10);
    }

    public void StartTimer(int duration)
    {
        StartCoroutine(TimerRoutine(duration));
    }

    IEnumerator TimerRoutine(int duration)
    {
        int t = duration;
        while ( t > 0 )
        {
            timerText.text = t.ToString();
            yield return new WaitForSeconds(1);
            t--;
        }
        timerText.text = t.ToString();
        Debug.Log("Timer finished");
    }
}
