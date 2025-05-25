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
}

public class ConsoleBattleLogger : IBattleLogger
{
    private const bool ShowBoringStats = true;

    public void LogDamage(BattleReport report)
    {
        Console.WriteLine("]:::=====-------------------------");
        Console.WriteLine($"The {report.Attacker.TypeOfUnit} of {report.Attacker.ArmyOrigin.CountryOrigin.Name} has attacked {report.Defender.TypeOfUnit} of {report.Defender.ArmyOrigin.CountryOrigin.Name}");

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
}
