using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool Invert;
    private Transform cameraTransform;


    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (Invert)
        {
            Quaternion DirToCamera = (cameraTransform.rotation);
            transform.rotation = Quaternion.Inverse(DirToCamera);
        }
        else
        {
            Quaternion DirToCamera = (cameraTransform.rotation);
            transform.rotation = DirToCamera;
        }

    }

    private void GetDirection()
    {

    }
}
