using System.Collections;
using System.Collections.Generic;

public abstract class Agent
{
    State currentState;
    Action currentAction;
    static HashSet<Action> actions;
    static List<Observation> observations;
    static protected List<State> states; 
    
    public Agent()
    {
        currentState = null;  // must be initialized in the agent controller using this.InitializeAgentState()
        currentAction = Action.No_Op;

        actions = new HashSet<Action>();
        foreach (Action a in System.Enum.GetValues(typeof(Action)))
            actions.Add(a);

        observations = new List<Observation>();
        foreach (Observation o in System.Enum.GetValues(typeof(Observation)))
            observations.Add(o);

        states = GenerateStates();

        InitializeAgentState();
    }
    
    public State CurrentState { get { return currentState; } set { currentState = value; } }

    public Action CurrentAction { get { return currentAction; } set { currentAction = value; } }

    public static HashSet<Action> Actions { get { return actions; } }

    public static List<Observation> Observations { get { return observations; } }

    public static List<State> States { get { return states; } }

    public abstract State GetNextState(Action action, State state);

    public abstract List<State> GenerateStates();

    public abstract void InitializeAgentState();

    public abstract bool HasFinished(State state);
}
