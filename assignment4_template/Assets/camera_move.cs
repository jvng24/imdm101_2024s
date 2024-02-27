// This code update Camera position
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_move : MonoBehaviour
{
    //This is Main Camera in the Scene
    Camera m_MainCamera;
    //This is the second Camera and is assigned in inspector
    public float speed = 5.0f;
    public float sensitivity = 5.0f;
    public float test = 0f;
    void Start()
    {
        //This gets the Main Camera from the Scene
        m_MainCamera = Camera.main;
        //This enables Main Camera
        m_MainCamera.enabled = true;
    }

    void Update()
    {
        // Move the camera forward, backward, left, and right
        m_MainCamera.transform.position += m_MainCamera.transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        m_MainCamera.transform.position += m_MainCamera.transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Debug.Log(m_MainCamera.transform.position);
        // Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        m_MainCamera.transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
    }
}
