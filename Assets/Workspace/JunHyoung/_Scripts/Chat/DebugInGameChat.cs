using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInGameChat : MonoBehaviour
{
    [SerializeField] InGameChatManager panel;
    [SerializeField] Light sun;

    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;
    [SerializeField] Button button4;

    [SerializeField] Text text1;
    [SerializeField] Text text2;
    [SerializeField] Text text3;

    private void Start()
    {
        button1.onClick.AddListener(ChangeIsChatable);
        button2.onClick.AddListener(ChangeIsMafia);
        button3.onClick.AddListener(ChangeIsDay);
        button4.onClick.AddListener(SubscribeGhost);
    }

    private void ChangeIsChatable()
    {
        panel.IsChatable = !panel.IsChatable;
    }

    private void ChangeIsMafia()
    {
       panel.isMafia = !panel.isMafia;
    }

    private void ChangeIsDay()
    {
        panel.isDay = !panel.isDay;
        sun.enabled = panel.isDay;
    }

    private void SubscribeGhost()
    {
        panel.SubscribleGhostChannel();
    }
}
