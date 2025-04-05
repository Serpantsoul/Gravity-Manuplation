using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    public Transform hologram;
    [SerializeField] Rigidbody rb;
    public float gravityStrength = 9.81f;
    public Vector3 currentGravity;

    public Vector3 gravityDirection = Vector3.down;
    public Quaternion changeGravityto;
    public Vector3 rotationsInput;
    public Transform holobody;

    private void Start()
    {
        rb.useGravity = false;
    }
    private void Update()
    {
        if (UiManager.instance.gameover)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            hologram.gameObject.SetActive(!hologram.gameObject.activeInHierarchy);
        }
        if (hologram.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                hologram.rotation = Quaternion.LookRotation(-hologram.forward, hologram.up);
                changeGravityto = hologram.rotation;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                hologram.rotation = Quaternion.LookRotation(hologram.forward, hologram.up);
                changeGravityto = hologram.rotation;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                hologram.rotation = Quaternion.LookRotation(-hologram.right, hologram.up);
                changeGravityto = hologram.rotation;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                hologram.rotation = Quaternion.LookRotation(hologram.right, hologram.up);
                changeGravityto = hologram.rotation;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (hologram.gameObject.activeInHierarchy)
            {
              

                SetGravityDirection(-hologram.transform.forward);
                AlignWithGravity();

                // transform.DORotate(rotationsInput, .5f).SetDelay(.5f);
            }
        }
    }

    void FixedUpdate()
    {
        if (UiManager.instance.gameover)
            return;
        rb.AddForce(gravityDirection.normalized * gravityStrength, ForceMode.Acceleration);

        //   holobody.rotation = changeGravityto;
    }
    public void SetGravityDirection(Vector3 newDirection)
    {
        gravityDirection = newDirection.normalized;
        hologram.gameObject.SetActive(!hologram.gameObject.activeInHierarchy);
    }
    public Vector3 GetGravityDirection()
    {
        return gravityDirection.normalized;
    }
    public void AlignWithGravity()
    {

        Vector3 gravityDir = GetGravityDirection(); // assumed normalized

        // Step 1: Determine the forward direction projected onto gravity's plane
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, gravityDir).normalized;

        // Fallback in case forward becomes invalid
        if (projectedForward.sqrMagnitude < 0.001f)
            projectedForward = Vector3.ProjectOnPlane(transform.up, gravityDir).normalized;

        // Step 2: Create a clean upright rotation based on gravity and projected forward
        Quaternion targetRot = Quaternion.LookRotation(projectedForward, -gravityDir);

        // Step 3: Snap each axis to nearest 90 degrees to avoid diagonal tilts
        Vector3 finalEuler = targetRot.eulerAngles;
        finalEuler.x = Mathf.Round(finalEuler.x / 90f) * 90f;
        finalEuler.y = Mathf.Round(finalEuler.y / 90f) * 90f;
        finalEuler.z = Mathf.Round(finalEuler.z / 90f) * 90f;


        transform.DORotate(finalEuler, 0.5f).SetEase(Ease.OutCubic);
    }
}
