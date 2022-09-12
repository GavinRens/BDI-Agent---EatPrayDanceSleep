
public class Goal : State
{
    public string name;

    public Goal() { }

    public Goal(string _name, (int, int) _position) : base(_position)
    {
        name = _name;
    }
}
