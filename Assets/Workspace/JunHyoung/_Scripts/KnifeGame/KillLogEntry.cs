using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillLogEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sender;
    [SerializeField] TextMeshProUGUI target;
    [SerializeField] RectTransform targetTransfrom;
    [SerializeField] Image icon;

    [SerializeField] Sprite[] icons;

    public void SetEntry(KillLogData data)
    {
        sender.text = data.sender;

        if(data.type == 1 ) // type 이 kill 이면
        {
            target.text = data.target;
        }
        else
        {
            targetTransfrom.sizeDelta = Vector2.zero;    
        }

        icon.sprite = icons[data.type];
    }
 }
