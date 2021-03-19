using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatVar", menuName = "SO_Variables/FloatVariable")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public delegate void OnValueChanged(float curVal, float maxVal);
    public event OnValueChanged Event_ValueChanged;

    public string varName = string.Empty;
    public float InitialValue;
    public float maxValue;
    public bool drainingVital = false;

    [NonSerialized]
	public float RuntimeValue;

    public void OnAfterDeserialize()
	{
		RuntimeValue = InitialValue;
	}

	public void OnBeforeSerialize() { }

    public void InitializeValue()
    {
        RuntimeValue = maxValue;
        this.Event_ValueChanged?.Invoke(RuntimeValue, maxValue);
    }

    public float GetCurrentValue()
    {
        return RuntimeValue;
    }

    public virtual void SetValue(float setVal)
    {
        RuntimeValue = setVal;
        this.Event_ValueChanged?.Invoke(RuntimeValue, maxValue);
    }

    public void ModfiyValue(float modVal)
    {
        RuntimeValue = Mathf.Clamp((RuntimeValue + modVal), 0, maxValue);
        this.Event_ValueChanged?.Invoke(RuntimeValue, maxValue);
    }
}
