using Cinemachine;
using StarterAssets;
using UnityEngine;

public class ThirdPersonShooterController: MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    private StarterAssetsInputs staterAssetsInputs;

    private void Awake()
    {
        staterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (staterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
        } else
        {
            aimVirtualCamera.gameObject.SetActive(false);
        }
    }

}
