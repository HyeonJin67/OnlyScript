using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://intrepidgeeks.com/tutorial/rotation-that-can-be-used-in-the-game-is-made-in-unity-quaternion#!
//이정표 회전용
public class PoleRotationHandle : MonoBehaviour
{
    float lerpTime; //Lerp의 도착때까지 총 소요시간
    float currentTime; //등속운동을 위한 델타타임 저장용
    Quaternion startposition; //Lerp에서 사용할 시작위치 고정용으로 첫 위치를 담아둘 변수

    [SerializeField]
    int direction; //이정표별 저장해둔 각도의 인덱스값 받아오기
    int[] directions = { 180, 270, 180, 90}; //골목길마다 있는 이정표별 회전해야될 각도 저장용

    private void Start()
    {
        StartCoroutine(Pole(direction));
    }
    IEnumerator Pole(int index)
    {
        lerpTime = 0.5f;
        currentTime = 0;
        startposition = transform.rotation;
        while ((currentTime / lerpTime) < 1) //Lerp의 등속
        {
            currentTime += Time.deltaTime;
            //Vector3.up축 즉, y축 기준으로 앞의 각도만큼 회전
            transform.rotation = Quaternion.Slerp(startposition, Quaternion.AngleAxis(directions[index], Vector3.up), currentTime / lerpTime); 
            yield return null;
        }
    }
}
