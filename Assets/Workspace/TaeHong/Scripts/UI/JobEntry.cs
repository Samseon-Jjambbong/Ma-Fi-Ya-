using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobEntry : BaseUI
{
    Image highlight;
    Image icon;

    private void Start()
    {
        highlight = GetUI<Image>("IMG-Outline");
        icon = GetUI<Image>("IMG-Icon");
    }

    public void InitJobEntry(MafiaRoleData data, bool isMine)
    {
        if (isMine)
            highlight.enabled = true;
        icon.sprite = data.roleIcon;
    }
}
