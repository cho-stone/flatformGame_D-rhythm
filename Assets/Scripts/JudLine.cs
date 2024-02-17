using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudLine : MonoBehaviour
{
    bool musicStart = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!musicStart) 
        {
            if (collision.CompareTag("Note"))
            {
                SoundManager.instance.playBGM("Tutorial");
                musicStart = true;
            }
        } 
    }
}
