using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewTimer : MonoBehaviour
{
    public PointAndClickMove playerReference;
    public double minutes;
    public double seconds;
    private double MaxTime;
    private double CurrentTime;
    bool loadedLevel = false;
    public GameObject Timer;
    public TextMeshProUGUI timerText;

    void SetTimer()
    {
        MaxTime = (minutes*60) + seconds;
        CurrentTime = MaxTime;
    }

    void OnTimerEnd()
    {
       Timer.SetActive(true);
    }

    private void Start()
    {
        Timer.SetActive(false);
        Debug.Log("This level will restart in " + minutes + " minutes and " + seconds + " seconds");
        SetTimer();
    }
    private void Update()
    {

        //Debug.Log("Running timer");
        if(!playerReference.interacting && CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
            int minutesLeft = Mathf.FloorToInt((float)CurrentTime / 60);
            int secondsLeft = Mathf.FloorToInt((float)CurrentTime % 60);
            timerText.text = "Time Remaining: " +minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");
        }
        else if(playerReference.interacting) 
        {
            //Debug.Log("Player is interacting; timer paused at " + (CurrentTime / MaxTime));
        }
        else if(!loadedLevel)
        {
            loadedLevel = true;
            //Debug.Log("EndTimer");
            OnTimerEnd();
        }
    }

    public double GetCurrentTime()
    {
        return CurrentTime;
    }

    public double GetMaxTime()
    {
        return MaxTime;
    }
}
