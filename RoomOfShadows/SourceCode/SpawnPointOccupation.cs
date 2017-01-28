using UnityEngine;
using System.Collections;

public class SpawnPointOccupation : MonoBehaviour {
    
    public bool isOccupied = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        isOccupied = true;
    }

    void OnTriggerExit(Collider other)
    {
        isOccupied = false;
    }

    void OnTriggerStay(Collider other)
    {
        isOccupied = true;
    }
}
