using TMPro;
using UnityEngine;

namespace UI
{
    public class UIFollowPlayer : MonoBehaviour
    {

        [SerializeField] private Transform playerCamera;
        [SerializeField] private TextMeshProUGUI textTask;
        [SerializeField] private float distance;
        [SerializeField] private float heightOffset;
        [SerializeField] private float speed;

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

        public void SetTextTask(string message)
        {
            textTask.text = message;
        }
    }

}
