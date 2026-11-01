using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float lookSensitivity = 2.0f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float jumpHeight = 1.0f;

        private CharacterController controller;
        private float pitch;
        private float verticalVelocity;
        private bool controlsInverted;

        public Transform CameraTransform => cameraPivot;

        public void SetControlsInverted(bool inverted) => controlsInverted = inverted;

        public void SetLookSensitivity(float value)
        {
            lookSensitivity = Mathf.Clamp(value, 0.5f, 5f);
        }

        public void ResetMotion()
        {
            verticalVelocity = -2f;
            pitch = 0f;
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.identity;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            Look();
            Move();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Look()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            float yaw = Input.GetAxisRaw("Mouse X") * lookSensitivity;
            float lookY = Input.GetAxisRaw("Mouse Y") * lookSensitivity;
            transform.Rotate(Vector3.up * yaw);
            pitch = Mathf.Clamp(pitch - lookY, -85f, 85f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void Move()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            if (controlsInverted) { x = -x; z = -z; }
            Vector3 planar = (transform.right * x + transform.forward * z).normalized;
            controller.Move(planar * moveSpeed * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (controller.isGrounded && Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;
            controller.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
        }
    }
}
