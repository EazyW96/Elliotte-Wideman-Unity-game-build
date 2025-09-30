using UnityEngine;

public class MyFirstScript : MonoBehaviour
{

    public int helloName(string name)
    {
        Debug.Log("Hello " + name);
        return 0;
    }   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        helloName("World");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
