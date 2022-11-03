using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{

    Camera faceCamera;

    void Start()
    {
        faceCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = faceCamera.transform.forward;
    }
}
