using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lerp로 등장할때 움직임 
public class AppearHandle : MonoBehaviour
{
    float lerpTime = 2f; //등장시 소요시간
    float currentTime = 0;
    Vector3 start;
    Vector3 startrotation;
    [SerializeField]
    Transform arrive; //도착지점

    Animator animator;
    void Start()
    {
        start = transform.position;
        StartCoroutine(Appear());
        if (GetComponent<Animator>()) 
        { 
            animator = GetComponent<Animator>();
            animator.SetInteger("Lieul_Quiz", 1);
        }
    }
    //void Update()
    //{
    //    currentTime += Time.deltaTime;
    //    transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation, currentTime / lerpTime);
    //}
    IEnumerator Appear()
    {
        while (currentTime / lerpTime < 1)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, arrive.position, currentTime / lerpTime);
            yield return null;
        }
        if (GetComponent<Animator>())
        {
            animator = GetComponent<Animator>();
            if (animator.parameterCount != 0)
                animator.SetInteger("Lieul_Quiz", 2);
            else
                animator.enabled = false;
        }
    }
}
