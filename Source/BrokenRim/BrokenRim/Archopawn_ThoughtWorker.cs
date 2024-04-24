using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Diagnostics;
using Verse;

namespace BrokenRimArchotech
{
    public class Archopawn_ThoughtWorker : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.def.defName != "Archopawn")
            {
                return false;
            }
            return true;
        }
    }
}
