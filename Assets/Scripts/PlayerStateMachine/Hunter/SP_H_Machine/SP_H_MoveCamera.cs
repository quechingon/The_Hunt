using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_H_MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
