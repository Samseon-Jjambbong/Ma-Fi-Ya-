using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;

    public Player Player { get; private set; }

    public void UpdateInfo(Player player)
    {
        Player = player;
        playerName.text = player.NickName;
        playerReady.text = player.GetReady() ? "Ready" : "";
        playerReadyButton.gameObject.SetActive((player.IsLocal));
    }
    
    public void Ready() // When Ready button is pressed
    {
        // Get player's Ready state and change
        bool ready = !Player.GetReady();
        
        // Update ready state
        Player.SetReady(ready);
    }

    public void ChangeCustomProperties(PhotonHashtable property)
    {
        bool ready = Player.GetReady();
        playerReady.text = ready ? "Ready" : "";
    }
}
