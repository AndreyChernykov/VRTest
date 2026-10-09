using UnityEngine;

namespace UI
{
    public class UIFollowPlayer : MonoBehaviour
    {

        [SerializeField] private Transform playerCamera;
        [SerializeField] private float distance = 2.0f;     // Дистанция
        [SerializeField] private float heightOffset = -0.2f; // Смещение по вертикали
        [SerializeField] private float speed = 3.0f;        // Скорость

        private void LateUpdate()
        {
            if (playerCamera == null) return;

            Vector3 targetPosition = playerCamera.position + playerCamera.forward * distance;
            targetPosition.y = playerCamera.position.y + heightOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

            Vector3 lookDir = transform.position - playerCamera.position;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

}
