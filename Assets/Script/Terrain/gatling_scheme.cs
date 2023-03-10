using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class gatling_scheme : MonoBehaviour
{

    public GatlingGun gatling;

    private Camera cameraPlayer;
    private float speedMovement = 55f;
    private List<GatlingGun> gatlingsList;

    public List<GatlingGun> GatlingsList { get => gatlingsList; set => gatlingsList = value; }

    private void Start()
    {
        cameraPlayer = Camera.main;
    }

    private void Update()
    {
        GestionSchemaGatling();
    }


    private void GestionSchemaGatling()
    {
        Ray ray = cameraPlayer.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Centrage sur les axes X et Z
            float x = Mathf.Floor(hit.point.x) + 0.5f;
            float z = Mathf.Floor(hit.point.z) + 0.5f;
            Vector3 targetPosition = new(x, transform.position.y, z);
            if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, speedMovement * Time.deltaTime);
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Tourne l'objet de 90 degrés vers la droite
                Vector3 currentRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 90f, currentRotation.z);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                CreationInstance(targetPosition, transform);
            }
        }
    }

    private void CreationInstance(Vector3 targetPos, Transform transform)
    {
        // Instancie un objet Gatling sur la position du curseur
        GatlingGun newGatling = Instantiate(gatling.gameObject, targetPos, transform.rotation).GetComponent<GatlingGun>();
        newGatling.transform.Rotate(Vector3.up, 90f);
        GatlingsList.Add(newGatling);
        newGatling.Position = targetPos;
    }
}
