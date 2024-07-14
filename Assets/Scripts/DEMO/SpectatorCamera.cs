using UnityEngine;

namespace DEMO
{
    public class SpectatorCamera : MonoBehaviour
    {
        public float moveSpeed = 5f;   // The speed at which the camera moves
        public float _moveSpeed = 5f;   // The speed at which the camera moves
        public float moveSpeedShift = 10f;   // The speed at which the camera moves
        public float rotateSpeed = 10f; // The speed at which the camera rotates

        void Update()
        {
            // Move the camera based on input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float up = Input.GetAxis("UpDown");

            if (Input.GetKey(KeyCode.LeftShift))
                moveSpeed = moveSpeedShift;
            else
            {
                moveSpeed = _moveSpeed;
            }
            
            transform.position += transform.forward * moveSpeed * vertical * Time.deltaTime;
            transform.position += transform.right * moveSpeed * horizontal * Time.deltaTime;
            transform.position += transform.up * moveSpeed * up * Time.deltaTime;

            // Rotate the camera based on input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            
            if(Input.GetKey(KeyCode.Mouse1)){
                // Calculate the new rotation based on the mouse input
                Vector3 newRotation = transform.eulerAngles;
                newRotation.x -= mouseY * rotateSpeed;
                newRotation.y += mouseX * rotateSpeed;
                transform.eulerAngles = newRotation;
        
                // Restrict the rotation around the z-axis to 0
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            }
        }
    }
}