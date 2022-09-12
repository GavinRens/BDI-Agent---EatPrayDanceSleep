using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelValidation : MonoBehaviour
{
    HMBDP_Agent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = new My_HMBDP_Agent(0,0,0);

        Debug.Log("-------- START TRANS FUNC VALIDATION --------");

        foreach(State s in Agent.States)
            foreach(Action a in agent.Actions)
            {
                float mass = 0;
                foreach (State ss in Agent.States)
                {
                    mass += agent.TransitionFunction(s, a, ss);
                }
                if(mass != 1f)
                    Debug.Log(s.position + ", " + a + ", " + mass);
            }

        Debug.Log("-------- END TRANS FUNC VALIDATION --------");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
