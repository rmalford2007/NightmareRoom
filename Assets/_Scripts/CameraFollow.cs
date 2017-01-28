using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float moveSpeed;
    private Vector3 startPosition;
	// Use this for initialization
	void Start () {
        //startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	    //Vector3.Lerp(transform.position, target.position, )
	}

    void FixedUpdate()
    {
        Vector3 camPos = transform.position;
        camPos.x = target.transform.position.x;
        camPos.z = target.transform.position.z;
        transform.position = camPos;
    }
}
