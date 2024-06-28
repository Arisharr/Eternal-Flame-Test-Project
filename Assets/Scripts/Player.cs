using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayableCharactersBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Destination")
        {
            StartCoroutine(GameManager.Instance.Win());
        }
    }

    public override void Kill()
    {
        base.Kill();
        StartCoroutine(GameManager.Instance.Lose());
    }
}

