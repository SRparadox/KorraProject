using UnityEngine;
using UnityEngine.InputSystem;

/* 
Elements of a Character Base Class
- Properties: health, speed, jump height, etc.
- Retrieve/handle input
- Handle Cooldowns
- Destructor
*/

public abstract class ElementalClass: MonoBehaviour
{
    // Character class variables

    void Start()
    {
        // Get references to player input
    }

    void Update()
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {

    }

    public abstract void performAttack1();
    public abstract void performAttack2();
    public abstract void performAbility1();
    public abstract void performAbility2();
    public abstract void performUltimate();
}
