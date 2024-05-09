using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;  // 포폰 전용 해시테이블 사용해야 함 (간단하게 이름으로 쓰게 설정하면 좋음)

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;

    private Player player;

    public Player Player { get { return player; } }

    public void SetPlayer(Player player)
    {
        this.player = player;
        playerName.text = player.NickName;
        playerReady.text = player.GetReady() ? "Ready" : "";
        playerReadyButton.gameObject.SetActive(player.IsLocal);
    }

    public void Ready()
    {
        /*PhotonHashTable customProperty = player.CustomProperties;
        bool ready;
        if (customProperty.TryGetValue("Ready", out object value))
        {
            ready = (bool)value;
        }
        else
        {
            ready = false;
        }

        PhotonHashTable ht = new PhotonHashTable();
        ht.Add("Ready", !ready);  // 변수명, 값 으로 생각하고 써주면 됨
        player.SetCustomProperties(ht);*/

        // but. 쓰다보면 실수나 헷갈릴 수 있음. -> CustomProperties를 확장 메소드로 만드는 게 좋음

        bool ready = player.GetReady();
        player.SetReady(!ready);
    }

    public void ChangeCustomProperty(PhotonHashTable property)
    {
        bool ready = player.GetReady();
        playerReady.text = ready ? "Ready" : "";
    }
}
