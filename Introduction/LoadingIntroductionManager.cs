using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//https://www.youtube.com/watch?v=MyVY-y_jK1I&t=346s
//https://wergia.tistory.com/59
public class LoadingIntroductionManager : MonoBehaviour
{
    #region 변수
    [SerializeField]
    IntroObjects tutorialObjects; //튜토리얼에 등장할 오브젝트들을 가지고 있는 Scriptable Object
    [SerializeField]
    GameObject[] objectPosi; //오브젝트들의 위치값 지정용
    [SerializeField]
    GameObject[] backPosi; //자음별 배경 위치값 지정용
    [SerializeField]
    Transform underPipe; //파이프 밑에 Lerp의 도착위치
    [SerializeField]
    Transform arrive1; //Lerp의 도착위치

    Vector3 startposition; //Lerp에서 사용할 시작위치 고정용으로 첫 위치를 담아둘 변수
    float lerpTime; //Lerp의 도착때까지 총 소요시간
    float currentTime; //등속운동을 위한 델타타임 저장용

    int i = 0;
    float term = 0; //오브젝트별로 일렬로 세우기 위한 Lerp의 도착위치 간격 조절용
    float plusTime = 3; //오브젝트별로 일렬로 세우기 위한 Lerp의 속도 조절용
    int objectsLength; //scriptable object에서 가져온 자음별 오브젝트의 갯수(길이) 저장용
    AudioSource perAudio; //오브젝트별 자기소개용 
    Animator anim;

    public static string nextScene; //씬재생 지연함수가 끝나면(튜토리얼이 끝나면) 재생할 씬의 이름 저장용

    public System.Action actionMask; //파이프밑에 도착시(특정조건)에 마스크(돋보기)를 켜주기 위한 이벤트
    #endregion
    #region 공부용
    //공부용
    //void Update()
    //{
    //    currentTime += Time.deltaTime;
    //    transform.position = Vector3.Lerp(startposition, arrive.position, currentTime / lerpTime); //등속
    //    //transform.position = Vector3.Lerp(startposition, endposition, Mathf.SmoothStep(0,1,currentTime / lerpTime)); //감속
    //    //transform.position = Vector3.Lerp(startposition, endposition, curve.Evaluate(currentTime / lerpTime)); //애니메이션커브로 속도 조절
    //}
    //IEnumerator Begin() //코루틴 호출을 지연(for문이랑 다르게 3초 간격으로 코루틴을 천천히 호출하기 위한 지연 함수)
    //{
    //    while(i < objectsLength) //오브젝트 갯수만큼만 반복
    //    {
    //        startposition = objectPosi[i].transform.position;
    //        //StartCoroutine(Move1(i, startposition, term));
    //        i++;
    //        term += 3f;
    //        yield return new WaitForSeconds(3); //3초 기다리기
    //    }
    //    //StartCoroutine(MoveOrdered());
    //}

