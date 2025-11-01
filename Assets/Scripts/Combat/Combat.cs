using UnityEngine;

public static class Combat
{
    public static bool RollCrit()
    {
        return Random.value <= PlayerStatistics.Instance.Combat.critChance;
    }

    public static TypeEffectiveness CalcEffectiveness(Element attElement, Element oppElement)
    {
        if (attElement == Element.Fire && oppElement == Element.Grass) return TypeEffectiveness.SuperEffective;
        if (attElement == Element.Water && oppElement == Element.Fire) return TypeEffectiveness.SuperEffective;
        if (attElement == Element.Grass && oppElement == Element.Water) return TypeEffectiveness.SuperEffective;
        if (attElement == Element.Ground && oppElement == Element.Rock) return TypeEffectiveness.SuperEffective;
        if (attElement == Element.Ghost && oppElement == Element.Ghost) return TypeEffectiveness.SuperEffective;
        if(attElement == Element.Wind && oppElement == Element.Fire) return TypeEffectiveness.SuperEffective;
        if(attElement == Element.Wind && oppElement == Element.Grass) return TypeEffectiveness.SuperEffective;
        
        if (attElement == Element.Fire && oppElement == Element.Water) return TypeEffectiveness.NotEffective;
        if (attElement == Element.Water && oppElement == Element.Grass) return TypeEffectiveness.NotEffective;
        if (attElement == Element.Grass && oppElement == Element.Fire) return TypeEffectiveness.NotEffective;
        if (attElement == Element.Rock && oppElement == Element.Ground) return TypeEffectiveness.NotEffective;

        return TypeEffectiveness.Normal;
    }

    public static float CalculateDamage(float damage, bool crit, TypeEffectiveness hitEffectiveness)
    {
        if(crit)
            damage *= PlayerStatistics.Instance.Combat.critMultiplier;

        switch (hitEffectiveness)
        {
            case TypeEffectiveness.Normal:
                break;
            case TypeEffectiveness.NotEffective:
                damage *= 0.8f;
                break;
            case TypeEffectiveness.SuperEffective:
                damage *= 1.2f;
                break;
        }
        
        return damage;
    }
}