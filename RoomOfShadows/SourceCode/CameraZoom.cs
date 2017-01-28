using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

    public Vector2 minMaxZoom;
    public float zoomSpeed = 4.0f;
    float orthoSize;
    // Use this for initialization
    void Start () {
        
    }
	
    void Awake()
    {
        orthoSize = Camera.main.orthographicSize;
    }

	// Update is called once per frame
	void Update () {
	    if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //backward - zoom out
            orthoSize = Camera.main.orthographicSize;
            orthoSize += zoomSpeed * Time.deltaTime;
            orthoSize = Mathf.Clamp(orthoSize, minMaxZoom.x, minMaxZoom.y);
            
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //forward - zoom in
            orthoSize = Camera.main.orthographicSize;
            orthoSize -= zoomSpeed * Time.deltaTime;
            orthoSize = Mathf.Clamp(orthoSize, minMaxZoom.x, minMaxZoom.y);
        }
	}

    void FixedUpdate()
    {
        Camera.main.orthographicSize = orthoSize;
    }
}
