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

public class Army : IMyObserver<UnitType, int>
{
    private double _battleSpirit;
    private double _scouting;

    public double AverageQuality => ArmyUnits.FindByType(UnitType.Soldier).Quality;
    public double BattleSpirit { get => _battleSpirit; private set { if (value >= 0 && value <= 10) _battleSpirit = value; } }
    public double Scouting { get => _scouting; private set { if (value >= 0 && value <= 10) _scouting = value; } }
    public List<ArmyUnit> ArmyUnits { get; private set; } = new List<ArmyUnit>();
    public Country CountryOrigin { get; private set; }
    public int ArmyCount => ArmyUnits.FindByType(UnitType.Soldier).Count;
    public bool CanFight => ArmyCount > 100;
    public List<Action<UnitType, int>> Actions { get; private set; } = new();

    public Army(Country country, double scouting, double quality, double battleSpirit, int armySize)
    {
        ArmyUnits.Add(new Soldier(quality, armySize, this));
        CountryOrigin = country;
        BattleSpirit = battleSpirit;
        Scouting = scouting;
    }

    public void TakeDamage(ArmyUnit unit, int damage, int troopsDamage)
    {
        if (ArmyUnits.FindByType(unit.TypeOfUnit) == null) 
        {
            Console.WriteLine("Can't take damage");
            Console.WriteLine("There is no such unit in " + CountryOrigin.Name + " army!");
            return; 
        }

        float emotionalDamage = (float)damage / unit.Count;

        ArmyUnits.FindByType(unit.TypeOfUnit).TakeDamage(damage, out bool unitDestroyed);
        ArmyUnits.FindByType(UnitType.Soldier).TakeDamage(troopsDamage, out bool armyDestroyed);

        BattleSpirit -= emotionalDamage;

        Invoke(unit.TypeOfUnit, damage);
        Invoke(UnitType.Soldier, troopsDamage);

        if (unitDestroyed) ArmyUnits.Remove(unit);
    }

    public void AddUnits(ArmyUnit unit)
    {
        if(ArmyUnits.FindByType(unit.TypeOfUnit) != null)
        {
            ArmyUnits.FindByType(unit.TypeOfUnit).AddUnits(unit);
            return;
        }
        ArmyUnits.Add(unit);
    }

    public void TrainTroops(float month)
    {

    }

    public void AttackCountry(Country target, Country place)
    {
        ArmyUnit unit = ArmyUnits.PickRandom();
        var posibleTargets = target.MainArmy.ArmyUnits.Intersect(unit.GetPosibleTargets(target.MainArmy.ArmyUnits)).ToList();
        var targetUnit = posibleTargets.PickRandom();
        if (Extensions.random.NextDouble() * 10 < Scouting * 0.5)
        {
            targetUnit = unit.FindPreferableTarget(posibleTargets);
        }
        unit.TryAttack(targetUnit, place);
    }

    public void Display()
    {
        Console.WriteLine("]:=:[]:=:[||||||||]:=:[]:=:[");
        Console.WriteLine("The army of " + CountryOrigin.Name);
        Console.WriteLine("]:::::===---------");
        Console.WriteLine(ArmyUnits.FindByType(UnitType.Soldier).ToString());
        Console.WriteLine("Army battle spirit: " + BattleSpirit);
        Console.WriteLine("]:::::===---------");
        foreach(ArmyUnit unit in ArmyUnits.Where(n => n.TypeOfUnit != UnitType.Soldier))
        {
            Console.WriteLine(unit.ToString());
        }
        Console.WriteLine("]:======:-:======:-:======:[");
    }

    public void Invoke(UnitType type, int number)
    {
        foreach (Action<UnitType, int> action in Actions)
        {
            action.Invoke(type, number);
        }
    }

    public void AddListener(Action<UnitType, int> action)
    {
        Actions.Add(action);
    }
}
