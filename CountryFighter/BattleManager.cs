namespace CountryFighter;

public static class BattleManager
{
    public static bool ShowBorringStaticstics = true;
    public static void Attack(Country attacker, Country defender)
    {
        if (attacker == defender) return;

        ArmyInfo attackerArmyInfo = new ArmyInfo(attacker);
        ArmyInfo defenderArmyInfo = new ArmyInfo(defender);
        while (defender.MainArmy.CanFight && attacker.MainArmy.CanFight)
        {
            for(int i = 0; i < 10; i++)
            {
                attacker.MainArmy.AttackCountry(defender, defender);
                defender.MainArmy.AttackCountry(attacker, defender);
                Thread.Sleep(100);
            }
            attackerArmyInfo.DisplayCombinedLosses();
            defenderArmyInfo.DisplayCombinedLosses();
            attackerArmyInfo.StartNewDay();
            defenderArmyInfo.StartNewDay();
            Console.ReadKey();
            Console.Clear();
        }
    }
}
