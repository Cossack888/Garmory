using System.Collections.Generic;
using UnityEngine;

public class WeaponColliderTrigger : StateMachineBehaviour
{
    [SerializeField] private List<float> enableTimes = new List<float>();
    private Weapon[] weapons;
    private bool collidersEnabled;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapons = animator.GetComponentsInChildren<Weapon>();

        if (weapons.Length > 0)
        {
            collidersEnabled = false;
            ToggleColliders(collidersEnabled);
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapons.Length > 0)
        {
            foreach (float enableTime in enableTimes)
            {
                if (stateInfo.normalizedTime >= enableTime && !collidersEnabled)
                {
                    collidersEnabled = true;
                    ToggleColliders(collidersEnabled);
                    break;
                }
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapons.Length > 0)
        {
            collidersEnabled = false;
            ToggleColliders(collidersEnabled);
        }
    }
    private void ToggleColliders(bool enable)
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.ToggleCollider(enable);
        }
    }
}
