using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 3, -5);

    public float mouseSensitivity = 200f;
    private float xRotation = 0f; // pitch

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // لمنع الدوران الكامل لفوق وتحت

        player.Rotate(Vector3.up * mouseX);

        transform.position = player.position + player.rotation * offset;
        transform.rotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0f);
    }
}