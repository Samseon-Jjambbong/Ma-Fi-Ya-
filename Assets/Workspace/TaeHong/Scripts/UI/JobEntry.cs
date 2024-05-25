using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JobEntry : BaseUI, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] MafiaRoleData curData;
    [SerializeField] JobToolTip toolTip;
    Image highlight;
    Image icon;

    private void OnEnable()
    {
        highlight = GetUI<Image>("IMG-OutLine");
        icon = GetUI<Image>("IMG-Icon");
        toolTip = MafiaManager.Instance.toolTip;
    }

    public void InitJobEntry(MafiaRoleData data)
    {
        curData = data;
        icon.sprite = data.roleIcon;
    }

    [SerializeField] Color highLightColor;
    public void Highlight()
    {
        highlight.color = highLightColor;
        prevColor = highLightColor;
    }
    Color prevColor;
    [SerializeField] Color mouseOverColor;

    #region IPointerHandler
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(true);
        toolTip.SetData(curData);
        highlight.color = mouseOverColor;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        highlight.color = prevColor;
        toolTip.gameObject.SetActive(false);
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        toolTip.gameObject.transform.position = eventData.position;
    }
    #endregion
}
