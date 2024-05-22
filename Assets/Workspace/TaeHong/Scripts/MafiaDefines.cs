using System.Collections.Generic;
using UnityEngine;

// Enums
public enum MafiaActionType { Block, Kill, Heal }
public enum MafiaRole { Mafia, Doctor, Police, Insane }
public enum MafiaResult { None, MafiaWin, CivilianWin }

// Classes/Structs
public struct MafiaAction
{
    public int sender;
    public int receiver;
    public MafiaActionType actionType;

    public MafiaAction(int sender, int receiver, MafiaActionType actionType)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.actionType = actionType;
    }

    public MafiaAction(int[] serialized)
    {
        sender = serialized[0];
        receiver = serialized[1];
        actionType = (MafiaActionType) serialized[2];
    }

    public int GetActionPrio()
    {
        return (int) actionType;
    }

    public int[] Serialize()
    {
        int[] serialized = { sender, receiver, (int) actionType };
        return serialized;
    }
}

public class MafiaActionPQ
{
    private List<MafiaAction> actions = new List<MafiaAction>();
    public int Count { get { return actions.Count; } }

    public void Enqueue(MafiaAction action)
    {
        Debug.Log($"Enqueued {action.actionType}");
        actions.Add(action);
    }

    public MafiaAction Dequeue()
    {
        int prioIndex = 0;
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
            {
                prioIndex = i;
            }
        }
        MafiaAction action = actions[prioIndex];
        actions.RemoveAt(prioIndex);
        return action;
    }

    public MafiaAction Peek()
    {
        int prioIndex = 0;
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
            {
                prioIndex = i;
            }
        }
        return actions[prioIndex];
    }
}

public class MafiaGame
{
    private int numMafias;
    private int numCivilians;

    public void AddPlayer(MafiaRole role)
    {
        if (role == MafiaRole.Mafia)
            numMafias++;
        else
            numCivilians++;
    }

    public MafiaResult RemovePlayer(MafiaRole removedRole) // True == Civilian Win, False == Mafia Win
    {
        if (removedRole == MafiaRole.Mafia)
        {
            numMafias--;
            if (numMafias == 0)
            {
                return MafiaResult.CivilianWin;
            }
            return MafiaResult.None;
        }
        else
        {
            numCivilians--;
            if (numMafias == numCivilians)
            {
                return MafiaResult.MafiaWin;
            }
            return MafiaResult.None;
        }
    }
}
