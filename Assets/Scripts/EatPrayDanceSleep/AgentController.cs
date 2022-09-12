using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public List<GameObject> positionsL; // In the inspector, populate the list w/ positions of ascending order, e.g., pos42 before pos43.
    Dictionary<(int,int), GameObject> positionsD;
    TextMeshPro actionStatusText;
    public My_HMBDP_Agent hmbdpAgent;

    enum Phase { Planning, Execution, Updating }
    Phase phase;
    NavMeshAgent navMeshAgent;
    bool alreadyPlanning;
    bool alreadyExecuting;
    bool waitingToGetPath;

    void Start()
    {
        positionsD = new Dictionary<(int, int), GameObject>();
        int listIndex = 0;
        for(int x=1; x <= 5; x++)
            for(int y=1; y <= 5; y++)
            {
                positionsD.Add((x, y), positionsL[listIndex]);
                listIndex++;
            }

        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.stoppingDistance = 1.9f;

        hmbdpAgent = new My_HMBDP_Agent(1f, 0.8f, 10);
        
        phase = Phase.Planning;

        alreadyPlanning = false;
        alreadyExecuting = false;
        waitingToGetPath = false;

        actionStatusText = this.gameObject.GetComponentInChildren<TextMeshPro>();
        
        Time.timeScale = 3f;
    }


    void LateUpdate()
    {
        if(phase == Phase.Planning)
        {
            //Debug.Log("----------------------------------");
            //Debug.Log("Entered Planning Phse");
            //Debug.Log("CurrentState: " + hmbdpAgent.CurrentState.position);
            //Debug.Log("waitingToGetPath: " + waitingToGetPath);
            //Debug.Log("alreadyPlanning: " + alreadyPlanning);


            if (!waitingToGetPath && !alreadyPlanning)
            {
                alreadyPlanning = true;
                hmbdpAgent.CurrentAction = hmbdpAgent.SelectAction(hmbdpAgent.CurrentState);
                actionStatusText.text = hmbdpAgent.CurrentAction.ToString();
                Debug.Log("CurrentAction: " + hmbdpAgent.CurrentAction);

                if (hmbdpAgent.isNavigationAction(hmbdpAgent.CurrentAction))
                {
                    State nextState = Environment.GetRealNextState(hmbdpAgent.CurrentState, hmbdpAgent.CurrentAction);
                    int x = nextState.position.Item1;
                    int y = nextState.position.Item2;
                    navMeshAgent.SetDestination(positionsD[(x,y)].transform.position);
                    waitingToGetPath = true;  // computation of the path might take longer than one frame
                }
                
                alreadyPlanning = false;
            }

            if (hmbdpAgent.isNavigationAction(hmbdpAgent.CurrentAction))
            {
                if (navMeshAgent.hasPath)
                {
                    waitingToGetPath = false;
                    phase = Phase.Execution;
                    //Debug.Log("----------------------------------");
                    //Debug.Log("Entered Execution Phse");
                }
            }
            else
            {
                phase = Phase.Execution;
                //Debug.Log("----------------------------------");
                //Debug.Log("Entered Execution Phse");
            }
        }

        if (phase == Phase.Execution)
        {
            if (hmbdpAgent.isNavigationAction(hmbdpAgent.CurrentAction))
            {
                //Debug.Log("remainingDistance: " + navMeshAgent.remainingDistance);
                //Debug.Log("hasPath: " + navMeshAgent.hasPath);

                if (navMeshAgent.remainingDistance < Parameters.AT_TARGET_DISTANCE)
                {
                    navMeshAgent.ResetPath();
                    phase = Phase.Updating;
                }
            }
            else if(!alreadyExecuting)
            {
                switch (hmbdpAgent.CurrentAction)
                {
                    case Action.Eat:
                        if(hmbdpAgent.CurrentState.position == (1,1))
                            Debug.Log("Eating");
                        break;
                    case Action.Pray:
                        if (hmbdpAgent.CurrentState.position == (5, 5))
                            Debug.Log("Praying");
                        break;
                    case Action.Sleep:
                        if (hmbdpAgent.CurrentState.position == (3,3))
                            Debug.Log("Sleeping");
                        break;
                    case Action.Dance:
                        if (hmbdpAgent.CurrentState.position == (5,1))
                            Debug.Log("Dancing");
                        break;
                    case Action.No_Op:
                        Debug.Log("Doing nothing");
                        break;
                }
                alreadyExecuting = true;
                Invoke("ChangePhaseToUpdateAfterSeconds", 2f);                
            }
        }

        if (phase == Phase.Updating)
        {
            //Debug.Log("----------------------------------");
            //Debug.Log("Entered Updating Phse");

            //Debug.Log("CurrentState: " + hmbdpAgent.CurrentState.name + ", CurrentAction: " + hmbdpAgent.CurrentAction + ", nextState: " + nextState.name + ", Observation: " + obs);

            hmbdpAgent.UpdateDesires(hmbdpAgent.CurrentAction, hmbdpAgent.CurrentState);
            hmbdpAgent.MaintainSatisfactionLevels(hmbdpAgent.CurrentAction, hmbdpAgent.CurrentState);
            hmbdpAgent.Focus();

            hmbdpAgent.CurrentState = Environment.GetRealNextState(hmbdpAgent.CurrentState, hmbdpAgent.CurrentAction); ;
            phase = Phase.Planning;

            // Printouts for debugging and validation
            Debug.Log("------------------------------------");
            hmbdpAgent.PrintCurrentIntentions();
            hmbdpAgent.PrintDesireLevels();
            hmbdpAgent.PrintSatLevelsHistory();
        }
    }

    void ChangePhaseToUpdateAfterSeconds()
    {
        phase = Phase.Updating;
        alreadyExecuting = false;
    }
}


