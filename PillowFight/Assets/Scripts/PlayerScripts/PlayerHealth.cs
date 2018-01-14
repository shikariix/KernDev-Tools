using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    private float startingHealth = 1000f;
    public float currentHealth;

    private bool isDead;

    void Awake() {
        currentHealth = startingHealth;
    }


    public void TakeDamage(float damage) {
        currentHealth -= damage;

        if (currentHealth <= 0 && !isDead) {
            // ... it should die.
            Death();
        }
    }

    void Death() {
        //Do something
    }
}
