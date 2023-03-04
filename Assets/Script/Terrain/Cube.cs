using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cube : MonoBehaviour
{

    private float moveTime = 1f;
    private Vector3 startPos, endPos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Récupérer la position de la souris sur l'écran
        Vector3 mousePos = Mouse.current.position.ReadValue();

        // Convertir la position de la souris en position dans le monde 3D
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.y));

        // Centrer la position dans la grille (sur les axes x et z uniquement)
        float newX = Mathf.Round(worldPos.x - 0.5f) + 0.5f;
        float newZ = Mathf.Round(worldPos.z - 0.5f) + 0.5f;
        Vector3 centeredPos = new Vector3(newX, transform.position.y, newZ);

        // Calculer la distance entre la position actuelle du cube et la position centrée
        float distance = Vector3.Distance(transform.position, centeredPos);

        // Déplacer le cube vers la position centrée dans la grille s'il est assez éloigné
        if (distance > 0.01f)
        {
            StartCoroutine(MoveObj(centeredPos - transform.position));
        }
    }

    IEnumerator MoveObj(Vector3 dir)
    {
        float nextMove = 0f;
        startPos = transform.position;
        endPos = startPos + dir;

        while (nextMove < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, nextMove / moveTime);
            nextMove += Time.fixedDeltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
