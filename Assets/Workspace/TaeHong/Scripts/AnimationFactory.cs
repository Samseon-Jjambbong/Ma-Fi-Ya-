using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationFactory : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector3 centerSpawnPoint;
    [SerializeField] Spring spring;

    // Player : go to house > use skill > come back
    public IEnumerator PlayerGoActionRoutine(MafiaAction action)
    {
        House origin = Manager.Mafia.Houses[action.sender - 1];
        House target = Manager.Mafia.Houses[action.receiver - 1];
        NightMafiaMove player = Instantiate(playerPrefab, origin.entrance.position, Quaternion.identity).GetComponent<NightMafiaMove>();
        //player.GetComponentInChildren<Renderer>().material.color = PhotonNetwork.LocalPlayer.GetPlayerColor();
        player.GetComponentInChildren<Renderer>().material.color = Color.green;
        yield return player.MoveToTargetHouse(target);
        player.gameObject.SetActive(false);
        yield return target.PlayEffect(action.actionType);
        player.gameObject.SetActive(true);
        yield return player.MoveToTargetHouse(origin);
        Destroy(player.gameObject);
    }

    // Dark Player : spawn in center > come to my house > use skill
    public IEnumerator PlayerComeActionRoutine(int houseOwnerId, MafiaActionType actionType)
    {
        House target = Manager.Mafia.Houses[houseOwnerId - 1];
        NightMafiaMove player = Instantiate(playerPrefab, centerSpawnPoint, Quaternion.identity).GetComponent<NightMafiaMove>();
        yield return player.MoveToTargetHouse(target);
        Destroy(player.gameObject);
        yield return target.PlayEffect(actionType);
    }

    // Kicked Player : spawn in house entrance > move to center > spring pop and fly at the same time
    public IEnumerator PlayerKickedActionRoutine(int playerID)
    {
        House house = Manager.Mafia.Houses[playerID - 1];
        Vector3 spawnPoint = house.entrance.position + house.entrance.transform.forward + Vector3.up;
        NightMafiaMove player = PhotonNetwork.InstantiateRoomObject("NightMafia", spawnPoint, Quaternion.identity).GetComponent<NightMafiaMove>();
        yield return player.MoveToTargetPos(centerSpawnPoint);
        spring.Pop();
        yield return StartCoroutine(player.Fly());
    }

    public IEnumerator PlayerKickedActionRoutine(House house)
    {
        Vector3 spawnPoint = house.entrance.position + house.entrance.transform.forward + Vector3.up;
        NightMafiaMove player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity).GetComponent<NightMafiaMove>();
        yield return player.MoveToTargetPos(centerSpawnPoint);
        yield return new WaitForSeconds(1);
        spring.Pop();
        yield return StartCoroutine(player.Fly());
    }

    // Dead Player : spawn in house entrance > faint
    public IEnumerator SpawnPlayerDie(House house) // Called only once by master
    {
        Vector3 spawnPoint = house.entrance.position + house.entrance.transform.forward + Vector3.up;
        Quaternion spawnRotation = Quaternion.LookRotation(house.entrance.forward);
        NightMafiaMove player = PhotonNetwork.InstantiateRoomObject("NightMafia", spawnPoint, spawnRotation).GetComponent<NightMafiaMove>();
       //NightMafiaMove player = Instantiate(playerPrefab, spawnPoint, spawnRotation).GetComponent<NightMafiaMove>();

        yield return player.DieAnimation();
    }
}
