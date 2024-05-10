using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerClicks : MonoBehaviour, IPointerClickHandler
{
    private void OnClick()
    {
        
    }

    public void OnPointerClick( PointerEventData eventData )
    {
        Debug.Log(eventData.position);
    }
}
