using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Automatically attach these scripts
[RequireComponent(typeof(VitalStream))]

public class WaterClass: ElementalClass
{
    VitalStream vitalStream;

    private void Start()
    {
        vitalStream = GetComponent<VitalStream>();
    }

    // Aqua Surge
    public override void performAttack1()
    {
        Debug.Log("Water Attack 1");
    }

    // Vital Stream
    public override void performAttack2()
    {
        Debug.Log("Water Attack 2");

        if (vitalStream != null)
        {
            Debug.Log("vital stream triggered");
            vitalStream.Trigger();
        }
    }

    public override void performAbility1()
    {
        Debug.Log("Water Ability 1");
    }

    public override void performAbility2()
    {
        Debug.Log("Water Ability 2");
    }

    public override void performUltimate()
    {
        Debug.Log("Water Ultimate");
    }
}
