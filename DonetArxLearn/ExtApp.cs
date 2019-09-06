using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn
{
    public class ExtApp : IExtensionApplication
    {
        public void Initialize()
        {
            Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("xs app loaded!");
        }

        public void Terminate()
        {
            Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("xs app unload!");
        }
    }
}
