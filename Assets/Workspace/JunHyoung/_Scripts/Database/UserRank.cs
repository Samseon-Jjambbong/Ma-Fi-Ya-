using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson.PunDemos;

public class UserRank : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] new TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI score;

    public void Set(int rank, string name, int score )
    {
        this.rank.text = rank.ToString();
        this.name.text = name;
        this.score.text = score.ToString();
    }

    private void OnDisable()
    {
        Destroy( this.gameObject );
    }
}
