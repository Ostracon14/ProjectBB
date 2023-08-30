using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerMain playerMain;

    /* ----------- 입력 값 저장 변수 ----------- */
    private Vector2 moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool attackInput;
    private bool platformPassInput;
    
    /* ------------ 동작 확인 변수 ------------- */
    private bool isMove = false;
    private bool isJump = false;
    private bool isDash = false;
    private bool isAttack = false;
    private bool isHit = false;
    private bool isDead = false;

    /* ----------------- 플랫폼 ---------------- */
    private GameObject platformObject = null;

    /* ------------ 박스 레이 조절 ------------- */
    private Vector2 boxCastSize = new Vector2(0.6f, 0.05f);
    private float boxCastMaxDistance = 1.0f;

    /* ---------------- 인스펙터 --------------- */
    [Header("설정")]
    [SerializeField, Range(0f, 10f)]
    private float moveSpeed = 5f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;

    [Space(10f)]
    [SerializeField, Range(0f, 100f)]
    private float dashPower = 20f;
    [SerializeField, Range(0f, 1f)]
    private float dashTime = 0.2f;
    [SerializeField, Range(0, 10)]
    private int dashStack = 3;
    [SerializeField, Range(0f, 10f)]
    private float dashCoolTime = 3f;

    [Space(10f)]
    [SerializeField, Range(0f, 10f)]
    private float attackCoolTime = 2f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMain = GetComponent<PlayerMain>();
    }

    void Update()
    {
        /* 피격/사망 체크 */
        StateCheck();

        /* 입력 값 저장 */
        GetInput();

        /* 플레이어 조작 */
        Move();
        Jump();
        PlatformPass();
        Dash();
        Attack();

        /* 바닥 체크 */
        GroundCheck();
    }

    private void StateCheck()
    {
        isHit = playerMain.GetIsHit();
        isDead = playerMain.GetIsDead();
    }

    private void GetInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetButtonDown("Dash");
        attackInput = Input.GetButtonDown("Fire1");
        platformPassInput = Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") < 0f;
    }

    private void Move()
    {
        if (isDash || isHit)
            return;

        isMove = moveInput.x != 0;
        //isMove = moveInput.magnitude != 0;

        if (isMove)
        {
            // 움직임 조작
            rigid.velocity = new Vector2(moveInput.x * moveSpeed, rigid.velocity.y);

            // 방향 전환
            transform.localScale = new Vector3(moveInput.x, transform.localScale.y, transform.localScale.z);
            //spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        else
        {
            // 미끄러짐 방지
            rigid.velocity = new Vector2(0f, rigid.velocity.y);
        }

        anim.SetBool("isRunning", isMove);
    }

    private void Jump()
    {
        if (!jumpInput || platformPassInput || isJump || isDash || isAttack || isHit)
            return;
        
        isJump = true;

        // Y축 속도 초기화
        //rigid.velocity = new Vector2(rigid.velocity.x, 0);

        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        anim.SetBool("isJumping", true);
    }

    private void PlatformPass()
    {
        if (!platformPassInput || platformObject == null || isDash || isAttack || isHit)
            return;
        
        Debug.Log("아래점프 ON " + platformObject.name); 
        platformObject.layer = 12; // Pass Ground 레이어
    }

    private void GroundCheck()
    {
        // 추락이 아닐 때
        if (rigid.velocity.y > 0) 
            return;

        // 점프 없이 낙하
        anim.SetBool("isFalling", true);

        // 바닥 저장(Ground, Pass Ground)
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground", "Pass Ground"));

        if (rayHit.collider != null) // 바닥 체크 될 때
        {
            if (rayHit.distance < 0.9f)
            {
                isJump = false;

                // Platform 태크 체크
                if (platformObject == null && rayHit.collider.tag == "Platform")
                {
                    platformObject = rayHit.collider.gameObject;
                    platformObject.layer = 6; // Ground 레이어
                }
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", false);
            }
        }
        else // 공중에 있을 때
        {
            // 플랫폼에서 벗어날 때
            if (platformObject != null)
            {
                platformObject.layer = 12; // Pass Ground 레이어
                platformObject = null;
            }
        }
    }

    private void Dash()
    {
        if (dashStack <= 0 || !dashInput || moveInput.magnitude == 0 || isDash || isAttack || isHit)
            return;

        isDash = true;
        dashStack -= 1;

        // 대시 중 이동,포물선,가속 방지
        moveSpeed = 0f;
        rigid.gravityScale = 0f;
        rigid.velocity = new Vector2(0f, 0f);

        rigid.AddForce(moveInput.normalized * dashPower, ForceMode2D.Impulse);
        anim.SetBool("isDashing", true);

        StartCoroutine(DashOut(dashTime));
        StartCoroutine(DashCoolDown(dashCoolTime));
    }

    IEnumerator DashCoolDown(float second)
    {
        // Max값을 지정하고 비교하는 방향으로 코드 수정 해야함

        yield return new WaitForSeconds(second);
        dashStack += 1;
    }

    IEnumerator DashOut(float second)
    {
        // 대시 탈출
        yield return new WaitForSeconds(second);
        isDash = false;
        rigid.velocity = new Vector2(0f, -1f);

        // 대시 탈출 후 회복
        yield return new WaitForSeconds(0.1f);
        moveSpeed = 5f;
        rigid.gravityScale = 1f;
        anim.SetBool("isDashing", false);
    }

    private void Attack()
    {
        if (!attackInput || isAttack || isDash || isHit)
            return;

        Debug.Log("Attack");
        isAttack = true;
        anim.SetTrigger("doAttack");

        StartCoroutine(AttackCoolOut(attackCoolTime));
    }

    IEnumerator AttackCoolOut(float second)
    {
        yield return new WaitForSeconds(second);
        isAttack = false;
    }

    /* -------------외부참조------------- */



        // -------------기타-------------

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
