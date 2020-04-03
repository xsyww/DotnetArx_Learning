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

namespace DonetArxLearn
{
    public class CommandEntry
    {
        private GeomUtil _geomUtil = new GeomUtil();


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

        [CommandMethod("xsCreateEmp")]
        public void CreateEmp()
        {
            var db = HostApplicationServices.WorkingDatabase;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                var centerPt = new Point3d(10, 10, 0);
                var btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                var layerId = _geomUtil.CreateLayer();

                _geomUtil.CreateCircle(centerPt, trans, btr, layerId);
                _geomUtil.CreateText(centerPt, trans, btr, layerId);
                _geomUtil.CreateEllipse(centerPt, trans, btr, layerId);

                trans.Commit();
            }
        }

        #endregion

        [CommandMethod("xt")]
        public void Test()
        {
            Application.SetSystemVariable("IMAGEFRAME", Convert.ToInt16(1));
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("done!");
        }

        [CommandMethod("xsCreate")]
        public void CreateEmployee()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                string empName = "Earnest Shackleton";
                string divName = "Sales";
                double salary = 10000;
                Point3d position = Point3d.Origin;
                bool gotPosition = false;

                var prName = new PromptStringOptions($"Enter Employee Name <{empName}>");
                var prDiv = new PromptStringOptions($"Enter Employee Division <{divName}>");
                var prSal = new PromptDoubleOptions($"Enter Employee Salary <{salary}>");
                var prPos = new PromptPointOptions($"Enter Employee Position or ");

                prPos.Keywords.Add("Name");
                prPos.Keywords.Add("Division");
                prPos.Keywords.Add("Salary");
                prPos.AllowNone = false;

                PromptResult prNameRes, prDivRes;
                PromptDoubleResult prSalRes;
                PromptPointResult prPosRes;

                while (!gotPosition)
                {
                    prPosRes = ed.GetPoint(prPos);
                    if (prPosRes.Status == PromptStatus.OK)
                    {
                        gotPosition = true;
                        position = prPosRes.Value;
                    }
                    else if (prPosRes.Status == PromptStatus.Keyword)
                    {
                        if (prPosRes.StringResult == "Name")
                        {
                            prName.AllowSpaces = true;
                            prNameRes = ed.GetString(prName);
                            if (prNameRes.Status != PromptStatus.OK)
                                return;
                            else if (prNameRes.StringResult != "")
                                empName = prNameRes.StringResult;
                        }
                        else if (prPosRes.StringResult == "Division")
                        {
                            prDiv.AllowSpaces = true;
                            prDivRes = ed.GetString(prDiv);
                            if (prDivRes.Status != PromptStatus.OK)
                                return;
                            else if (!string.IsNullOrEmpty(prDivRes.StringResult))
                                divName = prDivRes.StringResult;
                        }
                        else if (prPosRes.StringResult == "Salary")
                        {
                            prSalRes = ed.GetDouble(prSal);
                            if (prSalRes.Status != PromptStatus.OK)
                                return;
                            else
                                salary = prSalRes.Value;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("***Error in getting a point, exiting!!*** \r\n");
                        return;
                    }
                }

                _geomUtil.CreateEmployee(empName, divName, salary, position);

                var manager = _geomUtil.GetDivision(divName);
                if (string.IsNullOrEmpty(manager))
                {
                    ed.WriteMessage("\r\n");
                    var prManagerName = new PromptStringOptions("No manager set for the division! Enter Manager Name ");
                    prManagerName.AllowSpaces = true;
                    var prManagerNameRes = ed.GetString(prManagerName);
                    if (prManagerNameRes.Status != PromptStatus.OK)
                        return;

                    _geomUtil.CreateDivision(divName, prManagerNameRes.StringResult);
                }

                trans.Commit();
            }
        }

        [CommandMethod("xsList")]
        public void EmployeeDetail()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var res = ed.GetSelection();
            if (res.Status == PromptStatus.OK)
            {
                using (var trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
                {
                    var ids = res.Value.GetObjectIds();
                    foreach (var id in ids)
                    {
                        ed.WriteMessage("\r\n");
                        if (trans.GetObject(id, OpenMode.ForRead) is BlockReference br)
                            ed.WriteMessage(_geomUtil.GetEmployeeInfo(br, trans)?.ToString());
                    }
                }
            }
        }

        [CommandMethod("xsShowForm")]
        public void ShowForm()
        {
            var dlg = new EmployeeDetailForm();
            Application.ShowModalDialog(dlg);
        }
    }
}
