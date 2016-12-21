using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ComputerGraphics
{
    public class First
    {
        private List<Line> GetTriangle(Point3d startPoint, string direction)
        {
            Line line1, line2, line3, line4;

            switch (direction)
            {
                case "up":
                    line1 = new Line(startPoint, new Point3d(startPoint.X + 1, startPoint.Y + 2, startPoint.Z)); // \
                    line2 = new Line(startPoint, new Point3d(startPoint.X + -1, startPoint.Y + 2, startPoint.Z)); // /
                    line3 = new Line(new Point3d(startPoint.X + -1, startPoint.Y + 2, startPoint.Z), new Point3d(startPoint.X + 1, startPoint.Y + 2, startPoint.Z)); // _
                    line4 = new Line(new Point3d(startPoint.X, startPoint.Y + 2, startPoint.Z), new Point3d(startPoint.X, startPoint.Y + 4, startPoint.Z)); // |
                    break;
                case "right":
                    line1 = new Line(startPoint, new Point3d(startPoint.X + 2, startPoint.Y + 1, startPoint.Z)); // /
                    line2 = new Line(startPoint, new Point3d(startPoint.X + 2, startPoint.Y + -1, startPoint.Z));// \
                    line3 = new Line(new Point3d(startPoint.X + 2, startPoint.Y + 1, startPoint.Z), new Point3d(startPoint.X + 2, startPoint.Y + -1, startPoint.Z)); // |
                    line4 = new Line(new Point3d(startPoint.X + 2, startPoint.Y, startPoint.Z), new Point3d(startPoint.X + 4, startPoint.Y, startPoint.Z)); // _
                    break;
                case "down":
                    line1 = new Line(startPoint, new Point3d(startPoint.X + -1, startPoint.Y + -2, startPoint.Z)); // /
                    line2 = new Line(startPoint, new Point3d(startPoint.X + 1, startPoint.Y + -2, startPoint.Z)); // \
                    line3 = new Line(new Point3d(startPoint.X + -1, startPoint.Y + -2, startPoint.Z), new Point3d(startPoint.X + 1, startPoint.Y + -2, startPoint.Z)); // _
                    line4 = new Line(new Point3d(startPoint.X, startPoint.Y - 2, startPoint.Z), new Point3d(startPoint.X, startPoint.Y - 4, startPoint.Z)); // |
                    break;
                default: 
                    throw new System.Exception("Direction has to be one of: up, right, down");
            }

            return new List<Line>() { line1, line2, line3, line4 };
        }

        [CommandMethod("firstobject")]
        public void FirstObject()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            var editor = doc.Editor;

            var startPointInput = new PromptPointOptions("\nSelect starting point:");
            startPointInput.AllowNone = false;
            Point3d startPoint = editor.GetPoint(startPointInput).Value;

            var transaction = db.TransactionManager.StartTransaction();

            using (transaction)
            {
                var triangleUp = GetTriangle(startPoint, "up");
                var triangleRight = GetTriangle(startPoint, "right");
                var triangleDown = GetTriangle(startPoint, "down");

                var lineLeft = new Line(new Point3d(startPoint.X, startPoint.Y, startPoint.Z), new Point3d(startPoint.X - 1.5, startPoint.Y, startPoint.Z));
                var arcLine = new Line(new Point3d(lineLeft.EndPoint.X, startPoint.Y - 1.5, startPoint.Z), new Point3d(lineLeft.EndPoint.X, startPoint.Y + 1.5, startPoint.Z));

                var arc = new Arc(
                    new Point3d(arcLine.StartPoint.X, arcLine.StartPoint.Y + 1.5, arcLine.StartPoint.Z),
                    arcLine.Length / 2,
                    Math.PI / 2,
                    Math.PI * 3 / 2
                );

                var record = (BlockTableRecord) transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                foreach (var line in triangleUp)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in triangleRight)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in triangleDown)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                record.AppendEntity(lineLeft);
                transaction.AddNewlyCreatedDBObject(lineLeft, true);

                record.AppendEntity(arcLine);
                transaction.AddNewlyCreatedDBObject(arcLine, true);

                record.AppendEntity(arc);
                transaction.AddNewlyCreatedDBObject(arc, true);

                transaction.Commit();
            }
        }
    }
}
