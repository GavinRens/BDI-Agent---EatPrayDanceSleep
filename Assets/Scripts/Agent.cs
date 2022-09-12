using System.Collections.Generic;


public abstract class Agent
{
    State currentState;
    Action currentAction;
    HashSet<Action> actions;
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

    public HashSet<Action> Actions { get { return actions; } set { actions = value; } }

    public static List<Observation> Observations { get { return observations; } }

    public static List<State> States { get { return states; } }

    /// <summary>
    /// The state the agent expects to end up in if it executes the action in the current state
    /// </summary>
    /// <param name="action">The action executed</param>
    /// <param name="state">The state in which the action was executed</param>
    /// <returns>A successor state</returns>
    public abstract State GetNextState(Action action, State state);

    /// <summary>
    /// Generates the states that the agent can be in
    /// </summary>
    /// <returns>A list of agent states</returns>
    public abstract List<State> GenerateStates();

    /// <summary>
    /// Sets the state that the agent will start in
    /// </summary>
    public abstract void InitializeAgentState();

    /// <summary>
    /// Defines when the agent will stop being active, i.e., defines the end of an episode. Should always return false if the agent does not stop.
    /// </summary>
    /// <param name="state">An 'end' state; where the agent stops or the episode ends</param>
    /// <returns>true or false</returns>
    public abstract bool HasFinished(State state);
}
