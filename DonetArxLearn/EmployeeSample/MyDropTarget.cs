using Autodesk.AutoCAD.Geometry;
using DonetArxLearn.EmployeeSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonetArxLearn
{
    class MyDropTarget : Autodesk.AutoCAD.Windows.DropTarget
    {
        public override void OnDrop(DragEventArgs e)
        {
            var ctrl = e.Data.GetData(typeof(EmployeeDetailCtrl)) as EmployeeDetailCtrl;
            var util = new EmployeeGeomUtil();
            using (Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
                util.CreateEmployee(ctrl.txtName.Text, ctrl.txtDivision.Text, double.Parse(ctrl.txtSalary.Text), Point3d.Origin);
        }
    }
}
