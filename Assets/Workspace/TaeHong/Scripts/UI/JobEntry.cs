using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobEntry : BaseUI
{
    Image highlight;
    Image icon;

    private void OnEnable()
    {
        highlight = GetUI<Image>("IMG-OutLine");
        icon = GetUI<Image>("IMG-Icon");
    }

    public void InitJobEntry(MafiaRoleData data)
    {
        icon.sprite = data.roleIcon;
        highlight.enabled = false;
    }

    public void Highlight()
    {
        highlight.enabled = true;
    }
}
