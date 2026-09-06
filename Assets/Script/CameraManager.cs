using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    float width;
    float height;
    //Vector3 camPos;
    // Start is called before the first frame update
    void Start()
    {
        width = 5 * 0.4865f;
        height = 5;
        //camPos = Camera.main.transform.position;
        //Camera.main.transform.position = new Vector3(camPos.x, 0, camPos.z);
    }
    private void FixedUpdate()
    {
        //Camera.main.orthographicSize = width / Camera.main.aspect;
    }
}
