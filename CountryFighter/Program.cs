using CountryFighter;

var logger = new ConsoleBattleLogger();

var country1 = new Country("Genuia");
country1.AssignArmy(new Army(country1, 8, 8, 8, 1000));
country1.MainArmy.AddUnits(new Artillery(8, 60));
country1.MainArmy.AddUnits(new FighterPlane(8, 40));
country1.MainArmy.AddUnits(new Tank(8, 200));
country1.MainArmy.AddUnits(new AntiAircraftSystem(8, 100));
country1.MainArmy.AddUnits(new MissileLauncherSystem(8, 20));
logger.Display(country1.MainArmy);

var country2 = new Country("Idiotia");
country2.AssignArmy(new Army(country2, 4, 4, 4, 3000));
country2.MainArmy.AddUnits(new Artillery(4, 180));
country2.MainArmy.AddUnits(new FighterPlane(4, 120));
country2.MainArmy.AddUnits(new Tank(4, 600));
country2.MainArmy.AddUnits(new AntiAircraftSystem(4, 300));
logger.Display(country2.MainArmy);

BattleManager.Attack(country2, country1, logger);

logger.Display(country1.MainArmy);
logger.Display(country2.MainArmy);

