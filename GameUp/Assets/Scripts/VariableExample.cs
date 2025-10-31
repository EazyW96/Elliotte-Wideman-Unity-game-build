using UnityEngine;

public class VariableExample : MonoBehaviour
{
    public int CurrentAge = 30;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Debug.Log(30 + 1);
       Debug.Log(CurrentAge + 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
