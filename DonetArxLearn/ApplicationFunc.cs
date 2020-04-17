using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn
{
    class ApplicationFunc
    {
        /// <summary>
        /// 获取系统变量
        /// </summary>
        public static void GetSysVar()
        {
            var obj1 = Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("extmin");
            if (obj1 != null)
            {
                var ptMin = (Point3d)obj1;
            }
        }

        /// <summary>
        /// 设置系统变量
        /// </summary>
        public static void SetSysVar()
        {
            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("IMAGEFRAME", Convert.ToInt16(1));
            //Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("IMAGEFRAME", Convert.ToInt16(1));
        }
    }
}
