using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform spawnPoint;
    Vector3 spawnPos;
    Quaternion spawnRot;
    public int spawnNum = 1;
    public GameObject cubePrefab;
    public Text generationCounter;
    List<CubeController> cubeControllers = new List<CubeController>();
    List<CubeController> childControllers = new List<CubeController>();
    public int moveNum = 4;
    int generation = 0;
    int closestSeen = 50000;
    public bool complete = false;
    int childPopSize;
    public float mutationRate = 0.8f;
    public float[] bestSolution;
    bool running = false;
    bool simIsParent = true;
    float[][] survivors;
    int maxDist;
    public Button startButton;
    public Button resetButton;
    SliderScript sliderScript;
    public SolutionScript solutionScript;
    public DistanceGet distanceGet;
    public TimerScript timerScript;
    // Start is called before the first frame update
    void Start()
    {
        sliderScript = GetComponent<SliderScript>();
        spawnPos = spawnPoint.position;
        spawnRot = spawnPoint.rotation;
        CubeController.gm = this;
        int thirdOfPop = Mathf.RoundToInt(spawnNum * 0.3f);
        childPopSize = (thirdOfPop % 2 == 1) ? thirdOfPop + 1 : thirdOfPop;
        UpdateText();
    }

    IEnumerator Spawn()
    {
        generation++;
        UpdateText();
        for (int i = 0; i < spawnNum; i++)
        {
            cubeControllers.Add(Instantiate(cubePrefab, spawnPos, spawnRot).GetComponent<CubeController>());
            cubeControllers[i].SetName("Population: " + i.ToString());
        }
        for (int i = 0; i < childPopSize; i++)
        {
            childControllers.Add(Instantiate(cubePrefab, spawnPos, spawnRot).GetComponent<CubeController>());
            childControllers[i].SetName("Child: " + i.ToString());
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < spawnNum; i++)
        {
            cubeControllers[i].StartMovement();
        }
    }

    IEnumerator GameLoop()
    {
        maxDist = distanceGet.GetDistance();
        timerScript.StartTimer();
        running = true;
        startButton.interactable = false;
        resetButton.interactable = true;
        sliderScript.SetInteractibility(running);
        yield return StartCoroutine(Spawn());
        yield return new WaitForSeconds(moveNum * 3 + 2);
        while (!complete)
        {
            yield return StartCoroutine(IterateGeneration());
        }
    }

    IEnumerator IterateGeneration()
    {
        List<int> currentDistances = new List<int>();
        for (int i = 0; i < spawnNum; i++)
        {
            currentDistances.Add(cubeControllers[i].GetDist());
        }
        // perform parent selection and create & mutate children
        float[][] children = new float[childPopSize][];
        for (int index = 0; index < childPopSize; index = index + 2)
        {
            CubeController[] parents = RouletteSelect();
            float[][] tempKids = Crossover(parents);

            for (int i = 0; i < tempKids.Length; i++)
            {
                tempKids[i] = InversionMutate(tempKids[i], mutationRate / 10);
                tempKids[i] = Mutate(tempKids[i], mutationRate);
            }
            children[index] = tempKids[0];
            children[index + 1] = tempKids[1];
        }

        // perform survivor selection
        float[][] pop = new float[spawnNum][];
        for (int i = 0; i < spawnNum; i++)
        {
            pop[i] = cubeControllers[i].GetAngles();
        }
        yield return StartCoroutine(SurvivorSelection(pop, children));

        // check for highest fitness seen
        // (best this gen always lives at top of list)
        int indexOfBest = FindClosest(cubeControllers.ToArray());
        float[] bestThisGen = cubeControllers[indexOfBest].GetAngles();
        int fitness = currentDistances[indexOfBest];
        if (fitness < closestSeen)
        {
            closestSeen = fitness;
            bestSolution = bestThisGen;
        }

        for (int i = 0; i < spawnNum; i++)
        {
            cubeControllers[i].SetAngles(survivors[i]);
        }

        generation++;
        UpdateText();

        foreach (CubeController item in cubeControllers)
        {
            item.ResetPos();
            item.StartMovement();
        }
        yield return new WaitForSeconds(moveNum * 3 + 2);
    }

    public void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }

    void UpdateText()
    {
        string closestString = (closestSeen == 50000) ? "N/A" : (closestSeen.ToString() + "m");
        string currentSimString = (running) ? ((simIsParent) ? "Parents" : "Children") : "N/A";
        generationCounter.text = "Generation: " + generation.ToString() + System.Environment.NewLine + "Closest Distance: " + closestString +
        System.Environment.NewLine + "Current Simulation: " + currentSimString;
    }

    public void Reset()
    {
        StopAllCoroutines();
        generation = 0;
        running = false;
        bestSolution = new float[moveNum];
        startButton.interactable = true;
        resetButton.interactable = false;
        sliderScript.SetInteractibility(running);
        closestSeen = 50000;
        simIsParent = true;
        UpdateText();
        solutionScript.CleanUp();
        timerScript.ResetTimer();
        foreach (CubeController item in cubeControllers)
        {
            Destroy(item.gameObject);
        }
        foreach (CubeController item in childControllers)
        {
            Destroy(item.gameObject);
        }
        cubeControllers = new List<CubeController>();
        childControllers = new List<CubeController>();
        complete = false;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPos;
    }

    int FindClosest(CubeController[] controllers)
    {
        List<int> distances = new List<int>();
        for (int i = 0; i < spawnNum; i++)
        {
            distances.Add(controllers[i].GetDist());
        }
        int minVal = Mathf.Min(distances.ToArray());
        int minIndex = distances.IndexOf(minVal);
        return minIndex;
    }

    float[][] Crossover(CubeController[] controllers)
    {
        float[][] children = new float[2][];
        float[][] parents = new float[2][];
        for (int i = 0; i < 2; i++)
        {
            children[i] = new float[moveNum];
            parents[i] = controllers[i].GetAngles();
        }


        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < (moveNum / 2); j++)
            {
                children[i][j] = parents[i][j];
            }

            int tempCounter = (moveNum / 2);

            for (int j = 0; j < (moveNum / 2); j++)
            {
                int actualIndex = (j + (moveNum / 2)) % moveNum;
                children[i][tempCounter] = parents[(i + 1) % 2][actualIndex];
                tempCounter = tempCounter + 1;
            }
            tempCounter = (moveNum / 2);
        }


        return children;
    }

    public CubeController[] RouletteSelect()
    {
        CubeController[] parents = new CubeController[2];

        Random rand = new Random();

        int totalFitness = 0;

        for (int i = 0; i < spawnNum; i++)
        {
            totalFitness = totalFitness + (maxDist-cubeControllers[i].GetDist());
        }

        int[] parentPickerArray = new int[totalFitness];

        int startPoint = 0;

        for (int i = 0; i < spawnNum; i++)
        {
            for (int j = startPoint; j < ((maxDist-cubeControllers[i].GetDist()) + startPoint); j++)
            {
                parentPickerArray[j] = i;
            }

            startPoint = startPoint + (maxDist-cubeControllers[i].GetDist());
        }

        parents[0] = cubeControllers[parentPickerArray[Random.Range(0, totalFitness)]];

        parents[1] = parents[0];

        while (parents[1] == parents[0])
        {
            parents[1] = cubeControllers[parentPickerArray[Random.Range(0, totalFitness)]];
        }

        return parents;
    }

    IEnumerator SurvivorSelection(float[][] population, float[][] children)
    {
        float[][] newPop = new float[spawnNum][];

        int totalGenotypes = population.Length + children.Length;

        int[] indexes = new int[totalGenotypes];
        int[] fitnesses = new int[totalGenotypes];

        for (int i = 0; i < population.Length; i++)
        {
            indexes[i] = i + 1;
            fitnesses[i] = cubeControllers[i].GetDist();
        }
        simIsParent = false;
        UpdateText();
        foreach (CubeController item in childControllers)
        {
            item.ResetPos();
            item.StartMovement();
        }
        yield return new WaitForSeconds(moveNum * 3 + 2);

        List<int> childDistances = new List<int>();
        for (int i = 0; i < childPopSize; i++)
        {
            childDistances.Add(childControllers[i].GetDist());
        }

        for (int i = 0; i < children.Length; i++)
        {
            indexes[i + population.Length] = -1 * i - 1;

            fitnesses[i + population.Length] = cubeControllers[i].GetDist();
        }

        //Sort by fitness
        bool sorted = false;

        while (!sorted)
        {
            sorted = true;

            for (int i = 0; i < totalGenotypes - 1; i++)
            {
                int tempFit, tempInd;

                if (fitnesses[i] > fitnesses[i + 1])
                {
                    tempFit = fitnesses[i];
                    tempInd = indexes[i];
                    indexes[i] = indexes[i + 1];
                    fitnesses[i] = fitnesses[i + 1];
                    fitnesses[i + 1] = tempFit;
                    indexes[i + 1] = tempInd;

                    sorted = false;
                }
            }
        }

        for (int i = 0; i < population.Length; i++)
        {
            if (indexes[i] > 0)
            {
                newPop[i] = population[indexes[i] - 1];
            }
            else
            {
                newPop[i] = children[-1 * indexes[i] - 1];
            }
        }

        survivors = newPop;
        yield return new WaitForSeconds(0);
        simIsParent = true;
    }

    float[] Mutate(float[] angles, float p)
    {
        if (Random.Range(0f, 1f) < p)
        {
            int index1 = Random.Range(0, moveNum);
            int index2 = index1;
            while (index1 == index2)
            {
                index2 = Random.Range(0, moveNum);
            }
            float temp = angles[index1];
            angles[index1] = angles[index2];
            angles[index2] = temp;
        }
        return angles;
    }
    public float[] InversionMutate(float[] angles, float p)
    {
        Random rand = new Random();
        if (Random.Range(0f, 1f) < p)
        {
            int invIndex1 = Random.Range(0, moveNum);
            int invIndex2 = invIndex1;
            while (invIndex2 == invIndex1)
            {
                invIndex2 = Random.Range(0, moveNum);
            }

            int greaterIndex = Mathf.Max(invIndex1, invIndex2);
            int lesserIndex = Mathf.Min(invIndex1, invIndex2);

            float[] invGenotype = new float[moveNum];

            for (int i = 0; i < moveNum; i++)
            {
                if (i < lesserIndex || i > greaterIndex)
                {
                    invGenotype[i] = angles[i];
                }
                else
                {
                    invGenotype[greaterIndex - (i - lesserIndex)] = angles[i];
                }
            }

            return invGenotype;
        }
        else { return angles; }
    }

    public void StartComplete()
    {
        StopAllCoroutines();
        StartCoroutine(Complete());
    }
    IEnumerator Complete() 
    {
        complete = true;
        running = false;
        simIsParent = true;
        closestSeen = 0;
        UpdateText();
        timerScript.PauseTimer();
        /*foreach (CubeController item in cubeControllers)
        {
            Destroy(item.gameObject);
        }
        foreach (CubeController item in childControllers)
        {
            Destroy(item.gameObject);
        }
        cubeControllers = new List<CubeController>();
        childControllers = new List<CubeController>();*/
        yield return StartCoroutine(solutionScript.SolutionAnimation(bestSolution));
    }
}