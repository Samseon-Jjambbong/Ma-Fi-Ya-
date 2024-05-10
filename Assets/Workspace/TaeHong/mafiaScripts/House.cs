using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class House : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private GameObject useSkillUI;
    [SerializeField] private GameObject voteUI;
    
    public void OnPointerClick( PointerEventData eventData )
    {
        // If Skill Use Phase
        useSkillUI.gameObject.SetActive(true);
        
        // If Voting Phase
        //voteUI.gameObject.SetActive(true);
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        HideUI();
    }

    public void HideUI()
    {
        useSkillUI.gameObject.SetActive(false);
        voteUI.gameObject.SetActive(false);
    }
}
