using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    public event Action TimerFinished;

    private void Start()
    {
        //StartTimer(10);
    }

    public IEnumerator StartTimer(int duration)
    {
        return TimerRoutine(duration);
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
        TimerFinished?.Invoke();
    }
}
