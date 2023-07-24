using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private bool isHit = false;

    // -------------인스펙터-------------
    [Header("오브젝트 연결")]
    //[SerializeField]
    //private GameManager gameManager;

    [Header("설정")]
    [SerializeField, Range(0f, 100f)]
    private float bouncPower = 10f;
    [SerializeField, Range(0f, 5f)]
    private float invulnTIme = 0.8f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // 하위 오브젝트

        //OffHit();
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy" && !isHit)
    //        OnHit(collision.transform.position); // Enemy의 위치 정보 매개변수
    //    if (collision.gameObject.tag == "Trap")
    //        OnHit(collision.transform.position);
    //    if (collision.gameObject.tag == "Bullet")
    //        OnHit(collision.transform.position);
    //}

    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy Attack")
    //        OnHit(collision.transform.position);
    //}

    //void OnHit(Vector2 targetPos)
    //{
    //    if (!isHit)
    //    {
    //        gameObject.layer = 9; // Super Armor Layer
    //        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // 피격당했을 때 색 변경
    //        isHit = true;

    //        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1; // 피격시 튕겨나가는 방향 결정
    //        rigid.AddForce(new Vector2(dir, 1) * bouncPower, ForceMode2D.Impulse); // 튕겨나가기

    //        this.transform.Rotate(0, 0, dir * (-10)); // 회전
    //        Invoke("ReRotate", 0.4f);

    //        anim.SetTrigger("doHit"); // 애니메이션 트리거
    //        Invoke("OffHit", invulnTIme); // invulnTIme 후 무적 시간 끝

    //        //gameManager.HpDown();
    //    }
    //}

    //void OffHit()
    //{
    //    gameObject.layer = 8; // Player Layer
    //    spriteRenderer.color = new Color(1, 1, 1, 1f); // 색 변경
    //}

    //void ReRotate() // 회전 초기화, 다시 조작 가능
    //{
    //    this.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    isHit = false;
    //}

    // -------------외부참조-------------
    //public bool IsHit()
    //{
    //    return isHit;
    //}

    //public void OnDead()
    //{
    //    gameObject.layer = 9;
    //    isHit = true;

    //    spriteRenderer.color = new Color(1, 1, 1, 0.4f);
    //    anim.SetTrigger("doDead");

    //    CancelInvoke("ReRotate");
    //    CancelInvoke("OffHit");
    //}
}
