namespace CountryFighter;

public interface IDamagable
{
    void TakeDamage(int damage, out bool destroyed);
}
