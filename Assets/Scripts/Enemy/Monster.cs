using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    private int monsterHealth;
    private Collider2D playerCol; //감지된 콜라이더 담는 오브젝트
    private float radius;
    private Rigidbody2D rigid;
    private SpriteRenderer spriteFlip;
    private Animator anim;
    private SoundManager soundManager;
    private BoxCollider2D boxCollider2D;

    public int MonsterHealth { get => monsterHealth; set => monsterHealth = value; }
    public Collider2D PlayerCol { get => playerCol; set => playerCol = value; }
    public float Radius { get => radius; set => radius = value; }
    public Rigidbody2D Rigid { get => rigid; set => rigid = value; }
    public SpriteRenderer SpriteFlip { get => spriteFlip; set => spriteFlip = value; }
    public Animator Anima { get => anim; set => anim = value; }
    public SoundManager SoundMana { get => soundManager; set => soundManager = value; }
    public BoxCollider2D BoxCol { get => boxCollider2D; set => boxCollider2D = value; }

    protected abstract void Attack();
    protected abstract void Move();
    protected abstract void Anim();

    protected void OnDamaged()
    {
        if (monsterHealth > 1)
            monsterHealth -= 1;
        else
            Die();
    }

    protected void Die()
    {
        StartCoroutine(DieCo());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 8)
        {
            OnDamaged();
        }  
    }

    IEnumerator DieCo()
    {
        BoxCol.enabled = false;
        Anima.SetTrigger("Death");
        SoundMana.playSFX("enemyDie");
        yield return new WaitForSeconds(0.3f);
        this.gameObject.SetActive(false);
        yield return null;
    }
}
