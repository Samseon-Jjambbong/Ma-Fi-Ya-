using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// programmer : Yerin
/// 
/// For creating player's houses in Mafia game Scene
/// </summary>
public class HouseCreator : MonoBehaviour
{
    [SerializeField] GameObject housePrefab;

    [SerializeField] int radius;    // 21이 가장 이상적

    private void Start()
    {
        CreateHouses();
    }

    private void CreateHouses()
    {
        int angle = 180 / (Manager.Mafia.PlayerCount - 1);    // 각 집의 간격의 각도

        int currentAngle = 0;
        for (int i = 0; i < Manager.Mafia.PlayerCount; i++) 
        {
            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
            Transform house = Instantiate(housePrefab).transform;
            house.position = pos;

            Quaternion look = Quaternion.LookRotation(pos); // 센터를 바라보도록 rotation 조절
            house.rotation = look;

            currentAngle += angle;
        }
    }
}
