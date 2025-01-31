using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedStream: MonoBehaviour
{
    //[SerializeField] Spline spline;
    //[SerializeField] ExampleContortAlong contortAlong;

    public void Trigger()
    {
        Debug.Log("Triggered guided stream!");
        //StopAllCoroutines();
        //StartCoroutine(Coroutine_StreamBend());
    }
    //IEnumerator Coroutine_StreamBend()
    //{
    //    spline.gameObject.SetActive(false);

    //    ConfigureSpline();

    //    contortAlong.Init();
    //}

    //private void ConfigureSpline()
    //{

    //}

}
