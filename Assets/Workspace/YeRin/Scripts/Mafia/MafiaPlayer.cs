using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>

public enum MafiaAction { Kill, Heal, Block }
public enum MafiaRole { Mafia, Doctor, Insane, Police }

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

    private MafiaRole role;

    private void Start()
    {
        // 플레이어 역할 받기
        //role = MafiaManager.GetRole(PhotonNetwork.LocalPlayer.ActorNumber - 1);
    }

    // 플레이어 각 역할에 따른 스킬
    protected virtual void UseSkill( int targetPlayer )
    {
        if ( !canUseSkill )
        {
            // Manager.Mafia.PlayerUsedSkill(PhotonNetwork.LocalPlayer.ActorNumber - 1, targetPlayer, MafiaAction);
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
