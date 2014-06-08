using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Interfaces
{
    public interface Repository
    {
        void Save();

        void Load(string fileName);
    }
}
