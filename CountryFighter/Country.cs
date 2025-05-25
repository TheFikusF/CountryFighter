namespace CountryFighter;

public class Country
{
    public string Name { get; private set; }
    public Army MainArmy { get; private set; }

    public Country() { }
    public Country(string name, Army army)
    {
        Name = name;
        MainArmy = army;
    }

    public Country(string name)
    {
        Name = name;
    }

    public void AssignArmy(Army army) => MainArmy = army;
}

public class Army
{
    public delegate void DamageEvent(ArmyUnit unit, int amount);

    private double _battleSpirit;
    private double _scouting;

    public double AverageQuality => ArmyUnits.FindByType<Soldier>().Quality;
    public double BattleSpirit { get => _battleSpirit; private set { if (value >= 0 && value <= 10) _battleSpirit = value; } }
    public double Scouting { get => _scouting; private set { if (value >= 0 && value <= 10) _scouting = value; } }
    public List<ArmyUnit> ArmyUnits { get; private set; } = new List<ArmyUnit>();
    public Country CountryOrigin { get; private set; }
    public int ArmyCount => ArmyUnits.FindByType<Soldier>().Count;
    public bool CanFight => ArmyCount > 100;

    public event DamageEvent OnTakeDamage;

    public Army(Country country, double scouting, double quality, double battleSpirit, int armySize)
    {
        AddUnits(new Soldier(quality, armySize));
        CountryOrigin = country;
        BattleSpirit = battleSpirit;
        Scouting = scouting;
    }

    public void TakeDamage(ArmyUnit unit, int damage, int troopsDamage)
    {
        if (ArmyUnits.FindByType(unit.GetType()) == null) 
        {
            Console.WriteLine("Can't take damage");
            Console.WriteLine("There is no such unit in " + CountryOrigin.Name + " army!");
            return; 
        }

        float emotionalDamage = (float)damage / unit.Count;

        var damagedUnit = ArmyUnits.FindByType(unit.GetType());
        var soldiers = ArmyUnits.FindByType<Soldier>();

        damagedUnit.TakeDamage(damage, out bool unitDestroyed);
        soldiers.TakeDamage(troopsDamage, out bool armyDestroyed);

        BattleSpirit -= emotionalDamage;

        OnTakeDamage?.Invoke(damagedUnit, damage);
        OnTakeDamage?.Invoke(soldiers, troopsDamage);

        if (unitDestroyed)
        {
            ArmyUnits.Remove(unit);
        }
    }

    public void AddUnits(ArmyUnit unit)
    {
        unit.ArmyOrigin = this;

        if (ArmyUnits.FindByType(unit.GetType()) != null)
        {
            ArmyUnits.FindByType(unit.GetType()).AddUnits(unit);
            return;
        }
        ArmyUnits.Add(unit);
    }

    public void TrainTroops(float month)
    {

    }

    public void AttackCountry(Country target, Country place, IBattleLogger logger)
    {
        var unit = ArmyUnits.PickRandom();
        var posibleTargets = target.MainArmy.ArmyUnits.Intersect(unit.GetPosibleTargets(target.MainArmy.ArmyUnits)).ToList();

        var targetUnit = Extensions.random.NextDouble() * 10 < Scouting * 0.5
            ? unit.FindPreferableTarget(posibleTargets)
            : posibleTargets.PickRandom();

        unit.TryAttack(targetUnit, place, logger);
    }
}
