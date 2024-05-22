using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserKillDeathEntry : MonoBehaviour
{
    [SerializeField] new TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI killCount;
    [SerializeField] TextMeshProUGUI deathCount;

    public void Set(string name, int killCount, int deathCount)
    {
        this.name.text = name;
        this.killCount.text = killCount.ToString();
        this.deathCount.text = deathCount.ToString();
    }
}
