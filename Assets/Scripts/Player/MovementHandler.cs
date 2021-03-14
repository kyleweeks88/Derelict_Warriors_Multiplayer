using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController charCtrl = null;

    private readonly List<IMovementModifier> modifiers = new List<IMovementModifier>();

    private void Update() => Move();

    public void AddModifier(IMovementModifier modifier) => modifiers.Add(modifier);
    public void RemoveModifier(IMovementModifier modifier) => modifiers.Remove(modifier);

    void Move()
    {
        Vector3 movement = Vector3.zero;

        foreach (IMovementModifier modifier in modifiers)
        {
            movement += modifier.Value;
        }

        charCtrl.Move(movement * Time.deltaTime);
    }
}
