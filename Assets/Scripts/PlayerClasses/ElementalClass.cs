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

    public abstract void triggerAttack1();
    public abstract void triggerAttack2();
    public abstract void triggerAbility1();
    public abstract void triggerAbility2();
    public abstract void triggerUltimate();
}
