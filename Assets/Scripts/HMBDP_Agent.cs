using System;
using System.Collections;
using System.Collections.Generic;

public abstract class HMBDP_Agent : Agent
{
    Random rand;
    static List<Goal> goals;
    static Dictionary<string, HashSet<Goal>> nonCompatibleGoals;
    HashSet<Goal> intentions;
    Dictionary<string, float> desireLevel;
    Dictionary<string, float> goalWeight;
    Dictionary<string, Queue<float>> satisfactionLevels;  // Every goal has a record of (the most recent) at most MRY sat levels, i.e., every queue is at most MRY long
    float intentionSimilarityThreshold;
    float satisfactionThreshold;
    int memoryCapacity;

    public HMBDP_Agent(float _intentionSimilarityThreshold, float _satisfactionThreshold, int _memoryCapacity) : base()
    {
        rand = new Random();
        goals = new List<Goal>();// Define class Goal, that inherits from State, in the code (and same namespace) instantiating this HMBDP. Goals is static because we assume that HMBDP agents have the same goals
        goalWeight = new Dictionary<string, float>();
        nonCompatibleGoals = new Dictionary<string, HashSet<Goal>>();
        intentions = new HashSet<Goal>();
        desireLevel = new Dictionary<string, float>();
        satisfactionLevels = new Dictionary<string, Queue<float>>();// Every goal has a record of (the most recent) at most MRY sat levels, i.e., every queue is at most MRY long
        intentionSimilarityThreshold = _intentionSimilarityThreshold;
        satisfactionThreshold = _satisfactionThreshold;
        memoryCapacity = _memoryCapacity;
    }
    
    //static HashSet<Action> Actions { get; }  // Define enum Action in the code (and same namespace) instantiating this HMBDP
     //static List<Observation> Observations { get; }  // Define enum Observation in the code (and same namespace) instantiating this HMBDP
     //static List<State> States { get; }  // Define class State in the code (and same namespace) instantiating this HMBDP

    public static List<Goal> Goals
    {
        get { return goals; }
    }
    public Dictionary<string, float> GoalWeight
    {
        get { return goalWeight; }
    }
    public Dictionary<string, HashSet<Goal>> NonCompatibleGoals
    {
        get { return nonCompatibleGoals; }
    }
    public HashSet<Goal> Intentions 
    {
        get { return intentions; }
    }
    public Dictionary<string, float> DesireLevel 
    { 
        get { return desireLevel; }
    }
    public Dictionary<string, Queue<float>> SatisfactionLevels 
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

    // Specify what the goals are
    protected abstract void DefineGoals();

    // Specify the importance / weight of each goal
    protected abstract void SpecifyGoalWeights();

    // Specify goal (in)compatabilities
    protected abstract void SpecifyGoalNoncompatabilities();

    // Initialize intentions
    protected abstract void InitializeIntentions();

    // Define preference function; return a value between 0 and 1, representing how much a performed in s brings the agent closer to g
    public abstract float Preference(Action a, State s);

    // Define cost function; return a value between 0 and 1
    public abstract float Cost(Action a, State s);

    // Define satisfaction function; return a value between 0 and 1, representing how satisfied the agent is in s w.r.t. g
    public abstract float Satisfaction(Goal g, Action a, State s);

    // Define the function that returns a plan, given a set of intentions and the current state
    protected abstract (Action, ValueTuple) GetPlan(HashSet<Goal> intentions, State s);

    // Define the transition function; the probability that an action performed in stateFrom will end up in stateTo
    //protected abstract float TransitionFunction(State stateFrom, Action action, State stateTo);
    public abstract float TransitionFunction(State stateFrom, Action action, State stateTo); // public for Model Validation

    // Specify, for every action, whether an actions implies that the agent must move its position (i.e. navigate)
    public abstract bool isNavigationAction(Action a);

    // Define the function that maps action-state pairs to observations
    //public Observation ObservationFunction(Action a, State s);


    public override State GetNextState(Action action, State state)
    {
        float r = (float)rand.NextDouble();
        float mass = 0;
        foreach (State ss in States)
        {
            mass += TransitionFunction(state, action, ss);
            if (r <= mass)
                return ss;
        }
        return null;
    }
    
