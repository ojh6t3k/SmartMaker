using UnityEngine;
using System.Collections;

public class GyroInput : MonoBehaviour
{
    [Range(0f, 1f)]
    public float lowPassFilterFactor = 0.5f;

    // Use this for initialization
    void Start ()
    {
        Input.gyro.enabled = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Inverse(Input.gyro.attitude), lowPassFilterFactor);
	}
}
