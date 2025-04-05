using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] float min = 2;
    [SerializeField] float sec = 60;
    [SerializeField] TextMeshProUGUI timerText;
    Coroutine coroutine;
    public bool gameover;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (gameover)
            return;
        if (coroutine == null)
        {

            coroutine = StartCoroutine(StopWatch());
        }
    }

    IEnumerator StopWatch()
    {
        yield return new WaitForSeconds(1f);
        sec--;
        timerText.text = "Time- " + min + ":" + sec;
        if (sec == 0 && min == 0)
        {
            gameover = true;
        }
        if (sec == 0)
        {
            sec = 60;
            min--;
        }
        coroutine = null;
    }
}

