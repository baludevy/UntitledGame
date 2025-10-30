using UnityEngine;

public static class Combat
{
    public static bool RollCrit()
    {
        return Random.value <= PlayerStatistics.Instance.Combat.critChance;
    }

    public static HitEffectiveness CalcEffectiveness(Elements attElement, Elements oppElement)
    {
        if (attElement == Elements.Fire && oppElement == Elements.Grass) return HitEffectiveness.SuperEffective;
        if (attElement == Elements.Water && oppElement == Elements.Fire) return HitEffectiveness.SuperEffective;
        if (attElement == Elements.Grass && oppElement == Elements.Water) return HitEffectiveness.SuperEffective;
        if (attElement == Elements.Ground && oppElement == Elements.Rock) return HitEffectiveness.SuperEffective;
        if (attElement == Elements.Ghost && oppElement == Elements.Ghost) return HitEffectiveness.SuperEffective;
        
        if (attElement == Elements.Fire && oppElement == Elements.Water) return HitEffectiveness.NotEffective;
        if (attElement == Elements.Water && oppElement == Elements.Grass) return HitEffectiveness.NotEffective;
        if (attElement == Elements.Grass && oppElement == Elements.Fire) return HitEffectiveness.NotEffective;
        if (attElement == Elements.Rock && oppElement == Elements.Ground) return HitEffectiveness.NotEffective;

        return HitEffectiveness.Normal;
    }
}