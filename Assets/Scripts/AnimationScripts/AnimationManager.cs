using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Idle,
    Jump,
    SimpleAttack,
    ComboAttack,
    JumpAttack,
    UnarmedSingle,
    UnarmedCombo
}
public enum SingleAnimations
{
    draw,
    sheath,
    jump,
    death,
    blocked,
    hit
}
[System.Serializable]
public class AnimationArray
{
    public AnimationType animationType;
    public List<string> selectedAnimations = new List<string>();
}

public abstract class AnimationManager : MonoBehaviour
{
    public Animator animator;
    public enum AnimationLayer
    {
        Base,
        Shield,
        Attack
    }
    protected Statistics stats;
    protected bool defending;
    public AnimationArray[] animationArrays;
    protected string lastPlayedAnimation = null;
    [HideInInspector] public List<string> availableAnimationStates = new List<string>();
    protected Dictionary<AnimationType, List<string>> animationDict = new Dictionary<AnimationType, List<string>>();
    protected Health health;
    protected float baseAttackSpeed = 1f;
    protected float baseMovementSpeed = 1f;
    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("AttackSpeed", 1);
        stats = GetComponent<Statistics>();
        ChangeAttackSpeed(stats.AttackSpeed);
        ChangeMovementSpeed(stats.MovementSpeed);
    }
    protected virtual void Awake()
    {
        BuildAnimationDictionary();
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += PlayDeathAnimation;
            health.OnBlocked += PlaySingleAnimation;
            health.OnHit += PlaySingleAnimation;
        }
    }
    public void ChangeAttackSpeed(float percentageBonus)
    {
        float newAttackSpeed = baseAttackSpeed * (1 + percentageBonus / 100f);
        animator.SetFloat("AttackSpeed", newAttackSpeed);
        animator.Update(Time.deltaTime);
    }

    public void ChangeMovementSpeed(float percentageBonus)
    {
        float newMovementSpeed = baseMovementSpeed * (1 + percentageBonus / 100f);
        animator.SetFloat("MovementSpeed", newMovementSpeed);
        animator.Update(Time.deltaTime);
    }

    private void OnDisable()
    {
        health.OnDeath -= PlayDeathAnimation;
    }
    protected virtual void DisconectAllDelegates()
    {
    }
    private void BuildAnimationDictionary()
    {
        animationDict.Clear();
        foreach (var animArray in animationArrays)
        {
            if (!animationDict.ContainsKey(animArray.animationType))
            {
                animationDict[animArray.animationType] = new List<string>();
            }
            animationDict[animArray.animationType].AddRange(animArray.selectedAnimations);
        }
    }

    public void PlayDefenseAnimation(bool mode)
    {
        defending = mode;
        health.SetIsBlocking(mode);
        ToggleLayer(AnimationLayer.Shield, mode, 0.3f);
        ToggleLayer(AnimationLayer.Attack, !mode, 0.3f);
    }
    public void PlayDeathAnimation()
    {
        PlayDefenseAnimation(false);
        PlaySingleAnimation(SingleAnimations.death);
        DisconectAllDelegates();
    }
    public void PlaySingleAnimation(SingleAnimations singleAnimations)
    {
        if (singleAnimations == SingleAnimations.hit)
        {
            PlayDefenseAnimation(false);
        }
        animator.Play(singleAnimations.ToString());
    }
    public void PlayAttackAnimation(AnimationType animationType)
    {
        if (!defending)
        {
            PlayAnimation(animationType);
        }
    }
    public void PlayAnimation(AnimationType animationType)
    {
        if (animationDict.TryGetValue(animationType, out List<string> animations) && animations.Count > 0)
        {
            string randomAnimation = animations[Random.Range(0, animations.Count)];
            lastPlayedAnimation = randomAnimation;
            animator.Play(randomAnimation);
        }
        else
        {

        }
    }
    public void AnimateMovement(float x, float y)
    {
        float motion = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
        animator.SetFloat("xMotion", x);
        animator.SetFloat("yMotion", y);
        animator.SetFloat("Motion", motion);
    }
    public void AnimateToggleWeapon(bool state)
    {
        if (state)
        {
            PlaySingleAnimation(SingleAnimations.sheath);
        }
        else
        {
            PlaySingleAnimation(SingleAnimations.draw);
        }
    }
    IEnumerator SmoothLayerWeight(int layerIndex, float targetWeight, float duration)
    {
        float time = 0f;
        float startWeight = animator.GetLayerWeight(layerIndex);

        while (time < duration)
        {
            time += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, targetWeight, time / duration);
            animator.SetLayerWeight(layerIndex, newWeight);
            yield return null;
        }
        animator.SetLayerWeight(layerIndex, targetWeight);
    }
    public void ToggleLayer(AnimationLayer layer, bool enable, float duration)
    {
        float targetWeight = enable ? 1f : 0f;
        int layerIndex = animator.GetLayerIndex(layer.ToString());
        StartCoroutine(SmoothLayerWeight(layerIndex, targetWeight, duration));
    }
}
