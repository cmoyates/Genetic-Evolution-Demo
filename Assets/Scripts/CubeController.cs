using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Rigidbody rbody;
    public float scale = 5000f;
    static public int moveNum;
    [SerializeField]
    float[] angles;
    GameObject goal;
    Vector3 goalPos;
    public int dist;
    public static GameManager gm;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.enabled = false;
        goalPos = goal.transform.position;
        rbody.maxAngularVelocity = 50f;
    }

    private void Awake() 
    {
        //moveNum = gm.moveNum;
        rbody = GetComponent<Rigidbody>();
        goal = GameObject.FindGameObjectWithTag("Goal");
        meshRenderer = GetComponent<MeshRenderer>();
        angles = new float[moveNum];
        for (int i = 0; i < moveNum; i++)
        {
            angles[i] = Random.Range(0, 4)*90;
        }
    }

    public void StartMovement() {
        StartCoroutine(Movement());
    }

    public void SetName(string newName)
    {
        gameObject.name = newName;
    }

    IEnumerator Movement() 
    {
        rbody.isKinematic = false;
        meshRenderer.enabled = true;
        for (int i = 0; i < moveNum; i++)
        {
            Move(angles[i]);
            yield return new WaitForSeconds(3);
        }
        rbody.isKinematic = true;
        dist = Mathf.RoundToInt((transform.position - goalPos).magnitude);
        yield return new WaitForSeconds(2);
        meshRenderer.enabled = false;
    }

    void Move(float angle) 
    {
        float radConvert = (Mathf.PI / 180);
        float x = Mathf.Cos(angle * radConvert) * scale;
        float z = Mathf.Sin(angle * radConvert) * scale;
        rbody.AddTorque(x, 0f, z);
    }

    public void ResetPos() 
    {
        StopAllCoroutines();
        transform.position = gm.GetSpawnPoint();
        transform.rotation = Quaternion.identity;
    }

    public void SetAngles(float[] newAngles) 
    {
        angles = newAngles;
    }
    public float[] GetAngles()
    {
        return angles;
    }
    public int GetDist() 
    {
        return dist;
    }
}
