using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform playerPos;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
    }

    // LateUpdate is called after all Update methods
    void LateUpdate()
    {
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
    }
}
