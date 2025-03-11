using UnityEngine;

public class PoseSelection : MonoBehaviour
{
    private Animator animator;
    private enum PoseSelectionEnum{
        RunningPose,
        ReadyToGo,
        Magic,
        Lost,
        FistsUp
    };
    [SerializeField] private PoseSelectionEnum currentPose;
    public bool occasionalLost;
    public bool mirrorAnimations;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Mirror", mirrorAnimations);
        animator.SetInteger("CurrentAnimation", (int)currentPose);
        if(occasionalLost){
            InvokeRepeating("OccasionalLost", 6f, 12f);
        }
    }

    private void OccasionalLost(){
        animator.SetInteger("CurrentAnimation", (int)PoseSelectionEnum.Lost);
        Invoke("ReturnToPreviousPose", 1.8f);
    }

    private void ReturnToPreviousPose(){
        animator.SetInteger("CurrentAnimation", (int)currentPose);
    }

}
