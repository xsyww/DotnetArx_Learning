using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace DonetArxLearn
{
    public class LayoutPlotFunc
    {
        [CommandMethod("xsModelSpaceName")]
        public void ModelSapceName()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var editor = doc.Editor;
            Debug.Assert(BlockTableRecord.ModelSpace == "*MODEL-SPACE");
            editor.WriteMessage(BlockTableRecord.ModelSpace);
        }

        /// <summary>
        /// 列出所有布局
        /// </summary>
        [CommandMethod("xsListLayouts")]
        public void ListLayouts()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                var layoutDict = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;

                CommonUtil.PrintMessage("\nLayouts: ");
                foreach (var item in layoutDict)
                {
                    CommonUtil.PrintMessage($"\n {item.Key}");
                }
            }
        }

        /// <summary>
        /// 创建layout, 使用LayoutManager
        /// </summary>
        [CommandMethod("xsCreateLayout")]
        public void CreateLayout()
        {
            var db = Application.DocumentManager.MdiActiveDocument.Database;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                var layoutMgr = LayoutManager.Current;
                var id = layoutMgr.CreateLayout("MyTestLayout");

                // open it
                var layout = trans.GetObject(id, OpenMode.ForRead) as Layout;

                // set current
                if (!layout.TabSelected)
                {
                    layoutMgr.CurrentLayout = layout.LayoutName;
                    //layoutMgr.SetCurrentLayoutId(layout.Id);
                }

                // read some information
                CommonUtil.PrintMessage($"\nTab Order: {layout.TabOrder}" +
                                        $"\nTab Selected: {layout.TabSelected}" +
                                        $"\nBlock Table Record ID: {layout.BlockTableRecordId}");

                trans.Commit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [CommandMethod("xsImportLayout")]
        public void ImportLayout()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            var layoutName = "MyTestLayout";
            var dwgFile = @"E:\MyProject\DotnetArx_Learning\dwg\LayoutToBeImport.dwg";

            // Create a new database object and open the drawing into it
            var extDb = new Database(false, true);
            extDb.ReadDwgFile(dwgFile, FileOpenMode.OpenForReadAndAllShare, true, "");

            using (var extTrans = extDb.TransactionManager.StartTransaction())
            {
                var extLayoutDict = extTrans.GetObject(extDb.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                if (extLayoutDict.Contains(layoutName))
                {
                    // Get the layout and block objects from the external drawing
                    var extLayout = extLayoutDict.GetAt(layoutName).GetObject(OpenMode.ForRead) as Layout;
                    var extLayoutBlkRcd = extTrans.GetObject(extLayout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;

                    // Get the objects from the block associated with the layout
                    var idCollection = new ObjectIdCollection();
                    foreach (var id in extLayoutBlkRcd) 
                        idCollection.Add(id);

                    // Create a transation for current drawing
                    using (var curTrans = db.TransactionManager.StartTransaction())
                    {
                        var blkTbl = curTrans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                        using (var blkTblRcd = new BlockTableRecord())
                        {
                            var layoutCount = extLayoutDict.Count - 1;
                            blkTblRcd.Name = $"*Paper_Space{layoutCount}";
                            blkTbl.Add(blkTblRcd);
                            curTrans.AddNewlyCreatedDBObject(blkTblRcd, true);
                            extDb.WblockCloneObjects(idCollection, blkTblRcd.ObjectId, new IdMapping(), DuplicateRecordCloning.Ignore, false);

                            // Create a new layout and then copy properties between drawings
                            var layoutDict = curTrans.GetObject(db.LayoutDictionaryId, OpenMode.ForWrite) as DBDictionary;
                            using (var layout  = new Layout())
                            {
                                layout.LayoutName = layoutName;
                                layout.AddToLayoutDictionary(db, blkTblRcd.ObjectId);
                                curTrans.AddNewlyCreatedDBObject(layout, true);
                                layout.CopyFrom(extLayout);

                                var plsDict = curTrans.GetObject(db.PlotSettingsDictionaryId, OpenMode.ForRead) as DBDictionary;

                                // Check to see if a named page setup was assigned to the layout 
                                // if so then copy the page setup setting
                                if (!string.IsNullOrEmpty(layout.PlotSettingsName))
                                {
                                    if (!plsDict.Contains(layout.PlotSettingsName))
                                    {
                                        // change plsDict OpenMode from read to write
                                        curTrans.GetObject(db.PlotSettingsDictionaryId, OpenMode.ForWrite);

                                        using (var pls = new PlotSettings(layout.ModelType))
                                        {
                                            pls.PlotSettingsName = layout.PlotSettingsName;
                                            pls.AddToPlotSettingsDictionary(db);
                                            curTrans.AddNewlyCreatedDBObject(pls, true);

                                            // Get PlotSettings from external drawing where external layout used
                                            var extPlsDict = extTrans.GetObject(extDb.PlotSettingsDictionaryId, OpenMode.ForRead) as DBDictionary;
                                            var extPls = extPlsDict.GetAt(layout.PlotSettingsName).GetObject(OpenMode.ForRead) as PlotSettings;

                                            // Copy from it
                                            pls.CopyFrom(extPls);
                                        }
                                    }
                                }
                            }
                        }

                        doc.Editor.Regen();
                        curTrans.Commit();
                    }
                }
                else
                {
                    doc.Editor.WriteMessage($"\nLayout {layoutName} could not be imported from {dwgFile}.");
                }

                extTrans.Abort();
            }

            extDb.Dispose();
        }
    }
}
