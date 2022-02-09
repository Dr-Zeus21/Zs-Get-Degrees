using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMAterial : MonoBehaviour
{
    public float spinspeed;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, spinspeed, 0);
    }
}
