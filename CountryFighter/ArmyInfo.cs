namespace CountryFighter;

public class ArmyInfo : IDisposable
{
    private readonly Country _country;

    public Dictionary<ArmyUnit, int> TotalUnitsLoses { get; private set; } = new ();
    public Dictionary<ArmyUnit, int> CurrentDayUnitsLoses { get; private set; } = new ();

    public ArmyInfo(Country country)
    {
        _country = country;
        country.MainArmy.OnTakeDamage += AddLoses;
    }

    public void AddLoses(ArmyUnit unit, int number)
    {
        TotalUnitsLoses.AddToValue(unit, number);
        CurrentDayUnitsLoses.AddToValue(unit, number);
    }

    public void StartNewDay()
    {
        CurrentDayUnitsLoses.Clear();
    }

    public void DisplayCurrentDayLosses()
    {
        Console.WriteLine("]]=================[[");
        Console.WriteLine("Last day loses of " + _country.Name + " army:");
        Console.WriteLine("]]==---");
        foreach(var unit in CurrentDayUnitsLoses)
        {
            Console.WriteLine("-" + unit.Value + " of " + unit.Key.ToString());
        }
        Console.WriteLine("]]=================[[");
    }

    public void DisplayCombinedLosses()
    {
        Console.WriteLine("]]=================[[");
        Console.WriteLine("Last day loses of " + _country.Name + " army:");
        Console.WriteLine("]]==---");
        foreach (var unit in TotalUnitsLoses)
        {
            CurrentDayUnitsLoses.TryGetValue(unit.Key, out int value);
            if (_country.MainArmy.ArmyUnits.FindByType(unit.Key.GetType()) != null)
                Console.WriteLine(unit.Value + " (+" + value + ") of " + unit.Key.ToString() + " - (" + _country.MainArmy.ArmyUnits.FindByType(unit.Key.GetType()).Count + " left)");
            else
                Console.WriteLine(unit.Value + " (+" + value + ") of " + unit.Key.ToString() + " - (0 left)");
        }
        Console.WriteLine("]]=================[[");
    }

    public void Dispose()
    {
        _country.MainArmy.OnTakeDamage -= AddLoses;
    }
}
