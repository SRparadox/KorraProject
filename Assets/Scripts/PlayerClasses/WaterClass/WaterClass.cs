using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VitalStream))]

public class WaterClass: ElementalClass
{
    VitalStream vitalStream;

    private void Start()
    {
        vitalStream = GetComponent<VitalStream>();
    }

    // Aqua Surge
    public override void triggerAttack1()
    {
        Debug.Log("Water Attack 1");
    }

    // Vital Stream
    public override void triggerAttack2()
    {
        Debug.Log("Water Attack 2");

        if (vitalStream != null)
        {
            Debug.Log("vital stream triggered");
            vitalStream.Trigger();
        }
    }

    public override void triggerAbility1()
    {
        Debug.Log("Water Ability 1");
    }

    public override void triggerAbility2()
    {
        Debug.Log("Water Ability 2");
    }

    public override void triggerUltimate()
    {
        Debug.Log("Water Ultimate");
    }


}
