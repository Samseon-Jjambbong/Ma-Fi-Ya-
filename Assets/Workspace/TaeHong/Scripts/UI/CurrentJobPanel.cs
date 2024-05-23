using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentJobPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform grid;
    [SerializeField] JobEntry jobEntryPrefab;

    public void AddEntry(MafiaRoleData data, bool isMine)
    {
        JobEntry entry = Instantiate(jobEntryPrefab);
        entry.InitJobEntry(data, isMine);
        entry.transform.parent = grid;
    }
}
