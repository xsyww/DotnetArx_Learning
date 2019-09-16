using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn
{
    class GeomUtil
    {
        private string _acmeDictName = "ACME_DIVISION";
        private string _xrecName = "Department Manager";


        public ObjectId CreateCircle(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
        {
            var circle = new Circle(centerPoint, Vector3d.ZAxis, 2);
            circle.LayerId = layerId;
            var id = btr.AppendEntity(circle);
            trans.AddNewlyCreatedDBObject(circle, true);

            return id;
        }

        public ObjectId CreateText(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
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

        public ObjectId CreateEllipse(Point3d centerPoint, Transaction trans, BlockTableRecord btr, ObjectId layerId)
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

        public ObjectId CreateLayer()
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

        public ObjectId CreateBlkDefinition()
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
                    //CreateText(centerPt, trans, blk, layerId);
                    CreateAttributeDef(centerPt, trans, blk, layerId, db.Textstyle);
                    CreateEllipse(centerPt, trans, blk, layerId);

                    trans.Commit();
                }
            }

            return blkId;
        }

        public void CreateAttributeDef(Point3d centerPt, Transaction trans, BlockTableRecord btr, ObjectId layerId, ObjectId tsId)
        {
            var attDef = new AttributeDefinition(centerPt, "NoName", "Name:", "Enter Name", tsId);
            attDef.LayerId = layerId;

            attDef.HorizontalMode = TextHorizontalMode.TextCenter;
            attDef.VerticalMode = TextVerticalMode.TextVerticalMid;
            attDef.Height = 0.7;
            attDef.AlignmentPoint = centerPt;
            var id = btr.AppendEntity(attDef);
            trans.AddNewlyCreatedDBObject(attDef, true);
        }

        public DBDictionary CreateDictionary(Transaction trans, DBDictionary nod, string dicName)
        {
            DBDictionary acmeDict;
            if (nod.Contains(dicName))
                acmeDict = trans.GetObject(nod.GetAt(dicName), OpenMode.ForWrite) as DBDictionary;
            else
            {
                acmeDict = new DBDictionary();
                var dicId = nod.SetAt(dicName, acmeDict);
                trans.AddNewlyCreatedDBObject(acmeDict, true);
            }

            return acmeDict;
        }

        public ObjectId CreateXRecWithData(Transaction trans, DBDictionary dict, string name, ResultBuffer data)
        {
            Xrecord xrec = null;
            if (dict.Contains(name))
                xrec = trans.GetObject(dict.GetAt(name), OpenMode.ForWrite) as Xrecord;
            else
            {
                xrec = new Xrecord();
                dict.SetAt(name, xrec);
                trans.AddNewlyCreatedDBObject(xrec, true);
            }

            xrec.Data = data;

            return xrec.ObjectId;
        }

        public void CreateDivision()
        {
            var db = HostApplicationServices.WorkingDatabase;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var nod = trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                DBDictionary acmeDict = CreateDictionary(trans, nod, _acmeDictName);

                var data = new ResultBuffer(new TypedValue((int)DxfCode.Text, "Randolph P. Brokwell"));
                CreateXRecWithData(trans, acmeDict, _xrecName, data);

                trans.Commit();
            }
        }

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

                ins.CreateExtensionDictionary();
                var extDict = trans.GetObject(ins.ExtensionDictionary, OpenMode.ForWrite) as DBDictionary;

                var data = new ResultBuffer(
                    new TypedValue((int)DxfCode.Text, "Earest Shackleton"),
                    new TypedValue((int)DxfCode.Real, 72000),
                    new TypedValue((int)DxfCode.Text, "Sales"));
                CreateXRecWithData(trans, extDict, "EmploeeInfo", data);

                trans.Commit();
            }
        }

        public ObjectId CreateEmployee(string name, string division, double salary, Point3d position)
        {
            var db = HostApplicationServices.WorkingDatabase;
            var blkId = CreateBlkDefinition();
            ObjectId idIns = ObjectId.Null;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                var ins = new BlockReference(position, blkId);
                var curBtr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                idIns = curBtr.AppendEntity(ins);
                trans.AddNewlyCreatedDBObject(ins, true);

                var empBlk = trans.GetObject(blkId, OpenMode.ForRead) as BlockTableRecord;
                foreach (var id in empBlk)
                {
                    if (trans.GetObject(id, OpenMode.ForRead, false) is AttributeDefinition attDef)
                    {
                        var attRef = new AttributeReference();
                        attRef.SetPropertiesFrom(attDef);
                        attRef.Position = position;
                        attRef.Height = attDef.Height;
                        attRef.Rotation = attDef.Rotation;
                        attRef.Tag = attDef.Tag;
                        attRef.TextString = name;
                        attRef.HorizontalMode = attDef.HorizontalMode;
                        attRef.VerticalMode = attDef.VerticalMode;
                        attRef.AlignmentPoint = position;
                        attRef.AdjustAlignment(db);

                        ins.AttributeCollection.AppendAttribute(attRef);
                        trans.AddNewlyCreatedDBObject(attRef, true);
                    }
                }

                ins.CreateExtensionDictionary();
                var extDict = trans.GetObject(ins.ExtensionDictionary, OpenMode.ForWrite) as DBDictionary;

                var data = new ResultBuffer(
                    new TypedValue((int)DxfCode.Text, name),
                    new TypedValue((int)DxfCode.Real, salary),
                    new TypedValue((int)DxfCode.Text, division));
                CreateXRecWithData(trans, extDict, "EmploeeInfo", data);

                trans.Commit();
            }

            return idIns;
        }

        public ObjectId CreateDivision(string division, string manager)
        {
            var db = HostApplicationServices.WorkingDatabase;
            var id = ObjectId.Null;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var nod = trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                var acmeDict = CreateDictionary(trans, nod, _acmeDictName);

                var data = new ResultBuffer(new TypedValue((int)DxfCode.Text, manager));
                id = CreateXRecWithData(trans, acmeDict, division, data);

                trans.Commit();
            }

            return id;
        }

        public string GetDivision(string division)
        {
            var db = HostApplicationServices.WorkingDatabase;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var nod = trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                var acmeDict = CreateDictionary(trans, nod, _acmeDictName);

                if (acmeDict.Contains(division))
                {
                    var xRec = trans.GetObject(acmeDict.GetAt(division), OpenMode.ForWrite) as Xrecord;
                    var datas = xRec.Data.AsArray();
                    if (datas.Length > 0)
                        return datas[0].Value.ToString();
                }

                return "";
            }
        }

        public Employee GetEmployeeInfo(BlockReference br, Transaction trans)
        {
            var dict = trans.GetObject(br.ExtensionDictionary, OpenMode.ForRead) as DBDictionary;
            var xRec = trans.GetObject(dict.GetAt("EmploeeInfo"), OpenMode.ForRead) as Xrecord;
            var datas = xRec.Data.AsArray();
            if (datas == null || datas.Length < 1)
                return null;

            return new Employee
            {
                Name = (string)datas[0].Value,
                Salary = (double)datas[1].Value,
                Division = (string)datas[2].Value
            };
        }
    }
}
