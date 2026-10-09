using System;
using UnityEngine;

namespace Core
{
    public class LeverController : MonoBehaviour
    {
        public event Action OnLeverActivated;

        [SerializeField] private float activationAngle = 35f;
        [SerializeField] private Transform TargetAxis;

        private bool _isActivated = false;

        private void Update()
        {
            if (_isActivated) return;

            float currentAngle = TargetAxis.localEulerAngles.x;

            if (currentAngle > 180) currentAngle -= 360;

            if (currentAngle >= activationAngle)
            {
                _isActivated = true;
                OnLeverActivated?.Invoke();
            }
        }
    }

}

