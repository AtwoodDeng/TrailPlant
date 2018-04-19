using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] float smoothTime = 2f;

	// Use this for initialization
	void Start () {
		
	}

    Vector3 velocity;

	// Update is called once per frame
	void Update () {

        Vector3 toward = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        toward.z = -5f;
        transform.position = toward;
	}
}
