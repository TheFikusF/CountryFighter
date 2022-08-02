using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryFighter
{
    public interface IDamagable
    {
        void TakeDamage(int damage, out bool destroyed);
    }
}
