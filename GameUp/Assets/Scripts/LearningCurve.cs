using UnityEngine;

public class LearningCurve : MonoBehaviour
{
    public int CurrentAge = 30;
     public int YearsToAdd = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComputeAge();
    }


    void ComputeAge()
    {
        Debug.Log(CurrentAge + YearsToAdd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
