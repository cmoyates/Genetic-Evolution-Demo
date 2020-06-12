using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGet : MonoBehaviour
{
    float dist;
    // Start is called before the first frame update
    void Start()
    {
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        dist = (goal.transform.position - transform.position).magnitude;
    }

    public int GetDistance()
    {
        return Mathf.RoundToInt(dist);
    }
}
