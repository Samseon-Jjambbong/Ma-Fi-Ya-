using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Tae
{
    public enum Phase { Day, Night }
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Phase phase;
        [SerializeField] private Vector3 dayRotation;
        [SerializeField] private Vector3 nightRotation;
        [SerializeField] private float phaseChangeDuration;

        private bool isChanging;

        private void Start()
        {
            phase = Phase.Day;
            transform.rotation = Quaternion.Euler(dayRotation);
        }

        private void Update()
        {
            if ( Input.GetKeyDown(KeyCode.A) )
            {
                ChangePhase();
            }
        }

        public void ChangePhase()
        {
            if ( isChanging )
                return;
            
            if ( phase == Phase.Day )
            {
                StartCoroutine(ChangePhaseRoutine(dayRotation, nightRotation));
                phase = Phase.Night;
            }
            else
            {
                StartCoroutine(ChangePhaseRoutine(nightRotation, dayRotation));
                phase = Phase.Day;
            }
        }

        IEnumerator ChangePhaseRoutine(Vector3 startRot, Vector3 targetRot)
        {
            isChanging = true;
            float t = 0;
            while ( t < phaseChangeDuration )
            {
                transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, targetRot, t));
                t += Time.deltaTime;
                yield return null;
            }

            isChanging = false;
        }
    }
}
