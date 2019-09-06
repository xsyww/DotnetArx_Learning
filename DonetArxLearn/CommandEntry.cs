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

namespace DonetArxLearn
{
    public class CommandEntry
    {
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

                var layerId = CreateLayer();
                
                CreateCircle(centerPt, trans, btr, layerId);
                CreateText(centerPt, trans, btr, layerId);
                CreateEllipse(centerPt, trans, btr, layerId);

                trans.Commit();
            }
        }

        [CommandMethod("xsCreateBlk")]
        public void CreateEmpBlkDefinition()
        {
            CreateBlkDefinition();
        }

        [CommandMethod("xsCreateInsert")]
        public void CreateInsert()
        {
            var db = HostApplicationServices.WorkingDatabase;
            using (var trans = db.TransactionManager.StartTransaction())
            {

                var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var blkId = bt.Has("EmployeeBlock") ?
                    bt["EmployeeBlock"] : CreateBlkDefinition();

                var ins = new BlockReference(Point3d.Origin, blkId);
                var curBtr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                curBtr.AppendEntity(ins);
                trans.AddNewlyCreatedDBObject(ins, true);

                trans.Commit();
            }
        }

        private ObjectId CreateBlkDefinition()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var layerId = CreateLayer();
            var blkId = ObjectId.Null;

            using (var trans = db.TransactionManager.StartTransaction())
            {

                var centerPt = new Point3d(10, 10, 0);
                var bt = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;

                if (bt.Has("EmployeeBlock"))
                    blkId = bt["EmployeeBlock"];
                else
                {
                    var blk = new BlockTableRecord();
                    blk.Name = "EmployeeBlock";
                    blk.Origin = centerPt;
                    blkId = bt.Add(blk);
                    trans.AddNewlyCreatedDBObject(blk, true);

                    CreateCircle(centerPt, trans, blk, layerId);
                    CreateText(centerPt, trans, blk, layerId);
                    CreateEllipse(centerPt, trans, blk, layerId);

                    trans.Commit();
                }
            }

            return blkId;
        }

        private ObjectId CreateCircle(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
        {
            var circle = new Circle(centerPoint, Vector3d.ZAxis, 2);
            circle.LayerId = layerId;
            var id = btr.AppendEntity(circle);
            trans.AddNewlyCreatedDBObject(circle, true);

            return id;
        }

        private ObjectId CreateText(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
        {
            var text = new DBText();
            text.Height = 0.7;
            text.TextString = "xu.shuai";
            text.HorizontalMode = TextHorizontalMode.TextCenter;
            text.VerticalMode = TextVerticalMode.TextVerticalMid;
            text.Position = centerPoint;
            text.AlignmentPoint = centerPoint;
            text.LayerId = layerId;
            var id = btr.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);

            return id;
        }

        private ObjectId CreateEllipse(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
        {
            var ellipse = new Ellipse(
                centerPoint,
                Vector3d.ZAxis,
                new Vector3d(3, 0, 0),
                0.5,
                0,
                Math.PI * 2);
            ellipse.LayerId = layerId;
            var id = btr.AppendEntity(ellipse);
            trans.AddNewlyCreatedDBObject(ellipse, true);

            return id;
        }

        private ObjectId CreateLayer()
        {
            ObjectId layerId = ObjectId.Null;

            var db = HostApplicationServices.WorkingDatabase;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var lt = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                if (lt.Has("EmployeeLayer"))
                    layerId = lt["EmployeeLayer"];
                else
                {
                    var ltr = new LayerTableRecord();
                    ltr.Name = "EmployeeLayer";
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 2);
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }

                trans.Commit();
            }

            return layerId;
        }
    }
}
