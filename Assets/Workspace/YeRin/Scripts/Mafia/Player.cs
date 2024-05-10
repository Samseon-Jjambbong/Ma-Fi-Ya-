using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>
public class MafiaPlayer : MonoBehaviour
{
    // 플레이어 직업 진형
    private bool isMafia;
    public bool IsMafia { get { return isMafia; } }

    // 플레이어의 생존 여부
    private bool isAlive;
    public bool IsAlive { get { return isAlive; } }

    // 플레이어 각 역할에 따른 스킬
    protected virtual void UseSkill()
    {
    }
}
