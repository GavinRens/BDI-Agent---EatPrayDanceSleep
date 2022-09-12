using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My_HMBDP_Agent : HMBDP_Agent, Planner_Interface
{
    MCTS mctsPlanner;

    public My_HMBDP_Agent(float _intentionSimilarityThreshold, float _satisfactionThreshold, int _memoryCapacity)
    : base(_intentionSimilarityThreshold, _satisfactionThreshold, _memoryCapacity)
    {
        DefineGoals();
        SpecifyGoalWeights();
        SpecifyGoalNoncompatabilities();
        foreach (Goal goal in Goals)
        {
            DesireLevel.Add(goal, 1f);
            SatisfactionLevels.Add(goal, new Queue<float>());
        }
        InitializeAgentState();
        InitializeIntentions(new HashSet<Goal> { Goals[0] });
        mctsPlanner = new MCTS(this);
    }


    public override List<State> GenerateStates()
    {
        var states = new List<State>();
        for(int x=1; x<= 5; x++)
            for(int y=1; y<= 5; y++)
                states.Add(new State((x,y)));
        return states;
    }


    public override bool HasFinished(State state)
    {
        return false;
    }


    public override void InitializeAgentState()
    {
        CurrentState = States[0];
    }


    public override void DefineGoals()
    {
        var goal_15 = new Goal("EatGoal", (1, 5)); // Where the agent eats
        var goal_33 = new Goal("SleepGoal", (3, 3)); // Where the agent sleeps
        var goal_55 = new Goal("PrayGoal", (5, 5)); // Where the agent prays
        var goal_51 = new Goal("DanceGoal", (5, 1)); // Where the agent dances
        Goals.Add(goal_15);
        Goals.Add(goal_33);
        Goals.Add(goal_55);
        Goals.Add(goal_51);
    }


    public override void SpecifyGoalWeights()
    {
        GoalWeight.Add(Goals[0], 0.1f); // eats
        GoalWeight.Add(Goals[1], 0.4f); // sleeps
        GoalWeight.Add(Goals[2], 0.65f); // prays
        GoalWeight.Add(Goals[3], 1f); // dances
    }


    public override void InitializeIntentions(HashSet<Goal> intentions)
    {
        Intentions.Clear();
        foreach (Goal g in intentions)
            Intentions.Add(g);
    }


    public override void SpecifyGoalNoncompatabilities()
    {
        // The following spec is simply for testing purposes
        NonCompatibleGoals.Add(Goals[0], new HashSet<Goal> { Goals[2], Goals[3] });
        NonCompatibleGoals.Add(Goals[1], new HashSet<Goal>());
        NonCompatibleGoals.Add(Goals[2], new HashSet<Goal> { Goals[0], Goals[3] });
        NonCompatibleGoals.Add(Goals[3], new HashSet<Goal> { Goals[0], Goals[2] });
    }
    
    
    public override float Cost(Action a, State s)
    {
        return 0.1f;
    }


    // For general well-being / desire satisfaction.
    // Should be goal-agnostic (??)
    public override float Preference(Action a, State s)
    //public override float Preference(Goal g, Action a, State s)
    {
        return 0.1f;
    }


    // For (conscious) intentions.
    // Satisfaction of a particular goal might be action dependent.
    public override float Satisfaction(Goal g, Action a, State s)
    {
        float maxDist = -float.MaxValue;
        float distTo_15 = Mathf.Sqrt(Mathf.Pow(s.position.Item1 - 1, 2) + Mathf.Pow(s.position.Item2 - 5, 2));
        float distTo_33 = Mathf.Sqrt(Mathf.Pow(s.position.Item1 - 3, 2) + Mathf.Pow(s.position.Item2 - 3, 2));
        float distTo_55 = Mathf.Sqrt(Mathf.Pow(s.position.Item1 - 5, 2) + Mathf.Pow(s.position.Item2 - 5, 2));
        float distTo_51 = Mathf.Sqrt(Mathf.Pow(s.position.Item1 - 5, 2) + Mathf.Pow(s.position.Item2 - 1, 2));
        if (distTo_15 > maxDist) maxDist = distTo_15;
        if (distTo_33 > maxDist) maxDist = distTo_33;
        if (distTo_55 > maxDist) maxDist = distTo_55;
        if (distTo_51 > maxDist) maxDist = distTo_51;

        if (g.position == (1, 5))
        {
            if (a == Action.Eat && s.position == (1, 5))
                return 1;
            else
                return (maxDist - distTo_15) / maxDist / 20;  // division by 4 so that the best sat is not as mush as doing the right action in the right place
        }
        if (g.position == (3, 3))
        {
            if (a == Action.Sleep && s.position == (3, 3))
                return 1;
            else
                return (maxDist - distTo_33) / maxDist / 20;
        }
        if (g.position == (5, 5))
        {
            if (a == Action.Pray && s.position == (5, 5))
                return 1;
            else
                return (maxDist - distTo_55) / maxDist / 20;
        }
        if (g.position == (5, 1))
        {
            if (a == Action.Dance && s.position == (5, 1))
                return 1;
            else
                return (maxDist - distTo_51) / maxDist / 20;
        }
        return 0;
    }
    //public override float Satisfaction(Goal g, Action a, State s)
    //{
    //    if (g.position == (1, 5))
    //    {
    //        if (a == Action.Eat && s.position == (1, 5))
    //            return 1;
    //    }
    //    if (g.position == (3, 3))
    //    {
    //        if (a == Action.Sleep && s.position == (3, 3))
    //            return 1;
    //    }
    //    if (g.position == (5, 5))
    //    {
    //        if (a == Action.Pray && s.position == (5, 5))
    //            return 1;
    //    }
    //    if (g.position == (5, 1))
    //    {
    //        if (a == Action.Dance && s.position == (5, 1))
    //            return 1;
    //    }
    //    return 0;
    //}


    public override (Action, System.ValueTuple) GetPlan(HashSet<Goal> intentions, State s)
    {
        // Must still test adding written plans that can be used before plan generation
        // (Hand-written plans might be unnecessary)

        Action action = SelectAction(s, this);
        return (action, System.ValueTuple.Create());
    }


    //protected override float TransitionFunction(State stateFrom, Action action, State stateTo)
    public override float TransitionFunction(State stateFrom, Action action, State stateTo) // public for Model Validation
    {
        if(action == Action.Up)
        {
            if (stateFrom.position.Item2 == 5 && stateTo.position == stateFrom.position)
                return 1;
            else
            {
                if (stateTo.position.Item1 == stateFrom.position.Item1 && stateTo.position.Item2 == stateFrom.position.Item2 + 1)
                    return 1;
            }
        }

        if (action == Action.Down)
        {
            if (stateFrom.position.Item2 == 1 && stateTo.position == stateFrom.position)
                return 1;
            else
            {
                if (stateTo.position.Item1 == stateFrom.position.Item1 && stateTo.position.Item2 == stateFrom.position.Item2 - 1)
                    return 1;
            }
        }

        if (action == Action.Left)
        {
            if (stateFrom.position.Item1 == 1 && stateTo.position == stateFrom.position)
                return 1;
            else
            {
                if (stateTo.position.Item1 == stateFrom.position.Item1 - 1 && stateTo.position.Item2 == stateFrom.position.Item2)
                    return 1;
            }
        }

        if (action == Action.Right)
        {
            if (stateFrom.position.Item1 == 5 && stateTo.position == stateFrom.position)
                return 1;
            else
            {
                if (stateTo.position.Item1 == stateFrom.position.Item1 + 1 && stateTo.position.Item2 == stateFrom.position.Item2)
                    return 1;
            }
        }

        //if (action == Action.No_Op)
        //    if(stateTo.position == stateFrom.position)
        //        return 1;

        //if (action == Action.Eat)
        //    if (stateTo.position == stateFrom.position && stateFrom.position == (1,5))
        //        return 1;

        //if (action == Action.Dance)
        //    if (stateTo.position == stateFrom.position && stateFrom.position == (5,1))
        //        return 1;

        //if (action == Action.Sleep)
        //    if (stateTo.position == stateFrom.position && stateFrom.position == (3,3))
        //        return 1;

        //if (action == Action.Pray)
        //    if (stateTo.position == stateFrom.position && stateFrom.position == (5,5))
        //        return 1;

        if (action == Action.Eat || action == Action.Dance || action == Action.No_Op || action == Action.Sleep || action == Action.Pray)
            if (stateTo.position == stateFrom.position)
                return 1;

        return 0f;
    }


    public override Action SelectAction(State currentState, Agent agent = null)
    {
        return mctsPlanner.SelectAction(currentState, this);
    }


    public override bool isNavigationAction(Action a)
    {
        switch (a)
        {
            case Action.Up: return true;
            case Action.Down: return true;
            case Action.Left: return true;
            case Action.Right: return true;
            default: return false;
        }
    }


    public override void Focus()
    {
        var intentions = new HashSet<Goal>(Intentions);

        // Remove intentions that have been satisfied or are unsatisfiable at the moment
        foreach (Goal inten in intentions)
            if (ShouldRemove(inten))
            {
                SatisfactionLevels[inten].Clear();  // no record of satisfaction levels required for non-intentions
                Intentions.Remove(inten);
            }

        // Find the goal that currently has most intense desire level
        Goal mostIntense = new Goal(); // a temporary value
        float mostIntenseLevel = -float.MaxValue;

        //// But first norrow down the goals to those that are applicable to the current contect
        //List<Goal> applicableGoals;
        //if (RewardMachine.ActiveNode.name == "make_n_rest")
        //    applicableGoals = make_n_rest_Goals;
        //else if (RewardMachine.ActiveNode.name == "play_n_eat")
        //    applicableGoals = play_n_eat_Goals;
        //else
        //    applicableGoals = Goals; // will not be used

        foreach (Goal goal in Goals)
        //foreach (Goal goal in applicableGoals)
        {
            if (DesireLevel[goal] > mostIntenseLevel)
            {
                mostIntense = goal;
                mostIntenseLevel = DesireLevel[goal];
            }
        }

        // If the most intense goal is not already an intention and there is not an intention in the set that is incompatible w/ the most intense goal
        intentions = new HashSet<Goal>(Intentions);
        intentions.IntersectWith(NonCompatibleGoals[mostIntense]);
        if (!Intentions.Contains(mostIntense) && intentions.Count == 0)
        {
            Intentions.Add(mostIntense);
            SatisfactionLevels[mostIntense].Clear();  // to double-check to start a fresh record of satisfaction levels for mostIntense
        }
    }


    public void PrintCurrentIntentions()
    {
        var positions = new List<(int, int)>();
        foreach (Goal g in Intentions)
            positions.Add(g.position);
        Debug.Log("Intentions: " + string.Join(", ", positions));
    }


    public void PrintDesireLevels()
    {
        foreach (Goal g in Goals)
            Debug.Log("DesireLevel of " + g.position + ": " + DesireLevel[g].ToString());
    }


    public void PrintSatLevels()
    {
        float level = -1000f;
        foreach (Goal g in Goals)
            if (SatisfactionLevels[g].TryPeek(out level))
                Debug.Log("SatLevel of " + g.position + ": " + level.ToString());
            else
                Debug.Log("SatLevel of " + g.position + ": " + level.ToString());

    }
}
