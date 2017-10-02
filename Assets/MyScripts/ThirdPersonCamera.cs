using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Transform followPlayer;
    [SerializeField]
    private Vector3 offset = new Vector3(0.0f, 1.5f, 0.0f);
    [SerializeField]
    private float camSmoothDampTime = 0.1f;

    private Vector3 lookDir;
    private Vector3 targetPosition;
    private Vector3 velocityCamSmooth = Vector3.zero;


    void Start()
    {
        followPlayer = GameObject.FindWithTag("FollowPlayerPosition").transform;
	}

	void LateUpdate()
    {
        Vector3 characterOffset = followPlayer.position + offset;

        // Calculate direction from camera to player and normalize to give a valid direction with unit magnitude
        lookDir = characterOffset - transform.position;
        lookDir.y = 0.0f;
        lookDir.Normalize();
        Debug.DrawRay(transform.position, lookDir, Color.green);

        // Setting the target possition to be the correct offset from the player
        targetPosition = characterOffset + followPlayer.up * distanceUp - lookDir * distanceAway;

        //Debug.DrawRay(follow.position, follow.up * distanceUp, Color.red);
        //Debug.DrawRay(follow.position, -follow.forward * distanceAway, Color.blue);
        Debug.DrawLine(followPlayer.position, targetPosition, Color.green);

        SmoothPosition(transform.position, targetPosition);

        // Make sure the camera is looking the right way
        transform.LookAt(followPlayer);
	}

    private void SmoothPosition(Vector3 fromPosition, Vector3 toPosition)
    {
        // Make a smooth transition between camera's current position and the position it wants to be in
        transform.position = Vector3.SmoothDamp(fromPosition, toPosition, ref velocityCamSmooth, camSmoothDampTime);
    }
}
