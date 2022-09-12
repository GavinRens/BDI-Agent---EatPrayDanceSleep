using UnityEngine;


public class Environment : MonoBehaviour
{  
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
