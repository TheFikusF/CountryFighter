using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryFighter
{
    public class ArmyInfo
    {
        public Country ConflictSide { get; set; }

        public Dictionary<UnitType, int> TotalUnitsLoses { get; private set; } = new Dictionary<UnitType, int>();
        public Dictionary<UnitType, int> CurrentDayUnitsLoses { get; private set; } = new Dictionary<UnitType, int>();

        public ArmyInfo(Country country)
        {
            ConflictSide = country;
            country.MainArmy.AddListener(AddLoses);
        }

        public void AddLoses(UnitType unit, int number)
        {
            TotalUnitsLoses.AddToValue(unit, number);
            CurrentDayUnitsLoses.AddToValue(unit, number);
        }

        public void StartNewDay()
        {
            CurrentDayUnitsLoses = new Dictionary<UnitType, int>();
        }

        public void DisplayCurrentDayLosses()
        {
            Console.WriteLine("]]=================[[");
            Console.WriteLine("Last day loses of " + ConflictSide.Name + " army:");
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
            Console.WriteLine("Last day loses of " + ConflictSide.Name + " army:");
            Console.WriteLine("]]==---");
            foreach (var unit in TotalUnitsLoses)
            {
                int value = 0;
                CurrentDayUnitsLoses.TryGetValue(unit.Key, out value);
                if(ConflictSide.MainArmy.ArmyUnits.FindByType(unit.Key) != null)
                    Console.WriteLine(unit.Value + " (+" + value + ") of " + unit.Key.ToString() + " - (" + ConflictSide.MainArmy.ArmyUnits.FindByType(unit.Key).Count + " left)");
                else
                    Console.WriteLine(unit.Value + " (+" + value + ") of " + unit.Key.ToString() + " - (0 left)");
            }
            Console.WriteLine("]]=================[[");
        }
    }
}
