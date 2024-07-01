using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Search;
using UnityEngine;

public class LockOnManager : MonoBehaviour
{
    private Collider2D closeObject;

    private Collider2D preCloseObject;

    PlayerCollider playerCollider;

    GameEffectManager effectManager;

    GameObject effect;

    private enum LockOnState
    {
        Off,
        On,
        Show,
        EnemyChange,
        Dash,
    }

    LockOnState lockOnState = LockOnState.Off;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = FindObjectOfType<PlayerCollider>();
        effectManager = FindObjectOfType<GameEffectManager>();
        effect = GameObject.Find("LockOnEffect").transform.Find("LockOn").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockOnState != LockOnState.Dash)
        {
            UpdateObject();
            IsWall();
            LockOn();
        }     
    }

    //���� ����� ������Ʈ ����, ���� ������Ʈ ���� �� ��
    private void UpdateObject()
    {
        closeObject = playerCollider.getCloseObject();

        if(preCloseObject == null)
        {
            preCloseObject = closeObject;
        }
        else if(closeObject == null)
        {
            return;
        }

        else if (preCloseObject != closeObject)
        {
            lockOnState = LockOnState.EnemyChange;
            preCloseObject = closeObject;
        }
    }

    //���̿� ���� ����� ���� ����
    private void IsWall()
    {
        if(closeObject == null)
        {
            lockOnState = LockOnState.Off;
            return;
        }

        Vector2 dir = closeObject.transform.position - transform.position;

        RaycastHit2D isWall = Physics2D.Raycast(transform.position, dir, 7, LayerMask.GetMask("Platform"));

        if (isWall)
        {
            lockOnState = LockOnState.Off;
        }
        else
        {
            if(lockOnState != LockOnState.Show && lockOnState != LockOnState.EnemyChange)
                lockOnState = LockOnState.On;
        }
    }

    private void LockOn()
    {
        switch (lockOnState)
        {

            case LockOnState.Off:
                effect.SetActive(false);
                break;

            case LockOnState.On:
                effect.SetActive(true);
                effect.transform.position = closeObject.transform.position;
                effectManager.LockOnEffect();
                lockOnState = LockOnState.Show;
                break;

            case LockOnState.Show:
                effect.transform.position = closeObject.transform.position;
                break;

            case LockOnState.EnemyChange:
                effect.SetActive(false);
                lockOnState = LockOnState.On;
                break;
        }
    }

    public void CurrDash()
    {
        lockOnState = LockOnState.Dash;
    }

    public void EndDash()
    {
        lockOnState = LockOnState.Off;
    }

    public bool IsLockOn()
    {
        if(lockOnState != LockOnState.Show)
            return false;
        return true;
    }

    public Collider2D GetLockOnObj()
    {
        return closeObject;
    }
}
