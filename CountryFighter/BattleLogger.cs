namespace CountryFighter;

public record BattleReport(
    ArmyUnit Attacker,
    ArmyUnit Defender,
    int PureDamage,
    double LocationFactor,
    double QualityComparison,
    double QuantityComparison,
    double ScoutingComparison,
    int EntitiesDestroyed,
    int TroopsKilled);

public interface IBattleLogger
{
    void LogDamage(BattleReport report);
    void Display(Army army);
}

public class ConsoleBattleLogger : IBattleLogger
{
    private const bool ShowBoringStats = true;

    public void LogDamage(BattleReport report)
    {
        Console.WriteLine("]:::=====-------------------------");
        Console.WriteLine($"The {report.Attacker.Name} of {report.Attacker.ArmyOrigin.CountryOrigin.Name} has attacked {report.Defender.Name} of {report.Defender.ArmyOrigin.CountryOrigin.Name}");

        Console.WriteLine("]:=:=:=-");
        if (ShowBoringStats)
        {
            Console.WriteLine($"> damageFactor: {report.PureDamage}");
            Thread.Sleep(50);
            Console.WriteLine($"> qualityComparison: {report.QualityComparison}");
            Thread.Sleep(50);
            Console.WriteLine($"> scoutingComparison: {report.ScoutingComparison}");
            Thread.Sleep(50);
            Console.WriteLine($"> quantityComparison: {report.QuantityComparison} ({report.Attacker.Count}, {report.Defender.Count}), total army: ({report.Attacker.ArmyOrigin.ArmyCount}, {report.Defender.ArmyOrigin.ArmyCount})");
            Thread.Sleep(50);
            Console.WriteLine($"> locationFactor: {report.LocationFactor}");
            Thread.Sleep(50);
            Console.WriteLine("]:=:=:=-");
        }
        Console.WriteLine($"damage done to enemy weaponry: {report.EntitiesDestroyed}, troops killed: {report.TroopsKilled}");
        Console.WriteLine("]::===-----------");
    }

    public void Display(Army army)
    {
        Console.WriteLine("]:=:[]:=:[||||||||]:=:[]:=:[");
        Console.WriteLine("The army of " + army.CountryOrigin.Name);
        Console.WriteLine("]:::::===---------");
        Console.WriteLine(army.ArmyUnits.FindByType<Soldier>().ToString());
        Console.WriteLine("Army battle spirit: " + army.BattleSpirit);
        Console.WriteLine("]:::::===---------");
        foreach(var unit in army.ArmyUnits.Where(n => n is not Soldier))
        {
            Console.WriteLine(unit.ToString());
        }
        Console.WriteLine("]:======:-:======:-:======:[");
    }
}
