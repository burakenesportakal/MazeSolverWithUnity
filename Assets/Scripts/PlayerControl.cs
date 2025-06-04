using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private bool timerStarted = false;
    bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!canMove) return;

        // Hareket vektörü al
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // İlk kez hareket edilince zamanlayıcıyı başlat
        if ((moveX != 0 || moveZ != 0) && !timerStarted)
        {
            Timer.Instance.StartTimer();
            timerStarted = true;
        }

        // Fizik hareketi
        rb.velocity = move * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Timer.Instance.StopTimer();
            canMove = false;
            rb.velocity = Vector3.zero;
        }
    }
}
