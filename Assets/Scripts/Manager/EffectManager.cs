using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{

    [SerializeField] Animator noteHitAnimator = null;
    string hit = "Hit";

    [SerializeField] Animator judgementAnimator = null;
    [SerializeField] UnityEngine.UI.Image judgementImage = null;
    [SerializeField] Sprite[] judgementSprite = null;

    List<GameObject> notes = null;
    private float ChangeNoteTime = 0.5f;
    [SerializeField] GameObject containerLine = null;
    [SerializeField] GameObject NoteContainer = null;
    [SerializeField] GameObject JudLine = null;
    [SerializeField] Color redColor_containerLine;
    [SerializeField] Color whiteColor_containerLine;
    [SerializeField] Color redColor_JudLine;
    [SerializeField] Color whiteColor_JudLine;

    [SerializeField] GameObject DamageEffect;
    [SerializeField] GameObject MainCamera;
    private float DamageEffectTime = 1.0f;
    

    public void JudgementEffect(int p_num)
    {    
        judgementImage.sprite = judgementSprite[p_num];
        judgementAnimator.SetTrigger(hit);
    }
    
    public void NoteHitEffect()
    {
        noteHitAnimator.SetTrigger(hit);
    }

    public void NoteMissEffect()
    {
        notes = ObjectPool.instance.notes;
        StartCoroutine(noteMissEffectCo());
    }

    IEnumerator noteMissEffectCo()
    {
        for(int i = 0; i < notes.Count; i++)
        {
            notes[i].GetComponent<Note>().ChangeNoteImage();
        }
        containerLine.GetComponent<Image>().color = redColor_containerLine;
        NoteContainer.GetComponent<Image>().color = Color.red;
        JudLine.GetComponent<Image>().color = redColor_JudLine;

        yield return new WaitForSeconds(ChangeNoteTime);

        for (int i = 0; i < notes.Count; i++)
        {
            notes[i].GetComponent<Note>().ResetNoteImage();
        }
        containerLine.GetComponent<Image>().color = whiteColor_containerLine;
        NoteContainer.GetComponent<Image>().color = Color.white;
        JudLine.GetComponent<Image>().color = whiteColor_JudLine;
    }

    public void DamageFrame()
    {
        StartCoroutine(DamageEffectCo());
        StartCoroutine(MainCamera.GetComponent<CameraMove>().CameraShakeCo(0.2f, 0.5f));
    }

    IEnumerator DamageEffectCo()
    {
        DamageEffect.SetActive(true);
        yield return new WaitForSeconds(DamageEffectTime);
        DamageEffect.SetActive(false);
    }
}
