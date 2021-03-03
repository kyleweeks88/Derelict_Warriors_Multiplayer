using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HealthManager healthManager = null;
    [SerializeField] private CharacterStats myStats = null;

    [Header("Health Ref")]
    [SerializeField] private Image healthBarImage = null;

    //[Header("Stamina Ref")]
    //[SerializeField] Image staminaBarImage = null;


    private void OnEnable()
    {
        healthManager.Event_HealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        healthManager.Event_HealthChanged -= HandleHealthChanged;
    }

    void HandleHealthChanged(float curVal, float maxVal)
    {
        healthBarImage.fillAmount = curVal / maxVal;
    }

    // void HandleStaminaChanged(float curVal, float maxVal)
    // {
    //     staminaBarImage.fillAmount = curVal / maxVal;
    // }
}
