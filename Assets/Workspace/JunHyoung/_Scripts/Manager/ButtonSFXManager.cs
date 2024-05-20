using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  씬 내 모든 버튼들을 찾은 다음에 onClick 이벤트에 SFX 재생을 할당함
/// </summary>
public class ButtonSFXManager : MonoBehaviour
{
    [SerializeField] AudioClip[] buttonSFXs;

    void Awake()
    {
        BindAllButtons();
    }

    void BindAllButtons()
    {
        //Button[] buttons = FindObjectsOfType<Button>(); <- 비활성화된 오브젝트는 못찾음
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayRandomSFX);
        }
    }

    public void PlayRandomSFX()
    {
        Manager.Sound.PlaySFX(buttonSFXs[Random.Range(0, buttonSFXs.Length)]);
    }
}