    //Animator anim;
    /*    private void OnCollisionEnter(Collision collision)
        {
            print(collision.contacts[0].point); //충돌체 위치 포인트?
        }
    /*    private void sh()
        {
            anim.SetInteger(anim.GetComponent<Animator>().GetParameter(0).name,1); //애니메이터 파라미터 값 가져오기
        }*/
    #endregion
    #region 함수
    //시연용으로 Intro씬이 너무 길어서 넘기고 싶을 때 스페이스바 누르면 바로 다음 씬으로 전환
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(nextScene);
    }
    //씬호출시 LoadingTutorialManager.LoadScene("호출할 씬의 이름");로 참조
    public static void LoadScene(string sceneName) //객체 생성없이 바로 갖다 쓸수 있는 정적(static) 함수
    {
        print("다음씬이름 체크:" + sceneName);
        nextScene = sceneName;
        SceneManager.LoadScene("Introduction");
    }
    IEnumerator LoadScene() //씬 재생 전에 튜토리얼 먼저 재생해주는 비동기식 재생 지연 함수
    {
        yield return null;
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false;
        //float timer = 0f;
        while (!async.isDone)
        {
            yield return null;
            //timer += Time.deltaTime;
            //if (async.progress >= 0.9f)
            //{
                //yield return new WaitForSeconds(10); //90%진행 체크라 100%진행율을 위한 추가 10초
                async.allowSceneActivation = true;
            //}
        }
    }
    private void Start() //제일 먼저 Scriptable Object에서 가져온 프리팹들을 생성 및 시작위치에 위치시키고 이동을 시작하는 함수 Go()를 호출
    {
        tutorialObjects.Insert();
        for (i = 0; i < backPosi.Length; i++)
        {
            if (backPosi[i].name.Contains(nextScene.Split('_')[0]))
            {
                tutorialObjects.Eviroments[i].transform.localScale = backPosi[i].transform.localScale;
                backPosi[i] = Instantiate(tutorialObjects.Eviroments[i], backPosi[i].transform.position, backPosi[i].transform.rotation);
                backPosi[i].SetActive(true);
            }
        }
        if (tutorialObjects.GetIndex().TryGetValue(nextScene.Split('_')[0], out IntroductionObjects[] objects)) //nextScene이름의 언더바(_)앞에 글자가 key인 Dictionary가 가지고 있는 IntroductionObjects[]를 IntroductionObjects[]형태로 선언된 objects변수로 가져오기(out)  
        {
            objectsLength = objects.Length;
            for (i = 0 ; i <  objectsLength; i++)
            {
                print(objects[i].Object);
                objects[i].Object.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f); 
                objectPosi[i] = Instantiate(objects[i].Object, objectPosi[i].transform.position, Quaternion.LookRotation(Vector3.left)); //파괴도 해주기? //objects[i].Object.transform.position = objectPosi[i].transform.position;
                if (objectPosi[i].GetComponent<Animator>())
                {
                    anim = objectPosi[i].GetComponent<Animator>();
                    anim.SetInteger("Introduction", 1); // anim.SetInteger(anim.GetParameter(1).name, 1);
                }
                objectPosi[i].AddComponent<AudioSource>();
                perAudio = objectPosi[i].GetComponent<AudioSource>();
                perAudio.clip = objects[i].perAudio;
            }
        }
        Go();
    }
    void Go() //Lerp를 시작할 이동 시작 함수
    {
        //print(nextScene.Split('_')[0]); //소괄호안에 언더바를 기준으로 string값 쪼개기
        for ( i = 0; i < objectsLength; i++)
        {
            startposition = objectPosi[i].transform.position; //시작위치 고정용
            StartCoroutine(Move1(i, startposition, term, plusTime));
            term += 3f; //일렬 간격 조정용
            plusTime += 5f; //동시에 움직임으로 속도 조절로 일렬 유지
        }
        ////StartCoroutine(Begin());
        ////동시에 Lerp로 움직이게 하기 위해 for문으로 빠르게 돌려서 코루틴 호출하기 위한 것
        //for (int i = 0; i < objectPosi.Length; i++)
        //{
        //    startposition = objectPosi[i].transform.position; //시작위치 고정용
        //    StartCoroutine(Move1(i, startposition, term, plusTime));
        //    term += 3f; //일렬 간격 조정용
        //    plusTime += 5f; //동시에 움직임으로 속도 조절로 일렬 유지
        //}
    }
    IEnumerator Move1(int index, Vector3 startPosi, float term, float plusLerpTime)
    {
        lerpTime = plusLerpTime;
        currentTime = 0;
        while ((currentTime / lerpTime) < 1) //Lerp의 등속
        {
            currentTime += Time.deltaTime;
            objectPosi[index].transform.position = Vector3.Lerp(startPosi, new Vector3(arrive1.position.x + term, arrive1.position.y, arrive1.position.z), currentTime / lerpTime);
            yield return null;
        }
        yield return new WaitUntil(() => index == objectsLength - 1); //괄호안에 조건이 true가 될 때 밑에 줄 실행
        StartCoroutine(MoveOrdered());
    }
    IEnumerator MoveOrdered() //코루틴 호출을 지연
    {
        i = 0;
        while (i < objectsLength)
        {
            print(i + "번째 오브젝트 출발");
            startposition = objectPosi[i].transform.position;
            yield return StartCoroutine(Move3(i, startposition)); //Move3코루틴이 끝나야지 밑에 줄 실행
            yield return new WaitForSeconds(5); //여기서 소개멘트 (플레이되는 시간만큼 지연)
            yield return StartCoroutine(Move4(i)); //Move4코루틴이 끝나야지 밑에 줄 실행
            i++;
        }
    }
    IEnumerator Move3(int index, Vector3 startPosi)
    {
        lerpTime = 2f;
        currentTime = 0;
        while ((currentTime / lerpTime) < 1)
        {
            currentTime += Time.deltaTime;
            objectPosi[index].transform.position = Vector3.Lerp(startPosi, underPipe.position, currentTime / lerpTime);
            yield return null;
        }
        if(objectPosi[index].GetComponent<Animator>())
        {
            anim = objectPosi[index].GetComponent<Animator>();
            anim.SetInteger("Introduction", 2); /* anim.SetInteger(anim.GetParameter(1).name, 2);*/
        }
        objectPosi[index].transform.rotation = Quaternion.LookRotation(Vector3.back+Vector3.right); //왼쪽으로 대각선 방향 보게하기
        actionMask?.Invoke(); //마스크(돋보기) 켜주기 위한 이벤트 실행
        objectPosi[i].GetComponent<AudioSource>().Play(); //소개멘트 플레이
    }
    IEnumerator Move4(int index)
    {
        lerpTime = 0.5f;
        currentTime = 0;
        while ((currentTime / lerpTime) < 1)
        {
            currentTime += Time.deltaTime;
            objectPosi[index].transform.position = Vector3.Lerp(underPipe.position, new Vector3(underPipe.position.x, underPipe.position.y + 10, underPipe.position.z), currentTime / lerpTime);
            yield return null;
        }
        objectPosi[index].SetActive(false);
        if (index == objectsLength - 1) 
        {
            StopAllCoroutines();
            StartCoroutine(LoadScene()); //씬 재생
            print("마지막 코루틴끝");
        }
    }
    #endregion
}
