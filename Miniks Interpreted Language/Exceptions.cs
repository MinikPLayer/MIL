using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIL
{
    public class MILException : Exception
    {
        public MILException(string msg, int line)
            :base(msg + " in line " + line)
        {

        }
    }

    public class VariableNotFoundException : MILException
    {
        public VariableNotFoundException(string varName, int line)
            :base("Variable " + varName + " not found", line)
        {

        }
    }
}
