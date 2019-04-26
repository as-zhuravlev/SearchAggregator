using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAggregator.Tests.Infrastructure
{
    static class GuidHelper
    {
        public static Guid Int(int i)
        {
            return new Guid(i, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
