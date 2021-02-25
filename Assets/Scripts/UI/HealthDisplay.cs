using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats myStats = null;
    [SerializeField] private Image healthBarImage = null;


    private void OnEnable()
    {
        myStats.Event_HealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        myStats.Event_HealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
        Debug.Log("Healthbar UI changed");
    }
}
