using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health = 3; //�÷��̾� ü��
    private int maxHealth = 3; //�ִ� ü��
    private float recoveryTime = 30f; // ȸ�� �ð�


    //�÷��̾ �������� �Ծ��� ��
    public void Damaged()
    {
        if (health > 1)
            health -= 1;
        else
            Die();
    }

    //�׾��� ��
    public void Die()
    {

    }

    public void Attack()
    {

    }

    //���ظ� ���� ���� �� ȸ��
    public IEnumerator Recovery()
    {
        yield return new WaitForSeconds(recoveryTime);
        health = maxHealth;
        yield return null;
    }
}
