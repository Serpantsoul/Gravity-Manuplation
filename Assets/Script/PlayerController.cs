using Cinemachine;
using UnityEngine;
using UnityEngine.iOS;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform freeLookCam;
    public Transform groundCheck; // Empty GameObject at feet
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float gravityMultiplier = 2.5f;
    public float groundCheckRadius = 0.3f;

    private Rigidbody rb;
    public Vector3 moveDirection;
    private bool isGrounded;
    public GravityHandler gravityHandler;

    public Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (UiManager.instance.gameover)
            return;
        HandleInput();
        HandleJump();

    }

    private void FixedUpdate()
    {
        if (UiManager.instance.gameover)
            return;
        MovePlayer();
        RotatePlayer();
        ApplyExtraGravity();
    }

    void HandleInput()
    {

  
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        // Move in the player's local space (ignoring world or camera orientation)
        Vector3 localForward = transform.forward;
        Vector3 localRight = transform.right;


        moveDirection = (localForward * vertical + localRight * horizontal).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Debug.Log("run");
            anim.SetTrigger("run");

        }
        else
        {
            Debug.Log("idle");
            anim.SetTrigger("idle");
        }
    }

    void MovePlayer()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 targetPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
        }
    }

    void RotatePlayer()
    {
        //if (moveDirection.magnitude >= 0.1f)
        //{
        //    Quaternion targetRot = Quaternion.LookRotation(moveDirection);
        //    Quaternion smoothRot = Quaternion.Slerp(transform.rotation, transform.localRotation, rotationSpeed * Time.fixedDeltaTime);
        //    rb.MoveRotation(smoothRot);
        //}

        Vector3 localMoveDir = transform.InverseTransformDirection(moveDirection);

        // Calculate direction relative to player's local axes
        Vector3 targetDirection = transform.forward * localMoveDir.z + transform.right * localMoveDir.x;

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            // Use gravityDirection as the new "up" reference
            Vector3 customUp = -gravityHandler.GetGravityDirection(); // negative because "down" is gravity

            // Generate rotation towards movement direction while keeping feet aligned to gravity
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, customUp);

            // Smooth rotation
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }

    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ApplyExtraGravity()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "box")
        {
          Destroy(collision.gameObject);
        }
    }
}
