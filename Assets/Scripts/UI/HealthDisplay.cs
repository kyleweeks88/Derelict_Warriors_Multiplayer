using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats myStats = null;

    [Header("Health Ref")]
    [SerializeField] private Image healthBarImage = null;

    [Header("Stamina Ref")]
    [SerializeField] Image staminaBarImage = null;


    private void OnEnable()
    {
        myStats.Event_HealthChanged += HandleHealthChanged;
        myStats.Event_StaminaChanged += HandleStaminaChanged;
    }

    private void OnDisable()
    {
        myStats.Event_HealthChanged -= HandleHealthChanged;
        myStats.Event_StaminaChanged -= HandleStaminaChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }

    private void HandleStaminaChanged(float currentStam, float maxStam)
    {
        staminaBarImage.fillAmount = currentStam / maxStam;
    }
}
