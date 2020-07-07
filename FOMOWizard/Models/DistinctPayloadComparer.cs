using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class DistinctPayloadComparer : IEqualityComparer<Payload>
    {
        public bool Equals(Payload x, Payload y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(Payload payload)
        {
            return payload.ID.GetHashCode();
        }
    }
}
