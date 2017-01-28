using UnityEngine;
using System.Collections;

public class CreateGrid : MonoBehaviour {
    //Create a grid, houses all walls and maybe shadow spots
    public int lowerGridX;
    public int upperGridX;
    public int lowerGridY;
    public int upperGridY;
    public int gridX;
    public int gridY;
    private int[,] theGrid;
    public int roomSizeMin;
    public int roomSizeMax;
    public GameObject innerWall_V;
    public GameObject innerWall_H;
    public GameObject innerWall_C;
    public GameObject enemySpawnObject;
    public Material outerWallMat;
    public int innerWallCount = 0;

    public GameObject floor;
    public float wallLength;
    bool bGenWalls = true;

	// Use this for initialization
	void Start () {
        InitGrid();
	}
	
	// Update is called once per frame
	void Update () {
	    if(bGenWalls)
        {
            bGenWalls = false;
            GenWalls();
            PlaceInnerWalls();
            //PlaceOuterWalls();
            PlaceFloor();
            GameManager.theManager.SetWallGoal(innerWallCount);
        }
	}

    void InitGrid()
    {
        gridX = Random.Range(lowerGridX, upperGridX+1);
        gridY = Random.Range(lowerGridY, upperGridY + 1);

        if(roomSizeMax+4 > gridX)
        {
            roomSizeMax = gridX - 4;
        }

        if(roomSizeMax+4 > gridY)
        {
            roomSizeMax = gridY - 4;
        }

        if (roomSizeMin >= roomSizeMax)
            roomSizeMin = roomSizeMax - 1;
        //Initialize the grid based on gridSize
        theGrid = new int[gridX, gridY]; //how to make 2d array? dunno haha
    }

    void GenWalls()
    {
        int randomWidth = Random.Range(roomSizeMin, roomSizeMax + 1);
        int randomHeight = Random.Range(roomSizeMin, roomSizeMax + 1);
        //choose a random room size between min and max for each width and height for the next room
        bool lastEdge = false;
        for (int y = 0; y < gridY; y += randomHeight+1)
        {
            if (y != gridY - 2)
            {
                for (int i = 0; i < gridX; i++)
                {
                    theGrid[i, y] = 1; //1 is horizontal
                }
            }
            if (y == gridY - 1)
                lastEdge = true;
            randomHeight = Random.Range(roomSizeMin, roomSizeMax);
        }
        if(!lastEdge)
        {
            //missing last boundary edge
            int y = gridY - 1;
            for (int i = 0; i < gridX; i++)
            {
                theGrid[i, y] = 1; //1 is horizontal
            }
        }

        lastEdge = false;
        for (int y = 0; y < gridX; y += randomWidth+1)
        {
            if (y != gridX - 2)
            {
                for (int i = 0; i < gridY; i++)
                {
                    if (theGrid[y, i] == 1)
                    {
                        theGrid[y, i] = 3; //3 is cross
                        //Debug.Log("SetCross");
                    }
                    else
                        theGrid[y, i] = 2; //2 is vertical
                }
            }
            if (y == gridX - 1)
                lastEdge = true;
            randomWidth = Random.Range(roomSizeMin, roomSizeMax);
        }
        if (!lastEdge)
        {
            //missing last boundary edge
            int y = gridX - 1;
            for (int i = 0; i < gridY; i++)
            {
                if (theGrid[y, i] == 1)
                {
                    theGrid[y, i] = 3; //3 is cross
                    //Debug.Log("SetCross");
                }
                else
                    theGrid[y, i] = 2; //2 is vertical
            }
        }

    }

    void PlaceInnerWalls()
    {
        
        for(int i = 0; i < gridX; i++)
        {
            for(int m = 0; m < gridY; m++)
            {
                Vector3 spawnPos = new Vector3(i * wallLength, 0.0f, m * wallLength);
                spawnPos += transform.position + new Vector3(wallLength, 0.0f, wallLength);

                GameObject holder = null;

                if (theGrid[i,m] != 0)
                {
                    //place a wall
                    
                    if(theGrid[i, m] == 1)
                        holder = Instantiate(innerWall_H);
                    if (theGrid[i, m] == 2)
                        holder = Instantiate(innerWall_V);
                    if (theGrid[i, m] == 3)
                    {
                        //Debug.Log("Place Cross");
                        holder = Instantiate(innerWall_C);
                    }
                    if(holder != null)
                    {
                        holder.transform.parent = transform;
                        holder.transform.position = spawnPos;
                        innerWallCount++;
                        if(i == 0 || i == gridX-1 || m==0 || m== gridY-1)
                        {
                            //edge wall, non destrutable
                            WallAttributes holderScript = holder.GetComponent<WallAttributes>();
                            if(holderScript != null)
                            {
                                holderScript.isBreakable = false;
                                if (theGrid[i, m] != 3)
                                {
                                    MeshRenderer my_renderer = holder.GetComponent<MeshRenderer>();
                                    if (outerWallMat != null && my_renderer != null)
                                    {
                                        my_renderer.material = outerWallMat;
                                    }
                                }
                                else
                                {
                                    MeshRenderer my_renderer1 = holder.transform.GetChild(0).GetComponent<MeshRenderer>();
                                    if (outerWallMat != null && my_renderer1 != null)
                                    {
                                        my_renderer1.material = outerWallMat;
                                    }

                                    MeshRenderer my_renderer2 = holder.transform.GetChild(1).GetComponent<MeshRenderer>();
                                    if (outerWallMat != null && my_renderer2 != null)
                                    {
                                        my_renderer2.material = outerWallMat;
                                    }
                                }
                                innerWallCount--;
                            }
                        }
                    }

                    
                }
                else
                {
                    //place enemy spawner here

                    holder = Instantiate(enemySpawnObject);
                    holder.transform.parent = transform;
                    holder.transform.position = spawnPos;
                }
            }
        }
        
    }

    void PlaceFloor()
    {
        GameObject floorPlane = Instantiate(floor);
        floorPlane.transform.parent = transform;
        Vector3 pos = Vector3.zero;
        floorPlane.transform.position = pos;
        Vector3 resize = Vector3.one;
        if(gridX > gridY)
        {
            resize.x = gridX / 10 * wallLength;
            resize.z = gridX / 10 * wallLength;
        }
        floorPlane.transform.localScale = resize ;
    }
}
