using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BrokenRim
{
    [StaticConstructorOnStartup]
    public static class TestClass
    {

         static TestClass()
        {
            Log.Message("hello chat");
        }
    }
}
