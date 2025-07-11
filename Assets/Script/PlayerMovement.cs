using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject panelKemenanganLevel2;
    public int skorTargetLevel2 = 50;
    private bool level2SudahMenang = false;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -19.62f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45.0f;
    public int currentHealth = 3;
    public int currentScore = 0;
    public TextMeshProUGUI healthTextUI;
    public TextMeshProUGUI scoreTextUI;
    private bool isLoadingNextLevel = false;
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public GameObject ballPrefab;
    public Transform throwPoint;
    public float throwForce = 15f;
    public int maxBallsInScene = 10;
    public float raycastMaxDistance = 100f;

    private Queue<GameObject> thrownBallsQueue = new Queue<GameObject>();
    private Camera playerCamera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("PlayerMovement: Main Camera tidak ditemukan!");
            enabled = false;
            return;
        }

        if (throwPoint == null)
        {
            Transform existingThrowPoint = playerCamera.transform.Find("ThrowPoint");
            if (existingThrowPoint != null)
            {
                throwPoint = existingThrowPoint;
            }
            else
            {
                GameObject tp = new GameObject("ThrowPoint");
                tp.transform.SetParent(playerCamera.transform);
                tp.transform.localPosition = new Vector3(0, -0.1f, 0.7f);
                throwPoint = tp.transform;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateHealthUI();
        UpdateScoreUI();
        currentScore = 0;
        level2SudahMenang = false;
        UpdateScoreUI();
        UpdateHealthUI();

        if (panelKemenanganLevel2 != null)
        {
            panelKemenanganLevel2.SetActive(false);
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            if (ballPrefab != null && throwPoint != null && playerCamera != null)
            {
                ThrowBall();
            }
            else
            {
                if (ballPrefab == null) Debug.LogError("Ball Prefab belum di-assign!");
                if (throwPoint == null) Debug.LogError("Throw Point belum di-assign!");
                if (playerCamera == null) Debug.LogError("Player Camera tidak ditemukan!");
            }
        }
    }

    void ThrowBall()
    {
        if (thrownBallsQueue.Count >= maxBallsInScene)
        {
            GameObject oldestBall = thrownBallsQueue.Dequeue();
            Destroy(oldestBall);
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, raycastMaxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(raycastMaxDistance);
        }

        Vector3 directionToThrow = (targetPoint - throwPoint.position).normalized;
        GameObject newBall = Instantiate(ballPrefab, throwPoint.position, Quaternion.LookRotation(directionToThrow));
        Rigidbody ballRb = newBall.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            ballRb.AddForce(directionToThrow * throwForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Prefab Bola tidak memiliki komponen Rigidbody!");
        }

        thrownBallsQueue.Enqueue(newBall);
    }

    public void AddScore(int amount)
    {
        if (isLoadingNextLevel || level2SudahMenang) return;

        currentScore += amount;
        UpdateScoreUI();

        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            if (currentScore >= 30 && !isLoadingNextLevel)
            {
                Debug.Log("Skor di Level 1 mencapai target! Memuat Level 2...");
                LoadNextLevel("Level 2");
            }
        }

        else if (SceneManager.GetActiveScene().name == "Level 2")
        {
            if (currentScore >= skorTargetLevel2 && !level2SudahMenang)
            {
                TampilkanPopupMenangLevel2();
            }
        }
    }

    void LoadNextLevel(string levelName)
    {
        if (isLoadingNextLevel) return;

        isLoadingNextLevel = true;
        SceneManager.LoadScene(levelName);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthTextUI != null)
        {
            healthTextUI.text = "Health: " + currentHealth;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = "Skor: " + currentScore;
        }
    }

    void TampilkanPopupMenangLevel2()
    {
        level2SudahMenang = true;

        if (panelKemenanganLevel2 != null)
        {
            panelKemenanganLevel2.SetActive(true);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}