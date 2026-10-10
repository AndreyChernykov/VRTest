using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Core
{

    public class VRLeverController : XRBaseInteractable
    {
        public Action OnLeverActivated { get; set; }

        [SerializeField] private Transform handleTransform;

        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        [SerializeField] private float activationAngle;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private float leverSensitivity;

        [SerializeField] private Vector3 rotationAxis;
        [SerializeField] private Vector3 forwardAxis;

        private float _currentAngle = 0f;
        private float _startAngleOffset = 0f;
        private float _initialGrabAngle = 0f;
        private bool _isActivated = false;
        private bool _isFirstFrameOfGrab = false;

        // Метод срабатывает когда рука хватает рычаг
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _isFirstFrameOfGrab = true;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (_isActivated) return;

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
            {
                Transform interactorTransform = firstInteractorSelecting.transform;
                Vector3 directionToHand = interactorTransform.position - transform.position;
                Vector3 localDirection = transform.InverseTransformDirection(directionToHand);
                Vector3 projectedDirection = Vector3.ProjectOnPlane(localDirection, rotationAxis);

                float handAngle = Vector3.SignedAngle(forwardAxis, projectedDirection, rotationAxis);

                if (_isFirstFrameOfGrab)
                {
                    _isFirstFrameOfGrab = false;

                    Vector3 currentHandleForward = handleTransform.localRotation * forwardAxis;
                    Vector3 projectedHandleDir = Vector3.ProjectOnPlane(currentHandleForward, rotationAxis);
                    _currentAngle = Vector3.SignedAngle(forwardAxis, projectedHandleDir, rotationAxis);

                    _startAngleOffset = handAngle;
                    _initialGrabAngle = _currentAngle;
                }

                float handDeltaAngle = handAngle - _startAngleOffset;

                float targetAngle = _initialGrabAngle + (handDeltaAngle * leverSensitivity);
                targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

                _currentAngle = Mathf.MoveTowards(_currentAngle, targetAngle, smoothSpeed * Time.deltaTime);

                if (handleTransform != null)
                {
                    handleTransform.localRotation = Quaternion.AngleAxis(_currentAngle, rotationAxis);
                }

                bool isMovingForward = activationAngle >= _initialGrabAngle;
                bool isReached = isMovingForward ? (_currentAngle >= activationAngle) : (_currentAngle <= activationAngle);

                if (isReached)
                {
                    _isActivated = true;

                    if (interactionManager != null)
                    {
                        interactionManager.CancelInteractableSelection((IXRSelectInteractable)this);
                    }

                    if (handleTransform != null)
                    {
                        handleTransform.localRotation = Quaternion.AngleAxis(activationAngle, rotationAxis);
                    }

                    minAngle = activationAngle;
                    maxAngle = activationAngle;
                    enabled = false;

                    OnLeverActivated?.Invoke();
                }
            }
        }
    }
}


