using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: PerDocumentClass(typeof(DonetArxLearn.GetStartWithVisualStudio.PerDocData))]

namespace DonetArxLearn.GetStartWithVisualStudio
{
    // define a global class for constant values
    public static class CSPerDocConsts
    {
        public const string dbLocVarName = "dbLoc";
    }

    // define the main class
    public class PerDocCommands
    {
        [CommandMethod("DatabaseLocation")]
        public void DatabaseLocation()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            PerDocData pdd = (PerDocData)doc.UserData[CSPerDocConsts.dbLocVarName];
            if (pdd == null)
                doc.Editor.WriteMessage("\nNo user data assigned.");
            else
                doc.Editor.WriteMessage($"\n{pdd.DbLocation}");
        }
    }

    // Defines the class that will be used to initialize the per document data
    public class PerDocData
    {
        private string _dbLocation;

        public string DbLocation { get { return _dbLocation; } }

        public PerDocData(Document document)
        {
            _dbLocation = "";
            document.UserData.Add(CSPerDocConsts.dbLocVarName, this);
        }

        public static PerDocData Create(Document document)
        {
            var pdd = new PerDocData(document);
            pdd._dbLocation = $"C:\\MyApp\\ProjectData\\{document.Name.Remove(document.Name.Length - 4)}.db";

            return pdd;
        }
    }
}
