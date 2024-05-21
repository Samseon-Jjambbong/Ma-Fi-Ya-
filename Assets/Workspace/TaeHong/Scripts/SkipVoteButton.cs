using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkipVoteButton : BaseUI
{
    Button button;
    TextMeshProUGUI text;

    void Start()
    {
        button = GetUI<Button>("Skip Vote Button");
        text = GetUI<TextMeshProUGUI>("Skip Count Text");
        text.text = "0";
        button.onClick.AddListener(OnPressed);
        Manager.Mafia.SkipVoteCountChanged += OnVoteCountChanged;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnPressed);
        Manager.Mafia.SkipVoteCountChanged -= OnVoteCountChanged;
    }

    private void OnPressed()
    {
        Manager.Mafia.photonView.RPC("VoteForPlayer", RpcTarget.All, -1);
        button.interactable = false;
        Manager.Mafia.photonView.RPC("BlockVotes", PhotonNetwork.LocalPlayer);
    }

    private void OnVoteCountChanged()
    {
        text.text = Manager.Mafia.SkipVotes.ToString();
    }
}
