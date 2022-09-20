# EatPrayDanceSleep_HMB_Agent
An agent that eats, prays, dances and sleeps - controlled by a MCTS planner and based on the BDI architecture. 

## Description
A framework for controlling agents in Unity (3D real-time engine). The algorithm in the framework is based on my work with the Belief-Desire-Intention (BDI) architecture: The agent has a set of goals. The agent periodically selects a subset of these goals to pursue for a while. The currently selected goals are called *intentions*. In the framework in this project, an agent can pursue all or some intentions simultaneously. The agent designer can specify which goals can/cannot be pursued simultaneously, and the 'importance' of every goal can be set. The agent designer can also define what rewards the agent will get in general (besides for goals) and define the cost of each action. Taken together, these specifications and definitions produce emergent behavior, where an agent will keep selecting different intentions to pursue.

I implemented a Monte Carlo Tree Search (MCTS) planner, which plans over the current set of intentions, weighted by their importance. In this eat-pray-dance-sleep environment, the observation mapping function is deterministic. This means that the MCTS planner can be based on a (fully observable) Markov decision process (MDP). However, probabilistic observation function could still be implemented for an agent with noisy sensors. In that case, the environment would make use of a MCTS planner based on a partially observable Markov decision process (POMDP). See the project at https://github.com/GavinRens/Reward-Machine-Agent---Patrolling, which enforces the PO_Agent_Interface.

## Video

Notice that sometimes the agents do the 'wrong' thing, e.g. eating in the disco or praying before reaching the place of worship. This could be 'fixed' with some work in the code, but i left it as is for now to show some *emergent* bahavior. Goals are given the following weights/importance (between 0 and 1): 0.1 for eating, 0.4 for sleeping, 0.65 for praying, and 1 for dancing. This explains why the agents eat relatively seldomly and why they dance so often.

