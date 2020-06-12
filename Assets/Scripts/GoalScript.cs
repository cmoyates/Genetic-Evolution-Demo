using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public GameManager gm;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "PlayerCube") 
        {
            float[] solution = other.GetComponent<CubeController>().GetAngles();
            gm.bestSolution = solution;
            gm.StartComplete();
        }
    }
}