using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Data;

public class CurrentJobPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform grid;
    [SerializeField] MafiaRolesSO rolePools;
    [SerializeField] MafiaRoleDataSO roleData;
    [SerializeField] GameObject jobEntryPrefab;

    public void InitJobPanel()
    {
        //bool highlighted = false;
        MafiaRole myRole = PhotonNetwork.LocalPlayer.GetPlayerRole();
        
        foreach (var role in rolePools.GetRoles(4))
        {
            // Add entries for each role
            JobEntry entry = Instantiate(jobEntryPrefab).GetComponent<JobEntry>();
            entry.InitJobEntry(roleData.GetData(role));
            entry.transform.SetParent(grid);

            // Highlight if my role
            //if (!highlighted) // prevent highlighting more than once
            //{
            //    if (myRole == MafiaRole.Insane)
            //    {
            //        if (role == Manager.Mafia.Player.fakeRole)
            //        {
            //            entry.Highlight();
            //            highlighted = true;
            //        }
            //    }
            //    else
            //    {
            //        if (role == myRole)
            //        {
            //            entry.Highlight();
            //            highlighted = true;
            //        }
            //    }
            //}
        }
    }
}
