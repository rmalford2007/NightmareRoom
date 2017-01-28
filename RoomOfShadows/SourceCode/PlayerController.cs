using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 2.0f;
    public float attackSpeed = 1.0f; //times per second?
    public float attackRange = 1.0f; //length of weapon
    public Collider attackCollider;
    public Animator anim;
    public GameObject rebuildNode;
    private Vector3 moveDirection;
    private Rigidbody rb;
    int attackCount = 0;
    bool isAttacking = false;
    bool canAttack = true;
    public LayerMask attackableLayers;
    public Stats playerStats;
    public bool playerDead = false;
    bool isPaused = false;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.zero;
        if(anim == null)
            anim = GetComponent<Animator>();
        playerStats = GetComponent<Stats>();
    }
	
	// Update is called once per frame
	void Update () {
        moveDirection = Vector3.zero;
        if (!playerDead)
        {
            if (!isPaused)
            {
                DoKeyboardMove();
                DoMouseLook();
                DoAttack();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DoPause();
        }
        
    }

    void DoPause()
    {
        if(!isPaused)
        {
            isPaused = true;
            GameManager.theManager.SetPause(isPaused);
            //pause the game, open UI
        }
        else
        {
            isPaused = false;
            GameManager.theManager.SetPause(isPaused);
        }
    }

    void DoKeyboardMove()
    {
        
        if (Input.GetKey(KeyCode.W))
        {
            //move up - z axis
            moveDirection += Vector3.forward * moveSpeed;
            //rb.Add
            //rb.AddForce(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //move down - z axis
            moveDirection += -Vector3.forward * moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            //move left - x axis
            moveDirection += Vector3.left * moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            //move right - x axis
            moveDirection += Vector3.right * moveSpeed;
        }
        if (moveDirection != Vector3.zero)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);
    }

    public void TakeDamage(float damageAmount)
    {
        if(playerStats != null && !playerDead)
        {
            //send damage to our health stat
            playerStats.ApplyDamage(damageAmount);
            //Debug.Log("Took damage: " + damageAmount.ToString());
            GameManager.theManager.ResetChainMultiplier(); //Getting hit resets multiplier
            CheckHealth();
        }
    }

    void CheckHealth()
    {
        if (playerStats != null)
        {
            //check if we are dead
            if(playerStats.health.current <= 0.0f)
            {
                playerDead = true;
                OnDeath();
            }
        }
    }

    void OnDeath()
    {
        //call game over
        GameObject theEnvironment = GameObject.Find("Environment");
        if(theEnvironment != null)
        {
            EnemySpawner theSpawner = theEnvironment.GetComponent<EnemySpawner>();
            if(theSpawner != null)
            {
                Destroy(theSpawner);
            }
        }
        rb.freezeRotation = true;
        //Do death animation
        if(anim != null)
        {
            anim.Play("Death", -1, 0.0f);
        }
        GameManager.theManager.GameOver();
    }

    void DoMouseLook()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.y = transform.position.y;
        transform.LookAt(mousePos);
    }

    void DoAttack()
    {
        isAttacking = Input.GetMouseButton(0);

        Vector3 forward = transform.TransformDirection(Vector3.forward) * attackRange;
        Debug.DrawRay(transform.position, forward, Color.green);

        if (isAttacking && canAttack)
        {
            
            attackCount++;
            //Debug.Log("attack count: " + attackCount.ToString());
            if(anim != null)
            {
                anim.Play("Attack", -1, 0f);
            }
            canAttack = false;
            StartCoroutine(DoDelayedAttack());
        }
    }

    IEnumerator DoDelayedAttack()
    {
        yield return new WaitForSeconds(.25f);
        //do attack stuff
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, attackRange, attackableLayers))
        {
            //print("There is something in front of the object!");
            WallAttributes wallScript = hit.collider.GetComponent<WallAttributes>();
            if (wallScript != null)
            {
                if (wallScript.isBreakable)
                {
                    //In place of destroyed wall, put a transform here, for enemies to rebuild on
                    if (rebuildNode != null && hit.collider.tag != "Enemy")
                    {
                        //spawn node at hit colliders transform
                        
                        GameObject holderNode = Instantiate(rebuildNode);
                        if (holderNode != null)
                        {
                            holderNode.transform.position = hit.collider.transform.position;
                            holderNode.transform.parent = hit.collider.transform.parent;
                            hit.collider.transform.parent = holderNode.transform;
                        }

                        hit.collider.gameObject.SetActive(false);
                    }
                    else
                        Destroy(hit.collider.gameObject);
                    GameManager.theManager.IncrementChainMultiplier(); //destroying stuff increases your score multiplier
                    GameManager.theManager.HintGoalChanges();
                }
            }
        }
        StartCoroutine(ResetAttack());
        yield return null;
    }
    
    IEnumerator ResetAttack()
    {
        float waitTime = 1.0f / attackSpeed;
        waitTime -= .25f; //subtract the time it took to start the animation
        yield return new WaitForSeconds(waitTime);
        canAttack = true;
        yield return null;
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
            rb.velocity = moveDirection;
    }
}
