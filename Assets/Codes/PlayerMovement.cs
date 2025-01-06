using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public LineRenderer wireRenderer;
    public LayerMask grappleLayer;
    public Transform firePoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isGrappling;
    private Vector2 grapplePoint;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wireRenderer.enabled = false;
    }

    void Update()
    {
        Move();
        Jump();
        HandleGrappling();
    }


    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);

        if (moveInput == 0) animator.SetBool("isWalk", false);
        else animator.SetBool("isWalk", true);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("isJump", true);
        }
        if (isGrounded) animator.SetBool("isJump", false);
    }

    void HandleGrappling()
    {
        if (Input.GetMouseButtonDown(0) && !isGrappling && !isGrounded)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, mousePosition - (Vector2)firePoint.position, Mathf.Infinity, grappleLayer);

            if (hit.collider != null)
            {
                isGrappling = true;
                grapplePoint = hit.point;
                wireRenderer.enabled = true;
                wireRenderer.SetPosition(0, firePoint.position);
                wireRenderer.SetPosition(1, grapplePoint);
                animator.SetBool("isJump", true);
            }
        }

        if (Input.GetMouseButtonUp(0) && isGrappling)
        {
            isGrappling = false;
            wireRenderer.enabled = false;
        }

        if (isGrappling)
        {
            Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            wireRenderer.SetPosition(0, firePoint.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
