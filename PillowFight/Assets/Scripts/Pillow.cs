using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour {
    Rigidbody2D rb;
    public bool isThrown = false;

    private PlayerHealth nearPlayer;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Player" && isThrown) {
            nearPlayer = col.gameObject.GetComponent<PlayerHealth>();
            nearPlayer.TakeDamage(150f);
        }
        isThrown = false;
    }

    public void PickedUp(Player p) {
        transform.parent = p.transform;
        transform.position = p.transform.position;
        gameObject.SetActive(false);
    }

    public void Drop() {
        SpriteRenderer sr = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);
        if (sr.flipX) {
            transform.position = transform.parent.position + new Vector3(-1.5f, 0.6959152f, -0.2485f);
            rb.velocity = -(transform.right * 10);
        } else {
            transform.position = transform.parent.position + new Vector3(1.5f, 0.6959152f, -0.2485f);
            rb.velocity = transform.right * 10;
        }
        transform.parent = null;
    }
}
