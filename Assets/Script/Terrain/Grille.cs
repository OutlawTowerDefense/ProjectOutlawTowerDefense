using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grille : MonoBehaviour
{
    public int test;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.eKey.wasReleasedThisFrame)
        {
            Mafunc();
        }
    }

    private void Mafunc()
    {
        
        Debug.Log("bla");
    }
}
