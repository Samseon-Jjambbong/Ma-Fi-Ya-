using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobToolTip : MonoBehaviour
{
    [SerializeField] MafiaRoleData curData;

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textDescription;

    public void SetData(MafiaRoleData data)
    {
        curData = data;
        icon.sprite = curData.roleIcon;
        textName.text = curData.roleName;
        textDescription.text = curData.roleDescription;
    }
}
