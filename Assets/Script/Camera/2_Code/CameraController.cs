using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;

namespace KaizerWaldCode.RTTCamera
{
    public class CameraController : MonoBehaviour, Controls.ICameraControlActions
    {
        [SerializeField, Min(0)] private int RotationSpeed = 10; 
        [SerializeField, Min(0)] private int BaseMoveSpeed = 1; 
        [SerializeField, Min(0)] private int ZoomSpeed = 1;

        [Tooltip("How far in degrees can you move the camera Down")]
        [SerializeField] private float MaxClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera Top")]
        [SerializeField] private float MinClamp = -30.0f;
        
        public bool DontDestroy;
        
        //Cache Data
        private Controls controls;
        private Transform cameraTransform;

        //Inputs
        private bool isMoving;
        private bool isRotating;
        private bool isSprinting;
        private float zoom;

        private Vector2 mouseStartPosition, mouseEndPosition;
        private Vector2 moveAxis;
        
        //UPDATED MOVE SPEED
        private int SprintSpeed => BaseMoveSpeed * 2;
        private int MoveSpeed => isSprinting ? BaseMoveSpeed * SprintSpeed : BaseMoveSpeed;
        private void Awake()
        {
            if (controls == null)
            {
                controls = new Controls();
            }
            if (!controls.CameraControl.enabled)
            {
                controls.CameraControl.Enable();
                controls.CameraControl.SetCallbacks(this);
            }
            
            cameraTransform = transform;
            if(DontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            if (!isRotating && !isMoving/*moveAxis == Vector2.zero*/ && Approximately(zoom, 0)) return;
            // Rotation
            Quaternion newRotation = cameraTransform.rotation;
            if (isRotating)
            {
                newRotation = GetCameraRotation();
            }
            
            // Position Left/Right/Front/Back
            Vector3 newPosition = cameraTransform.position;
            if (/*moveAxis != Vector2.zero*/ isMoving)
            {
                newPosition = GetCameraPosition(newPosition, cameraTransform.forward, cameraTransform.right);
            }
            // Position Up/Down
            if (!Approximately(zoom, 0))
            {
                newPosition = Vector3.up * (ZoomSpeed * zoom) + newPosition;
            }
            
            // Update Camera Transform
            cameraTransform.SetPositionAndRotation(newPosition, newRotation);
        }

        private Vector3 GetCameraPosition(in Vector3 cameraPosition, in Vector3 cameraForward, in Vector3 cameraRight)
        {
            //real forward of the camera (aware of the rotation)
            Vector3 cameraForwardXZ = new (cameraForward.x, 0, cameraForward.z);
            
            Vector3 xDirection = Approximately(moveAxis.x,0) ? Vector3.zero : (moveAxis.x > 0 ? cameraRight : -cameraRight);
            Vector3 zDirection = Approximately(moveAxis.y,0) ? Vector3.zero : (moveAxis.y > 0 ? cameraForwardXZ : -cameraForwardXZ);

            float heightMultiplier = max(1f, cameraPosition.y); //plus la caméra est haute, plus elle est rapide
            float speedMultiplier  = heightMultiplier * MoveSpeed * Time.deltaTime;
            
            return cameraPosition + (xDirection + zDirection) * speedMultiplier;
        }

        private Quaternion GetCameraRotation()
        {
            if (mouseEndPosition == mouseStartPosition) return cameraTransform.rotation;
            Quaternion rotation = cameraTransform.rotation;
            
            Vector2 distanceXY = (mouseEndPosition - mouseStartPosition) * RotationSpeed;
            
            rotation = Utils.RotateFWorld(rotation ,0f, distanceXY.x * Time.deltaTime,0f);//Rotation Horizontal
            rotation = Utils.RotateFSelf(rotation,-distanceXY.y * Time.deltaTime, 0f, 0f);//Rotation Vertical
            
            float clampedXAxis = Utils.ClampAngle(rotation.eulerAngles.x, MinClamp, MaxClamp);
            rotation.eulerAngles = new Vector3(clampedXAxis, rotation.eulerAngles.y, 0);
            
            mouseStartPosition = mouseEndPosition;
            return rotation;
        }
        
        //==============================================================================================================
        // INPUTS EVENTS CALLBACK
        
        public void OnMouvement(InputAction.CallbackContext context)
        {
            if (!context.canceled)
            {
                moveAxis = context.ReadValue<Vector2>();
                isMoving = true;
            }
            else
            {
                moveAxis = Vector2.zero;
                isMoving = false;
            }
            //moveAxis = !context.canceled ? context.ReadValue<Vector2>() : Vector2.zero;
        }

        public void OnRotation(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    mouseStartPosition = context.ReadValue<Vector2>();
                    isRotating = true;
                    break;
                case InputActionPhase.Performed:
                    mouseEndPosition = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Canceled:
                    isRotating = false;
                    break;
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            zoom = !context.canceled ? context.ReadValue<float>() : 0;
        }

        public void OnFaster(InputAction.CallbackContext context)
        {
            isSprinting = !context.canceled;
        }
        //==============================================================================================================
    }
}
