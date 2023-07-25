using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerMain playerMain;

    // 입력 값 저장 변수
    private Vector2 moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool attackInput;

    // 동작 확인 변수
    private bool isMove = false;
    private bool isJump = false;
    private bool isDash = false;
    private bool isAttack = false;

    // 이건 어디로 가야하는지 모르겠는 변수
    private int dashStack = 3;
    private int dashCoolSec = 5; //성장에 따라 바뀌는값이면 좋겟다^0^

    // 박스레이 조절
    private Vector2 boxCastSize = new Vector2(0.6f, 0.05f);
    private float boxCastMaxDistance = 1.0f;

    // 인스펙터
    [Header("설정")]
    [SerializeField, Range(0f, 10f)]
    private float moveSpeed = 5f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;
    [SerializeField, Range(0f, 100f)]
    private float dashPower = 20f;
    [SerializeField, Range(0f, 1f)]
    private float dashSec = 0.2f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMain = GetComponent<PlayerMain>();
    }

    void Update()
    {
        // 입력 값 저장
        GetInput();

        // 플레이어 조작
        Move();
        Jump();
        Dash();
        Attack();

        // 바닥 체크
        GroundCheck();
    }

    private void GetInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetButtonDown("Dash");
        attackInput = Input.GetButtonDown("Fire1");
    }

    private void Move()
    {
        if (isDash/*playerMain.IsHit()*/)
            return;

        //isMove = moveInput.magnitude != 0;
        isMove = moveInput.x != 0;

        if (isMove)
        {
            // 움직임 조작
            rigid.velocity = new Vector2(moveInput.x * moveSpeed, rigid.velocity.y);

            // 방향 전환
            //spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            transform.localScale = new Vector3(moveInput.x, transform.localScale.y, transform.localScale.z);
        }
        else
            rigid.velocity = new Vector2(0f, rigid.velocity.y); // 미끄러짐 방지

        //anim.SetBool("isRunning", isMove);
    }

    private void Jump()
    {
        if (!jumpInput || isJump || isDash || isAttack/* || playerMain.IsHit()*/)
            return;
        
        isJump = true;

        //rigid.velocity = new Vector2(rigid.velocity.x, 0); // Y축 속도 초기화
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        //anim.SetBool("isJumping", true);
    }

    private void GroundCheck()
    {
        // 추락이 아닐 때
        if (rigid.velocity.y > 0) 
            return;

        //anim.SetBool("isFalling", true); // 점프 없이 낙하

        // 바닥 판정
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));
        if (rayHit.collider != null) // 바닥 감지를 위해서 레이저
        {
            if (rayHit.distance < 0.9f)
            {
                isJump = false;

                //anim.SetBool("isJumping", false);
                //anim.SetBool("isFalling", false);
            }
        }
    }

    private void Dash()
    {
        if (dashStack <= 0 || !dashInput || !isMove || isDash || isAttack/* || playerMain.IsHit()*/)
            return;

        isDash = true;
        dashStack -= 1;
        Debug.Log("dash!");

        moveSpeed = 0f; // 대시 중 이동 방지
        rigid.gravityScale = 0f; // 포물선 방지
        rigid.velocity = new Vector2(0f, 0f); // 속도 초기화

        rigid.AddForce(moveInput.normalized * dashPower, ForceMode2D.Impulse);
       
        StartCoroutine(DashOut(dashSec));
        StartCoroutine(DashCoolDown(dashCoolSec));
    }

    // 최대 스택이 아니라면 돌린다
    // Max값을 지정하고 비교하는 방향으로 코드 수정
    IEnumerator DashCoolDown(float second) {
        yield return new WaitForSeconds(second);
        dashStack += 1;
    }

    IEnumerator DashOut(float second)
    {
        yield return new WaitForSeconds(second);
        isDash = false;
        rigid.velocity = new Vector2(0f, -1f);

        yield return new WaitForSeconds(0.1f);
        moveSpeed = 5f;
        rigid.gravityScale = 1f;
    }

    private void Attack()
    {
        //if (playerMain.IsHit())
        //    return;

        // 공격 (현재 실행 중인 애니메이션이 공격이 아니면)
        if (Input.GetButtonDown("Fire1")/* && !anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_AttackFront")*/)
            anim.SetTrigger("doAttack");
    }

    //void OnDrawGizmos() // 사각 레이 기즈모
    //{
    //    RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Map"));

    //    Gizmos.color = Color.red;
    //    if (raycastHit.collider != null)
    //    {
    //        Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
    //        Gizmos.DrawWireCube(transform.position + Vector3.down * raycastHit.distance, boxCastSize);
    //    }
    //    else
    //    {
    //        Gizmos.DrawRay(transform.position, Vector2.down * boxCastMaxDistance);
    //    }
    //}

}
