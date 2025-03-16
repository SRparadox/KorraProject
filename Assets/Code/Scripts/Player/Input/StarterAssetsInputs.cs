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
        
        [Header("One Handed Special Code")]
        [SerializeField] private PlayerInput playerInput;
        public int selectedAbility = 0;
        private bool readyToScroll = true;



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
        public void OnUseAttack()
        {
            if (playerInput.currentControlScheme != "OneHanded") return;
            characterClass.UseAbility(selectedAbility);
        }

        public void onNextAbility(float value){
            Debug.Log("Scrolling Detected: " + value);
            if (value < 0) return; //Scrolling down
            Debug.Log("Scrolling up");
            if (!readyToScroll) return;
            
        }
#endif


        public void nextAbility(){
            if (isOneHanded()) {
                selectedAbility = (selectedAbility + 1) % 5;
                readyToScroll = false;
                StartCoroutine(ResetScroll());
            }
        }
        IEnumerator ResetScroll(){
            yield return new WaitForSeconds(0.2f);
            readyToScroll = true;
        }


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("RoundKick"))
            {
                return;
            }
            else if (move.y < 0){
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
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("RoundKick"))
            {
                animator.SetLayerWeight(2, 0);
            }
            if (isOneHanded()){
                if (playerInput.actions["NextAbility"].ReadValue<float>() > 0 && readyToScroll){
                    nextAbility();
                }
                else if (playerInput.actions["NextAbility"].ReadValue<float>() < 0){
                    JumpInput(true);
                }
            }
            
        }

        public bool isOneHanded(){
            return playerInput.currentControlScheme == "OneHanded";
        }

        private void enableOneHanded(){
            playerInput.SwitchCurrentControlScheme("OneHanded", Mouse.current);
        }

        private void DisableOneHanded(){
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }

        public void setOneHanded(bool val){
            Debug.Log("Step 3" + val);
            if (val) enableOneHanded();
            else DisableOneHanded();
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