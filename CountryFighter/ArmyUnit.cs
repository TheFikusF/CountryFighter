namespace CountryFighter;

public abstract class ArmyUnit : IDamagable
{
    private const double HOME_TERRITORY_BONUS = 2.0;
    private const double ENEMY_TERRITORY_PENALTY = 0.8;
    private const double NEUTRAL_TERRITORY_FACTOR = 1.0;
    private const double QUANTITY_BALANCE_FACTOR = 0.5;

    public virtual string Name => GetType().Name;

    private double _quality;
    public double Quality
    {
        get => _quality;
        protected set => _quality = Math.Clamp(value, 0, 10);
    }

    private int _count;
    public int Count
    {
        get => _count;
        set
        {
            if (value < 1)
            {
                Console.WriteLine("The " + Name + " of " + ArmyOrigin.CountryOrigin.Name + " has been destroyed!");
            }
            _count = value;
        }
    }

    public int OperatorCount { get; private set; }
    public Army ArmyOrigin { get; set; }

    public ArmyUnit(double quality, int count, int operators)
    {
        Count = count;
        Quality = quality;
        OperatorCount = operators;
    }

    public virtual double GetPureDamage(ArmyUnit unit) => 0;

    public List<ArmyUnit> GetPosibleTargets(List<ArmyUnit> targets)
    {
        return targets.Where(x => GetPureDamage(x) > 0).ToList();
    }

    public int GetAttackDamage(ArmyUnit defender, Country battlePoint, out int troopsDamage, IBattleLogger logger)
    {
        //TODO:
        //add scouting factor and squad forming;
        //add suply(logistics) factor
        //add strategic strikes

        if (IsValid() == false || defender.IsValid() == false)
        {
            Console.WriteLine("Can't deal damage");
            Console.WriteLine("There is no such unit in army!");
            troopsDamage = 0;
            return 0;
        }

        double locationFactor = CalculateLocationFactor(defender, battlePoint);
        double qualityComparison = CalculateQualityFactor(defender);
        double scoutingComparison = CalculateScoutingFactor(defender);
        double quantityComparison = CalculateQuantityComparison(defender);

        int pureDamage = (int)Math.Ceiling(GetPureDamage(defender));

        var report = GenerateBattleReport(defender,
            out troopsDamage,
            locationFactor,
            qualityComparison,
            scoutingComparison,
            quantityComparison,
            pureDamage);

        logger.LogDamage(report);

        return report.EntitiesDestroyed;
    }

    private bool IsValid()
    {
        return Count > 0;
    }

    private BattleReport GenerateBattleReport(ArmyUnit defender, out int troopsDamage, double locationFactor, double qualityComparison, double scoutingComparison, double quantityComparison, int pureDamage)
    {
        int entitiesDestroyed = 0;
        troopsDamage = (int)(pureDamage * locationFactor * qualityComparison * quantityComparison * scoutingComparison);
        if (GetType() != typeof(Soldier))
        {
            entitiesDestroyed = troopsDamage;
            troopsDamage = Extensions.random.Next(entitiesDestroyed * defender.OperatorCount);
        }

        return new BattleReport(this, defender,
            pureDamage,
            locationFactor,
            qualityComparison,
            quantityComparison,
            scoutingComparison,
            entitiesDestroyed,
            troopsDamage);
    }

    private double CalculateQuantityComparison(ArmyUnit defender)
    {
        double quantityComparison = 1;

        if (GetType() == defender.GetType())
        {
            double attackerFactor = Count;
            double defenderFactor = defender.Count;
            if (Count > defender.Count) attackerFactor *= QUANTITY_BALANCE_FACTOR;
            else defenderFactor *= QUANTITY_BALANCE_FACTOR;

            quantityComparison = (double)attackerFactor / (double)defenderFactor;
        }

        return quantityComparison;
    }

    private double CalculateScoutingFactor(ArmyUnit defender)
    {
        return Extensions.DividingTheBigger(ArmyOrigin.Scouting, defender.ArmyOrigin.Scouting, 0.5);
    }

