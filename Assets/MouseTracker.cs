using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        new_pos.z = 0;
        transform.position = new_pos;
    }
}
