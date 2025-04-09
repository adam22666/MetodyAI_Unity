using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7.0f; 
    public float gravity = 9.81f;  

    private CharacterController myController; 

    void Start()
    {
        myController = gameObject.GetComponent<CharacterController>();
        if (myController == null)
        {
            Debug.LogError("CharacterController is missing from the GameObject!");
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 movementZ = Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime;
        Vector3 movementX = Input.GetAxis("Horizontal") * Vector3.right * moveSpeed * Time.deltaTime;
        Vector3 movement = transform.TransformDirection(movementZ + movementX);

        if (!myController.isGrounded)
        {
            movement.y -= gravity * Time.deltaTime;
        }

        myController.Move(movement);
    }
}
