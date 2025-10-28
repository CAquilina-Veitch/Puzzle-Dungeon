using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    PlayerControls playerControls;
    public Rigidbody rb;
    public Weapon weapon;
    public PlayerInput Input;

    public Vector3 moveVector;
    public Vector2 inputVector;
    public Vector3 jumpVector;
    public Vector3 rotVector;

    public float moveSpeed;
    public float jumpStrength;
    public float gravityStrenth;

    public bool grounded;
    public float playerheight;
    public LayerMask whatIsGround;

    //slope handling
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
    }
    void Start()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerControls.Enable();
        weapon = GetComponentInChildren<Weapon>();
    }

    private void OnAttack()
    {
     if (weapon.attacking == false) weapon.attack();
    }
    private void OnJump()
    {
        if (grounded)
        {
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Force);
        }
    }

    private void OnMovement(InputValue InputValue)
    {
        inputVector = InputValue.Get<Vector2>();
        moveVector.x = inputVector.x;
        moveVector.z = inputVector.y;

        if (moveVector.x != 0 || moveVector.z != 0)
        {
            rotVector.x = -moveVector.z;
            rotVector.z = moveVector.x;
        }
    }
 
    public void AirMove()
    {
        transform.position = transform.position + jumpVector * moveSpeed * Time.deltaTime;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerheight * 0.5f + 0.3f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveVector, slopeHit.normal).normalized;
    }
    public void Move()
    {
        if (weapon.attacking == false)
        {
            if (OnSlope())
            {
                transform.position = transform.position + GetSlopeMoveDirection() * moveSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(rotVector);
            }
            else
            { 
                transform.position = transform.position + moveVector * moveSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(rotVector);
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        { 
            jumpVector = -jumpVector;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10) jumpVector = -jumpVector;
    }



    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.1f, whatIsGround);
    }

    private void FixedUpdate()
    {
        if (grounded) Move();
        else AirMove();

        rb.AddForce(Vector3.down * gravityStrenth);

        if (grounded) jumpVector = moveVector;
    }
}
