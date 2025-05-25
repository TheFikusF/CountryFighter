using Xunit;
using FluentAssertions;

namespace CountryFighter.Tests;

public class ArmyTests
{
    private Country CreateTestCountry(string name = "TestCountry")
    {
        return new Country(name);
    }

    [Fact]
    public void Constructor_ShouldInitializeArmyCorrectly()
    {
        // Arrange
        var country = CreateTestCountry();
        double scouting = 5;
        double quality = 7;
        double battleSpirit = 8;
        int armySize = 1000;

        // Act
        var army = new Army(country, scouting, quality, battleSpirit, armySize);

        // Assert
        army.CountryOrigin.Should().Be(country);
        army.Scouting.Should().Be(scouting);
        army.BattleSpirit.Should().Be(battleSpirit);
        army.ArmyCount.Should().Be(armySize);
        army.AverageQuality.Should().Be(quality);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void BattleSpirit_ShouldBeConstrainedBetween0And10(double invalidValue)
    {
        // Arrange
        var army = new Army(CreateTestCountry(), 5, 5, 5, 100);

        // Act
        army.TakeDamage(new Soldier(5, 10, army), 1000, 0);

        // Assert
        army.BattleSpirit.Should().BeGreaterThanOrEqualTo(0);
        army.BattleSpirit.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public void AddUnits_ShouldMergeWithExistingUnits()
    {
        // Arrange
        var army = new Army(CreateTestCountry(), 5, 5, 5, 100);
        var newSoldiers = new Soldier(5, 50, army);

        // Act
        army.AddUnits(newSoldiers);

        // Assert
        army.ArmyCount.Should().Be(150);
    }

    [Fact]
    public void TakeDamage_ShouldReduceUnitCountAndBattleSpirit()
    {
        // Arrange
        var army = new Army(CreateTestCountry(), 5, 5, 5, 100);
        var soldiers = army.ArmyUnits.FindByType(UnitType.Soldier);
        double initialBattleSpirit = army.BattleSpirit;

        // Act
        army.TakeDamage(soldiers, 20, 10);

        // Assert
        army.BattleSpirit.Should().BeLessThan(initialBattleSpirit);
        army.ArmyCount.Should().BeLessThan(100);
    }

    [Fact]
    public void CanFight_ShouldReturnCorrectValue()
    {
        // Arrange
        var armyLarge = new Army(CreateTestCountry(), 5, 5, 5, 150);
        var armySmall = new Army(CreateTestCountry(), 5, 5, 5, 50);

        // Assert
        armyLarge.CanFight.Should().BeTrue();
        armySmall.CanFight.Should().BeFalse();
    }

    [Fact]
    public void TakeDamage_ShouldHandleNonExistentUnit()
    {
        // Arrange
        var army = new Army(CreateTestCountry(), 5, 5, 5, 100);
        var nonExistentUnit = new Artillery(5, 10, army);

        // Act
        army.TakeDamage(nonExistentUnit, 20, 10);

        // Assert
        army.ArmyCount.Should().Be(100); // Should remain unchanged
    }
}