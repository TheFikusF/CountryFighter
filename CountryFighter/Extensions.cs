﻿namespace CountryFighter;

public static class Extensions
{
    public static Random random = new Random();

    public static ArmyUnit FindByType(this List<ArmyUnit> units, Type type)
    {
        return units.FirstOrDefault(n => n.GetType() == type);
    }

    public static ArmyUnit FindByType<T>(this List<ArmyUnit> units) where T : ArmyUnit
    {
        return units.FirstOrDefault(n => n is T);
    }

    public static T PickRandom<T>(this List<T> list) => list[random.Next(list.Count)];

    public static void AddToValue<T>(this Dictionary<T, int> dict, T type, int value)
    {
        if (value < 1)
        {
            return;
        }

        if (dict.ContainsKey(type))
        {
            dict[type] += value;
            return;
        }

        dict.Add(type, value);
    }

    public static double DividingTheBigger(double number1, double number2, double multiplier)
    {
        if (number1 > number2) return (number1 * multiplier) / number2;
        else return number1 / (number2 * multiplier);
    }
}

public interface IMyObserver<T1, T2>
{
    List<Action<T1, T2>> Actions { get; }

    void AddListener(Action<T1, T2> action);
    void Invoke(T1 a, T2 b);
}
