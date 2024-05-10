using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>
public class MafiaPlayer : MonoBehaviourPun
{
    // 플레이어 직업 진형
    private bool isMafia;
    public bool IsMafia { get { return isMafia; } set { isMafia = value; } }

    // 플레이어의 생존 여부
    private bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }

    private bool isHealed;
    public bool IsHealed { get { return isHealed; } set { isHealed = value; } }

    private bool canUseSkill;
    public bool CanUseSkill { get { return canUseSkill; } set { canUseSkill = value; } }

    // 플레이어 각 역할에 따른 스킬
    protected virtual void UseSkill( MafiaPlayer targetPlayer )
    {
        if ( !canUseSkill )
        {
            return;
        }
    }

    IEnumerator SKillTime()
    {
        canUseSkill = true;
        yield return new WaitForSeconds(Manager.Mafia.SkillTime);
        canUseSkill = false;
    }
}
