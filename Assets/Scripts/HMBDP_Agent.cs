using System;
using System.Collections;
using System.Collections.Generic;

public abstract class HMBDP_Agent : Agent, HMBDP_Interface
{
    Random rand;
    static List<Goal> goals;
    static Dictionary<Goal, HashSet<Goal>> nonCompatibleGoals;
    HashSet<Goal> intentions;
    Dictionary<Goal, float> desireLevel;
    Dictionary<Goal, float> goalWeight;
    Dictionary<Goal, Queue<float>> satisfactionLevels;  // Every goal has a record of (the most recent) at most MRY sat levels, i.e., every queue is at most MRY long
    float intentionSimilarityThreshold;
    float satisfactionThreshold;
    int memoryCapacity;

    public HMBDP_Agent(float _intentionSimilarityThreshold, float _satisfactionThreshold, int _memoryCapacity) : base()
    {
        rand = new Random();
        goals = new List<Goal>();// Define class Goal, that inherits from State, in the code (and same namespace) instantiating this HMBDP. Goals is static because we assume that HMBDP agents have the same goals
        goalWeight = new Dictionary<Goal, float>();
        nonCompatibleGoals = new Dictionary<Goal, HashSet<Goal>>();
        intentions = new HashSet<Goal>();
        desireLevel = new Dictionary<Goal, float>();
        satisfactionLevels = new Dictionary<Goal, Queue<float>>();// Every goal has a record of (the most recent) at most MRY sat levels, i.e., every queue is at most MRY long
        intentionSimilarityThreshold = _intentionSimilarityThreshold;
        satisfactionThreshold = _satisfactionThreshold;
        memoryCapacity = _memoryCapacity;
    }


    // For Agent (other methods to be implemented in final agent instance)

    public override State GetNextState(Action a, State s)
    {
        float r = (float)rand.NextDouble();
        float mass = 0;
        foreach (State ss in States)
        {
            mass += TransitionFunction(s, a, ss);
            if (r <= mass)
                return ss;
        }
        return new State();
    }
    
    
    // For HMBDP_Interface
    
    public static List<Goal> Goals
    {
        get { return goals; }
    }
    public Dictionary<Goal, float> GoalWeight
    {
        get { return goalWeight; }
    }
    public Dictionary<Goal, HashSet<Goal>> NonCompatibleGoals
    {
        get { return nonCompatibleGoals; }
    }
    public HashSet<Goal> Intentions 
    {
        get { return intentions; }
    }
    public Dictionary<Goal, float> DesireLevel 
    { 
        get { return desireLevel; }
    }
    public Dictionary<Goal, Queue<float>> SatisfactionLevels 
    {
        get { return satisfactionLevels; }
    }   
    public float IntentionSimilarityThreshold 
    {
        get { return intentionSimilarityThreshold; }
    }
    public float SatisfactionThreshold 
    { 
        get { return satisfactionThreshold; }
    }
    public int MemoryCapacity 
    {
        get { return memoryCapacity; }
    }

    public abstract void DefineGoals();

    public abstract void SpecifyGoalWeights();

    public abstract void SpecifyGoalNoncompatabilities();

    public abstract void InitializeIntentions(HashSet<Goal> intentions);

    public abstract float Preference(Action a, State s);

    public abstract float Cost(Action a, State s);

    public abstract float Satisfaction(Goal g, Action a, State s);

    public abstract (Action, System.ValueTuple) GetPlan(HashSet<Goal> intentions, State s);

    public abstract bool isNavigationAction(Action a);

    public abstract void Focus();


    // Define the transition function; the probability that an action performed in stateFrom will end up in stateTo
    //protected abstract float TransitionFunction(State stateFrom, Action action, State stateTo);
    public abstract float TransitionFunction(State stateFrom, Action action, State stateTo); // public for Model Validation


    // Define the function that maps action-state pairs to observations
    //public Observation ObservationFunction(Action a, State s);


    public void UpdateDesires(Action a, State s)
    {
        //foreach (KeyValuePair<string, float> kvp in GoalWeight)
        //    UnityEngine.Debug.Log("kvp in GoalWeight: " + kvp.Key + ", " + kvp.Value);

        foreach (Goal g in Goals)
        {
            float w = GoalWeight[g];
            DesireLevel[g] += w - Satisfaction(g, a, s) * (DesireLevel[g] + w);
            //DesireLevel[g] += (1 - IsIntention(g)) * GoalWeight[g] * (0.5f - Satisfaction(g, a, s));
            //DesireLevel[g.name] += w * (1f - Satisfaction(g, a, s)) + (1 - DesireLevel[g.name]) * Satisfaction(g, a, s);
            //DesireLevel[g] += GoalWeight[g] * MathF.Exp(1/ memoryCapacity - Satisfaction(g, a, s) * DesireLevel[g]) * (1f - Satisfaction(g, a, s));
        }
    }


    public int IsIntention(Goal g)
    {
        if(Intentions.Contains(g))
            return 1;
        else
            return 0;
    }


    public void MaintainSatisfactionLevels(Action a, State s)
    {
        foreach (Goal g in Intentions)
        {
            SatisfactionLevels[g].Enqueue(Satisfaction(g, a, s));
            if (SatisfactionLevels[g].Count > MemoryCapacity)
                SatisfactionLevels[g].Dequeue();
        }
    }


    /// <summary>
    /// Whether a goal should no longer be an intention
    /// </summary>
    /// <param name="g">The goal (intention) to consider</param>
    /// <returns>true of false</returns>
    public bool ShouldRemove(Goal g)
    {
        float averageSatLevelChange(Goal gg)
        {
            float total = 0f;
            var satLevelsList = new List<float>(SatisfactionLevels[gg]);
            for (int i = 1; i < satLevelsList.Count; i++)
                total += satLevelsList[i] - satLevelsList[i - 1];
            return total / (satLevelsList.Count - 1);
        }

        if (SatisfactionLevels[g].Count < MemoryCapacity)
            return false; // not enough time has been spend pursuing g
        else if (averageSatLevelChange(g) < SatisfactionThreshold)
            return true; // g is not being satisfied
        else
            return false; // g is being satisfied
    }


    /// <summary>
    /// Helps decide which stores (hand-written?) plan to select, given the current set of intentions
    /// </summary>
    /// <param name="intentions1">Set of intentions</param>
    /// <param name="intentions2">Set of intentions</param>
    /// <returns>true or false</returns>
    bool DoMatch(HashSet<Goal> intentions1, HashSet<Goal> intentions2)
    {
        float similarty = 0f;
        HashSet<Goal> intersection = new HashSet<Goal> (intentions1);
        HashSet<Goal> union = new HashSet<Goal> (intentions1);
        intersection.IntersectWith(intentions2);
        union.UnionWith(intentions2);
        similarty = intersection.Count / union.Count;
        if(similarty > IntentionSimilarityThreshold)
            return true;
        return false;
    }


    // For Planner_Interface

    public abstract Action SelectAction(State currentState, Agent agent = null);
}
