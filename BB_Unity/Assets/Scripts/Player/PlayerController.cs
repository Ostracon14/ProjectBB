using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerMain playerMain;

    private Vector2 boxCastSize = new Vector2(0.6f, 0.05f);
    private float boxCastMaxDistance = 1.0f;

    // ---------입력값 저장 변수---------
    private Vector2 moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool attackInput;

    // -------------인스펙터-------------
    [Header("설정")]
    [SerializeField, Range(0f, 10f)]
    private float maxSpeed = 5f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMain = GetComponent<PlayerMain>();
    }

    // 입력 함수
    void Update()
    {
        //GetInput();
    }

    // 물리 함수
    void FixedUpdate()
    {
        Move();
        Jump();
        Attack();
        GroundCheck();
    }

    private void GetInput()
    {

    }

    private void Move()
    {
        if (playerMain.IsHit())
            return;

        // 움직임 조작
        float h = Input.GetAxisRaw("Horizontal"); // 횡으로 키를 누르면
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); // 이동한다

        // 최고 속도
        if (rigid.velocity.x > maxSpeed) // 오른족
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // 움직임 멈춤
        if (Input.GetButtonUp("Horizontal") || Input.GetAxisRaw("Horizontal") == 0) // 바로 멈춤
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.0000001f, rigid.velocity.y);

        // 방향 전환
        //if (Input.GetButton("Horizontal"))
        //    spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") != 0) // 좌우키 동시 입력 버그 방지
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), transform.localScale.y, transform.localScale.z);

        // 미끄러짐 방지
        if (!(Input.GetButton("Horizontal")))
            rigid.velocity = new Vector2(0, rigid.velocity.y);

        //// 달리기 애니메이션
        //if (Mathf.Abs(rigid.velocity.x) < 0.3) // 절댓값x가 크면 달리기
        //    anim.SetBool("isRunning", false);
        //else
        //    anim.SetBool("isRunning", true); 
    }

    private void Jump()
    {
        if (playerMain.IsHit())
            return;
        
        // 점프 (점프를 누르고, 점프 중이 아니고, 피격 상태가 아니면)
        if (Input.GetButtonDown("Jump")/* && !anim.GetBool("isJumping")*/)
        {
            Debug.Log("jump!");
            //rigid.velocity = new Vector2(rigid.velocity.x, 0); // 점프 속도 초기화
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            //anim.SetBool("isJumping", true);
        }
    }

    private void GroundCheck()
    {
        if (playerMain.IsHit())
            return;

        // 바닥 판정
        //anim.SetBool("isFalling", true); // 점프 없이 낙하

        //Debug.DrawRay(rigid.position, Vector3.down*2, new Color(0, 1, 0)); // 레이 시각화
        //RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Map"));
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));
        if (rayHit.collider != null) // 바닥 감지를 위해서 레이저
        {
            if (rayHit.distance < 0.9f)
            {
                //anim.SetBool("isJumping", false);
                //anim.SetBool("isFalling", false);
            }
        }
    }

    private void Attack()
    {
        if (playerMain.IsHit())
            return;

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
