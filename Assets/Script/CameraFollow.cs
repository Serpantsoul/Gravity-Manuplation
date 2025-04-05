using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0f, 2f, -6f);
    public float followSpeed = 8f;
    public float rotationSpeed = 6f;
    public LayerMask obstructionMask;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (!target) return;

        // Desired camera position based on local space of player
        Vector3 desiredPosition = target.TransformPoint(localOffset);

        // Raycast from player to desired camera position
        Vector3 directionToCamera = desiredPosition - target.position;
        float distance = directionToCamera.magnitude;
        Ray ray = new Ray(target.position, directionToCamera.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, distance, obstructionMask))
        {
            // If something is in the way, move the camera to the hit point (just in front of the object)
            desiredPosition = hit.point - directionToCamera.normalized * 0.2f; // pull it slightly forward
        }

        // Smooth position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1 / followSpeed);

        // Rotate to look at the player using their up vector
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
