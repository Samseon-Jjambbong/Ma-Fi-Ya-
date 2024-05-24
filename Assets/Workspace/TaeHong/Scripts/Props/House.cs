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
public class House : MonoBehaviourPun, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private GameObject useSkillUI;
    [SerializeField] private GameObject voteUI;
    [SerializeField] private Outlinable outline;
    [SerializeField] private TextMeshProUGUI voteCountText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private MafiaRoleDataSO dataSO;
    public Transform entrance;

    [Header("Skill Effects")]
    [SerializeField] GameObject healEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] GameObject blockEffect;

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

    /******************************************************
    *                    Click Events
    ******************************************************/
    // What UI should be shown when a house is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!outline.enabled)
            return;

        // Day = Vote
        if (Manager.Mafia.IsDay)
        {
            if (outline.OutlineParameters.Color == Color.yellow)
                return;

            // Click
            if (!voteUI.gameObject.activeInHierarchy)
            {
                // Close other house's UI
                Manager.Mafia.DeactivateHouseUIs();

                voteUI.gameObject.SetActive(true);
            }
            // Unclick
            else
            {
                // Nothing
            }
        }
        // Night = Use Skill
        else
        {
            // Unclick
            if (useSkillUI.gameObject.activeInHierarchy || outline.OutlineParameters.Color == Color.red)
            {
                HideUI();
                Manager.Mafia.PlayerAction = null;
                skillIcon.gameObject.SetActive(false);
                outline.OutlineParameters.Color = Color.green;
                Manager.Mafia.ActivateHouseOutlines();
            }
            // Click
            else
            {
                // Close other house's UI
                Manager.Mafia.DeactivateHouseUIs();

                useSkillUI.gameObject.SetActive(true);
            }
        }
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

                MafiaActionType actionType = Manager.Mafia.Player.actionType;
                MafiaRoleData data = dataSO.GetData(actionType);
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

            house.DeactivateOutline(); // Turn off other house outlines
        }
    }

    public void ClickedVoteUI()
    {
        foreach (House house in Manager.Mafia.Houses)
        {
            if (house.HouseOwner == houseOwner)
            {
                HideUI();
                outline.OutlineParameters.Color = Color.yellow;
                continue;
            }

            house.DeactivateOutline();
        }

        Vote();
    }

    public void Vote()
    {
        Manager.Mafia.photonView.RPC("VoteForPlayer", RpcTarget.All, houseOwnerId);
        Manager.Mafia.photonView.RPC("BlockVotes", PhotonNetwork.LocalPlayer);
    }

    /******************************************************
    *                    UI Display
    ******************************************************/
    // Outline makes house clickable
    public void ActivateOutline()
    {
        if (Manager.Mafia.IsDay)
        {
            HideUI();
            ShowVoteCount(false);
        }
        else
        {
            HideUI();
            skillIcon.gameObject.SetActive(false);
        }

        outline.OutlineParameters.Color = Color.green;
        outline.enabled = true;
    }

    public void DeactivateOutline()
    {
        if (Manager.Mafia.IsDay)
        {
            HideUI();
            ShowVoteCount(false);
        }
        else
        {
            HideUI();
            skillIcon.gameObject.SetActive(false);
        }
        
        outline.enabled = false;
    }

    // Show vote count during voting phase
    public void ShowVoteCount(bool show)
    {
        if (!show)
            voteCountText.text = "0";
        voteCountText.gameObject.SetActive(show);
    }

    public void HideUI()
    {
        useSkillUI.gameObject.SetActive(false);
        voteUI.gameObject.SetActive(false);
    }

    /******************************************************
    *                    Effects
    ******************************************************/
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
        else if (mafiaActionType == MafiaActionType.Block)
        {
            yield return PlayBlockEffectRoutine(2);
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

    private IEnumerator PlayBlockEffectRoutine(float duration)
    {
        blockEffect.SetActive(true);
        yield return new WaitForSeconds(duration);
        blockEffect.SetActive(false);
    }

    /******************************************************
    *                    RPCs
    ******************************************************/
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
}
