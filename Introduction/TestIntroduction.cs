using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//씬 로드시 튜토리얼씬 먼저 재생
public class TestIntroduction : MonoBehaviour
{
    void Start()
    {
        LoadingIntroductionManager.LoadScene("Giyeok_Mission");
    }
}
