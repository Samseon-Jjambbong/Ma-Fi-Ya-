using EPOOutline;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private Image skillIcon;
    [SerializeField] private MafiaRoleDataSO dataSO;
    public Transform entrance;
    [SerializeField] GameObject healEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] GameObject speechBubble;
    [SerializeField] TMP_Text bubbleText;

    [Header("Mafia")]
    [SerializeField] private MafiaPlayer houseOwner;
    public MafiaPlayer HouseOwner { get { return houseOwner; } set { houseOwner = value; } }
    public int houseOwnerId;
    [SerializeField] private int visitorId;

    Coroutine bubble;

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

    public void Vote()
    {
        Manager.Mafia.photonView.RPC("VoteForPlayer", RpcTarget.All, houseOwnerId);
        Manager.Mafia.photonView.RPC("BlockVotes", PhotonNetwork.LocalPlayer);
    }

    // What UI should be shown when a house is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!outline.enabled || outline.OutlineParameters.Color == Color.yellow)
            return;

        // Cancel Skill
        if (outline.OutlineParameters.Color == Color.red)
        {
            Manager.Mafia.PlayerAction = null;
            skillIcon.gameObject.SetActive(false);
            Manager.Mafia.ActivateHouseOutlines();
            return;
        }

        voteUI.gameObject.SetActive(Manager.Mafia.IsDay);      // Day == vote
        useSkillUI.gameObject.SetActive(!Manager.Mafia.IsDay); // Night == skill
    }

    // Show vote count during voting phase
    public void ShowVoteCount(bool show)
    {
        voteCountText.gameObject.SetActive(show);
    }

    // Hide UI if cursor exits house
    public void OnPointerExit(PointerEventData eventData)
    {
        HideUI();
    }

    public void HideUI()
    {
        useSkillUI.gameObject.SetActive(false);
        voteUI.gameObject.SetActive(false);
    }

    public void ActivateOutline(bool activate)
    {
        // Check if owner is dead
        if (PhotonNetwork.CurrentRoom.Players[houseOwnerId].GetDead())
            return;

        if (activate == false)
        {
            skillIcon.gameObject.SetActive(false);
        }
        if (outline.OutlineParameters.Color != Color.green)
        {
            outline.OutlineParameters.Color = Color.green;
        }

        outline.enabled = activate;
    }

    public void ClickedUseSkillUI()
    {
        foreach (House house in Manager.Mafia.Houses)
        {
            if (house.HouseOwner == houseOwner)
            {
                // Turn off UI
                useSkillUI.gameObject.SetActive(false);
                // Set outline to red
                outline.OutlineParameters.Color = Color.red;
                // Show skill icon
                MafiaRole playerRole = PhotonNetwork.LocalPlayer.GetPlayerRole();
                MafiaRoleData data = dataSO.GetData(playerRole);
                skillIcon.sprite = data.roleIcon;
                skillIcon.gameObject.SetActive(true);
                // Store action info
                Manager.Mafia.PlayerAction = new MafiaAction(
                    PhotonNetwork.LocalPlayer.ActorNumber,
                    houseOwnerId,
                    Manager.Mafia.Player.actionType
                );
                continue;
            }

            house.ActivateOutline(false); // Turn off other house outlines
        }
    }

    public void ClickedVoteUI()
    {
        foreach (House house in Manager.Mafia.Houses)
        {
            if (house.HouseOwner == houseOwner)
            {
                voteUI.gameObject.SetActive(false);
                outline.OutlineParameters.Color = Color.yellow;
                continue;
            }

            house.ActivateOutline(false);
        }

        Vote();
    }

    public IEnumerator PlayEffect(MafiaActionType mafiaActionType)
    {
        if(mafiaActionType == MafiaActionType.Kill)
        {
            yield return PlayKillEffectRoutine(2);
        }
        else if(mafiaActionType == MafiaActionType.Heal)
        {

            yield return PlayHealEffectRoutine(2);
        }
        yield return null;
    }

    private IEnumerator PlayKillEffectRoutine(float duration)
    {
        killEffect.SetActive(true);
        yield return new WaitForSeconds(duration);
        killEffect.SetActive(false);
    }

    private IEnumerator PlayHealEffectRoutine(float duration)
    {
        // Only play effect if player actually got revived
        if (Manager.Mafia.sharedData.healedPlayers[houseOwnerId - 1])
        {
            healEffect.SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        healEffect.SetActive(false);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    #region Speech Bubble

    [PunRPC]
    public void OpenSpeechBubble(string userName, string sendText)
    {
        if (speechBubble.activeSelf)
        {
            StopCoroutine(bubble);
        }
        else
        {
            speechBubble.SetActive(true);
        }

        bubbleText.text = $"<#9b111e>{userName}</color>\n{sendText}";

        bubble = StartCoroutine(CloseSpeechBubble());
    }
    IEnumerator CloseSpeechBubble()
    {
        yield return new WaitForSeconds(3f);

        speechBubble.SetActive(false);
    }
    #endregion
}
