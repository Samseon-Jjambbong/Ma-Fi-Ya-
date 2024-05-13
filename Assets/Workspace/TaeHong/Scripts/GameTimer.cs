using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour, IPunObservable
{
    [SerializeField] private TextMeshProUGUI timerText;

    public event Action TimerFinished;

    private void Start()
    {
        timerText.text = "";
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

    public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        if (stream.IsWriting)
        {
            // We own this object, send the data to others
            stream.SendNext(timerText.text);
        }
        else
        {
            // Network player, receive data
            timerText.text = ( string ) stream.ReceiveNext();
        }
    }
}
