using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Vector3 dayRotation;
        [SerializeField] private Vector3 nightRotation;
        [SerializeField] private float phaseChangeDuration;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(dayRotation);
        }

        public IEnumerator ChangePhase(bool isDay)
        {
            return (isDay
                ? ChangePhaseRoutine(dayRotation, nightRotation)
                : ChangePhaseRoutine(nightRotation, dayRotation));
        }

        IEnumerator ChangePhaseRoutine(Vector3 startRot, Vector3 targetRot)
        {
            float t = 0;
            while ( t < phaseChangeDuration )
            {
                transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, targetRot, t));
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}
