using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public GameObject playerCameraPrefab;
    public Vector3 runCameraPosition = new Vector3(-0.2f, 0.9f, 0.3f);
    public Vector3 walkCameraPosition = new Vector3(0, 0.9f, 0.15f);
    public float smoothFactor = 5f;

    private GameObject cameraObject;
    private Animator playerAnimator;

    void Start()
    {
        if (playerCameraPrefab == null)
        {
            Debug.LogError("PlayerCameraPrefab is not assigned.");
            return;
        }

        cameraObject = Instantiate(playerCameraPrefab, transform);
        cameraObject.transform.localPosition = walkCameraPosition;
        cameraObject.transform.localRotation = Quaternion.identity;

        Camera cameraComponent = cameraObject.GetComponent<Camera>();
        if (cameraComponent != null) cameraComponent.tag = "MainCamera";
        else Debug.LogError("Camera component is missing!");

        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null) Debug.LogError("Animator is missing!");
    }

    void Update()
    {
        if (playerAnimator == null) return;

        bool isRunning = playerAnimator.GetBool("Run");
        Vector3 targetPosition = isRunning ? runCameraPosition : walkCameraPosition;

        // Plynuly presun kamery
        cameraObject.transform.localPosition = Vector3.Lerp(
            cameraObject.transform.localPosition,
            targetPosition,
            smoothFactor * Time.deltaTime
        );
    }
}
