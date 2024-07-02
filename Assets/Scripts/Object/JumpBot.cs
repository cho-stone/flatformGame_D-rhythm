using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class JumpBot : MonoBehaviour
{
    Animator animator;
    BoxCollider2D boxCollider2D;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            StartCoroutine(DieCo());
        }
    }

    IEnumerator DieCo()
    {
        boxCollider2D.enabled = false;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(4f);
        spriteRenderer.enabled = true;
        animator.ResetTrigger("Death");  // 트리거 초기화
        animator.Play("JumpBotIdle");
        boxCollider2D.enabled = true;
        yield return null;
    }
}
