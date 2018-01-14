using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PLAYER: Script is assigned to every player. Input must be variable.
 * Players can:
 * Walk left and right
 * Jump
 * Attack by throwing or slapping with a pillow
 * 
 * Players have:
 * Health
 * An alive or dead state
 * An amount of wins
 */

public class Player : MonoBehaviour {
    //Setting up the variables
    public string inputH;
    public string inputJump;
    public string inputSQR;
    public string inputO;
    public string inputTRI;
    public string inputR1;

    private enum Direction {
        left,
        right
    }
    private Direction dir;

    public List<Pillow> nearPillows = new List<Pillow>();
    private List<Pillow> heldPillows = new List<Pillow>();
    private PlayerHealth nearPlayer;

    private bool isGrounded;
    private bool isBlocking;

    private float jumpVelocity = 8f;
    private float fall = 4f;
    private float lowJump = 2f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public List<string> inputQueue = new List<string>();
    public float inputTimer;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Ground") {
            return;
        }

        if (col.gameObject.tag == "Pillow") {
            nearPillows.Add(col.gameObject.GetComponent<Pillow>());
        }
        if (col.gameObject.tag == "Player") {
            nearPlayer = col.gameObject.GetComponent<PlayerHealth>();
        }
    }
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Pillow") {
            nearPillows.Remove(col.gameObject.GetComponent<Pillow>());
        }
        if (col.gameObject.tag == "Player") {
            nearPlayer = null;
        }
    }

    //Movement functions
    public void Move() {
        float moveX = Input.GetAxis(inputH) / 4;
        transform.position += new Vector3(moveX, 0);
        if (Input.GetAxis(inputH) < 0) {
            sr.flipX = true;
        } else if (Input.GetAxis(inputH) > 0) {
            sr.flipX = false;
        }

        //input
        if (Input.GetButton(inputSQR)) {
            Block();
        } else if (!Input.GetButtonDown(inputSQR)) {
            isBlocking = false;
        }
        if (Input.GetButtonDown(inputO) && nearPillows.Count > 0) {
            //this errors when there is no pillow near
            //fix later
            PickupPillow(nearPillows[0]);
        }

        if (Input.GetButtonDown(inputTRI) && heldPillows != null) {
            SlapAttack();
        }
        if (Input.GetButtonDown(inputR1) && heldPillows.Count > 0) {
            ThrowAttack(heldPillows[0]);
        }

        if (Input.GetButtonDown(inputJump) && isGrounded) {
            isGrounded = false;
            rb.velocity = Vector2.up * jumpVelocity;
        }
        if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fall - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton(inputJump)) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJump - 1) * Time.deltaTime;
        }
        
    }

    //Attack functions
    //A pillow needs to be picked up before any attack works
    void PickupPillow(Pillow p) {
        heldPillows.Add(p);
        p.PickedUp(this);
        
    }
    void DropPillow(Pillow p) {
        p.Drop();
        heldPillows.RemoveAt(0);

    }
    
    void ThrowAttack(Pillow p) {
        Debug.Log("BLAMO!!");
        //pillow is 'dropped' when thrown
        p.isThrown = true;
        DropPillow(p);
    }

    void SlapAttack() {
        Debug.Log("SLAPO!!");
        if (nearPlayer != null ) { 
            nearPlayer.TakeDamage(100f);
        }
    }

    void Block() {
        Debug.Log("Blocked!");
        isBlocking = true;
    }
}
