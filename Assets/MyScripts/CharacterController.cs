using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float directionDampTime = 0.25f;
    [SerializeField]
    private ThirdPersonCamera gameCamera;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float rotationDegPerSec = 120.0f;

    private float speed = 0.0f;
    private float direction = 0.0f;
    private float horizontal = 0.0f;
    private float vertical = 0.0f;
    private AnimatorStateInfo stateInfo;

    // Hashes of Strings used in code
    private int m_LocomotionId = 0;


	void Start()
    {
        animator = GetComponent<Animator>();

        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1.0f);
        }

        // Hash all animations names for performance
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
	}

	void Update()
    {
        if(animator)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Pull values from controller/keyboard
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            // Translate controls stick coordinates into world/cam/character space
            StickToWorldspace(transform, gameCamera.transform, ref direction, ref speed);

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
        }
	}

    void FixedUpdate()
    {
        // Rotate character if stick is tilted right or left, but only if character is moving in that direction
        if(IsInLocomotion() && ((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0)))
        {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegPerSec * (horizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (transform.rotation * deltaRotation);
        }
    }

    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

        speedOut = stickDirection.sqrMagnitude;

        // Get camera rotation
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

        // Convert joystick input in Worldspace coordinates
        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

        angleRootToMove /= 180f;

        directionOut = angleRootToMove * directionSpeed;
    }

    public bool IsInLocomotion()
    {
        return stateInfo.nameHash == m_LocomotionId;
    }
}