    private double CalculateQualityFactor(ArmyUnit defender)
    {
        double attackerQuality = Quality + ArmyOrigin.AverageQuality + ArmyOrigin.BattleSpirit;
        double defenderQuality = defender.ArmyOrigin.AverageQuality + defender.ArmyOrigin.BattleSpirit + defender.Quality;
        double qualityComparison = Extensions.DividingTheBigger(attackerQuality, defenderQuality, 0.5);
        return qualityComparison;
    }

    private double CalculateLocationFactor(ArmyUnit defender, Country battlePoint)
    {
        double locationFactor;
        if (battlePoint == ArmyOrigin.CountryOrigin) locationFactor = HOME_TERRITORY_BONUS;
        else if (battlePoint == defender.ArmyOrigin.CountryOrigin) locationFactor = ENEMY_TERRITORY_PENALTY;
        else locationFactor = NEUTRAL_TERRITORY_FACTOR;
        return locationFactor;
    }

    public void TryAttack(ArmyUnit defender, Country battlePoint, IBattleLogger logger)
    {
        if (GetPureDamage(defender) <= 0)
        {
            Console.WriteLine("There is no such unit in army!");
            return;
        }

        defender.ArmyOrigin.TakeDamage(defender, GetAttackDamage(defender, battlePoint, out int troopsDamage, logger), troopsDamage);
    }

    public void UpgradeUnits(float month)
    {

    }

    public void AddUnits(ArmyUnit unit)
    {
        double multiplier = (double)Count / (double)unit.Count;
        Count += unit.Count;
        Quality = (multiplier * Quality + unit.Quality) / 1 + multiplier;
    }

    public ArmyUnit FindPreferableTarget(List<ArmyUnit> list)
    {
        ArmyUnit bestTarget = list.FirstOrDefault();
        foreach (ArmyUnit unit in list)
        {
            if (GetPureDamage(unit) > GetPureDamage(bestTarget)) bestTarget = unit;
        }
        return bestTarget;
    }

    public override string ToString()
    {
        return $"{Name} |=| Count: {Count}, Quality: {Quality}";
    }

    public void TakeDamage(int damage, out bool destroyed)
    {
        destroyed = false;
        Count -= damage;
        if (Count <= 0)
        {
            destroyed = true;
        }
    }
}

#region CONCRETE_TROOPS
public class Soldier(double quality, int count) : ArmyUnit(quality, count, 1)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Tank => 0.8,
        FighterPlane => 0.6,
        Artillery => 0.3,
        AntiAircraftSystem => 2.5,
        MissileLauncherSystem => 1,
        Helicopter => 0.7,
        _ => base.GetPureDamage(unit),
    };
}

public class Tank(double quality, int count) : ArmyUnit(quality, count, 3)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Soldier => 3,
        Artillery => 2,
        AntiAircraftSystem => 2.5,
        MissileLauncherSystem => 2,
        Helicopter => 0.6,
        _ => base.GetPureDamage(unit),
    };
}

public class FighterPlane(double quality, int count) : ArmyUnit(quality, count, 1)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Soldier => 3,
        Tank => 2,
        Artillery => 2.5,
        AntiAircraftSystem => 2,
        MissileLauncherSystem => 2.5,
        Helicopter => 2.3,
        _ => base.GetPureDamage(unit),
    };
}

public class Artillery(double quality, int count) : ArmyUnit(quality, count, 3)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Soldier => 3,
        Tank => 2,
        AntiAircraftSystem => 2,
        MissileLauncherSystem => 2,
        _ => base.GetPureDamage(unit),
    };
}

public class AntiAircraftSystem(double quality, int count) : ArmyUnit(quality, count, 3)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        FighterPlane => 3,
        Helicopter => 3,
        _ => base.GetPureDamage(unit),
    };
}

public class MissileLauncherSystem(double quality, int count) : ArmyUnit(quality, count, 3)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Soldier => 4,
        Tank => 4,
        Artillery => 4,
        AntiAircraftSystem => 4,
        _ => base.GetPureDamage(unit),
    };
}

public class Helicopter(double quality, int count) : ArmyUnit(quality, count, 3)
{
    public override double GetPureDamage(ArmyUnit unit) => unit switch
    {
        Soldier => 3,
        Tank => 3,
        Artillery => 2,
        AntiAircraftSystem => 2,
        MissileLauncherSystem => 2,
        _ => base.GetPureDamage(unit),
    };
}
#endregion