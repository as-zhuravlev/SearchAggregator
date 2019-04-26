using System;
using System.Collections.Generic;
using System.Text;

namespace SearchAggregator.Core.Shared
{
    public class BaseException : Exception
    {
        public BaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
