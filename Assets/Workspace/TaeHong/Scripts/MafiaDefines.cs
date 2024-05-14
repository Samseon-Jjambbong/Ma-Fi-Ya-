using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MafiaActionType { Block, Kill, Heal }

public enum MafiaRole { Mafia, Doctor, Insane, Police }

public struct MafiaAction
{
    private int sender;
    private int receiver;
    private MafiaActionType actionType;

    public MafiaAction(int sender, int receiver, MafiaActionType action)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.actionType = action;
    }

    public int GetActionPrio()
    {
        return (int) actionType;
    }
}

public class MafiaActionPQ
{
    List<MafiaAction> actions;

    public void Enqueue(MafiaAction action)
    {
        actions.Add(action);
    }

    public MafiaAction Dequeue()
    {
        int prioIndex = 0;
        for(int i = 0; i < actions.Count; i++)
        {
            if(actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
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
        for(int i = 0; i < actions.Count; i++)
        {
            if(actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
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
        if(role == MafiaRole.Mafia)
            numMafias++;
        numCivilians++;
    }

    public bool RemovePlayer(MafiaRole removedRole) // True == Civilian Win, False == Mafia Win
    {
        if(removedRole == MafiaRole.Mafia)
        {
            numMafias--;
            if(numMafias == 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            numCivilians--;
            if(numMafias == numCivilians)
            {
                return false;
            }
            return true;
        }
    }
}