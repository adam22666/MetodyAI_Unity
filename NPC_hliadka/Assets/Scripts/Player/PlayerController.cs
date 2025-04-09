using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float gravity = -10f;

    private CharacterController myController;
    private Animator animator;
    private float verticalVelocity = 0f;

    private string animRun = "Run";
    private string animWalk = "Walk";

    public PlayerManager playerManager;
    public WeaponHit weaponScript;

    public static bool isWalking = false;

    public void EnableWeaponEvent()
    {
        weaponScript.EnableWeapon();
    }
    public void DisableWeaponEvent()
    {
        weaponScript.DisableWeapon();
    }

    void Start()
    {
        myController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (myController == null) Debug.LogError("Chyba CharacterController!");
        if (animator == null) Debug.LogError("Chyba Animator!");

        if (playerManager == null)
        {
            playerManager = GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogError("Chyba PlayerManager! prirad ho v Inspector.");
            }
        }
    }

    void Update()
    {
        HandleMovement();
        HandleAnimations();
        HandleAttackInput();
        isWalking = Input.GetKey(KeyCode.LeftShift);
    }

    void HandleMovement()
    {
        bool isWalking = Input.GetKey(KeyCode.LeftShift);

        if (playerManager != null && playerManager.GetStamina() <= 1f)
        {
            isWalking = true;
        }

        float currentSpeed = isWalking ? walkSpeed : runSpeed;

        if (playerManager != null && playerManager.GetStamina() <= 1f)
        {
            currentSpeed = walkSpeed;
        }

        // pohyb
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = transform.TransformDirection(new Vector3(horizontal, 0, vertical)) * currentSpeed;

        // Gravity
        if (myController.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement.y = verticalVelocity;
        myController.Move(movement * Time.deltaTime);

        // (0=idle,1=walk,2=run)
        int movementState = 0;
        bool isMoving = (vertical != 0 || horizontal != 0);
        if (isMoving)
        {
            movementState = isWalking ? 1 : 2;
        }

        if (playerManager != null)
        {
            playerManager.UpdateStamina(movementState);
        }
    }

    void HandleAnimations()
    {
        bool isMoving = (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0);
        bool isWalking = Input.GetKey(KeyCode.LeftShift) && isMoving;

        animator.SetBool(animWalk, isWalking);

        bool isRunning = isMoving && !isWalking;
        animator.SetBool(animRun, isRunning);
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }
}
