using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpriteFollower : MonoBehaviour
{
    public GameObject target;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
    }
}
