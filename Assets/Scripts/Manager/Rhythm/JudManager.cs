using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudManager : MonoBehaviour
{

    public List<GameObject> boxNoteList = new List<GameObject>();

    [SerializeField] Transform Center = null;
    [SerializeField] RectTransform[] timingRect = null;
    Vector2[] timingBoxs = null;

    EffectManager effectManager;
    ScoreManager scoreManager;
    ComboManager comboManager;
    SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.instance;
        effectManager = FindObjectOfType<EffectManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        comboManager = FindObjectOfType<ComboManager>();

        //판정 설정
        timingBoxs = new Vector2[timingRect.Length];

        for(int i = 0; i < timingRect.Length; i++)
        {
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width / 2, Center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }

    public bool CheckTiming()
    {
        for(int i = 0; i < boxNoteList.Count; i++)
        {
            float t_notePosX = boxNoteList[i].transform.localPosition.x;

            for(int j = 0; j < timingBoxs.Length; j++)
            {
                if (timingBoxs[j].x <= t_notePosX && t_notePosX <= timingBoxs[j].y)
                {
                    //노트제거
                    boxNoteList[i].GetComponent<Note>().HideNote();
                    boxNoteList.RemoveAt(i);

                    //이펙트 보여주기
                    if (j < timingBoxs.Length - 1)
                        effectManager.NoteHitEffect();
                    effectManager.JudgementEffect(j);

                    //사운드 들려주기
                    soundManager.playSFX("noteHit");

                    //점수
                    scoreManager.IncreaseScore(j);
                    return true;
                }
            }
        }

        comboManager.ResetCombo();
        effectManager.JudgementEffect(timingBoxs.Length);
        soundManager.playSFX("noteMiss");

        effectManager.NoteMissEffect();

        return false;
    }
}
