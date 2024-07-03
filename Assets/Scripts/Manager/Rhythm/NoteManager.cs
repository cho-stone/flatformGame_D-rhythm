using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d;

    bool noteActive = true;

    [SerializeField] Transform tfNoteAppear = null;

    JudManager judManager;
    EffectManager effectManager;
    ComboManager comboManager;

    void Start()
    {
        effectManager = FindObjectOfType<EffectManager>();
        comboManager = FindObjectOfType<ComboManager>();
        judManager = GetComponent<JudManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(noteActive)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= 60d / bpm)
            {
                GameObject t_note = ObjectPool.instance.noteQueue.Dequeue();
                t_note.transform.position = tfNoteAppear.position;
                t_note.SetActive(true);
                judManager.boxNoteList.Add(t_note);
                currentTime -= 60d / bpm;
            }
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            if(collision.GetComponent<Note>().GetNoteFlag())
            {
                //effectManager.JudgementEffect(4);
                comboManager.ResetCombo();
            }
                
            judManager.boxNoteList.Remove(collision.gameObject);

            ObjectPool.instance.noteQueue.Enqueue(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    public void RemoveNote()
    {
        noteActive = false;

        for(int i = 0; i <  judManager.boxNoteList.Count; i++)
        {
            judManager.boxNoteList[i].SetActive(false);
            ObjectPool.instance.noteQueue.Enqueue(judManager.boxNoteList[i]);
        }
    }
}
