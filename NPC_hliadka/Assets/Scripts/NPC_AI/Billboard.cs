using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private string cameraTag = "Camera"; 
    private Camera targetCamera;

    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag(cameraTag);

        if (camObj != null)
        {
            targetCamera = camObj.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning($"Billboard: Nebola nájdená kamera s tagom \"{cameraTag}\"!");
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;
        
        // Billboard natocenie podla playera
        transform.rotation = Quaternion.LookRotation(
            transform.position - targetCamera.transform.position
        );
    }
}
