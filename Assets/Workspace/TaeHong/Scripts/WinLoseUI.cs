using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinLoseUI : BaseUI
{
    GameObject Win;
    GameObject Lose;

    private void Start()
    {
        Win = GetUI<Image>("Win Back").gameObject;
        Lose = GetUI<Image>("Lose Back").gameObject;
    }

    public void ShowWin(int points)
    {
        Win.SetActive(true);
        GetUI<TextMeshProUGUI>("Win Point Text").text = points.ToString();
    }

    public void ShowLose(int points)
    {
        Lose.SetActive(true);
        GetUI<TextMeshProUGUI>("Win Point Text").text = points.ToString();
    }
}
