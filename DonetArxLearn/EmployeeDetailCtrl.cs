using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AAA = Autodesk.AutoCAD.ApplicationServices;

namespace DonetArxLearn
{
    public partial class EmployeeDetailCtrl : UserControl
    {
        public EmployeeDetailCtrl()
        {
            InitializeComponent();
        }

        private void Label5_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DoDragDrop(this, this, DragDropEffects.All, new MyDropTarget());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AAA.Application.SetSystemVariable("IMAGEFRAME", Convert.ToInt16(0));
            var editor = AAA.Application.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("done!");
        }
    }
}
