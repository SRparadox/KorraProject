using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Automatically attach these scripts
[RequireComponent(typeof(VitalStream))]

public class WaterClass: CharacterClass
{
    VitalStream vitalStream;

    private void Start()
    {
        vitalStream = GetComponent<VitalStream>();
    }

    // Aqua Surge
    public override void PerformAttack1()
    {
        Debug.Log("Water Attack 1");
    }

    // Vital Stream
    public override void PerformAttack2()
    {
        Debug.Log("Water Attack 2");

        if (vitalStream != null)
        {
            Debug.Log("vital stream triggered");
            vitalStream.Trigger();
        }
    }

    public override void PerformAbility1()
    {
        Debug.Log("Water Ability 1");
    }

    public override void PerformAbility2()
    {
        Debug.Log("Water Ability 2");
    }

    public override void PerformUltimate()
    {
        Debug.Log("Water Ultimate");
    }
}