    // Define the desire update rule that updates the desire level of any goal
    //public void UpdateDesire(Goal g, Action a, State s)
    //{
    //    // With this definition of the rule, the desire level of goals that are intentions cannot change
    //    DesireLevel[g] += (1 - IsIntention(g)) * GoalWeight[g] * (0.5f - Satisfaction(g, a, s));
    //}
    public void UpdateDesires(Action a, State s)
    {
        foreach (KeyValuePair<string, float> kvp in GoalWeight)
            UnityEngine.Debug.Log("kvp in GoalWeight: " + kvp.Key + ", " + kvp.Value);
        
        foreach (Goal g in Goals)
        {
            float w = GoalWeight[g.name];
            DesireLevel[g.name] += w - Satisfaction(g, a, s) * (DesireLevel[g.name] + w);
            //DesireLevel[g.name] += w * (1f - Satisfaction(g, a, s)) + (1 - DesireLevel[g.name]) * Satisfaction(g, a, s);
            //DesireLevel[g] += GoalWeight[g] * MathF.Exp(1/ memoryCapacity - Satisfaction(g, a, s) * DesireLevel[g]) * (1f - Satisfaction(g, a, s));
        }

    }


    // Return 1 iff g is currently an intention
    public int IsIntention(Goal g)
    {
        if(Intentions.Contains(g))
            return 1;
        else
            return 0;
    }


    // Define how satisfaction levels are recorded
    //public void MaintainSatisfactionLevelsOf(Goal g, Action a, State s)
    //{
    //    SatisfactionLevels[g.name].Enqueue(Satisfaction(g, a, s));
    //    if(SatisfactionLevels[g.name].Count > MemoryCapacity)
    //        SatisfactionLevels[g.name].Dequeue();
    //}
    public void MaintainSatisfactionLevels(Action a, State s)
    {
        foreach (Goal g in Intentions)
        {
            SatisfactionLevels[g.name].Enqueue(Satisfaction(g, a, s));
            if (SatisfactionLevels[g.name].Count > MemoryCapacity)
                SatisfactionLevels[g.name].Dequeue();
        }
    }


    // Define the function that refocuses on a new set of intentions when applicable
    public void Focus()
    {
        var intentions = new HashSet<Goal>(Intentions);

        // Remove intentions that have been satisfied or are unsatisfiable at the moment
        foreach (Goal inten in intentions)
            if (ShouldRemove(inten))
            {
                SatisfactionLevels[inten.name].Clear();  // no record of satisfaction levels required for non-intentions
                Intentions.Remove(inten);
            }
                

        // Find the goal that currently has most intense desire level
        Goal mostIntense = null;
        float mostIntenseLevel = -float.MaxValue;
        foreach(Goal goal in Goals)
        {
            if (DesireLevel[goal.name] > mostIntenseLevel)
            {
                mostIntense = goal;
                mostIntenseLevel = DesireLevel[goal.name];
            }
        }

        // If the most intense goal is not already an intention and there is not an intention in the set that is incompatible w/ the most intense goal
        intentions = new HashSet<Goal>(Intentions);
        intentions.IntersectWith(NonCompatibleGoals[mostIntense.name]);
        if (!Intentions.Contains(mostIntense) && intentions.Count == 0)
        {
            Intentions.Add(mostIntense);
            SatisfactionLevels[mostIntense.name].Clear();  // to double-check to start a fresh record of satisfaction levels for mostIntense
        }
    }

    
    bool DoMatch(HashSet<Goal> intentions1, HashSet<Goal> intentions2)
    {// used when selecting plan
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
        

    bool ShouldRemove(Goal g)
    {// used in Focus()

        float averageSatLevelChange(Goal gg)
        {
            float total = 0f;
            var satLevelsList = new List<float>(SatisfactionLevels[gg.name]);
            for(int i = 1; i < satLevelsList.Count; i++)
                total += satLevelsList[i] - satLevelsList[i-1];
            return total / (satLevelsList.Count-1);
        }

        if (SatisfactionLevels[g.name].Count < MemoryCapacity)
            return false; // not enough time has been spend pursuing g
        else if (averageSatLevelChange(g) < SatisfactionThreshold)
            return true; // g is not being satisfied
        else
            return false; // g is being satisfied
    }
}
