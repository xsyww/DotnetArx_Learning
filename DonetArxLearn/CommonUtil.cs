using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn
{
    internal static class CommonUtil
    {
        public static void PrintMessage(string message)
        {
            var ed = Application.DocumentManager.MdiActiveDocument?.Editor;
            if (ed != null)
            {
                ed.WriteMessage(message);
            }
        }
    }
}
