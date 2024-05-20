using EPOOutline;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// programmer : TaeHong
/// 
/// The House class handles onClick events made by the player on the house.
/// Click events should only happen in specific phases.
/// </summary>
public class House : MonoBehaviourPun, IPointerClickHandler, IPointerExitHandler, IPunObservable
{
    [Header("Components")]
    [SerializeField] private GameObject useSkillUI;
    [SerializeField] private GameObject voteUI;
    [SerializeField] private Outlinable outline;
    [SerializeField] private TextMeshProUGUI voteCountText;

    [Header("Mafia")]
    [SerializeField] private MafiaPlayer houseOwner;
    public MafiaPlayer HouseOwner { get { return houseOwner; } set { houseOwner = value; } }

    public int houseOwnerId;
    [SerializeField] private int visitorId;

    [Header("Misc")]
    public bool debugMode;

    private void Start()
    {
        if ( debugMode )
        {
            ActivateOutline(true);
        }

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC("AddList", RpcTarget.All);
        //}
    }

    private void OnEnable()
    {
        Manager.Mafia.VoteCountChanged += OnVoteCountChanged;
    }

    private void OnDisable()
    {
        Manager.Mafia.VoteCountChanged -= OnVoteCountChanged;
    }

    private void OnVoteCountChanged()
    {
        voteCountText.text = Manager.Mafia.Votes[houseOwnerId - 1].ToString();
    }

    public void ChooseTarget()
    {
        // Send information about who clicked on who's house
        int sender = PhotonNetwork.LocalPlayer.ActorNumber;
        int receiver = houseOwnerId;
        MafiaActionType actionType = Manager.Mafia.Player.actionType;
        MafiaAction action = new MafiaAction(sender, receiver, actionType);
        Manager.Mafia.Player.photonView.RPC("OnChooseTarget", RpcTarget.All, action.Serialize());
        //Manager.Event.pairEventDic["useSkill"].RaiseEvent((sender, receiver));
    }

    public void Vote()
    {
        Manager.Mafia.GetComponent<PhotonView>().RPC("VoteForPlayer", RpcTarget.All, houseOwnerId);
    }

    // What UI should be shown when a house is clicked
    public void OnPointerClick( PointerEventData eventData )
    {
        if ( !outline.enabled || outline.OutlineParameters.Color == Color.red )
            return;
        
        voteUI.gameObject.SetActive(Manager.Mafia.IsDay);      // Day == vote
        useSkillUI.gameObject.SetActive(!Manager.Mafia.IsDay); // Night == skill
    }

    // Show vote count during voting phase
    public void ShowVoteCount(bool show)
    {
        voteCountText.gameObject.SetActive(show);
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
        if ( outline.OutlineParameters.Color == Color.red )
        {
            outline.OutlineParameters.Color = Color.green;
        }

        outline.enabled = activate;
    }

    public void ClickedUseSkillUI()
    {
        foreach ( House house in Manager.Mafia.Houses )
        {
            if (house.HouseOwner == houseOwner)
            {
                useSkillUI.gameObject.SetActive(false);
                outline.OutlineParameters.Color = Color.red;
                continue;
            }

            house.ActivateOutline(false);
        }

        ChooseTarget();
    }

    public void ClickedVoteUI()
    {
        foreach (House house in Manager.Mafia.Houses)
        {
            if (house.HouseOwner == houseOwner)
            {
                useSkillUI.gameObject.SetActive(false);
                outline.OutlineParameters.Color = Color.yellow;
                continue;
            }

            house.ActivateOutline(false);
        }

        Vote();
    }

    public void MafiaComesHome()
    {
        photonView.RPC("ComesHome", PhotonNetwork.PlayerList[houseOwnerId - 1]);
    }

    [PunRPC]
    private void ComesHome()
    {
        GameObject obj = Instantiate(Manager.Mafia.NightMafia, Manager.Mafia.NightMafiaPos, Manager.Mafia.NightMafia.transform.rotation);

        NightMafiaMove mafia = obj.GetComponent<NightMafiaMove>();

        mafia.Target = gameObject;
        mafia.MoveToTarget();
    }

    [PunRPC]
    private void AddHouse(int id)
    {
        houseOwnerId = id;
        Manager.Mafia.Houses.Add(this);
    }

    [PunRPC]
    private void SetVisitorId(int id)
    {
        visitorId = id;
    }

    public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        
    }
}
