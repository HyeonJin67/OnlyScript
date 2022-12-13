using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 도착시에 마스크 오브젝트 켜주기위한 이벤트 연결
public class MaskHandle : MonoBehaviour
{
    [SerializeField]
    GameObject mask;

    private void Start()
    {
        FindObjectOfType<LoadingIntroductionManager>().actionMask += Cover;
    }
    void Cover()
    {
        mask.SetActive(true);
    }
}
