using UnityEngine;

public class NPCCameraSetup : MonoBehaviour
{
    public GameObject npcCameraPrefab; 
    private Camera npcCamera; 

    private GameObject cameraObj; 
    public Vector3 cameraOffset = new Vector3(0, 0.5f, 0.3f); 
    public float smoothSpeed = 3f; 

    void Start()
    {
        if (npcCameraPrefab == null)
        {
            Debug.LogError("npcCameraPrefab is not assigned in the Inspector.");
            return;
        }

        cameraObj = Instantiate(npcCameraPrefab, transform);
        cameraObj.transform.localPosition = cameraOffset; 
        cameraObj.transform.localRotation = Quaternion.identity;

        npcCamera = cameraObj.GetComponent<Camera>();
        if (npcCamera != null)
        {
            npcCamera.rect = new Rect(0.75f, 0.75f, 0.25f, 0.25f); 
        }
    }

    void LateUpdate()
    {
        if (cameraObj != null)
        {
            Vector3 targetPosition = transform.position + transform.TransformDirection(cameraOffset);
            cameraObj.transform.position = Vector3.Lerp(cameraObj.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
           
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            cameraObj.transform.rotation = Quaternion.Slerp(cameraObj.transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
    }
}
