using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Knifes/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] Sprite image;
    [SerializeField] new string name;
    [SerializeField] string speed;
    [SerializeField] string range;

    public Sprite Image => image;
    public string Name => name; 
    public string Speed => speed;
    public string Range => range;
}
