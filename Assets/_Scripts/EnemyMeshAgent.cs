using UnityEngine;
using System.Collections;
//using UnityEngine.AI;

public class EnemyMeshAgent : MonoBehaviour {
    UnityEngine.AI.NavMeshAgent myAgent;
    public bool isBuilder = false;
    private GameObject target;
    public float interactRange = .4f;
    public float reloadTime = 4.0f; //reload time on interaction, attacking or building
    public float attackDamage = 5.0f; 
    private bool interactReady = true;
    private bool inRange = false;
    bool gettingTarget = false;
    UnityEngine.AI.NavMeshPath nextPath;
    // Use this for initialization
    void Start () {
        myAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        target = null;
    }

    void Awake()
    {
        StartCoroutine(PollInRange());
        nextPath = new UnityEngine.AI.NavMeshPath();
    }
	
	// Update is called once per frame
	void Update () {
        if (target == null && !gettingTarget)
        {
            StartCoroutine(AcquireTarget());
        }
        if (target != null)
        {
            
            if (myAgent.CalculatePath(target.transform.position, nextPath))
            {
               
                if (nextPath.status == UnityEngine.AI.NavMeshPathStatus.PathPartial)
                {
                    target = null;
                    //Debug.Log("Partially blocked - Not reachable");
                }
                else
                    myAgent.SetPath(nextPath);
            }
            else
            {
                target = null;
                //Debug.Log("Not reachable");
            }
        }

        if(inRange)
        {
            if(isBuilder)
            {
                //do rebuild
                RebuildNode();
            }

            if(!isBuilder)
            {
                //do attack
                AttackTarget();
                //Debug.Log("In attack range of player");
            }
        }
        
    }

    IEnumerator ReloadInteraction()
    {
        yield return new WaitForSeconds(reloadTime);
        interactReady = true;
        yield return null;
    }

    void AttackTarget()
    {
        if(interactReady)
        {
            interactReady = false;

            PlayerController thePlayer = target.GetComponent<PlayerController>();
            if(thePlayer != null)
            {
                thePlayer.TakeDamage(attackDamage);
                if (thePlayer.playerDead)
                {
                    myAgent.Stop();
                    target = null;
                }
            }
            //ATTACK stuff
            StartCoroutine(ReloadInteraction());
        }
    }

    void RebuildNode()
    {
        if (interactReady)
        {
            interactReady = false;

            if (target.GetComponent<SpawnPointOccupation>().isOccupied)
            {
                target = null;
                interactReady = true;
                inRange = false;
            }
            else
            {
                Transform childTransform = target.transform.GetChild(0);
                childTransform.parent = target.transform.parent;
                childTransform.gameObject.SetActive(true);
                Destroy(target);
                GameManager.theManager.HintGoalChanges();
                target = null;
                inRange = false;
                StartCoroutine(ReloadInteraction());
            }
        }
    }

    IEnumerator AcquireTarget()
    {
        gettingTarget = true;

        while (gettingTarget)
        {

            if (!isBuilder)
            {
                target = GameObject.FindGameObjectWithTag("Player");
                if(target != null)
                {
                    PlayerController thePlayer = target.GetComponent<PlayerController>();
                    if(thePlayer != null)
                    {
                        if(thePlayer.playerDead)
                        {
                            myAgent.Stop();
                            target = null;
                        }
                    }
                    else
                        target = null; //target something besides player? no thanks, no target
                }
            }
            else
            {
                GameObject[] rebuildableWalls = GameObject.FindGameObjectsWithTag("Rebuild");
                if (rebuildableWalls != null && rebuildableWalls.Length > 0)
                {
                    int rebuildIndex = Random.Range(0, rebuildableWalls.Length);
                    target = rebuildableWalls[rebuildIndex];
                }
            }
            if(target != null)
            {
                gettingTarget = false;
                break;
            }
            yield return new WaitForSeconds(0.5f); //attempt to find a target every second
        }
        yield return null;
    }

    IEnumerator PollInRange()
    {
        //while I'm active, poll my targets range
        while (gameObject.activeSelf)
        {
            //if in range do something
            if (target != null)
            {
                //check distance
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist <= interactRange)
                {
                    inRange = true;

                }
                else
                {
                    inRange = false;
                }
            }
            else
                inRange = false;

            yield return new WaitForSeconds(0.5f); //every half second check range
        }
        yield return null;
    }

}
