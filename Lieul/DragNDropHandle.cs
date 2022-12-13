using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//마우스포인트따라 오브젝트 drag&drop으로 움직이게
public class DragNDropHandle : MonoBehaviour
{
    private float z_saved; //z값 부여용
    private Vector3 posi;
    Collider collider;
    [SerializeField]
    GameObject crashLieul;
    [SerializeField]
    GameObject resetLieul;
    [SerializeField]
    GameObject fakeActive;
    Vector3 originPosi;
    private void OnEnable()
    {
        FindObjectOfType<QuizManager>().AddNRemove = gameObject;
        originPosi = transform.position;
    }
    Vector3 GetMouseWorldPosition() //마우스포인트의 월드좌표값 부여용
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = z_saved;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    private void OnMouseDown()
    {
        z_saved = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        posi = gameObject.transform.position - GetMouseWorldPosition();
        crashLieul.SetActive(true);
        GetComponent<Rigidbody>().useGravity = false;
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + posi;
    }
    //3번째 퀘스트용 //드래그범위를 벗어났을 때 페이크로 두 개의 오브젝트 껐다켰다해서 드래그범위 밖으로 못나가게 하기
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("Block"))
        {
            print("가게랑 충돌했다");
            resetLieul.SetActive(true);
            gameObject.transform.position = originPosi;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            FindObjectOfType<QuizManager>().AddNRemove = gameObject;
            gameObject.SetActive(false);       
        }
    }
    private void OnMouseUp()
    {
        GetComponent<Rigidbody>().useGravity = true;
        collider = CheckOb();
        if (collider != null && collider.gameObject == crashLieul)
        {
            print("충돌체크확인");
            gameObject.SetActive(false);
            collider.gameObject.SetActive(false);
            fakeActive.SetActive(true);
            FindObjectOfType<QuizManager>().AddNRemove = gameObject;
        }
    }
    Collider CheckOb()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit))
        {
            print("충돌처리 감지" + hit.collider);
            return hit.collider;
        }
        else return null;
    }
}
