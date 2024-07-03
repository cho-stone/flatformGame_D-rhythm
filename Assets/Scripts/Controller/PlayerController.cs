using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public static bool s_canPressKey = true; //골 플레그에 닿았을 때

    // x좌표 이동
    //이동
    float moveSpeed = 15;
    float maxMoveSpeed = 15;

    //대쉬
    float dashSpeed = 20;
    float duration_Of_Dash = 0.3f; //대쉬 시간(거리) 설정
    bool isDash = false; //대쉬 중 여부
    int dashCount = 0; //대쉬 횟수
    int dashAbleCount = 2; //대쉬 가능 횟수

    // +y좌표 이동
    float jumpSpeed = 25;
    float jumpHeight = 0.15f; //점프 높이
    float duration_Of_Flight = 0.3f; //체공시간 설정

    // -y좌표 이동
    float landSpeed = 20;
    float maxLandSpeed = 30;
    bool isLand = false; //착지를 눌렀는지

    //벽점프
    enum WallState
    {
        none,
        left,
        right
    }
    WallState wallState = WallState.none;
    bool isWall = false;
    int wallCount = 0;
    float wallJumpSpeed = 15;

    //락온
    Collider2D lockOnObject; //락온이 된 대상을 담는 변수
    LockOnManager lockOnManager; //락온 매니저 가져오기
    Vector3 destPos;
    bool doDashAction = false;
    bool isHit = false;
    float HitSpeed = 0.3f;
    float HitandRunSpeed = 20;

    //중력 설정 값
    float noneGravity = 0;
    float setGravity = 3;

    Vector3 dir = new Vector3();

    JudManager judManager;

    Rigidbody2D rigid;

    //커멘트 불리언들
    bool canJump = true; //점프 중복 불가
    bool canLand = false; //착지 가능 여부
    bool canDash = false; //대쉬 가능 여부

    //키가 입력되었을 때
    bool doAction = false;

    SoundManager soundManager;

    //코루틴들
    IEnumerator DashCo;
    IEnumerator JumpCo;

    //애니메이션
    SpriteRenderer spriteRenderer;
    Animator animator;

    void Start()
    {
        soundManager = SoundManager.instance;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        judManager = FindObjectOfType<JudManager>();
        lockOnManager = FindObjectOfType<LockOnManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!s_canPressKey)
        {
            return;
        }

        if (!isHit)
        {
            Move();
            Dash();
            Jump();
            Land();
            LockOn();
        }
        AnimCheck();
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            CheckFalling(); //떨어지는 중 인지

            disWallCount(); //벽에 떨어졌을 경우 쿨다운
        }
        
        if (doAction)
        {
            StartAction();
            doAction = false;
        }

        if (doDashAction)
        {
            SpecialMove();
            doDashAction = false;
        }

        if (rigid.velocity.x == 0)
        {
            animator.SetBool("Move", false);
        }
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) //쉬프트 누를 시 입력 안되게
            {
                if (judManager.CheckTiming())
                {
                    doAction = true;
                }
            }
        }
    }

    void Dash()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (canDash && dashCount < dashAbleCount)
            {
                isDash = true; //대쉬 중 인지
                dashCount++; //대쉬 횟수 증가

                if (judManager.CheckTiming())
                {
                    doAction = true;
                }
            }
            else
            {
                soundManager.playSFX("missCommand");
            }
        }
    }

    void Jump()
    {
        //점프
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (canJump)
            {
                canDash = true;
                if (judManager.CheckTiming())
                {
                    doAction = true;
                }
            }
            else
            {
                soundManager.playSFX("missCommand");
            }
        }
    }

    void Land()
    {
        //착지
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (canLand && !isWall)
            {
                isLand = true; //착지 중 인지

                if (judManager.CheckTiming())
                {
                    doAction = true;
                }
            }
            else
            {
                soundManager.playSFX("missCommand");
            }
        }
    }

    void LockOn()
    {
        //락온
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (lockOnManager.IsLockOn())
            {
                if (judManager.CheckTiming())
                {
                    doDashAction = true;
                }
            }
            else
            {
                soundManager.playSFX("missCommand");
            }

        }
    }

    IEnumerator JumpC()
    {
        canJump = false;
        
        yield return new WaitForSeconds(jumpHeight); //점프 높이
        rigid.velocity = Vector2.zero; //체공
        rigid.gravityScale = noneGravity; //
        yield return new WaitForSeconds(duration_Of_Flight); //체공시간
        rigid.gravityScale = setGravity; // 중력 재설정
    }

    IEnumerator DashC()
    {
        yield return new WaitForSeconds(duration_Of_Dash); //대쉬 길이

        rigid.velocity = Vector2.zero; //대쉬 종료
        rigid.gravityScale = setGravity; //중력 재설정
    }

    void StartAction()
    {
        //방향 계산
        dir.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        StopAllCoroutines();

        rigid.gravityScale = setGravity; // 이동 커맨드 순간 중력 1만들기 = 그래비티가 0으로 되는 버그 방지

        //양옆 이동
        if (dir.x != 0)
        {
            ActionMove();        
        }

        //점프
        if(dir.y > 0)
        {           
            ActionJump();
        }

        //착지
        if(dir.y < 0)
        {
            ActionLand();
        }
    }

    void ActionMove()
    {
        if (isDash && Input.GetKey(KeyCode.LeftShift))
        {
            isDash = false; //다음 키 눌렀을 때 바로 else로 넘어가도록

            rigid.velocity = Vector2.zero; //대쉬 사전 준비
            rigid.gravityScale = noneGravity; //

            rigid.AddForce(dir * dashSpeed, ForceMode2D.Impulse); //대쉬!

            DashCo = DashC();
            StartCoroutine(DashCo);
        }
        else
        {
            rigid.velocity = Vector2.zero; //순간적으로 속도 0
            rigid.AddForce(dir * moveSpeed, ForceMode2D.Impulse);

            CheckSpeed(); //스피드가 비정상 적으로 빨라지는 버그 방지  
        }
    }

    void ActionJump()
    {
        if(isWall)
        {
            switch (wallState)
            {
                case WallState.left:
                    rigid.velocity = Vector2.zero; //순간적으로 속도 0
                    rigid.AddForce(new Vector2(1, 1) * wallJumpSpeed, ForceMode2D.Impulse);
                    spriteRenderer.flipX = false;
                    break;
                case WallState.right:
                    rigid.velocity = Vector2.zero; //순간적으로 속도 0
                    rigid.AddForce(new Vector2(-1, 1) * wallJumpSpeed, ForceMode2D.Impulse);
                    spriteRenderer.flipX = true;
                    break;
            }     
        }
        else
        {
            rigid.velocity = Vector2.zero; //순간적으로 속도 0
            rigid.AddForce(dir * jumpSpeed, ForceMode2D.Impulse);

            JumpCo = JumpC();
            StartCoroutine(JumpCo);
        }    
    }

    void ActionLand()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(dir * landSpeed, ForceMode2D.Impulse);

        CheckLandSpeed(); //스피드가 비정상 적으로 빨라지는 버그 방지     
    }

    void CheckFalling()
    {
        //떨어지는 속력이 0 이하 일 때 실행
        if(rigid.velocity.y < 0)
        {
            animator.SetBool("Land", true);
            animator.SetBool("Jump", false);

            //아래 방향 레이를 쏴서 히트 되는지 체크 - 바닥에 닿았을 때
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null) //히트된 콜라이더가 있는지
            {
                if (rayHit.distance < 1.22f) //바닥에 닿았을 때
                {
                    
                    canJump = true; //점프 가능

                    dashCount = 0; //대쉬 횟수 초기화 = 대쉬 충전
                    canDash = false; //대쉬 불가 = 바닥에 닿아있음

                    isWall = false; //땅에 닿으면 벽에 없는 상태
                    wallCount = 0; //

                    animator.SetBool("Move", false);
                    animator.SetBool("Jump", false);
                    animator.SetBool("Land", false);
                }            
            }
        }
        

        //x좌표
        RaycastHit2D rayLand = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));

        //거리가 0이 되면 더이상 바닥에 선이 닿지 않음 = 착지 가능 거리
        if (rayLand.distance == 0)
        {
            //히트된 그라운드가 없을 때 착지 가능
            canLand = true;
        }
        else
        {
            //히트된 그라운드가 있을 시 착지 불가
            canLand = false;
        }


        //착지시 바닥을 뚫는 버그 방지 -> 레이 거리가 1.2이하면 속도 조정
        if (rayLand.collider != null && isLand && rayLand.distance <= 1.2f)
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.down, ForceMode2D.Impulse);
            isLand = false;
        }



        //공중에서 1.2이상 떨어지면 실행
        if(rayLand.distance >= 1.2f || rayLand.distance == 0)
        {
            CheckWall();
        }
    }

    void CheckWall()
    {
        //y좌표레이
        RaycastHit2D rayWall_R = Physics2D.Raycast(rigid.position, Vector3.right, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D rayWall_L = Physics2D.Raycast(rigid.position, Vector3.left, 1, LayerMask.GetMask("Platform"));

        //벽에 붙었을 경우 - 조건 : wallCount가 1이하여야함 => 쿨다운 전 한번만 실행하기 위해
        if (0 < rayWall_L.distance && rayWall_L.distance <= 0.4f && wallCount < 1)
        {
            isWall = true;
            wallState = WallState.left;
            wallCount += 10;

            rigid.velocity = Vector2.zero;
            rigid.gravityScale = noneGravity;
            StopAllCoroutines();
            canJump = true;
        }
        else if (0 < rayWall_R.distance && rayWall_R.distance <= 0.4f && wallCount < 1)
        {
            isWall = true;
            wallState = WallState.right;
            wallCount += 10;

            rigid.velocity = Vector2.zero;
            rigid.gravityScale = noneGravity;
            StopAllCoroutines();
            canJump = true;
        }
        else if ((rayWall_L.distance == 0 || rayWall_L.distance >= 0.4f) && wallState == WallState.left)
        {
            wallState = WallState.none;
            isWall = false;
            canJump = false;
        }
        else if((rayWall_R.distance == 0 || rayWall_R.distance >= 0.4f) && wallState == WallState.right)
        {
            wallState = WallState.none;
            isWall = false;

            canJump = false;
        }

    }

    void disWallCount()
    {
        if (wallCount > 0 && !isWall)
        {
            wallCount -= 1;
        }
    }

    //스피드가 maxMoveSpeed보다 넘지 않도록
    void CheckSpeed()
    {
        if (rigid.velocity.x > maxMoveSpeed)
            rigid.velocity = new Vector2(maxMoveSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxMoveSpeed * (-1))
            rigid.velocity = new Vector2(maxMoveSpeed * (-1), rigid.velocity.y);
    }

    //스피드가 maxLandSpeed보다 넘지 않도록
    void CheckLandSpeed()
    {
        if (rigid.velocity.y > maxLandSpeed)
            rigid.velocity = new Vector2(rigid.velocity.x, maxLandSpeed);
        else if (rigid.velocity.y < maxLandSpeed * (-1))
            rigid.velocity = new Vector2(rigid.velocity.x, maxLandSpeed * (-1));
    }

    //락온 시 z키를 눌렀을 때
    void SpecialMove()
    {
        StopAllCoroutines();
        lockOnObject = lockOnManager.GetLockOnObj(); //락온된 오브젝트 가져오기
        lockOnManager.CurrDash(); //락온 매니저에서 대쉬중으로 상태 변경 
        this.gameObject.layer = 9; //플레이어를 공격상태로 변경

        isHit = true;

        soundManager.playSFX("LockOnDash");

        rigid.velocity = Vector2.zero; //순간 속도 0
        rigid.gravityScale = noneGravity; //중력 없음

        destPos = lockOnObject.transform.position;

        StartCoroutine(HitCo());
    }

    IEnumerator HitCo()
    {
        yield return new WaitForSeconds(0.2f);
        while (Vector3.SqrMagnitude(transform.position - destPos) >= 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, HitSpeed);
            yield return null;
        }
        
        transform.position = destPos;

        soundManager.playSFX("enemyHit");

        rigid.velocity = Vector2.zero;
        rigid.gravityScale = setGravity;
        rigid.AddForce(Vector2.up * HitandRunSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        
        this.gameObject.layer = 3;
        lockOnManager.EndDash();
        isHit = false;
    }

    void AnimCheck()
    {
        //방향전환
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            animator.SetBool("Move", true);
        }

        if (Input.GetButtonDown("Vertical"))
        {
            if(Input.GetAxisRaw("Vertical") > 0)
            {
                animator.SetBool("Jump", true);
            }
            else if(Input.GetAxisRaw("Vertical") < 0)
            {
                animator.SetBool("Land", true);
            }
        }
            
    }
}
