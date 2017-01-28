using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    private GameObject[] spawnPoints;
    public GameObject[] enemyObjects;
    public float respawnTime = 3.0f;
    public Transform playerPosition;
    public LayerMask respawnLayer;
    public LayerMask ignoreRaycastLayer;
    bool gameEnded = false;
    bool spawningEnemy = false;
    int maxEnemies = 5;
    // Use this for initialization
    void Start () {
        spawnPoints = new GameObject[0];
	}
	
	// Update is called once per frame
	void Update () {
	    if(spawnPoints.Length == 0)
        {
            CreateGrid gridScript = GetComponent<CreateGrid>();
            if (gridScript != null)
            {
                maxEnemies = gridScript.gridX * gridScript.gridY / 4; // can have 1/4 the number cells as enemies
                //maxEnemies = 1;
            }
            spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
            if(spawnPoints.Length > 0)
            {
                //Debug.Log("SpawnPoint count: " + spawnPoints.Length.ToString());
                //there are some spawn points
                StartCoroutine(WaitingToSpawn());
            }
        }
	}

    IEnumerator WaitingToSpawn()
    {
        while (!gameEnded)
        {
            
            GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (totalEnemies != null && totalEnemies.Length < maxEnemies)
            {
                if (spawningEnemy == false)//add wallcount here as well, if no walls, stop spawning
                {
                    spawningEnemy = true;
                    StartCoroutine(SpawnEnemy());
                }
            }
            yield return new WaitForSeconds(respawnTime);

        }
        yield return null;
    }

    IEnumerator SpawnEnemy()
    {
        
        bool foundPoint = false;
        //Debug.Log("Looking for a spawn point.");
        //game is running, find a location to spawn enemy
        while(!foundPoint) //add wallcount here as well, if no walls, stop looking
        {
            if (!gameEnded)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                if(playerPosition != null)
                {
                    
                    //HERE check if there is already an enemy in range?
                    //spawnPoints[randomIndex].layer = 0;
                    //Debug.Log(spawnPoints[randomIndex].layer.ToString() + " : Placing object in layer pre: " + respawnLayer.value.ToString());
                    //spawnPoints[randomIndex].layer = respawnLayer.value;
                    RaycastHit hit;
                    StartCoroutine(DrawDebugRay(playerPosition.position, spawnPoints[randomIndex].transform.position - playerPosition.position, Vector3.Distance(playerPosition.position, spawnPoints[randomIndex].transform.position)));
                    if (spawnPoints[randomIndex].GetComponent<SpawnPointOccupation>().isOccupied)
                    {
                        //Debug.Log("Occupied");
                        yield return null;
                        continue; //if spawn point has an enemy on it, jump back up to while loop and start over
                    }
                    if (Physics.Raycast(playerPosition.position, spawnPoints[randomIndex].transform.position - playerPosition.position, out hit, Vector3.Distance(playerPosition.position, spawnPoints[randomIndex].transform.position), ignoreRaycastLayer))
                    {
                        //Debug.Log("Hit collider name: " + hit.collider.name.ToString());
                        if (hit.collider.tag != "Respawn")
                        {
                            foundPoint = true;
                            Vector3 spawnPosition = spawnPoints[randomIndex].transform.position;
                            //Debug.Log("Hit collider name: " + hit.collider.name.ToString() + " Spawning Enemy at : " + spawnPosition.ToString());
                            if (enemyObjects != null && enemyObjects.Length > 0)
                            {
                                int spawnId = Random.Range(0, enemyObjects.Length);
                                Instantiate(enemyObjects[spawnId], spawnPosition, Quaternion.identity);
                                GameManager.theManager.HintGoalChanges();
                                //Debug.Log("Spawned enemy");
                            }

                        }
                    }
                    else
                    {
                        yield return null;
                    }
                    //Debug.Log("Placing object in layer post: " + ignoreRaycastLayer.value.ToString());
                    //spawnPoints[randomIndex].layer = ignoreRaycastLayer.value;
                }
            }
            else
                break;
            yield return null;
        }
        spawningEnemy = false;
        yield return null;
    }
    
    IEnumerator DrawDebugRay(Vector3 origin, Vector3 direction, float dist)
    {
        direction.Normalize();
        direction *= dist;
        float elapsedTime = 0.0f;
        while (elapsedTime < 2.0f)
        {
            elapsedTime += Time.deltaTime;
            Debug.DrawRay(origin, direction, Color.red);
            yield return null;
        }
        yield return null;
    }
}
