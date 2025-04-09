using UnityEngine;

public class PlayerCameraSetup : MonoBehaviour
{
    public GameObject playerCameraPrefab; 

    void Start()
    {
        if (playerCameraPrefab == null)
        {
            Debug.LogError("PlayerCameraPrefab is not assigned.");
            return;
        }

    
        GameObject cameraObject = Instantiate(playerCameraPrefab, transform);
        cameraObject.transform.localPosition = new Vector3(0, 0.9f, 0.15f); 
        cameraObject.transform.localRotation = Quaternion.identity; 

        Camera cameraComponent = cameraObject.GetComponent<Camera>();
        if (cameraComponent != null)
        {
            cameraComponent.tag = "MainCamera";
            Debug.Log("Camera successfully attached to player.");
        }
        else
        {
            Debug.LogError("Camera component is missing on PlayerCameraPrefab!");
        }
    }
}
