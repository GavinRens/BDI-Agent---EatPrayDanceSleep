
//public class Goal : State
//{
//    public string name;

//    public Goal() { }

//    public Goal(string _name, (int, int) _position) : base(_position)
//    {
//        name = _name;
//    }
//}

public enum Goal
{
    EatGoal, // (1, 5)); // Where the agent eats
    SleepGoal, // (3, 3)); // Where the agent sleeps
    PrayGoal, // (5, 5)); // Where the agent prays
    DanceGoal // (5, 1)); // Where the agent dances
}
