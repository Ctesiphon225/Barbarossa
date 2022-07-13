using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int mDelta = 10; // Pixels. The width border at the edge in which the movement work
    public float mSpeed = 3.0f; // Scale. Speed of the movement
    
    public float xSensitivity = 5.0f;
    public float ySensitivity = 5.0f;
    public float zSensitivity = 3.0f;

    private Vector3 mForwardDirection; // What direction does our camera start looking at
    private Vector3 mRightDirection; // The inital "right" of the camera
    private Vector3 mUpDirection; // The inital "up" of the camera

    private bool fixCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        mForwardDirection = transform.forward;
        mRightDirection = transform.right;
        mUpDirection = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            fixCamera = !fixCamera;
        }

        if (!fixCamera) {
            if (Input.mousePosition.x >= Screen.width - mDelta)
            {
                transform.position += mRightDirection * Time.deltaTime * mSpeed;
            }  
            if (Input.mousePosition.x <= 0 + mDelta)
            {
                transform.position -= mRightDirection * Time.deltaTime * mSpeed;
            }                
            if (Input.mousePosition.y >= Screen.height - mDelta)
            {
                transform.position += mUpDirection * Time.deltaTime * mSpeed;
            } 
            if (Input.mousePosition.y <= 0 + mDelta)
            {
                transform.position -= mUpDirection * Time.deltaTime * mSpeed;
            }
        }
        

        // camera zoom in/out
        float y = Input.mouseScrollDelta.y;
        if (y >= 1.0f && Camera.main.orthographicSize >= 1.0f + zSensitivity) {
            Camera.main.orthographicSize -= 1.0f * zSensitivity;
        } 
        if (y <= -1.0f && Camera.main.orthographicSize <= 10.0f) {
            Camera.main.orthographicSize += 1.0f * zSensitivity;
        }
    }
}
