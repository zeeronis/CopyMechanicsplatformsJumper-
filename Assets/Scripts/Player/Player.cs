using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
#pragma warning disable CS0649
    private Rigidbody2D rigidbody2d;
    private Collider2D collider2d;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
#pragma warning restore CS0649

    [Space]
    [Range(0f, 0.5f)]
    [SerializeField] private float maxImageScale = 0.4f;
    [SerializeField] private float maxVelocityForceY = 12f;
    [SerializeField] private float startVelocityForceY = 3.5f;
    [SerializeField] private float velocityForceYSpeed = 0.18f;

    private float currVelocityForceY = 0;

    public bool IsGrounded
    {
        get
        {
            Vector2 position = (Vector2)collider2d.transform.position - new Vector2(0, collider2d.bounds.size.y / 2);
            Vector2 direction = Vector2.down;
            float distance = collider2d.bounds.size.y / 2 + 0.025f;

#if UNITY_EDITOR
            Debug.DrawLine(position, position + new Vector2(0, -distance), Color.red);
#endif

            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
            if (hit.collider != null)
            {
                return true;
            }
            return false;
        }
    }
    
    public float CurrVelocityForceY
    {
        get
        {
            return currVelocityForceY;
        }
        set
        {
            var lastValue = currVelocityForceY;
            currVelocityForceY = value < maxVelocityForceY
                ? value
                : maxVelocityForceY;

            if (lastValue != currVelocityForceY)
            {
                ChangeImageScale();
            }
        }
    }

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded || !GameManager.Instance.GameIsReadyForStart)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        if (Input.GetMouseButton(0))
        {
            TryStartGame();
            AddVelocityForce();
        }
        else if (CurrVelocityForceY > 0)
        {
            Jump();
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR

        if (Input.touches.Length > 0)
        {
            TryStartGame();
            AddVelocityForce();
        }
        else if (CurrVelocityForceY > 0)
        {
            Jump();
        }
#endif
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded && rigidbody2d.velocity.y < 2)
        {
            GameManager.Instance.SetPlayerPlatform(collision.transform);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        {
            GameManager.Instance.GameOver(true);
        }
    }

    private void TryStartGame()
    {
        if (!GameManager.Instance.GameIsRunned && GameManager.Instance.GameIsReadyForStart)
        {
            GameManager.Instance.StartGame();
        }
    }

    private void AddVelocityForce()
    {
        if (CurrVelocityForceY != 0)
        {
            CurrVelocityForceY += velocityForceYSpeed;
        }
        else
        {
            CurrVelocityForceY = startVelocityForceY;
        }
    }

    private void Jump()
    {
        transform.parent = null;
        rigidbody2d.simulated = true;
        rigidbody2d.velocity = new Vector2(0, CurrVelocityForceY);
        CurrVelocityForceY = 0;
    }

    private void ChangeImageScale()
    {
        spriteRenderer.transform.localScale = new Vector3(
            1,
            1 - (maxImageScale * (currVelocityForceY / maxVelocityForceY)),
            1);
    }
}
