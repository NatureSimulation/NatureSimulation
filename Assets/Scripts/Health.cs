using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static float maxHealth = 1000f;
    public float currentHealth = maxHealth;
    public RectTransform healthbar;

    public bool TakeDamage(float amount) {
        currentHealth -= amount;

        if (currentHealth <= 0) {
            currentHealth = 0f;
        }

        healthbar.sizeDelta = new Vector2(currentHealth * 0.1f, healthbar.sizeDelta.y);

        return currentHealth == 0;
    }
}
