using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerObj playerObj;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject movementBoundsRect;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibleDuration = 2f;
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI gameTimerText;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private Vector2 movementInput;
    private Vector2 movementBoundsMin;
    private Vector2 movementBoundsMax;

    private Color spriteColor;

    private Shoot_script shootScript;

    private float gameStartTime;

    private bool isInvincible = false;
    private bool isFiring = false;
    private bool blinkToggle = false;

    private void Awake()
    {
        // Initialize references and data
        playerTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shootScript = GetComponent<Shoot_script>();

        // Initialize movement bounds from MovementRect's two children positions
        movementBoundsMin = movementBoundsRect.transform.GetChild(0).position;
        movementBoundsMax = movementBoundsRect.transform.GetChild(1).position;

        // Initialize player life
        playerObj.life = 3;

        spriteColor = spriteRenderer.color;
    }

    private void Start()
    {
        gameStartTime = Time.time;
    }

    private void Update()
    {
        // Update UI
        lifeText.text = $"Life: {playerObj.life}";
        gameTimerText.text = $"Timer: {(Time.time - gameStartTime):0.0}";
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + movementInput * speed * Time.fixedDeltaTime;

        // Clamp position inside movement bounds
        float clampedX = Mathf.Clamp(newPosition.x, movementBoundsMin.x, movementBoundsMax.x);
        float clampedY = Mathf.Clamp(newPosition.y, movementBoundsMin.y, movementBoundsMax.y);

        rb.MovePosition(new Vector2(clampedX, clampedY));
    }

    /// <summary>
    /// Callback for movement input.
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback for fire toggle input.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && !isInvincible)
        {
            isFiring = !isFiring;
            shootScript.IsFire = isFiring;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("EnemiesBullet") || collision.CompareTag("Enemy")) && !isInvincible)
        {
            playerObj.life--;
            shootScript.ResetBall();
            shootScript.ResetSpeed();
            StartCoroutine(InvincibilityRoutine());
            CheckDeath();
        }
    }

    private void CheckDeath()
    {
        if (playerObj.life <= 0)
        {
            playerObj.timer = Time.time - gameStartTime;
            gameObject.SetActive(false);
            SceneManager.LoadScene(2); // Assuming scene index 2 is Game Over or next scene
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        isFiring = false;
        shootScript.IsFire = false;

        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            blinkToggle = !blinkToggle;
            spriteColor.a = blinkToggle ? 0.1f : 1f;
            spriteRenderer.color = spriteColor;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Reset sprite visibility and invincibility
        spriteColor.a = 1f;
        spriteRenderer.color = spriteColor;
        isInvincible = false;
    }

    public Vector2 GetMovement() => movementInput;

    public void AddLife(int amount = 1) => playerObj.life += amount;
}
