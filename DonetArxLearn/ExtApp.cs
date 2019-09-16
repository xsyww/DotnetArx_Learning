using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn
{
    public class ExtApp : IExtensionApplication
    {
        #region 成员

        public static string sDivisionDefault = "Sales";
        public static string sDivisionManager = "Fiona Q. Farnsby";

        private ContextMenuExtension _menu = null;
        private PaletteSet _ps = null;

        #endregion

        #region 接口实现

        public void Initialize()
        {
            Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("xs app loaded!");
            AddContextMenu();
            CreatePalette();
        }

        public void Terminate()
        {
            Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("xs app unload!");
            RemoveContextMenu();
        }

        #endregion

        #region 右键菜单

        private void AddContextMenu()
        {
            try
            {
                _menu = new ContextMenuExtension();
                _menu.Title = "Acme Employee Menu";

                var mi = new MenuItem("Create Employee");
                mi.Click += Mi_Click;
                _menu.MenuItems.Add(mi);

                Application.AddDefaultContextMenuExtension(_menu);
            }
            catch
            {
            }
        }

        private void RemoveContextMenu()
        {
            try
            {
                if (_menu != null)
                {
                    Application.RemoveDefaultContextMenuExtension(_menu);
                    _menu = null;
                }
            }
            catch
            {
            }
        }

        private void Mi_Click(object sender, EventArgs e)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                new CommandEntry().CreateEmployee();
            }
        }

        #endregion

        #region 浮动窗口

        [CommandMethod("xsPalette")]
        private void CreatePalette()
        {
            _ps = new PaletteSet("Test Palette Set");
            _ps.MinimumSize = new System.Drawing.Size(300, 300);
            var ctrl = new EmployeeDetailCtrl();
            _ps.Add("test", ctrl);
            _ps.Visible = true;
            _ps.Dock = DockSides.Left;

        }

        #endregion
    }
}
