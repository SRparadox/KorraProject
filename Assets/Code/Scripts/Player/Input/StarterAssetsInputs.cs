using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs: MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move = Vector2.zero;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool aim;
        public bool attack;
        public Animator animator;
        private bool LayerActive = true;
        private float strafingValue = 0;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
        public CharacterClass characterClass;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
            if (value.isPressed) {
                if (LayerActive) {
                    StopAllCoroutines();
                    StartCoroutine(slowlyDecreaseLayerWeight(1));
                    LayerActive = false;
                }
            }
            else {
                if (!LayerActive) {
                    StopAllCoroutines();
                    StartCoroutine(slowlyIncreaseLayerWeight(1));
                    LayerActive = true;
                }
            }
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }

        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }
        public void OnAttack1(InputValue value)
        {
            characterClass.UseAbility(0);
        }
        public void OnAttack2(InputValue value)
        {
            characterClass.UseAbility(1);
        }
        public void OnAbility1()
        {
            characterClass.UseAbility(2);
        }
        public void OnAbility2()
        {
            characterClass.UseAbility(3);
        }
        public void OnUltimate()
        {
            characterClass.UseAbility(4);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
            if (move.y < 0){
                animator.SetLayerWeight(2, math.abs(move.y));
                animator.SetBool("backwards", true);
            }
            else {
                animator.SetBool("backwards", false);
                animator.SetLayerWeight(2, math.abs(move.x));
            }
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        void Update()
        {
            strafingValue = math.lerp(strafingValue, move.x, Time.deltaTime * 1);
            animator.SetFloat("Strafe", strafingValue);
            
        }


        IEnumerator slowlyDecreaseLayerWeight(int layerIndex)
        {
            while (animator.GetLayerWeight(layerIndex) > .2)
            {
                animator.SetLayerWeight(layerIndex, animator.GetLayerWeight(layerIndex) - 0.1f);
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        IEnumerator slowlyIncreaseLayerWeight(int layerIndex)
        {
            while (animator.GetLayerWeight(layerIndex) < 1)
            {
                animator.SetLayerWeight(layerIndex, animator.GetLayerWeight(layerIndex) + 0.1f);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }


    
}