using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class GrilleSystem : MonoBehaviour
{    
    public Object obj;
    private bool isMoving = false;
    private float moveTime = 0.2f;
    private Vector3 startPos, endPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(!isMoving) StartCoroutine(MoveObj(new Vector3(2,0,0)));

    }

    IEnumerator MoveObj(Vector3 dir)
    {
        isMoving = true;
        float nextMove = 0f;
        startPos = transform.position;
        endPos = startPos + dir;

        while(nextMove < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, nextMove / moveTime);
            nextMove += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        isMoving = false;
    }


}
