using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionParticleHandler : MonoBehaviour
{
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    private List<GameObject> allParticles = new List<GameObject>();

    private void Start()
    {
        allParticles.Add(right);
        allParticles.Add(left);
        allParticles.Add(up);
        allParticles.Add(down);
        ResetParticles();
    }

    public void ResetParticles()
    {
        foreach (var o in allParticles)
        {
            o.SetActive(false);
        }
    }

    public void ShowDirection(Direction _dir)
    {
        ResetParticles();
        switch (_dir)
        {
            case Direction.Right:
                right.SetActive(true);
                break;
            case Direction.Left:
                left.SetActive(true);
                break;
            case Direction.Up:
                up.SetActive(true);
                break;
            case Direction.Down:
                down.SetActive(true);
                break;
        }
    }
}
