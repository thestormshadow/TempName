using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.EntitiesManager
{
    public interface IMigration
    {
        void Upgrade();
    }
}
