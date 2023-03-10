using UnityEngine;
using UnityEngine.InputSystem;

public class Cube : MonoBehaviour
{
    
    private Camera cameraPlayer;
   private float speedMovement = 55f;

    private void Start()
    {
        cameraPlayer = Camera.main;
    }

    private void Update()
    {
       MagnetTargetPosition();
    }

    
    private void MagnetTargetPosition()
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
        }
    }
}