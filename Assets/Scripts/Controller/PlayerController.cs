using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public static bool s_canPressKey = true; //�� �÷��׿� ����� ��

    // x��ǥ �̵�
    //�̵�
    float moveSpeed = 15;
    float maxMoveSpeed = 15;

    //�뽬
    float dashSpeed = 20;
    float duration_Of_Dash = 0.3f; //�뽬 �ð�(�Ÿ�) ����
    bool isDash = false; //�뽬 �� ����
    int dashCount = 0; //�뽬 Ƚ��
    int dashAbleCount = 2; //�뽬 ���� Ƚ��

    // +y��ǥ �̵�
    float jumpSpeed = 25;
    float jumpHeight = 0.15f; //���� ����
    float duration_Of_Flight = 0.3f; //ü���ð� ����

    // -y��ǥ �̵�
    float landSpeed = 20;
    float maxLandSpeed = 30;
    bool isLand = false; //������ ��������

    //������
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

    //����
    Collider2D lockOnObject; //������ �� ����� ��� ����
    LockOnManager lockOnManager; //���� �Ŵ��� ��������
    Vector3 destPos;
    bool doDashAction = false;
    bool isHit = false;
    float HitSpeed = 0.3f;
    float HitandRunSpeed = 20;

    //�߷� ���� ��
    float noneGravity = 0;
    float setGravity = 3;

    Vector3 dir = new Vector3();

    JudManager judManager;

    Rigidbody2D rigid;

    //Ŀ��Ʈ �Ҹ����
    bool canJump = true; //���� �ߺ� �Ұ�
    bool canLand = false; //���� ���� ����
    bool canDash = false; //�뽬 ���� ����

    //Ű�� �ԷµǾ��� ��
    bool doAction = false;

    SoundManager soundManager;

    //�ڷ�ƾ��
    IEnumerator DashCo;
    IEnumerator JumpCo;

    //�ִϸ��̼�
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
            CheckFalling(); //�������� �� ����

            disWallCount(); //���� �������� ��� ��ٿ�
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
            if (!Input.GetKey(KeyCode.LeftShift)) //����Ʈ ���� �� �Է� �ȵǰ�
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
                isDash = true; //�뽬 �� ����
                dashCount++; //�뽬 Ƚ�� ����

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
        //����
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
        //����
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (canLand && !isWall)
            {
                isLand = true; //���� �� ����

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
        //����
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
        
        yield return new WaitForSeconds(jumpHeight); //���� ����
        rigid.velocity = Vector2.zero; //ü��
        rigid.gravityScale = noneGravity; //
        yield return new WaitForSeconds(duration_Of_Flight); //ü���ð�
        rigid.gravityScale = setGravity; // �߷� �缳��
    }

    IEnumerator DashC()
    {
        yield return new WaitForSeconds(duration_Of_Dash); //�뽬 ����

        rigid.velocity = Vector2.zero; //�뽬 ����
        rigid.gravityScale = setGravity; //�߷� �缳��
    }

    void StartAction()
    {
        //���� ���
        dir.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        StopAllCoroutines();

        rigid.gravityScale = setGravity; // �̵� Ŀ�ǵ� ���� �߷� 1����� = �׷���Ƽ�� 0���� �Ǵ� ���� ����

        //�翷 �̵�
        if (dir.x != 0)
        {
            ActionMove();        
        }

        //����
        if(dir.y > 0)
        {           
            ActionJump();
        }

        //����
        if(dir.y < 0)
        {
            ActionLand();
        }
    }

    void ActionMove()
    {
        if (isDash && Input.GetKey(KeyCode.LeftShift))
        {
            isDash = false; //���� Ű ������ �� �ٷ� else�� �Ѿ����

            rigid.velocity = Vector2.zero; //�뽬 ���� �غ�
            rigid.gravityScale = noneGravity; //

            rigid.AddForce(dir * dashSpeed, ForceMode2D.Impulse); //�뽬!

            DashCo = DashC();
            StartCoroutine(DashCo);
        }
        else
        {
            rigid.velocity = Vector2.zero; //���������� �ӵ� 0
            rigid.AddForce(dir * moveSpeed, ForceMode2D.Impulse);

            CheckSpeed(); //���ǵ尡 ������ ������ �������� ���� ����  
        }
    }

    void ActionJump()
    {
        if(isWall)
        {
            switch (wallState)
            {
                case WallState.left:
                    rigid.velocity = Vector2.zero; //���������� �ӵ� 0
                    rigid.AddForce(new Vector2(1, 1) * wallJumpSpeed, ForceMode2D.Impulse);
                    spriteRenderer.flipX = false;
                    break;
                case WallState.right:
                    rigid.velocity = Vector2.zero; //���������� �ӵ� 0
                    rigid.AddForce(new Vector2(-1, 1) * wallJumpSpeed, ForceMode2D.Impulse);
                    spriteRenderer.flipX = true;
                    break;
            }     
        }
        else
        {
            rigid.velocity = Vector2.zero; //���������� �ӵ� 0
            rigid.AddForce(dir * jumpSpeed, ForceMode2D.Impulse);

            JumpCo = JumpC();
            StartCoroutine(JumpCo);
        }    
    }

    void ActionLand()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(dir * landSpeed, ForceMode2D.Impulse);

        CheckLandSpeed(); //���ǵ尡 ������ ������ �������� ���� ����     
    }

    void CheckFalling()
    {
        //�������� �ӷ��� 0 ���� �� �� ����
        if(rigid.velocity.y < 0)
        {
            animator.SetBool("Land", true);
            animator.SetBool("Jump", false);

            //�Ʒ� ���� ���̸� ���� ��Ʈ �Ǵ��� üũ - �ٴڿ� ����� ��
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null) //��Ʈ�� �ݶ��̴��� �ִ���
            {
                if (rayHit.distance < 1.22f) //�ٴڿ� ����� ��
                {
                    
                    canJump = true; //���� ����

                    dashCount = 0; //�뽬 Ƚ�� �ʱ�ȭ = �뽬 ����
                    canDash = false; //�뽬 �Ұ� = �ٴڿ� �������

                    isWall = false; //���� ������ ���� ���� ����
                    wallCount = 0; //

                    animator.SetBool("Move", false);
                    animator.SetBool("Jump", false);
                    animator.SetBool("Land", false);
                }            
            }
        }
        

        //x��ǥ
        RaycastHit2D rayLand = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));

        //�Ÿ��� 0�� �Ǹ� ���̻� �ٴڿ� ���� ���� ���� = ���� ���� �Ÿ�
        if (rayLand.distance == 0)
        {
            //��Ʈ�� �׶��尡 ���� �� ���� ����
            canLand = true;
        }
        else
        {
            //��Ʈ�� �׶��尡 ���� �� ���� �Ұ�
            canLand = false;
        }


        //������ �ٴ��� �մ� ���� ���� -> ���� �Ÿ��� 1.2���ϸ� �ӵ� ����
        if (rayLand.collider != null && isLand && rayLand.distance <= 1.2f)
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.down, ForceMode2D.Impulse);
            isLand = false;
        }



        //���߿��� 1.2�̻� �������� ����
        if(rayLand.distance >= 1.2f || rayLand.distance == 0)
        {
            CheckWall();
        }
    }

    void CheckWall()
    {
        //y��ǥ����
        RaycastHit2D rayWall_R = Physics2D.Raycast(rigid.position, Vector3.right, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D rayWall_L = Physics2D.Raycast(rigid.position, Vector3.left, 1, LayerMask.GetMask("Platform"));

        //���� �پ��� ��� - ���� : wallCount�� 1���Ͽ����� => ��ٿ� �� �ѹ��� �����ϱ� ����
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

    //���ǵ尡 maxMoveSpeed���� ���� �ʵ���
    void CheckSpeed()
    {
        if (rigid.velocity.x > maxMoveSpeed)
            rigid.velocity = new Vector2(maxMoveSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxMoveSpeed * (-1))
            rigid.velocity = new Vector2(maxMoveSpeed * (-1), rigid.velocity.y);
    }

    //���ǵ尡 maxLandSpeed���� ���� �ʵ���
    void CheckLandSpeed()
    {
        if (rigid.velocity.y > maxLandSpeed)
            rigid.velocity = new Vector2(rigid.velocity.x, maxLandSpeed);
        else if (rigid.velocity.y < maxLandSpeed * (-1))
            rigid.velocity = new Vector2(rigid.velocity.x, maxLandSpeed * (-1));
    }

    //���� �� zŰ�� ������ ��
    void SpecialMove()
    {
        StopAllCoroutines();
        lockOnObject = lockOnManager.GetLockOnObj(); //���µ� ������Ʈ ��������
        lockOnManager.CurrDash(); //���� �Ŵ������� �뽬������ ���� ���� 
        this.gameObject.layer = 9; //�÷��̾ ���ݻ��·� ����

        isHit = true;

        soundManager.playSFX("LockOnDash");

        rigid.velocity = Vector2.zero; //���� �ӵ� 0
        rigid.gravityScale = noneGravity; //�߷� ����

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
        //������ȯ
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
