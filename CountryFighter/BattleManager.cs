namespace CountryFighter;

public static class BattleManager
{
    public static void Attack(Country attacker, Country defender, IBattleLogger logger)
    {
        if (attacker == defender)
        {
            return;
        }

        using var attackerArmyInfo = new ArmyInfo(attacker);
        using var defenderArmyInfo = new ArmyInfo(defender);
        while (defender.MainArmy.CanFight && attacker.MainArmy.CanFight)
        {
            for (int i = 0; i < 10; i++)
            {
                attacker.MainArmy.AttackCountry(defender, defender, logger);
                defender.MainArmy.AttackCountry(attacker, defender, logger);
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
