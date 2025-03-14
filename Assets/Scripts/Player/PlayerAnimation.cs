
public class PlayerAnimation : AnimationManager
{
    private FightControlls fightControlls;
    protected override void Awake()
    {
        base.Awake();
        fightControlls = GetComponent<FightControlls>();
        if (fightControlls != null)
        {
            fightControlls.OnAttack += PlayAttackAnimation;
            fightControlls.OnDefend += PlayDefenseAnimation;
            fightControlls.OnToggleWeapon += AnimateToggleWeapon;
        }
    }

    private void OnDisable()
    {
        if (fightControlls != null)
            fightControlls.OnAttack -= PlayAttackAnimation;
        fightControlls.OnDefend -= PlayDefenseAnimation;
        fightControlls.OnToggleWeapon -= AnimateToggleWeapon;
    }


}
