using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    //AgentController agentController;
    //Test_NMRDP_Agent agent;
    My_HMBDP_Agent agent;

    
    void Start()
    {
        GameObject agentGO = GameObject.FindGameObjectWithTag("agent");
        agent = agentGO.GetComponent<AgentController>().hmbdpAgent;
    }


    //public static State GetNextState(State currentState, Action action)
    //{
    //    if (currentState.number == 0 && action == Action.Goal1)
    //        foreach (State s in Test_NMRDP_Agent.States)
    //            if (s.number == 1)
    //                return s;
    //    if (currentState.number == 1 && action == Action.Goal2)
    //        foreach (State s in Test_NMRDP_Agent.States)
    //            if (s.number == 2)
    //                return s;

    //    if (currentState.number == 2)
    //        return currentState;

    //    if(action == Action.No_Op)
    //        return currentState;

    //    return null;
    //}

    public static State GetNextState(State currentState, Action action)
    {
        if (action == Action.Up)
        {
            if (currentState.position.Item2 == 5)
                return currentState;
            else
            {
                foreach(State s in Agent.States)
                    if (s.position.Item1 == currentState.position.Item1 && s.position.Item2 == currentState.position.Item2 + 1)
                        return s;
            }
        }

        if (action == Action.Down)
        {
            if (currentState.position.Item2 == 1)
                return currentState;
            else
            {
                foreach (State s in Agent.States) 
                    if (s.position.Item1 == currentState.position.Item1 && s.position.Item2 == currentState.position.Item2 - 1)
                        return s;
            }
        }

        if (action == Action.Left)
        {
            if (currentState.position.Item1 == 1)
                return currentState;
            else
            {
                foreach (State s in Agent.States) 
                    if (s.position.Item1 == currentState.position.Item1 - 1 && s.position.Item2 == currentState.position.Item2)
                        return s;
            }
        }

        if (action == Action.Right)
        {
            if (currentState.position.Item1 == 5)
                return currentState;
            else
            {
                foreach (State s in Agent.States) 
                    if (s.position.Item1 == currentState.position.Item1 + 1 && s.position.Item2 == currentState.position.Item2)
                        return s;
            }
        }

        if (action == Action.Eat || action == Action.Dance || action == Action.No_Op || action == Action.Sleep || action == Action.Pray)
            return currentState;

        return currentState; 
    }
}