[eatpraydancesleep_005.webm](https://user-images.githubusercontent.com/41202408/189694579-67c9c756-ad30-40d6-88d8-7af6991a227f.webm)

## Installation
- The project is developed with Unity Editor version 2021.3.3f1 and C# version 9.0 on a Windows operating system.

- The project can be cloned from [GitHub](https://github.com/GavinRens/Reward-Machine-Agent---Treasure-Hunting).

- In your command line interface, run `git clone <URL>` in the local directory of your choice, where `<URL>` is the url displayed under Code -> HTTPS of the GitHub repo landing page.

- Then, 'Open' the project in your Unity Hub. (Find the project folder in Windows Explorer.)

- Once the project has opened in the Unity editor, select the EatPrayDanceSleep scene in Assets/Scenes of the editor.

- The scene is now playable.

## Usage / API Reference
 All files that have content which require method implementations are in Assets/Scripts/EatPrayDanceSleep.
 - Actions.cs: Provide action names.
 - Observation.cs: Provide observation names.
 - State.cs: Define what features matter to the agent.
 - Environment.cs: Define the 'ground truth' of environment dynamics: what will the next state be.
 - My_HMBDP_Agent inherits HMBDP_Agent. The agent designer must implement the following methods.
    - `GenerateStates` defines which states are possible in the environment.
    - `HasFinished` which specifies when episodes end.
    - `InitializeAgentState` initializes the agent's state.
    - `DefineGoals` which defined the available agent goals.
    - `SpecifyGoalWeights` specifies the importance of each goal.
    - `InitializeIntentions` says which goals must be intentions when the agent becomes active.
    - `SpecifyGoalNoncompatabilities` specifies which goals may not be pursued simultaneously.
    - `Cost` defines the cost for performing each action.
    - `Preference` defines the agent's preference for particular actions and/or states not directly related to goal achievement.
    - `Satisfaction` defines how satisfied the agent will be with each goal's achievement.
    - `TransitionFunction` is the agent's model of how actions take the agent from one state to the next.
    - `SelectAction` recommends the agent's next action, given its current state. Implementing this method satisfies the Planner_Interface in the background. Your planner of choice must also satisfy the Planner_Interface. In this project, `SelectAction` in My_HMBDP_Agent simply calls `SelectAction` in the MCTS planner.
    - `isNavigationAction` is required in the agent controller script.
    - `Focus` determines when and which goals become intentions.
 - AgentController.cs: LateUpdate() cycles thru three control phases: `Phase.Planning`, `Phase.Execution` and `Phase.Updating`:
    - `Phase.Planning`: Here, the `SelectAction` method is called, and the scene is updated according to the selected action. Navigation actions affect how NavMeshAgent.SetDestination() is called.
    - `Phase.Execution`: Here, if the action is for navigation, the destination, previously set, is pursued. Also here, the programmer (you) must define what happens if the action is not about navigation.
    - `Phase.Updating`: This phase will typically not need your attention, but the sequence of method calls here is instructive:
     <pre><code>
     hmbdpAgent.UpdateDesires(hmbdpAgent.CurrentAction, hmbdpAgent.CurrentState);
     hmbdpAgent.MaintainSatisfactionLevels(hmbdpAgent.CurrentAction, hmbdpAgent.CurrentState);
     hmbdpAgent.Focus();
     hmbdpAgent.CurrentState = Environment.GetRealNextState(hmbdpAgent.CurrentState, hmbdpAgent.CurrentAction); ;
     </code></pre>

## Parameters
Found in Parameters.cs

- MAX_NUOF_ACTIONS is the number of actions/steps the agent will look into the future when planning. If you want the agent to consider rewards h steps in the future when deciding on its next action, then MAX_NUOF_ACTIONS should be at least h.
- ITERATIONS is the number of times the MCTS search tree will be expanded. For the Patrolling environment, i used 100, but for environs with more states and/or actions, a larger number might be needed. You should use the smallest number of iterations that yields the desired behavior.
- DISCOUNT_FACTOR as typically used in MDPs (0.95 is a typical value).
- STOCHASTICITY_FACTOR is a value between 0 and 1. It can be used to standardize how uncertain the effects of actions and/or observations are (used in `TransitionFunction` and/or `ObservationFunction`). Conventionally, a value closer to 0 means leass uncertainty. STOCHASTICITY_FACTOR is not used in the Patrolling environ.
- AT_TARGET_DISTANCE is used in the agent controller to specify at what distance we consider an agent to have arrived at a target.

## Environment design
The agent should be designed on paper first. This is an iterative process that should be done before any coding. When implementing the agent with code, some inconsistencies might be noticed. These can then be fixed during programming.

1. Start by thinking what the agent is expected to do; what should its behavior be?
2. Then decide what features will make up a state.
3. Decide on the tasks (goals) that need to be pusued, their relative importance, their compatability, and how the agent will decide how far it is from fully achieving each goal.
4. Decide what actions the agent will be able to do, e.g., `go_north`, `go_west`, `sleep` or `dance` to achieve its various goals. 
5. Design the transition function.
6. Activate the ModelValidation game object in the Unity hierarchy to validate that the transition function is a true probability distribution. Tip: Just while running the model validator, choose state feature parameters that generate less than one or two thousand states; if there are too many states, the validator will take very long to finish. With the model validator deactivated, the normal number of states can be used (within computation limits). Play the scene to check the output in the console. There is no output from ModelValidation.cs, if and only if the models are good. Designing the transition function can be tricky, and it is perhaps a weakness of MDP-based architectures when this function has to be designed by hand.

## References
Rens, G., Moodley, D. (2017): A hybrid POMDP-BDI agent architecture with online stochastic planning and plan caching. Journal of Cognitive Systems Research, 43, January 2017, 1-20.

Note, the architecture in this project is based on a (fully observable) MDP, not a POMDP. Also, i implemented several details differently to the paper when i saw opportunities for improvement, specifically, the desire update rule and the focus function.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)

