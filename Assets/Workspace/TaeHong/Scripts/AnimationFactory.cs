using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationFactory : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector3 centerSpawnPoint;

    public IEnumerator SpawnPlayerGo(MafiaAction action)
    {
        House origin = Manager.Mafia.Houses[action.sender - 1];
        House target = Manager.Mafia.Houses[action.receiver - 1];
        NightMafiaMove player = Instantiate(playerPrefab, origin.entrance.position, Quaternion.identity).GetComponent<NightMafiaMove>();
        //player.GetComponentInChildren<Renderer>().material.color = PhotonNetwork.LocalPlayer.GetPlayerColor();
        player.GetComponentInChildren<Renderer>().material.color = Color.green;
        yield return player.MoveToTargetHouse(target);
        yield return target.PlayEffect(action.actionType);
        player = Instantiate(playerPrefab, target.entrance.position, Quaternion.identity).GetComponent<NightMafiaMove>();
        yield return player.MoveToTargetHouse(origin);
    }

    public IEnumerator SpawnPlayerCome(int houseOwnerId, MafiaActionType actionType)
    {
        House target = Manager.Mafia.Houses[houseOwnerId - 1];
        NightMafiaMove player = Instantiate(playerPrefab, centerSpawnPoint, Quaternion.identity).GetComponent<NightMafiaMove>();
        yield return player.MoveToTargetHouse(target);
        yield return target.PlayEffect(actionType);
    }

    // Called only once by master
    public IEnumerator SpawnPlayerDie(House house)
    {
        Vector3 spawnPoint = house.entrance.position + house.entrance.transform.forward + Vector3.up;
        Quaternion spawnRotation = Quaternion.LookRotation(house.entrance.forward);
        //NightMafiaMove player = PhotonNetwork.InstantiateRoomObject("NightMafia", spawnPoint, spawnRotation).GetComponent<NightMafiaMove>();
        NightMafiaMove player = Instantiate(playerPrefab, spawnPoint, spawnRotation).GetComponent<NightMafiaMove>();

        yield return player.DieAnimation();
    }
}
