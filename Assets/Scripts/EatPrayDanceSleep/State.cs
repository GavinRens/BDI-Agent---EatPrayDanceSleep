using System.Collections;
using System.Collections.Generic;

//public class State
//{
//    // Example definition; deplace with own definition
//    public int number;

//    public State(int _number)
//    {
//        number = _number;
//    }
//}

//public class State
//{
//    // Example definition; deplace with own definition
//    public string name;

//    public State(string _name)
//    {
//        name = _name;
//    }
//}

public class State
{
    // Example definition; deplace with own definition
    public (int, int) position;

    public State((int, int) _position)
    {
        position = _position;
    }
}