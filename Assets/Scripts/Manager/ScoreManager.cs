using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textScore = null;

    [SerializeField] int plusScore = 10;
    int currScore = 0;

    [SerializeField] float[] weight = null;
    [SerializeField] int comboBonusScore = 10;

    Animator animator;
    string animScoreUp = "ScoreUp";

    ComboManager comboManager;

    // Start is called before the first frame update
    void Start()
    {
        comboManager = FindObjectOfType<ComboManager>();
        animator = GetComponent<Animator>();
        currScore = 0;
        textScore.text = "0";
    }

    public void IncreaseScore(int judState)
    {
        //콤보증가
        comboManager.IncreaseCombo();

        //콤보 증가 가중치
        int combo = comboManager.GetCombo();
        int bonusCombo = (combo /10) * comboBonusScore;
        

        //점수 증가 가중치
        int score = plusScore + bonusCombo;
        score = (int)(score * weight[judState]);

        //점수 증감
        currScore += score;
        textScore.text = string.Format("{0:#,##0}", currScore);

        //점수 애니
        animator.SetTrigger(animScoreUp);
    }
}
