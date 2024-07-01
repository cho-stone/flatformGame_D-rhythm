using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textCombo = null;

    int currCombo = 0;

    Animator animator;
    string animComboUp = "ComboUp";

    void Start()
    {
        animator = GetComponent<Animator>();
        textCombo.gameObject.SetActive(false);
    }

    public void IncreaseCombo(int num = 1)
    {
        currCombo += num;
        textCombo.text = string.Format("{0:#,##0}", currCombo);

        if(currCombo > 2)
        {
            textCombo.gameObject.SetActive(true);
            animator.SetTrigger(animComboUp);
        }
    }

    public int GetCombo()
    {
        return currCombo;
    }

    public void ResetCombo()
    {
        currCombo = 0;
        textCombo.text = "0";
        textCombo.gameObject.SetActive(false);
    }
}
