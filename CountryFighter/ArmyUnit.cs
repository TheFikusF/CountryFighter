namespace CountryFighter;

public enum UnitType
{
    Soldier, Tank, FighterPlane, Artillery, AntiAircraftSystem, MissileLauncherSystem, Helicopter
}

public abstract class ArmyUnit : IDamagable
{
    private const double HOME_TERRITORY_BONUS = 2.0;
    private const double ENEMY_TERRITORY_PENALTY = 0.8;
    private const double NEUTRAL_TERRITORY_FACTOR = 1.0;
    private const double QUANTITY_BALANCE_FACTOR = 0.5;

    private double _quality;
    public double Quality { get => _quality; protected set { if (value >= 0 && value <= 10) _quality = value; } }
    private int _count;
    public int Count 
    { 
        get => _count; 
        set 
        {   
            if(value < 1)
            {
                Console.WriteLine("The " + TypeOfUnit.ToString() + " of " + ArmyOrigin.CountryOrigin.Name + " has been destroyed!");
            }
            _count = value; 
        } 
    }
    public int OperatorCount { get; private set; }
    public UnitType TypeOfUnit { get; private set; }
    public Army ArmyOrigin { get; private set; }
    protected Dictionary<UnitType, double> _damageTable = new Dictionary<UnitType, double>();

    public ArmyUnit(double quality, int count, UnitType type, Army origin, int operators)
    {
        Count = count;
        Quality = quality;
        TypeOfUnit = type;
        _damageTable.Add(TypeOfUnit, 1);
        ArmyOrigin = origin;
        OperatorCount = operators;
    }

    public double GetPureDamage(ArmyUnit unit) => _damageTable.ContainsKey(unit.TypeOfUnit) ? _damageTable[unit.TypeOfUnit] : 0;

    public List<ArmyUnit> GetPosibleTargets(List<ArmyUnit> targets)
    {
        List<ArmyUnit> t = new List<ArmyUnit>();
        foreach (ArmyUnit target in targets)
        {
            if (GetPureDamage(target) > 0) t.Add(target);
        }
        return t;
    }

    public int GetAttackDamage(ArmyUnit defender, Country battlePoint, out int troopsDamage, IBattleLogger logger)
    {
        //TODO:
        //add scouting factor and squad forming;
        //add suply(logistics) factor
        //add strategic strikes

        if (Count == 0 || defender.Count == 0)
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

    private BattleReport GenerateBattleReport(ArmyUnit defender, out int troopsDamage, double locationFactor, double qualityComparison, double scoutingComparison, double quantityComparison, int pureDamage)
    {
        int entitiesDestroyed = 0;
        troopsDamage = (int)(pureDamage * locationFactor * qualityComparison * quantityComparison * scoutingComparison);
        if (defender.TypeOfUnit != UnitType.Soldier)
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

        if (TypeOfUnit == defender.TypeOfUnit)
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
        foreach(ArmyUnit unit in list)
        {
            if(GetPureDamage(unit) > GetPureDamage(bestTarget)) bestTarget = unit;
        }
        return bestTarget;
    }

    public override string ToString()
    {
        return TypeOfUnit.ToString() + " |=| Count: " + Count + ", Quality: " + Quality;
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
public class Soldier : ArmyUnit
{
    public Soldier(double quality, int count, Army army) : base(quality, count, UnitType.Soldier, army, 1)
    {
        _damageTable.Add(UnitType.Tank, 0.8);
        _damageTable.Add(UnitType.FighterPlane, 0.6);
        _damageTable.Add(UnitType.Artillery, 0.3);
        _damageTable.Add(UnitType.AntiAircraftSystem, 2.5);
        _damageTable.Add(UnitType.MissileLauncherSystem, 1);
        _damageTable.Add(UnitType.Helicopter, 0.7);
    }
}

public class Tank : ArmyUnit
{
    public Tank(double quality, int count, Army army) : base(quality, count, UnitType.Tank, army, 3)
    {
        _damageTable.Add(UnitType.Soldier, 3);
        _damageTable.Add(UnitType.Artillery, 2);
        _damageTable.Add(UnitType.AntiAircraftSystem, 2.5);
        _damageTable.Add(UnitType.MissileLauncherSystem, 2);
        _damageTable.Add(UnitType.Helicopter, 0.6);
    }
}

public class FighterPlane : ArmyUnit
{
    public FighterPlane(double quality, int count, Army army) : base(quality, count, UnitType.FighterPlane, army, 1)
    {
        _damageTable.Add(UnitType.Soldier, 3);
        _damageTable.Add(UnitType.Tank, 2);
        _damageTable.Add(UnitType.Artillery, 2.5);
        _damageTable.Add(UnitType.AntiAircraftSystem, 2);
        _damageTable.Add(UnitType.MissileLauncherSystem, 2.5);
        _damageTable.Add(UnitType.Helicopter, 2.3);
    }
}

public class Artillery : ArmyUnit
{
    public Artillery(double quality, int count, Army army) : base(quality, count, UnitType.Artillery, army, 3)
    {
        _damageTable.Add(UnitType.Soldier, 3);
        _damageTable.Add(UnitType.Tank, 2);
        _damageTable.Add(UnitType.AntiAircraftSystem, 2);
        _damageTable.Add(UnitType.MissileLauncherSystem, 2);
    }
}

public class AntiAircraftSystem : ArmyUnit
{
    public AntiAircraftSystem(double quality, int count, Army army) : base(quality, count, UnitType.AntiAircraftSystem, army, 3)
    {
        _damageTable.Add(UnitType.FighterPlane, 3);
        _damageTable.Add(UnitType.Helicopter, 3);
    }
}

public class MissileLauncherSystem : ArmyUnit
{
    public MissileLauncherSystem(double quality, int count, Army army) : base(quality, count, UnitType.MissileLauncherSystem, army, 3)
    {
        _damageTable.Add(UnitType.Soldier, 4);
        _damageTable.Add(UnitType.Tank, 4);
        _damageTable.Add(UnitType.FighterPlane, 2);
        _damageTable.Add(UnitType.Artillery, 4);
        _damageTable.Add(UnitType.AntiAircraftSystem, 4);
        _damageTable.Add(UnitType.Helicopter, 2);
    }
}

public class Helicopter : ArmyUnit
{
    public Helicopter(double quality, int count, Army army) : base(quality, count, UnitType.Helicopter, army, 3)
    {
        _damageTable.Add(UnitType.Soldier, 1.5);
        _damageTable.Add(UnitType.Tank, 1.5);
        _damageTable.Add(UnitType.FighterPlane, 1.5);
        _damageTable.Add(UnitType.Artillery, 2);
        _damageTable.Add(UnitType.AntiAircraftSystem, 2);
        _damageTable.Add(UnitType.MissileLauncherSystem, 2);
    }
}
#endregion