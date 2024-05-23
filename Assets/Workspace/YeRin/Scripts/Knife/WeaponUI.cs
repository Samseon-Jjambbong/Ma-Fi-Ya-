using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Image image;
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text range;

    [Header("WeaponData")]
    [SerializeField] List<WeaponData> weaponDatas = new List<WeaponData>();

    public void SetWeaponUI()
    {
        WeaponData weapon = weaponDatas[(int) KnifeGameManager.Instance.Knife];

        image.sprite = weapon.Image;
        name.text = weapon.Name;
        speed.text = weapon.Speed;
        range.text = weapon.Range;
    }
}
