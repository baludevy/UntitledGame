using UnityEngine;

public static class Combat
{
    public static bool RollCrit()
    {
        return Random.value <= PlayerStatistics.Instance.Combat.critChance;
    }

    public static HitEffectiveness CalcEffectiveness(Element attElement, Element oppElement)
    {
        if (attElement == Element.Fire && oppElement == Element.Grass) return HitEffectiveness.SuperEffective;
        if (attElement == Element.Water && oppElement == Element.Fire) return HitEffectiveness.SuperEffective;
        if (attElement == Element.Grass && oppElement == Element.Water) return HitEffectiveness.SuperEffective;
        if (attElement == Element.Ground && oppElement == Element.Rock) return HitEffectiveness.SuperEffective;
        if (attElement == Element.Ghost && oppElement == Element.Ghost) return HitEffectiveness.SuperEffective;
        if(attElement == Element.Wind && oppElement == Element.Fire) return HitEffectiveness.SuperEffective;
        if(attElement == Element.Wind && oppElement == Element.Grass) return HitEffectiveness.SuperEffective;
        
        if (attElement == Element.Fire && oppElement == Element.Water) return HitEffectiveness.NotEffective;
        if (attElement == Element.Water && oppElement == Element.Grass) return HitEffectiveness.NotEffective;
        if (attElement == Element.Grass && oppElement == Element.Fire) return HitEffectiveness.NotEffective;
        if (attElement == Element.Rock && oppElement == Element.Ground) return HitEffectiveness.NotEffective;

        return HitEffectiveness.Normal;
    }

    public static float CalculateDamage(float damage, bool crit, HitEffectiveness hitEffectiveness)
    {
        if(crit)
            damage *= PlayerStatistics.Instance.Combat.critMultiplier;

        switch (hitEffectiveness)
        {
            case HitEffectiveness.Normal:
                break;
            case HitEffectiveness.NotEffective:
                damage *= 0.8f;
                break;
            case HitEffectiveness.SuperEffective:
                damage *= 1.2f;
                break;
        }
        
        return damage;
    }
}