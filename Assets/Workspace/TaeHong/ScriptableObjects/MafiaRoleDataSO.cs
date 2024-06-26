using System;
using System.Data;
using UnityEngine;

[Serializable]
public class MafiaRoleData
{
    public Sprite roleIcon;
    public string roleName;
    [TextArea]
    public string roleDescription;
}

[CreateAssetMenu(menuName = "Mafia/MafiaRoleData", fileName = "MafiaRoleData")]
public class MafiaRoleDataSO : ScriptableObject
{
    public MafiaRoleData mafiaData;
    public MafiaRoleData doctorData;
    public MafiaRoleData policeData;
    public MafiaRoleData insaneData;

    public MafiaRoleData GetData(MafiaRole role)
    {
        switch (role)
        {
            case MafiaRole.Mafia:
                return mafiaData;
            case MafiaRole.Doctor:
                return doctorData;
            case MafiaRole.Police:
                return policeData;
            case MafiaRole.Insane:
                return insaneData;
            default:
                return null;
        }
    }

    public MafiaRoleData GetData(MafiaActionType actionType)
    {
        switch (actionType)
        {
            case MafiaActionType.Kill:
                return mafiaData;
            case MafiaActionType.Heal:
                return doctorData;
            case MafiaActionType.Block:
                return policeData;
            default:
                return null;
        }
    }
}