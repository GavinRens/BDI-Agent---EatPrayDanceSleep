# EatPrayDanceSleep_HMB_Agent
An agent that eats, prays, dances and sleeps - controlled by a MCTS planner and based on the BDI architecture. 

## Description
A framework for controlling agents in Unity (3D real-time engine). The algorithm in the framework is based on my work with the Belief-Desire-Intention (BDI) architecture: The agent has a set of goals. The agent periodically selects a subset of these goals to pursue for a while. The currently selected goals are called *intentions*. In the framework in this project, an agent can pursue all or some intentions simultaneously. The agent designer can specify which goals can/cannot be pursued simultaneously, and the 'importance' of every goal can be set. The agent designer can also define what rewards the agent will get in general (besides for goals) and define the cost of each action. Taken together, these specifications and definitions produce emergent behavior, where an agent will keep selecting different intentions to pursue.

I implemented a Monte Carlo Tree Search (MCTS) planner, which plans over the current set of intentions, weighted by their importance. In this eat-pray-dance-sleep environment, the observation mapping function is deterministic. This means that the MCTS planner can be based on a (fully observable) Markov decision process (MDP).  machine agent can deal with probabilistic observations. The patrolling environment thus makes use of a MCTS planner based on a partially observable Markov decision process (POMDP).

## Video

