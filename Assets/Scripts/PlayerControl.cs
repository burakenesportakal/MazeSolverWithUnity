using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;
    public float turnSpeed = 3f;
    float moveX, moveZ;

    [SerializeField] public GameObject victoryParticlePrefab1;
    [SerializeField] public GameObject victoryParticlePrefab2;
    [SerializeField] public GameObject victoryParticlePrefab3;
    public AudioClip victoryClip;
    private AudioSource audioSource;

    private bool timerStarted = false;
    private bool isMoving = false;
    bool canMove = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (!canMove) return;
        PlayerMovementController();
        AnimationController();
    }
    private void AnimationController()
    {
        animator.SetFloat("Horizontal", moveX, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", moveZ, 0.1f, Time.deltaTime);
    }
    private void PlayerMovementController()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");
        if ((moveX != 0 || moveZ != 0) && !timerStarted)
        {
            Timer.Instance.StartTimer();
            timerStarted = true;
        }

        movement = new Vector3(moveX, 0f, moveZ).normalized;

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Timer.Instance.StopTimer();
            canMove = false;
            rb.velocity = Vector3.zero;
            PlaySound();
            SpawnVictoryEffect();
            StartCoroutine(ShowFinishScreenDelayed(1.5f));
        }
    }
    private void SpawnVictoryEffect()
    {
        Vector3 goalPos = FindObjectOfType<MazeGenerator>().GoalWorldPosition;
        Instantiate(victoryParticlePrefab1, goalPos, Quaternion.identity);
        Instantiate(victoryParticlePrefab2, goalPos, Quaternion.identity);
        Instantiate(victoryParticlePrefab3, goalPos, Quaternion.identity);
    }
    void FixedUpdate()
    {

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    IEnumerator ShowFinishScreenDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<FinishPanelManager>().ShowPause();
    }


    private void PlaySound()
    {
        if (victoryClip != null && audioSource != null)
            audioSource.PlayOneShot(victoryClip);
    }

}
