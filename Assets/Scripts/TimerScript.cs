using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public Text timerText;
    float currentTime;
    int currentSeconds = 0;
    bool timerShown = false;
    bool timerActive = false;


    // Update is called once per frame
    void Update()
    {
        if (timerShown) 
        {
            if (timerActive) 
            {
                currentTime += Time.unscaledDeltaTime;
                SetTimer(currentTime);
                UpdateText();
            }
        }
    }

    public void StartTimer() 
    {
        currentTime = 0f;
        SetTimer(currentTime);
        timerActive = true;
        timerShown = true;
        timerText.gameObject.SetActive(true);
    }

    public void PauseTimer() 
    {
        timerActive = false;
    }

    public void ResetTimer() 
    {
        timerActive = false;
        timerShown = false;
        timerText.gameObject.SetActive(false);
    }

    void SetTimer(float timeFloat) 
    {
        currentSeconds = Mathf.RoundToInt(timeFloat);
    }

    void UpdateText() 
    {
        int hour = Mathf.FloorToInt(currentTime / 3600);
        int min = Mathf.FloorToInt((currentTime / 60) % 60);
        int sec = Mathf.FloorToInt(currentTime % 60);
        string minString = (min > 9) ? (min.ToString()) : ("0" + min.ToString());
        string secString = (sec > 9) ? (sec.ToString()) : ("0" + sec.ToString());
        timerText.text = (hour > 0) ? (hour.ToString() + ":" + minString + ":" + secString) : (minString + ":" + secString);
    }
}
