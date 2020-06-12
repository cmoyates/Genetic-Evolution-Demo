using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider timeScaleSlider;
    Text timeScaleSliderText;
    public Slider spawnNumSlider;
    Text spawnNumSliderText;
    public Slider moveNumSlider;
    Text moveNumSliderText;
    public Slider mutationRateSlider;
    Text mutationRateSliderText;
    public Slider sensitivitySlider;
    Text sensitivitySliderText;
    public Slider goalDistanceSlider;
    Text goalDistanceSliderText;
    GameManager gm;
    ExtendedFlycam camController;
    Transform goal;
    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GameManager>();
        camController = Camera.main.GetComponent<ExtendedFlycam>();
        timeScaleSliderText = timeScaleSlider.GetComponentInChildren<Text>();
        spawnNumSliderText = spawnNumSlider.GetComponentInChildren<Text>();
        moveNumSliderText = moveNumSlider.GetComponentInChildren<Text>();
        mutationRateSliderText = mutationRateSlider.GetComponentInChildren<Text>();
        sensitivitySliderText = sensitivitySlider.GetComponentInChildren<Text>();
        goalDistanceSliderText = goalDistanceSlider.GetComponentInChildren<Text>();
        goal = GameObject.FindGameObjectWithTag("Goal").transform;
        SetTimeScale();
        SetSpawnNum();
        SetMoveNum();
        SetMutationRate();
        SetSensitivity();
        SetGoalDistance();
    }

    public void SetTimeScale() 
    {
        float val = timeScaleSlider.value;
        Time.timeScale = val;
        timeScaleSliderText.text = "Time Scale: " + val.ToString("F3");
    }

    public void SetSpawnNum() 
    {
        int val = Mathf.RoundToInt(spawnNumSlider.value);
        gm.spawnNum = val;
        spawnNumSliderText.text = "Pop. Count: " + val.ToString();
    }

    public void SetMoveNum() 
    {
        int val = Mathf.RoundToInt(moveNumSlider.value);
        gm.moveNum = val;
        CubeController.moveNum = val;
        moveNumSliderText.text = "Move Count: " + val.ToString();
    }

    public void SetMutationRate() 
    {
        float val = mutationRateSlider.value;
        gm.mutationRate = val;
        mutationRateSliderText.text = "Mut. Rate: " + val.ToString("F3");
    }

    public void SetSensitivity() 
    {
        int val = Mathf.RoundToInt(sensitivitySlider.value);
        camController.cameraSensitivity = val*2;
        sensitivitySliderText.text = "Sensitivity: " + val.ToString();
    }

    public void SetGoalDistance() 
    {
        int val = Mathf.RoundToInt(goalDistanceSlider.value);
        goal.position = new Vector3(2.98f, 1.91f, val);
        goalDistanceSliderText.text = "Goal Z Coord: " + val.ToString();
    }

    public void SetInteractibility(bool running) 
    {
        spawnNumSlider.interactable = !running;
        moveNumSlider.interactable = !running;
        mutationRateSlider.interactable = !running;
        goalDistanceSlider.interactable = !running;
    }
}
