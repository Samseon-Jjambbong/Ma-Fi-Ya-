using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// programmer : TaeHong
/// 
/// The House class handles onClick events made by the player on the house.
/// Click events should only happen in specific phases.
/// </summary>
public class House : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private GameObject useSkillUI;
    [SerializeField] private GameObject voteUI;
    [SerializeField] private Outlinable outline;
    
    // What UI should be shown when a hosue is clicked
    public void OnPointerClick( PointerEventData eventData )
    {
        // If Skill Use Phase
        useSkillUI.gameObject.SetActive(true);
        
        // If Voting Phase
        //voteUI.gameObject.SetActive(true);
    }

    // Hide UI if cursor exits house
    public void OnPointerExit( PointerEventData eventData )
    {
        HideUI();
    }
    
    public void HideUI()
    {
        useSkillUI.gameObject.SetActive(false);
        voteUI.gameObject.SetActive(false);
    }

    public void ActivateOutline( bool activate )
    {
        outline.enabled = activate;
    }
}
