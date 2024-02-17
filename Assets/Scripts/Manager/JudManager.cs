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

        //���� ����
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
                    //��Ʈ����
                    boxNoteList[i].GetComponent<Note>().HideNote();
                    boxNoteList.RemoveAt(i);

                    //����Ʈ �����ֱ�
                    if (j < timingBoxs.Length - 1)
                        effectManager.NoteHitEffect();
                    effectManager.JudgementEffect(j);

                    //���� ����ֱ�
                    soundManager.playSFX("noteHit");

                    //����
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
