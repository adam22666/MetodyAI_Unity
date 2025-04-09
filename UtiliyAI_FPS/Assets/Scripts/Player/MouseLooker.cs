using UnityEngine;
using System.Collections;

public class MouseLooker : MonoBehaviour
{
    public float mouseSensitivity = 280f;

    public Transform cameraTransform;

    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    void Start()
    {
        // Skryt a uzamknúť kurzor myši
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Nastavenie počiatočného pohľadu dopredu
        SetInitialRotation();
    }

    void Update()
    {
        // Získaj vstup z myši
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        horizontalRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle); // Obmedzenie vertikálneho otáčania
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Voliteľné: Umožniť uvoľnenie kurzora stlačením klávesy Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetInitialRotation()
    {
        // Resetovanie horizontálnej rotácie objektu (hráča)
        horizontalRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        // Resetovanie vertikálnej rotácie kamery
        verticalRotation = 0f;
        cameraTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}