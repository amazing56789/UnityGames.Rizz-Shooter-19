using UnityEngine;

public sealed class PlayerMovementController : MonoBehaviour {
    [SerializeField] float
        gravityY = Physics.gravity.y,
        
        mouseXSens = 1.0f,
        mouseYSens = 1.0f,
        
        walkSpeed = 7.0f,
        runSpeed = 20.0f,
        
        airSpeedSmoothTime = 0.5f,
        speedSmoothTime = 0.2f,
        
        fallAdditionalGravity = -0.2f,
        highJumpVelocity = -0.05f,
        lowJumpVelocity = 5.0f,
        jumpForce = 7.0f;

    public bool freeze = false;
    
    readonly int forwardSpeedPercentAnimatorParameter = Animator.StringToHash("ForwardSpeedPercent");
    CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Confined; // #Debug CursorLockMode.Locked;
    }

    Vector3 inputMotion;
    float rotationX; // this and rotationY are delta rotations for current frame only
    float rotationY;
    bool running;
    bool jumping;
    void Update() {
        running = Input.GetKey(KeyCode.LeftShift);
        jumping = Input.GetKey(KeyCode.Space);
        
        rotationX = Mathf.Clamp(rotationX - mouseXSens * Input.GetAxisRaw("Mouse Y"), -90, 90);
        rotationY += mouseYSens * Input.GetAxis("Mouse X");
        
        inputMotion.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputMotion = (running && inputMotion.z > 0 ? runSpeed : walkSpeed) *
            Vector3.ClampMagnitude(inputMotion, 1);

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) velocityY = jumpForce; // only on 1st frame

        if (PlayerDataSingleton.Instance.HeldWeapon != PlayerDataSingleton.Weapon.NONE)
            PlayerDataSingleton.Instance.WeaponAnimator.SetFloat(forwardSpeedPercentAnimatorParameter, velocityXZ.z / runSpeed);
    }
    Vector3 speedSmoothVelocity;
    Vector3 totalVelocity;
    Vector3 velocityXZ;
    float velocityY = 0.0f;
    void FixedUpdate() {
        if (freeze) return;
        
        transform.rotation = ToQuaternion(rotationX, rotationY);

        velocityXZ = Vector3.SmoothDamp(
            velocityXZ,
            inputMotion,
            ref speedSmoothVelocity,
            controller.isGrounded ? speedSmoothTime : airSpeedSmoothTime
        );
        
        if (!controller.isGrounded) { 
            velocityY += Time.deltaTime * gravityY;
            if (velocityY < (jumping ? highJumpVelocity : lowJumpVelocity )) velocityY += fallAdditionalGravity;
        }

        totalVelocity = Vector3.up * velocityY + transform.TransformDirection(velocityXZ);

        controller.Move(Time.deltaTime * totalVelocity);
    }

    /// <summary>
    /// Custom Euler angles -> quaternion function to first evaluate Y rotation then X rotation
    /// <param name="xAngle">Angle in degrees to rotate around X-axis (evaluated second)</param>
    /// <param name="yAngle">Angle in degrees to rotate around Y-axis (evaluated first)</param>
    /// <returns></returns>
    Quaternion ToQuaternion(float xAngle, float yAngle) {
        xAngle = Mathf.Deg2Rad * xAngle / 2;
        yAngle = Mathf.Deg2Rad * yAngle / 2;
        float sinX = Mathf.Sin(xAngle), sinY = Mathf.Sin(yAngle),
            cosX = Mathf.Cos(xAngle), cosY = Mathf.Cos(yAngle);
        return new Quaternion(cosY * sinX, sinY * cosX, -sinY * sinX, cosY * cosX);
    }
}