using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyStates{
        Patrol,
        Chase,
        Search
        
    }

    [SerializeField]
    private EnemyStates currentState;
    GameObject[] patrolPoints;
    Vector3 nextDestination;
    int currentDestinationIndex = 0;
    UnityEngine.AI.NavMeshAgent agent;
    [SerializeField]
    Transform enemyEyes;
    [SerializeField]
    GameObject player;
    float distanceToPlayer;


    [SerializeField]
    float fieldOfView = 45f;
    [SerializeField]
    float chaseDistance = 10;
    Transform player_pos;

    Transform currentPos;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Initialize();
    }

    void Initialize(){
        patrolPoints = GameObject.FindGameObjectsWithTag("patrolpoints");
        currentState = EnemyStates.Patrol;
        player = GameObject.FindGameObjectWithTag("Player");
        FindNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        switch(currentState){
            case EnemyStates.Patrol:
                UpdatePatrolState();
                break;
            case EnemyStates.Chase:
                UpdateChaseState();
                break;
            case EnemyStates.Search:
                UpdateSearchState();
                break;
        }
    }

    void UpdatePatrolState(){
        agent.stoppingDistance = 0;
        agent.speed = 3.5f;
         if(Vector3.Distance(transform.position, nextDestination) < 2){
            FindNextPoint();
        }
        if(distanceToPlayer <= chaseDistance && IsPlayerInClearFOV()){
            currentState = EnemyStates.Chase;
        }
        FaceTarget(nextDestination);

        agent.SetDestination(nextDestination);
    }

    void FindNextPoint(){
        nextDestination = patrolPoints[currentDestinationIndex].transform.position;

        currentDestinationIndex = (currentDestinationIndex + 1) % patrolPoints.Length;

        agent.SetDestination(nextDestination);        
    }

    bool IsPlayerInClearFOV(){
        Vector3 directionToPlayer = player.transform.position - enemyEyes.position;
        if(Vector3.Angle(directionToPlayer, enemyEyes.forward) <= fieldOfView){
            RaycastHit hit;
            if(Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, chaseDistance)){
                if(hit.collider.CompareTag("Player")){
                    return true;
                }
            }
        }
        return false;
    }

    void UpdateChaseState(){
        nextDestination = player.transform.position;
        if((distanceToPlayer > chaseDistance) || (!IsPlayerInClearFOV())){
            currentPos = gameObject.transform;
            currentState = EnemyStates.Search;
        }
        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
    }

    void UpdateSearchState(){
        if(IsPlayerInClearFOV()){
                currentState = EnemyStates.Chase; //if we spot the player, start chasing again
        } else {
            if(Vector3.Distance(transform.position, nextDestination) < 2){ //if we reach the location
                if(Scan()){
                    currentState = EnemyStates.Patrol;
                }
            }
        }
    }

    void FaceTarget(Vector3 target){
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        Quaternion lookDirection = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 10 * Time.deltaTime);
    }

    bool Scan(){
        Vector3 target = transform.eulerAngles + 180f * Vector3.up;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 10 * Time.deltaTime);
        if(transform.rotation == currentPos.rotation){ //definitely need to fix this at some point
            return true;
        }
        return false;
    }
}