using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] float radius = 0f; //������ �ݶ��̴� ������
    [SerializeField] LayerMask layer; //�����ؼ� ������ ���̾� ����
    private Collider2D[] objects; //������ �ݶ��̴� ��� ������Ʈ
    private Collider2D closeObject; //���� ����� ������Ʈ ��°�

    EffectManager effectManager = null;
    SoundManager soundManager = null;
    float InvincibleTime = 2f; //���� �ð�
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
        //�÷��̾ Ư�� layer�� ���� ������Ʈ�� �����ϸ� �� ������Ʈ ��������
        objects = Physics2D.OverlapCircleAll(transform.position, radius, layer);

        // BoxCollider2D �� ���͸�
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
            float minDistance = Vector2.Distance(transform.position, objects[0].transform.position); //�ϴ� 0���� ��
            foreach (Collider2D collider in objects) //���� ������ �ϳ��� �� - ���� ����� �� ����
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
