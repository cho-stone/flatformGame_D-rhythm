using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFlag : MonoBehaviour
{
    NoteManager noteManager;

    private void Start()
    {
        noteManager = FindObjectOfType<NoteManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            PlayerController.s_canPressKey = false;
            noteManager.RemoveNote();
        }
    }
}
