using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Monster
{
    [SerializeField] LayerMask layer;
    private Vector2 dir;
    void Start()
    {
        SoundMana = SoundManager.instance;
        MonsterHealth = 1;
        Radius = 16f;
        Rigid = GetComponent<Rigidbody2D>();
        SpriteFlip = GetComponent<SpriteRenderer>();
        Anima = GetComponent<Animator>();
        BoxCol = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        PlayerCol = Physics2D.OverlapCircle(transform.position, Radius, layer);
        if (PlayerCol != null )
        {
            Move();
        }
    }

    private void Update()
    {
        Anim();
    }

    protected override void Attack()
    {
        
    }

    protected override void Move()
    {
        dir = PlayerCol.transform.position - transform.position;

        RaycastHit2D moveRay = Physics2D.Raycast(transform.position, dir, Radius, layer);

        //이동 경로 설정
        if(9 < moveRay.distance && moveRay.distance < 16)
        {
            Rigid.velocity = new Vector2(PlayerCol.transform.position.x , PlayerCol.transform.position.y).normalized * 3;
        }
        else if(0 < moveRay.distance && moveRay.distance < 8)
        {
            Rigid.velocity = new Vector2(-PlayerCol.transform.position.x, -PlayerCol.transform.position.y).normalized * 3;
        }
        else
        {
            Rigid.velocity = Vector2.zero;
        }

        //플레이어를 바라보게 하기
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void Anim()
    {
        //캐릭터 뒤집기
        if (dir.x > 0)
        {
            SpriteFlip.flipY = false;
        }
        else if (dir.x < 0)
        {
            SpriteFlip.flipY = true;
        }

        //애니메이션 - 이동
        if (SpriteFlip.flipY)
        {
            if (Rigid.velocity.x > 0)
            {
                Anima.SetBool("Back", true);
            }
            else if (Rigid.velocity.x < 0)
            {
                Anima.SetBool("Forward", true);
            }
            else
            {
                Anima.SetBool("Forward", false);
                Anima.SetBool("Back", false);
            }
        }
        else
        {
            if (Rigid.velocity.x > 0)
            {
                Anima.SetBool("Forward", true);
            }
            else if (Rigid.velocity.x < 0)
            {
                Anima.SetBool("Back", true);
            }
            else
            {
                Anima.SetBool("Forward", false);
                Anima.SetBool("Back", false);
            }
        }    
    }
}
