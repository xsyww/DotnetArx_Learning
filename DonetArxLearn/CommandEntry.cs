using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows.Data;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Ribbon;
using Autodesk.AutoCAD.Customization;
using DonetArxLearn.EmployeeSample;

namespace DonetArxLearn
{
    public class CommandEntry
    {
        #region Test Function

        [CommandMethod("xsHelloWorld")]
        public void HelloWorld()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Hello World!");
        }

        [CommandMethod("xsSelectPoint")]
        public void SelectPoint()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var ppo = new PromptPointOptions("Select a point");
            var ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK)
                ed.WriteMessage("Error!");
            else
                ed.WriteMessage("Your select point: " + ppr.Value.ToString());
        }

        [CommandMethod("xsGetDistance")]
        public void GetDistance()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var pdo = new PromptDistanceOptions("Find distance, select first point:");
            var pdr = ed.GetDistance(pdo);
            if (pdr.Status != PromptStatus.OK)
                ed.WriteMessage("Error!");
            else
                ed.WriteMessage("The distance is: " + pdr.Value.ToString());
        }

        #endregion
    }
}
