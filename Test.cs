using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        //MidiPlayer.Instance.ChangeInstrument(1);
        //StartCoroutine(testCoroutine());
    }
    
    // private IEnumerator testCoroutine()
    // {
    //     while (true)
    //     {
    //         MidiPlayer.Instance.PlayNote(60, 1500);
    //         yield return new WaitForSeconds(2.3f);
    //     }
    // }

    private void OnValidate()
    {
        
    }
}
