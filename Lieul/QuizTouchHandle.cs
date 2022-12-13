using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//퀘스트별로 정답처리용 OnMouse관련 기능
public class QuizTouchHandle : MonoBehaviour
{
    [Tooltip("정답용 오브젝트이면 체크")]
    [SerializeField]
    bool answerCheck; //정답인 오브젝트 확인용 파라미터
    int completedQuizOrder;
    private void OnEnable()
    {
        if(answerCheck) FindObjectOfType<QuizManager>().AddNRemove = gameObject;
    }
    //퀘스트의 마지막 한 개의 오브젝트에서 몇번째 퀘스트인지 알려주는 프로퍼티
    public int CompletedQuizOrder
    {
        set
        {
            completedQuizOrder = value;
        }
    }
    private void OnMouseUp()
    {
        if (GetComponent<Outline>() && GetComponent<Animator>()) //4번째 퀘스트용 아웃라인
        {
            GetComponent<Outline>().enabled = true;
            GetComponent<Animator>().enabled = false;
        }
        if (answerCheck)
        {
            print("정답클릭!");
            if (GetComponent<Rigidbody>()) //2번째 퀘스트용 떨어뜨리기
            {
                GetComponent<Rigidbody>().useGravity = true;
            }
            if (completedQuizOrder != 3) //3번째 퀘스트만 제외하고 지연시키기
            {
                StartCoroutine(DelayComplete());
            }
            else //나머지 퀘스트들은 "바로" 완료처리
            {
                FindObjectOfType<QuizManager>().AddNRemove = gameObject;
                Destroy(GetComponent<QuizTouchHandle>()); //같은 오브젝트를 두번이상 눌렀을 경우 또다시 정답처리 오브젝트 리스트에 추가되는 일을 방지하기 위한 컴포넌트 제거
            }
        }
        else
        {
            print("꽝");
            //여기에 틀렸을 경우의 소리 넣기
        }
    }
    //퀘스트완료 지연 함수
    IEnumerator DelayComplete()
    {
        yield return new WaitForSeconds(2.5f);
        FindObjectOfType<QuizManager>().AddNRemove = gameObject;
        Destroy(GetComponent<QuizTouchHandle>());
    }
}
