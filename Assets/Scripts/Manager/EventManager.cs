using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, IntPairEventChannelSO> pairEventDic = new Dictionary<string, IntPairEventChannelSO>();
    //public Dictionary<string, DirectionEventChannelSO> dirEventDic = new Dictionary<string, DirectionEventChannelSO>();

    private void Start()
    {
        pairEventDic.Add("useSkill", Manager.Resource.Load<IntPairEventChannelSO>("Events/UseSkillEvent"));
    }

    private void OnDestroy()
    {
        foreach(var entry in pairEventDic)
        {
            entry.Value.OnEventRaised = null;
        }
    }
}
