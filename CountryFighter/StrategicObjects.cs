namespace CountryFighter
{
    public class StrategicObject : IDamagable
    {
        public string Name { get; private set; }
        public double MaxHealth { get; private set; }
        public double Health { get; private set; }

        public StrategicObject(string name, double maxhp)
        {
            Name = name;
            MaxHealth = maxhp;
            Health = MaxHealth;
        }

        public void TakeDamage(int damage, out bool destroyed)
        {
            destroyed = false;
            Health -= damage;
            if(Health <= 0)
            {
                destroyed = true;
            }
        }
    }
}
