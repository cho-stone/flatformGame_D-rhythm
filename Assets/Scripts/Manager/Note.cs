using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public float noteSpeed = 400;

    UnityEngine.UI.Image noteImage;

    [SerializeField] Sprite noteMissImage = null;
    [SerializeField] Sprite normalNoteImage = null;

    void OnEnable()
    {
        if(noteImage == null)
            noteImage = GetComponent<UnityEngine.UI.Image>();
        noteImage.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.right * noteSpeed * Time.deltaTime;
    }

    public void HideNote()
    {
        noteImage.enabled = false;
    }

    public bool GetNoteFlag()
    {
        return noteImage.enabled;
    }

    public void ChangeNoteImage()
    {
        noteImage.sprite = noteMissImage;
    }

    public void ResetNoteImage()
    {
        noteImage.sprite = normalNoteImage;
    }
}
