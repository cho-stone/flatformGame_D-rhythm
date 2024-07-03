using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] float radius = 0f; //적감지 콜라이더 사이즈
    [SerializeField] LayerMask layer; //감지해서 가져올 레이어 선택
    private Collider2D[] objects; //감지된 콜라이더 담는 오브젝트
    private Collider2D closeObject; //가장 가까운 오브젝트 담는거

    EffectManager effectManager = null;
    SoundManager soundManager = null;
    float InvincibleTime = 2f; //무적 시간
    IEnumerator RecoveryCo;
    IEnumerator InvincibleCo;

    Player player = null;

    void Start()
    {
        soundManager = SoundManager.instance;
        effectManager = FindObjectOfType<EffectManager>();
        player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        //플레이어에 특정 layer을 가진 오브젝트가 접근하면 그 오브젝트 가져오기
        objects = Physics2D.OverlapCircleAll(transform.position, radius, layer);

        // BoxCollider2D 만 필터링
        List<Collider2D> boxColliders = new List<Collider2D>();
        foreach (var collider in objects)
        {
            if (collider is BoxCollider2D)
            {
                boxColliders.Add(collider);
            }
        }

        CalcCloseObject(boxColliders.ToArray());
    }

    private void CalcCloseObject(Collider2D[] objects)
    {
        if (objects.Length == 1)
        {
            closeObject = objects[0];
        }
        else if (objects.Length > 0)
        {
            float minDistance = Vector2.Distance(transform.position, objects[0].transform.position); //일단 0부터 비교
            foreach (Collider2D collider in objects) //순차 적으로 하나씩 비교 - 가장 가까운 적 색출
            {
                float temp = Vector2.Distance(transform.position, collider.transform.position);
                if (minDistance >= temp)
                {
                    minDistance = temp;
                    closeObject = collider;
                }
            }
        }
        else
        {
            closeObject = null;
        }
        
    }

    public Collider2D getCloseObject()
    {
        return closeObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 11 || collision.gameObject.layer == 12)
        {
            return;
        }

        if(this.gameObject.layer != 9)
        {
            OnDamaged();
            InvincibleCo = SetInvincible();
            StartCoroutine(InvincibleCo);
        }
    }
    private void OnDamaged()
    {
        effectManager.DamageFrame();
        soundManager.playSFX("Damage");
        player.Damaged();
        if(RecoveryCo != null)
        {
            StopCoroutine(RecoveryCo);
        }
        RecoveryCo = player.Recovery();
        StartCoroutine(RecoveryCo);
    }

    IEnumerator SetInvincible()
    {
        this.gameObject.layer = 10;
        yield return new WaitForSeconds(InvincibleTime);
        this.gameObject.layer = 3;
        yield return null;
    }
}
