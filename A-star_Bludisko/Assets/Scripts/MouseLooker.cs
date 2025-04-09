using UnityEngine;
using System.Collections;

public class MouseLooker : MonoBehaviour {

	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	
	private Quaternion m_CharacterTargetRot;
	private Quaternion m_CameraTargetRot;
	private Transform character;
	private Transform cameraTransform;

	void Start() {
        LockCursor(true);
        character = gameObject.transform;
        UpdateCameraReference();
        m_CharacterTargetRot = character.localRotation;
        if (cameraTransform != null)
            m_CameraTargetRot = cameraTransform.localRotation;
    }

    void Update() {

		if (Time.timeScale == 0)
        return;
        if (cameraTransform == null) {
            UpdateCameraReference();
            if (cameraTransform == null) {
                return; 
            }
        }

        LookRotation();

        if(Input.GetButtonDown("Cancel")){
            LockCursor(false);
        }
        if(Input.GetButtonDown("Fire1")){
            LockCursor(true);
        }
    }

    void UpdateCameraReference() {
    if (Camera.main != null) {
        cameraTransform = Camera.main.transform;
        m_CameraTargetRot = cameraTransform.localRotation;
    }
}

	
	private void LockCursor(bool isLocked)
	{
		if (isLocked) 
		{
			// make the mouse pointer invisible
			Cursor.visible = false;

			// lock the mouse pointer within the game area
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			// make the mouse pointer visible
			Cursor.visible = true;

			// unlock the mouse pointer so player can click on other windows
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public void LookRotation()
	{
		//get the y and x rotation based on the Input manager
		float yRot = Input.GetAxis("Mouse X") * XSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

		// calculate the rotation
		m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

		// clamp the vertical rotation if specified
		if(clampVerticalRotation)
			m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

		// update the character and camera based on calculations
		if(smooth) // if smooth, then slerp over time
		{
			character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
			                                            smoothTime * Time.deltaTime);
			cameraTransform.localRotation = Quaternion.Slerp (cameraTransform.localRotation, m_CameraTargetRot,
			                                         smoothTime * Time.deltaTime);
		}
		else // not smooth, so just jump
		{
			character.localRotation = m_CharacterTargetRot;
			cameraTransform.localRotation = m_CameraTargetRot;
		}
	}
	
	// Some math ... eeck!
	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}
}
