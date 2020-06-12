using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolutionScript : MonoBehaviour
{
    public GameObject youDidIt;
    public GameObject[] solutionDisplay;
    public GameObject resetReminder;
    public float animSpeedScale = 1.5f;
    Text solutionText;
    string[] directions = {"Forward", "Left", "Backward", "Right"};
    // Start is called before the first frame update
    void Start()
    {
        solutionText = solutionDisplay[1].GetComponent<Text>();
    }

    public IEnumerator SolutionAnimation(float[] solution) 
    {
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(1 * Time.timeScale / animSpeedScale);
            youDidIt.SetActive(!youDidIt.activeInHierarchy);
        }
        yield return new WaitForSeconds(2 * Time.timeScale / animSpeedScale);
        solutionText.text = ConvertSolutionToString(solution);
        foreach (GameObject item in solutionDisplay)
        {
            item.SetActive(true);
        }
        yield return new WaitForSeconds(3 * Time.timeScale / animSpeedScale);
        resetReminder.SetActive(true);
    }

    public void CleanUp() 
    {
        youDidIt.SetActive(false);
        foreach (GameObject item in solutionDisplay)
        {
            item.SetActive(false);
        }
        resetReminder.SetActive(false);
    }

    private string ConvertSolutionToString(float[] solution) 
    {
        string numberString = "";
        string wordString = "";
        for (int i = 0; i < solution.Length; i++)
        {
            int number = Mathf.RoundToInt(solution[i]/90);
            numberString += (number.ToString() + " - ");
            wordString += (directions[number] + " - ");
        }
        numberString = numberString.Substring(0, numberString.Length-3);
        wordString = wordString.Substring(0, wordString.Length - 3);

        string output = numberString + System.Environment.NewLine + "or" + System.Environment.NewLine + wordString;

        return output;
    }
}
