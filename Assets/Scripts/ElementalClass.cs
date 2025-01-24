using UnityEngine;
using UnityEngine.InputSystem;

public class ElementalClass: MonoBehaviour
{
    public float health = 100;

    PlayerInput playerInput;

    InputAction attack1Action, attack2Action, ability1Action, ability2Action, ultimateAction;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        // Attacks
        attack1Action = playerInput.actions.FindAction("Attack1");
        attack2Action = playerInput.actions.FindAction("Attack2");

        // Abilities
        ability1Action = playerInput.actions.FindAction("Ability1");
        ability2Action = playerInput.actions.FindAction("Ability2");

        ultimateAction = playerInput.actions.FindAction("Ultimate");

    }

    void Update()
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        if (attack1Action.triggered)
        {
            triggerAttack1();
        }

        if (attack2Action.triggered)
        {
            triggerAttack2();
        }

        if (ability1Action.triggered)
        {
            triggerAbility1();
        }

        if (ability2Action.triggered)
        {
            triggerAbility2();
        }

        if (ultimateAction.triggered)
        {
            triggerUltimate();
        }
    }

    public virtual void triggerAttack1()
    {
    }

    public virtual void triggerAttack2()
    {
    }

    public virtual void triggerAbility1()
    {
    }

    public virtual void triggerAbility2()
    {
    }

    public virtual void triggerUltimate()
    {
    }
}
