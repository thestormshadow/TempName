using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.EntitiesManager.Entities
{
    public interface IEntity
    {
        string ID { get; set; }
        DateTime ModifiedOn { get; set; }
        bool Status { get; set; }
    }
}
