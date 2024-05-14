using UnityEngine;

[CreateAssetMenu(menuName = "Mafia/MafiaRoles", fileName = "MafiaRoles")]
public class MafiaRolesSO : ScriptableObject
{
    public MafiaRole[] pool4 = new MafiaRole [4];
    public MafiaRole[] pool5 = new MafiaRole [5];
    public MafiaRole[] pool6 = new MafiaRole [6];
    public MafiaRole[] pool7 = new MafiaRole [7];
    public MafiaRole[] pool8 = new MafiaRole [8];

    public MafiaRole[] GetRoles(int i)
    {
        switch(i)
        {
            case 4:
                return pool4;
            case 5:
                return pool5;
            case 6:
                return pool6;
            case 7:
                return pool7;
            case 8:
                return pool8;
            default:
                return null;
        }
    }
}
