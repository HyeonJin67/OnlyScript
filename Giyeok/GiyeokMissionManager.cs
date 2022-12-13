using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
// https://flowtree.tistory.com/46
// https://wolstar.tistory.com/5

public class GiyeokMissionManager : MonoBehaviour
{
    #region 변수
    [SerializeField]
    ScoreHandler scoreCase; //미션완료시 공통 별 프리팹
    [SerializeField]
    GameObject pico; //피코 캐릭터
    [SerializeField]
    GameObject hand; //드래그 유도용 손
    [SerializeField]
    GameObject invisible; //화면 비활성화용 투명 오브젝트
    [SerializeField]
    Image zoomPosition; //돋보기가 이동할 위치용
    private Vector3 posi; //3d오브젝트를 마우스드래그 따라 같이 움직이게 하기 위함용
    private float z_saved; //3d오브젝트를 마우스드래그 따라 같이 움직이게 하기 위함용 //x,y값만 가지고 있는 마우스의 위치에게 z값을 부여해주기 위한 저장용 변수
    new Collider collider; //충돌처리시 콜라이더 저장용
    GameObject savedOb; //드래그 중 돋보기에 충돌처리시 애니메이션을 껏다켯다 하기 위한 충돌 콜라이더 저장용 파라미터
    [SerializeField]
    GameObject startRayPosi; //돋보기 렌즈 한가운데에서 레이를 쏘는 위치용 돋보기의 자식 오브젝트
    [Tooltip("이 씬의 자음 이름을 입력해 주세요")]
    [SerializeField]
    string obName; //이 씬의 자음 이름

    //private Animator animatorPico; //원래 피코캐릭터가 등장했었음
    private Animator animatorColl;
    private Animator animatorSave;
    int count = 0; //충돌완료 체크용 파라미터
    bool playstart; //소리 재생 파일 바꿀 때 전에꺼가 재생중인지 아닌지 체크하는용 파라미터
    bool check; //마우스다운시에 처음에 한번만 성우 안내 목소리 나오게끔 하는 파라미터
    SpeakerHandler speakerHandler;

