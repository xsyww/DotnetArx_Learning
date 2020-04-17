using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonetArxLearn.EmployeeSample
{
    public partial class EmployeeDetailForm : Form
    {
        public EmployeeDetailForm()
        {
            InitializeComponent();
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            this.Hide();
            var prEnt = new PromptEntityOptions("Select an Employee");
            var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            var res = ed.GetEntity(prEnt);
            this.Show();
            if (res.Status == PromptStatus.OK)
            {
                using (var trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
                {
                    if (trans.GetObject(res.ObjectId, OpenMode.ForRead) is BlockReference br)
                    {
                        var empInfo = new EmployeeGeomUtil().GetEmployeeInfo(br, trans);
                        if (empInfo == null)
                            return;

                        txtName.Text = empInfo.Name;
                        txtDivision.Text = empInfo.Division;
                        txtSalary.Text = empInfo.Salary.ToString();
                    }
                }
            }
        }
    }
}
