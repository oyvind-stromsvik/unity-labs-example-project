using UnityEngine;

public class RandomStateSMB : StateMachineBehaviour
{
    public int numberOfStates = 7;          // The number of random states to choose between.


    private readonly int m_HashRandomIdlePara = Animator.StringToHash("RandomIdle");    // For referencing the RandomIdle animator parameter.


    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        // Set RandomIdle based on how many states there are.
        int randomSelection = Random.Range(0, numberOfStates);
        animator.SetInteger(m_HashRandomIdlePara, randomSelection);
    }
}