    float currentTime; //lerp용 deltaTime증가용
    Vector3 startPosi; //lerp의 시작위치 고정용
    #endregion
    private void Awake()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine(ZoomMove(6f));
    }
    private void Start()
    {
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForFixedUpdate();
        invisible.SetActive(true);
        speakerHandler = FindObjectOfType<SpeakerHandler>();
        speakerHandler.SoundByNum2(0);
        speakerHandler.SoundByNum(1);
        //animatorPico = pico.GetComponent<Animator>();
        check = true;
        scoreCase.SceneComplete += MissionComplete;
        yield break;
    }
    #region 함수
    //돋보기가 씬 안으로 천천히 들어오게 만들어주는 지연 함수
    IEnumerator ZoomMove(float lerpTime)
    {
        currentTime = 0;
        startPosi = transform.position;
        while((currentTime/lerpTime) < 1)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosi, zoomPosition.transform.position, currentTime / lerpTime);
            //transform.position = Vector3.Lerp(startPosi, zoomPosition.transform.position, Mathf.SmoothStep(0, 1, currentTime / lerpTime));
            yield return null;
        }
        //---------------------------------------------------------------- Lerp 잘못사용했을 때-----------------------------------------------------------------------
        //while (Vector3.Distance(transform.position, zoomPosition.transform.position) > 0.2f)//둘사이의 거리가 있는 동안 //첨에 0으로 했다가 너무 느려서 10으로 바꿈
        //{
        //    transform.position = Vector3.Lerp(transform.position, zoomPosition.transform.position, Time.deltaTime * 0.7f);
        //    yield return new WaitForSeconds(Time.deltaTime*0.2f); //제자리로 돌아갈때 속도 조절하는 곳
        //    if (Vector3.Distance(transform.position, zoomPosition.transform.position) <= 0.2f)
        //    {
        //        break;
        //    }
        //}
        //transform.position = zoomPosition.transform.position;
        if (lerpTime == 6f) //Awake에서 호출시에만 해야할 일
        {
            invisible.SetActive(false);
            GetComponent<CapsuleCollider>().enabled = true;
            hand.SetActive(true);
        }
    }
    private void OnMouseDown()
    {
        if (check)
        {
            speakerHandler.SoundByNum(2);
        }
        check = false;
        hand.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true); //돋보기를 드래그시작할 때 돋보기 주변 어둡게해주는 역마스크 
        z_saved = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        posi = gameObject.transform.position - GetMouseWorldPosition();
        count = 0;
    }
    //드래그 중 충돌처리된 오브젝트의 애니메이션이 겹치지 않게 애니메이션을 그때그때 바꿔줌
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + posi;
        speakerHandler.SoundByNum(8);
        //animatorPico.SetInteger("PicoAction", 4);
        collider = CheckOb(); //충돌처리 함수 호출
        if(collider != null)
        {
            if (!playstart)
            {
                speakerHandler.SoundByNum(9);
                if (collider.GetComponentInChildren<AudioSource>()) collider.GetComponentInChildren<AudioSource>().Play();
                playstart = true;
            }
            animatorColl = collider.gameObject.GetComponent<Animator>();
            if (collider.transform.parent.name.Contains(obName))
            {
                if (savedOb != null && savedOb.GetComponent<CapsuleCollider>().enabled && savedOb.transform.parent.name.Contains(obName))
                { //정답처리가 안된 오브젝트는 캡슐콜라이더가 켜져있다
                    animatorSave = savedOb.GetComponent<Animator>();
                    animatorSave.SetInteger(savedOb.gameObject.name + "Ani", 0);
                }
                savedOb = collider.gameObject; //직전의 충돌 오브젝트를 기억해뒀다가 애니메이션을 바꿔주기 위한 저장용 
                animatorColl = collider.gameObject.GetComponent<Animator>();
                animatorColl.SetInteger(collider.name + "Ani", 2);
            }
            else
            { //정답처리가 완료된 오브젝트들은 캡슐콜라이더 꺼져있다
                if (animatorColl != null && animatorColl.GetComponent<CapsuleCollider>().enabled) animatorColl.SetInteger(collider.name + "Ani", 0);
                if (savedOb != null && savedOb.GetComponent<CapsuleCollider>().enabled) animatorSave.SetInteger(savedOb.gameObject.name + "Ani", 0);
            }
        }
        else
        {
            playstart = false;
        }
    }
    //마우스 따라 움직이는 돋보기 이동용 함수
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; 
        mousePoint.z = z_saved; //마우스포인트에 z값 만들어주기
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    // 돋보기와 충돌처리용 함수
    Collider CheckOb()
    {
        Ray ray = new Ray(startRayPosi.transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit))
        {
            print("충돌처리 감지" + hit.collider);
            return hit.collider;
        }
        else return null;
    }    
    private void OnMouseUp()
    {
        playstart = false;
        collider = CheckOb();
        if (collider != null)
        {
            if (collider.transform.parent.name.Contains(obName))
            {
                speakerHandler.SoundByNum(3);
                invisible.transform.SetAsLastSibling();
                invisible.SetActive(true);
                count++;
                if (count == 1)
                {
                    print("ㄱ포함단어 확인");
                    //animatorPico.SetInteger("PicoAction", 1);
                    animatorColl.SetInteger(collider.name + "Ani", 1);
                    scoreCase.SetScore();
                    collider.gameObject.GetComponent<CapsuleCollider>().enabled = false; //정답처리 완료된 오브젝트의 충돌처리를 방지하기위해 콜라이더 꺼주기
                }
            }
            else
            {
                //animatorPico.SetInteger("PicoAction", 2);
                speakerHandler.SoundByNum(10);
                StartCoroutine(ZoomMove(0.5f));
            }
        }
    }
    private void OnMouseExit()
    {
        playstart = false;
    }
    //미션완료시 ScoreCase프리팹의 SceneComplete이벤트에 넣어줄 함수
    void MissionComplete()
    {
        print("미션끝 : 피토 win(hi-host) 애니메이션 플레이");
        invisible.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = false; //돋보기 못하게 하기
        transform.GetChild(0).gameObject.SetActive(false); //마스크 꺼주기
        StartCoroutine(ZoomMove(1f));
        //animatorPico.SetInteger("PicoAction", 3); //피코애니메이션 중에 3번 hi-host켜기   
    }
    #endregion
}
