using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class KnifeGameScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] RectTransform content;
    [SerializeField] UserKillDeathEntry entry;



    void Start()
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        //if (changedProps.ContainsKey(CustomProperty))
        {
          
        }
    }
}
