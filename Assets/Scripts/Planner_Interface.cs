using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Planner_Interface
{
    public Action SelectAction(State currentState, Agent agent = null);
}
