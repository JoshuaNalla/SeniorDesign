using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesign.src
{
    internal class Utils
    {
        /// <summary>
        /// Throws an error if an object is null.
        /// </summary>
        /// <param name="args"></param>
        public static void ThrowIfNull(params object[] args)
        {
            foreach (object arg in args)
            {
                ArgumentNullException.ThrowIfNull(arg);
            }
        }
    }
}
