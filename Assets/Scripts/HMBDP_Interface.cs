using System;
using System.Collections.Generic;

public interface HMBDP_Interface
{
    public static List<Goal> Goals
    {
        get;
    }
    public Dictionary<Goal, float> GoalWeight
    {
        get;
    }
    public Dictionary<Goal, HashSet<Goal>> NonCompatibleGoals
    {
        get;
    }
    public HashSet<Goal> Intentions
    {
        get;
    }
    public Dictionary<Goal, float> DesireLevel
    {
        get;
    }
    public Dictionary<Goal, Queue<float>> SatisfactionLevels
    {
        get;
    }
    public float IntentionSimilarityThreshold
    {
        get;
    }
    public float SatisfactionThreshold
    {
        get;
    }
    public int MemoryCapacity
    {
        get;
    }

    /// <summary>
    /// Specify what the goals are
    /// </summary>
    void DefineGoals();

    /// <summary>
    /// Specify the importance / weight of each goal
    /// </summary>
    void SpecifyGoalWeights();

    /// <summary>
    /// Specify goal (in)compatabilities
    /// </summary>
    void SpecifyGoalNoncompatabilities();

    /// <summary>
    /// Initialize intentions
    /// </summary>
    /// <param name="intentions">The initial intentions</param>
    void InitializeIntentions(HashSet<Goal> intentions);

    /// <summary>
    /// The function that defines the agent's preference for particular actions in particular states,
    /// where the actions are not related directly to goals or intentions
    /// </summary>
    /// <param name="a">An action</param>
    /// <param name="s">An environment state</param>
    /// <returns>A value between 0 and 1</returns>
    float Preference(Action a, State s);

    /// <summary>
    /// The function that defines the cost of particular actions in particular states
    /// </summary>
    /// <param name="a">An action</param>
    /// <param name="s">An environment state</param>
    /// <returns>A value between 0 and 1</returns>
    float Cost(Action a, State s);

    /// <summary>
    /// Defines the agent's satisfaction for performing an action in a state in order to achieve goal.
    /// Satisfaction of a particular goal might be action dependent.
    /// </summary>
    /// <param name="g">A goal to pursue</param>
    /// <param name="a">An action</param>
    /// <param name="s">An environment state</param>
    /// <returns>A value between 0 and 1, representing how satisfied the agent is w.r.t. g</returns>
    float Satisfaction(Goal g, Action a, State s);

    /// <summary>
    /// Returns a plan, given a set of intentions and the current state
    /// </summary>
    /// <param name="intentions">A set of intentions (goals)</param>
    /// <param name="s">An environment state</param>
    /// <returns>A plan, which is a pair: the first action of the plan and the rest of the plan</returns>
    (Action, ValueTuple) GetPlan(HashSet<Goal> intentions, State s);

    /// <summary>
    /// The state transition function; 
    /// </summary>
    /// <param name="from">The originating state</param>
    /// <param name="action">An action</param>
    /// <param name="to">The successor state</param>
    /// <returns>The probability that an action performed in state "from" will end up in state "to"</returns>
    float TransitionFunction(State from, Action action, State to);

    /// <summary>
    /// Specifies, for every action, whether an actions implies that the agent must move its position (i.e. navigate)
    /// </summary>
    /// <param name="a">An action</param>
    /// <returns>"true" iff "a" is a navigation action</returns>
    bool isNavigationAction(Action a);

    /// <summary>
    /// The desire update rule that updates the desire level of any goal
    /// </summary>
    /// <param name="a">An action</param>
    /// <param name="s">An environment state</param>
    void UpdateDesires(Action a, State s);

    /// <summary>
    /// Defines whether the given goal is currently an intention
    /// </summary>
    /// <param name="g">A goal</param>
    /// <returns>1 if it is, else 0</returns>
    int IsIntention(Goal g);

    /// <summary>
    /// Keeps a record - with MemoryCapacity many entries per goal - of the satisfaction levels for each goal
    /// </summary>
    /// <param name="a">An action</param>
    /// <param name="s">An environment state</param>
    void MaintainSatisfactionLevels(Action a, State s);

    /// <summary>
    /// The function that refocuses on a new set of intentions when applicable
    /// </summary>
    public void Focus();
}