[treasure_hunter_agent.webm](https://user-images.githubusercontent.com/41202408/189546964-c766f38e-e00b-4429-bdbc-01a6dadd2d42.webm)


## Installation
- The project is developed with Unity Editor version 2021.3.3f1 and C# version 9.0 on a Windows operating system.

- The project can be cloned from [GitHub](https://github.com/GavinRens/Reward-Machine-Agent---Treasure-Hunting).

- In your command line interface, run `git clone <URL>` in the local directory of your choice, where `<URL>` is the url displayed under Code -> HTTPS of the GitHub repo landing page.

- Then, 'Open' the project in your Unity Hub. (Find the project folder in Windows Explorer.)

- Once the project has opened in the Unity editor, select the TreasureHunter scene in Assets/Scenes of the editor.

- The scene is now playable.

## Usage / API Reference
 All files that have content which require method implementations are in Assets/Scripts/TreasureHunterAgent.
 - Actions.cs: Provide action names.
 - Observation.cs: Provide observation names.
 - State.cs: Define what features matter to the agent.
 - Environment.cs: Define the 'ground truth' of environment dynamics: what will the next state be and what observations will be made.
 - TreasureHunter_NMRDP_Agent inherits NMRDP_Agent. The agent designer must implement the following methods.
    - `DefineRewardMachine` defines the agent's reward machine, which should specify when the agent gets rewards and how much.
    - `GenerateStates` defines which states are possible in the environment.
    - `TransitionFunction` is the agent's model of how actions take the agent from one state to the next.
    - `GetObservation` defines the agent's model for receiving observations; transitions in the reward machine depend on these observations. That is, depending on which node in the reward machine is active, the agent gets a reward, depending on what it observes.
    - `SelectAction` recommends the agent's next action, given its current state. Implementing this method satisfies the Planner_Interface in the background. Your planner of choice must also satisfy the Planner_Interface. In this project, `SelectAction` in TreasureHunter_NMRDP_Agent simply calls `SelectAction` in the MCTS planner.
    - `isNavigationAction` is required in the agent controller script.
 - AgentController.cs: LateUpdate() cycles thru three control phases: `Phase.Planning`, `Phase.Execution` and `Phase.Updating`:
    - `Phase.Planning`: Here, the `SelectAction` method is called, and the scene is updated according to the selected action. Navigation actions affect how NavMeshAgent.SetDestination() is called.
    - `Phase.Execution`: Here, if the action is for navigation, the destination, previously set, is pursued. Also here, the programmer (you) must define what happens if the action is not about navigation.
    - `Phase.Updating`: This phase will typically not need your attention, but the sequence of method calls here is instructive:
     <pre><code>
     State nextState = Environment.GetRealNextState(nmrdpAgent.CurrentState, nmrdpAgent.CurrentAction);
     Observation obs = Environment.GetRealObservation(nmrdpAgent.CurrentAction, nextState);
     nmrdpAgent.RewardMachine.AdvanceActiveNode(obs);
     nmrdpAgent.CurrentState = nextState;
     </code></pre>

## Parameters
Found in Parameters.cs

- MAX_NUOF_ACTIONS is the number of actions/steps the agent will look into the future when planning. If you want the agent to consider rewards h steps in the future when deciding on its next action, then MAX_NUOF_ACTIONS should be at least h.
- ITERATIONS is the number of times the MCTS search tree will be expanded. For the Patrolling environment, i used 100, but for environs with more states and/or actions, a larger number might be needed. You should use the smallest number of iterations that yields the desired behavior.
- BELIEF_SIZE_FACTOR determines how many states are used to represent the agents beliefs. If there are S states representing the environment, then the number of states representing any/every belief is S/BELIEF_SIZE_FACTOR. Setting BELIEF_SIZE_FACTOR = 10 seems to be a good rule of thumb.
- DISCOUNT_FACTOR as typically used in MDPs (0.95 is a typical value).
- STOCHASTICITY_FACTOR is a value between 0 and 1. It can be used to standardize how uncertain the effects of actions and/or observations are (used in `TransitionFunction` and/or `ObservationFunction`). Conventionally, a value closer to 0 means leass uncertainty. STOCHASTICITY_FACTOR is not used in the Patrolling environ.
- AT_TARGET_DISTANCE is used in the agent controller to specify at what distance we consider an agent to have arrived at a target.

## Environment design
A book can be written about this topic.

The agent should be designed on paper first. This is an iterative process that should be done before any coding. When implementing the agent with code, some inconsistencies might be noticed. These can then be fixed during programming.

1. Start by thinking what the agent is expected to do; what should its behavior be?
2. Then decide what features will make up a state. 
3. Decide what actions the agent will be able to do, e.g., `go_to_next_waypoint`, `jump` or `pick_up_axe`. 
4. Now design the transition function.
5. Deciding on the observations and designing the reward machine (RM) can be done together: transitions in the RM depend on observations, and when a transition in the RM happens, a reward is output. Note, transitions in the RM are not transitions between (environment) states.
6. Design `GetObservation` and `GetRealObservation` so that the observation made for a given action performed in a given state causes the desired transition in the RM. The reason why actions are not used to trigger RM transitions is because different action-state pairs might produce the same observation, i.e., we want the agent to get the same reward (at a particular/active RM node) for the same observation, independent of action and state. For instance, in `state_13` the agent observes `axe_in_hand` after performing a 'Get_Axe' spell on an axe five meters away, and in `state_42` the agent observes `axe_in_hand` after picking up an axe.
7. Activate the ModelValidation game object in the Unity hierarchy to validate that the transition function is a true probability distribution. Tip: Just while running the model validator, choose state feature parameters that generate less than one or two thousand states; if there are too many states, the validator will take very long to finish. With the model validator deactivated, the normal number of states can be used (within computation limits). Play the scene to check the output in the console. There is no output from ModelValidation.cs, if and only if the models are good. Designing the transition function can be tricky, and it is perhaps a weakness of MDP-based architectures when this function has to be designed by hand.

## References
Rens, G., Raskin, J.-F., Reyonard, R., Marra, G. (2021): Online Learning of Non-Markovian Reward Models. Proceedings of Thirteenth Intl. Conf. on Agents and Artif. Intell. (ICAART 2021).

Note, the architecture in this project does not use the *learning* aspect of Non-Markovian Reward Models (i.e. Non-Markovian Reward Decision Processes (NMRDPs)).

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)

