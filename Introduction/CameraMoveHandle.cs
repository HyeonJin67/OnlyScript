using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 이동
public class CameraMoveHandle : MonoBehaviour
{
    [SerializeField]
    Transform camArrive;
    [SerializeField]
    AnimationCurve curve;
    Vector3 startposition;

    float lerpTime;
    float currentTime;
    float move;
    void Start()
    {
        StartCoroutine(Move2());   
    }
    IEnumerator Move2()
    {
        GetComponent<Camera>().depth = 2;
        startposition = transform.position;
        lerpTime = 4f;
        currentTime = 0;
        while (move < 1)
        {
            currentTime += Time.deltaTime;
            move = currentTime / lerpTime;
            //transform.position = Vector3.Lerp(startposition, camPosi.position, move);
            transform.position = Vector3.Lerp(startposition, camArrive.position, curve.Evaluate(currentTime / lerpTime));
            yield return null;
        }
    }
}
