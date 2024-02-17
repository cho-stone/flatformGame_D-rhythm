using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health = 3; //플레이어 체력
    private int maxHealth = 3; //최대 체력
    private float recoveryTime = 30f; // 회복 시간


    //플레이어가 데미지를 입었을 때
    public void Damaged()
    {
        if (health > 1)
            health -= 1;
        else
            Die();
    }

    //죽었을 때
    public void Die()
    {

    }

    public void Attack()
    {

    }

    //피해를 입지 않을 때 회복
    public IEnumerator Recovery()
    {
        yield return new WaitForSeconds(recoveryTime);
        health = maxHealth;
        yield return null;
    }
}
