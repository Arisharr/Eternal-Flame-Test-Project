using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private KeyCode pauseKey;
    [SerializeField] private List<PlayableCharactersBehaviour> allPCBs = new List<PlayableCharactersBehaviour>();
    [SerializeField] private TMP_Text text;
    [SerializeField] private Volume dofVolume;

    private bool isGamePaused = false;
    private PlayableCharactersBehaviour firstPCB;
    private PlayableCharactersBehaviour secondPCB;
    [SerializeField] private Color loseTextColor;
    [SerializeField] private Color winTextColor;


    private void Awake()
    {
        Instance = this;

        allPCBs = FindObjectsOfType<PlayableCharactersBehaviour>().ToList();
    }

    private void Start()
    {
        StartCoroutine(StartLevel());
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (!isGamePaused) PauseGame();
            else ResumeGame();
        }
    }

    private void ReplaceActions()
    {
        PCBAction firstPcbTempPcbAction = firstPCB.currentPcbAction;

        firstPCB.currentPcbAction = secondPCB.currentPcbAction;
        firstPCB.OnRoleChange.Invoke();
        
        secondPCB.currentPcbAction = firstPcbTempPcbAction;
        secondPCB.OnRoleChange.Invoke();

        firstPCB.Selection(false);
        secondPCB.Selection(false);
        
        firstPCB = null;
        secondPCB = null;
    }

    public void AddPCBs(PlayableCharactersBehaviour _pcb)
    {
        if (firstPCB == null)
        {
            firstPCB = _pcb;
            firstPCB.Selection(true);
        }
        else 
        {
            secondPCB = _pcb;
            secondPCB.Selection(false);
        }
        ReplaceActions();
    }

    private void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
        
        allPCBs.ForEach(pc => pc.ShowDirection());
    }

    private void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        allPCBs.ForEach(pc => pc.DisableDirection());
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator StartLevel()
    {
        text.text = "level " + SceneManager.GetActiveScene().buildIndex.ToString();
        
        dofVolume.weight = 1;
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 200), 1f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(2f);
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 500), .5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(
            () =>
            {
                dofVolume.weight = 0;
            });
    }

    public IEnumerator Win()
    {
        text.color = winTextColor;
        text.text = "win!";
        
        dofVolume.weight = 1;
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 200), 1f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(2f);
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 500), .5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(
            () =>
            {
                NextLevel();
            });
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public IEnumerator Lose()
    {
        text.color = loseTextColor;
        text.text = "try again!";
        
        dofVolume.weight = 1;
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 200), 1f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(2f);
        LeanTween.move(text.gameObject, (text.transform.position - Vector3.up * 500), .5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(
            () =>
            {
                ResetGame();
            });
    }
}
