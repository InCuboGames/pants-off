using UnityEngine;
using System.Collections;

public class MainMenuNPC : MonoBehaviour {

    private NavMeshAgent navAgent;
    [SerializeField] GameObject waypointRest;
    [SerializeField] GameObject waypointPath;
	[SerializeField] GameObject tutorialRest;
    [SerializeField] Animator anim;

    bool walkingAround;

    public bool WalkingAround
    {
        get
        {
            return walkingAround;
        }

        set
        {
            walkingAround = value;

            if(walkingAround)
            {
                randomizePath = true;
                wanderPath = false;
                UpdatePath(waypointPath);
                
                ExecuteAIState = Idle;
                return;
            }
            else
            {
                randomizePath = false;
                wanderPath = true;
                UpdatePath(Camera.main.GetComponent<MainMenuCameraController>().onTutorial? tutorialRest : waypointRest);
                
                ExecuteAIState = Idle;
                return;
            }
        }
    }

    private Transform[] aWaypoints;
    private int currentWaypoint;

    public bool randomizePath, isWaiting, wanderPath = false;
    float wanderWaitTime;

    delegate void AIState();
    AIState ExecuteAIState;

    private int randomRotWaypoint;


	// Use this for initialization
	void Start () {
	    
		navAgent = GetComponent<NavMeshAgent>();
        WalkingAround = false;

	}
	
	// Update is called once per frame
	void Update () 
    {
        #region Animator Variables
        
        anim.SetBool("input", navAgent.velocity.magnitude > 0.1f);
        
        #endregion

        ExecuteAIState();
	}

    public void UpdatePath(GameObject path)
    {
        navAgent.Stop();

        aWaypoints = path.GetComponentsInChildren<Transform>();

        if (randomizePath)
        {
            currentWaypoint = Random.Range(0, aWaypoints.Length);
        }
        else
        {
            currentWaypoint = 0;
        }

        navAgent.SetDestination(aWaypoints[currentWaypoint].position);
    }

    private void Idle()
    {   
        if (navAgent.stoppingDistance != 1) navAgent.stoppingDistance = 1;

        if (!isWaiting)
        {
            navAgent.SetDestination(aWaypoints[currentWaypoint].position);
            
            float wpDist = Vector3.Distance(transform.position, aWaypoints[currentWaypoint].position);
            if (wpDist <= navAgent.stoppingDistance)
            {
                if (wanderPath)
                {
                    if (!isWaiting)
                    {
                        isWaiting = true;
                    }
                }
                
                if (randomizePath)
                {
                    randomRotWaypoint = currentWaypoint;
                    currentWaypoint = Random.Range(0, aWaypoints.Length);
                }
                else
                {
                    currentWaypoint++;
                    if (currentWaypoint >= aWaypoints.Length)
                    {
                        currentWaypoint = 0;
                    }
                }
            }
        }
        else
        {
            if (wanderWaitTime >= 3)
            {
                isWaiting = false;
                wanderWaitTime = 0;
            }
            else
            {
                wanderWaitTime += Time.deltaTime;
                
                if (!randomizePath)
                {
                    var rotWaypoint = 0;
                    if (currentWaypoint <= 0)
                    {
                        rotWaypoint = aWaypoints.Length - 1;
                    }
                    else
                    {
                        rotWaypoint = currentWaypoint - 1;
                    }
                    
                    transform.rotation = Quaternion.Slerp(transform.rotation, aWaypoints[rotWaypoint].rotation, 0.05f);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, aWaypoints[randomRotWaypoint].rotation, 0.05f);
                }
            }
        } 
    }
}
