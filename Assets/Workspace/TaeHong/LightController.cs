using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Tae
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Vector3 dayRotation;
        [SerializeField] private Vector3 nightRotation;
        [SerializeField] private float phaseChangeDuration;

        private bool isChanging;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(dayRotation);
        }

        public void ChangePhase()
        {
            if ( isChanging )
                return;
            
            if ( Manager.Mafia.IsDay )
            {
                StartCoroutine(ChangePhaseRoutine(dayRotation, nightRotation));
            }
            else
            {
                StartCoroutine(ChangePhaseRoutine(nightRotation, dayRotation));
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
