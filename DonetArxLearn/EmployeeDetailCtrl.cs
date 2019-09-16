using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    }
}
