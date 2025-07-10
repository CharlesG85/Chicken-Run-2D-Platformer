using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool canControl = true;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canGrabVine = true;
    [HideInInspector] public bool hasMoved;
    [HideInInspector] public bool isResetting;

    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private BoxCollider2D slidingHitbox;

    public Rigidbody2D RB;

    [Header("Moving")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveAcceleration = 10f;
    [SerializeField] private float groundDrag = 20f;
    [SerializeField] private float airDrag = 5f;

    [Header("Sliding")]
    [SerializeField] private float slideMovementThreshhold = 1f;
    [SerializeField] private float slideAccelerationBoost = 1f;
    [SerializeField] private float slideWallBounceVelocityMultiplier;
    [SerializeField] private float slideJumpAccelerationBoost = 1f;
    [SerializeField] private float slideDrag = 1f;
    [SerializeField] private float slideAirDrag = 0f;
    [SerializeField] private float minimumSlideTime = 0.5f;

    [Header("Jumping")]
    [SerializeField] private float jumpApexBonusSpeed = 1.5f;
    [SerializeField] private float jumpAccelerationBoost = 3f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float jumpApexThreshHold = 1f;
    [SerializeField] private float jumpApexGravityMultiplier = 0.5f;
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;
    [SerializeField] private float maxFallSpeed = 10f;

    [Header("Other")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask vineLayer;
    [SerializeField] private LayerMask movingPlatformLayer;

    [SerializeField] private Transform landParticles;
    [SerializeField] private Transform deathParticles;

    //[SerializeField] private float maxTilt = 10f;
    [SerializeField] private float tiltAcceleration = 5f;

    private bool m_grounded = true;
    private bool m_isFacingRight = true;
    private bool m_jumpQueued;
    private bool m_isJumping;
    private bool m_isSliding;
    private bool m_wasTryingToSlide;
    private bool m_isTouchingVine;
    private float m_grabVineTimer;
    private float m_jumpQueuedTimer;
    private float m_coyoteTimer;
    private float m_slideTimer;
    private float m_moveSpeed;
    private float m_slideWallBounceTimer;

    public Animator anim;
    private HingeJoint2D m_hinge;

    private void Start()
    {
        m_hinge = GetComponent<HingeJoint2D>();
        anim.SetBool("Grounded", m_grounded);
    }

    private void Update()
    {
        if (transform.position.y <= -7 && isResetting == false)
        {
            GameManager.Instance.StartCoroutine("ResetPlayer");
            isResetting = true;
        }

        if (!canControl)
            return;

        Slide();
        Gravity();
        Swing();
        Jump();

        if (!canMove)
            return;

        Move();
    }

    private void Slide()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // Only want to start new slide when the input for the current slide is over
        if (Input.GetAxisRaw("Vertical") >= -0.1f && m_wasTryingToSlide == true)
        {
            m_wasTryingToSlide = false;
        }

        // Begin Sliding
        if (m_isSliding == false && Mathf.Abs(RB.velocity.x) >= slideMovementThreshhold && Input.GetAxisRaw("Vertical") < -0.1f && m_wasTryingToSlide == false && m_grounded)
        {
            Vector2 velocityBoost = new Vector2(Mathf.Sign(inputX) * slideAccelerationBoost, 0);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Clamp(RB.velocity.y, maxFallSpeed * -1, Mathf.Infinity)) + velocityBoost;
            m_isSliding = true;
            m_wasTryingToSlide = true;

            canMove = false;

            hitbox.enabled = false;
            slidingHitbox.enabled = true;
        }

        m_slideWallBounceTimer += Time.deltaTime;

        // During Slide
        if (m_isSliding == true)
        {
            if (m_grounded)
            {
                RB.velocity -= new Vector2(RB.velocity.x * slideDrag * Time.deltaTime, 0f);
            }
            else
            {
                RB.velocity -= new Vector2(RB.velocity.x * slideAirDrag * Time.deltaTime, 0f);
            }

            //Bounce off of walls
            int dir = m_isFacingRight == true ? 1 : -1;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.right * dir, 1f, groundLayer);
            Debug.DrawRay(transform.position, Vector2.right * dir * 1f, Color.red); 

            if (hit && m_slideWallBounceTimer > 0.1f && Mathf.Abs(RB.velocity.x) > slideMovementThreshhold)
            {
                DisableCollisionTemporarily();

                Debug.Log(slideWallBounceVelocityMultiplier);
                RB.velocity = new Vector2(RB.velocity.x * slideWallBounceVelocityMultiplier * -1, RB.velocity.y);
                Flip();

                m_slideWallBounceTimer = 0f;
            }

            m_slideTimer += Time.deltaTime;
        }

        // End Sliding
        if ((m_isSliding == true && m_slideTimer > minimumSlideTime) || m_isTouchingVine) // && isAttemptingBounce == false
        {
            if (Mathf.Abs(RB.velocity.x) < slideMovementThreshhold || Input.GetAxisRaw("Vertical") >= -0.1f)
            {
                m_isSliding = false;
                m_slideTimer = 0f;

                canMove = true;

                hitbox.enabled = true;
                slidingHitbox.enabled = false;
            }
        }

        anim.SetBool("isSliding", m_isSliding);
    }

    private IEnumerator DisableCollisionTemporarily()
    {
        Collider2D col = null;
        if (hitbox.enabled) col = hitbox;
        if (slidingHitbox.enabled) col = slidingHitbox;

        col.enabled = false;
        yield return new WaitForFixedUpdate();
        col.enabled = true;
    }

    private void Move()
    {
        float targetVelocity = Input.GetAxisRaw("Horizontal") * m_moveSpeed;
        float move = Mathf.Lerp(RB.velocity.x, targetVelocity, moveAcceleration * Time.deltaTime);
        anim.SetFloat("Speed", Mathf.Abs(targetVelocity));

        // Flip
        if (move > 0.2f && !m_isFacingRight) Flip();
        if (move < -0.2f && m_isFacingRight) Flip();

        // Move
        if (targetVelocity != 0f)
        {
            RB.velocity = new Vector2(move, Mathf.Clamp(RB.velocity.y, maxFallSpeed * -1, Mathf.Infinity));
            hasMoved = true;
        }

        // Drag
        if (targetVelocity == 0f && m_grounded && m_isJumping == false)
        {
            RB.velocity -= new Vector2(RB.velocity.x * groundDrag * Time.deltaTime, 0f);
        }
        else if (targetVelocity == 0f)
        {
            RB.velocity -= new Vector2(RB.velocity.x * airDrag * Time.deltaTime, 0f);
        }
    }

    private void Flip()
    {
        m_isFacingRight = !m_isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(0.7f, 0.25f, 0f));
    }

    private void Swing()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, vineLayer);
        m_isTouchingVine = colliders.Length >= 1;

        // Grab the vine
        if (m_isTouchingVine && canGrabVine)
        {
            canGrabVine = false;

            Rigidbody2D vineSegment = colliders[0].GetComponent<Rigidbody2D>();

            m_hinge.connectedBody = vineSegment;
            m_hinge.connectedAnchor = new Vector2(0f, -1f);
            m_hinge.enabled = true;

            Vector2 vel = new Vector2(RB.velocity.x * 1.5f, 0f);
            vineSegment.velocity = vel;
            RB.velocity = vel;

            m_grabVineTimer = 0.4f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && m_hinge.enabled == true)
        {
            // Disconnect from vine
            m_hinge.connectedBody = null;
            m_hinge.enabled = false;

            // Jump from vine
            m_isJumping = true;
            RB.velocity = new Vector2(RB.velocity.x, jumpForce);
            m_jumpQueued = false;
        }

        if (m_grabVineTimer <= 0f)
        {
            canGrabVine = true;
        }
        if (canGrabVine == false && m_isTouchingVine == false)
        {
            m_grabVineTimer -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        bool wasGrounded = m_grounded;

        Collider2D[] cols = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(0.7f, 0.25f), 0, groundLayer);
        m_grounded = cols.Length >= 1;

        // Test for landing event
        if (!wasGrounded && m_grounded)
        {
            Instantiate(landParticles, groundCheck.transform.position + Vector3.down * 0.2f, Quaternion.identity);
            m_isJumping = false;
            anim.SetTrigger("Landed");

        }
        anim.SetBool("Grounded", !m_isJumping);

        // Que up jump 
        if (!m_grounded && (Input.GetKeyDown(KeyCode.Space)))
        {
            m_jumpQueued = true;
            m_jumpQueuedTimer = 0.1f;
        }

        // Jump 
        if ((m_grounded || m_coyoteTimer >= 0f) && (Input.GetKeyDown(KeyCode.Space) || m_jumpQueued))
        {
            float jumpSpeedBoost = m_isSliding ? Mathf.Sign(RB.velocity.x) * slideJumpAccelerationBoost : jumpAccelerationBoost;

            m_isJumping = true;
            RB.velocity = new Vector2(RB.velocity.x + jumpSpeedBoost, jumpForce);
            m_jumpQueued = false;
            hasMoved = true;
        }

        m_jumpQueuedTimer -= Time.deltaTime;
        if (m_jumpQueued && m_jumpQueuedTimer <= 0f)
        {
            m_jumpQueued = false;
        }

        m_coyoteTimer -= Time.deltaTime;
        if (m_grounded)
        {
            m_coyoteTimer = coyoteTime;
        }
    }

    private void Gravity()
    {
        // Fall faster when not holding jump 
        RB.gravityScale = gravityScale;

        m_moveSpeed = moveSpeed;
        if (m_grounded == false && Mathf.Abs(RB.velocity.y) <= jumpApexThreshHold && Input.GetKey(KeyCode.Space))
        {
            RB.gravityScale = gravityScale * jumpApexGravityMultiplier;
            m_moveSpeed *= jumpApexBonusSpeed;
        }

        if (m_grounded == false && !Input.GetKey(KeyCode.Space))
        {
            RB.gravityScale = gravityScale * fallGravityMultiplier;
        }
    }

    public void Bounce(float force, float horizontalMultiplier = 1f)
    {
        m_isJumping = true;
        RB.velocity = new Vector2(RB.velocity.x * horizontalMultiplier, force);
        m_jumpQueued = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "DieOnCollision" && isResetting == false)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);

            GameManager.Instance.StartCoroutine("ResetPlayer");
            isResetting = true;
        }
    }
}
