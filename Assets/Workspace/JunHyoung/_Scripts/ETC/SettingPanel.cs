using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : PopUpUI
{
    [SerializeField] Button closeButton;
    [SerializeField] Button exitGameButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;


    protected override void Awake()
    {
        closeButton.onClick.AddListener(Close);
        exitGameButton.onClick.AddListener(ExitGame);
        bgmSlider.onValueChanged.AddListener(ChangeBGMVol);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVol);
    }

    void ChangeBGMVol(float val)
    {
        Manager.Sound.BGMVolme = val;
    }

    void ChangeSFXVol(float val)
    {
        Manager.Sound.SFXVolme = val;
    }

    public void Open()
    {
        Manager.UI.ShowPopUpUI(this);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
