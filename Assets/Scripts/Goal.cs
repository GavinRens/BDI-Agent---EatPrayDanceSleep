using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : State
{
    public string name;

    public Goal(string _name, (int, int) _position) : base(_position)
    {
        name = _name;
    }
}
