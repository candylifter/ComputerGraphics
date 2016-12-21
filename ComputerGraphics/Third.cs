using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ComputerGraphics
{
    public class Third
    {
        private List<Line> DrawBase(Point3d startPoint)
        {
            var baseBottom = new Line(
                new Point3d(startPoint.X - 0.5, startPoint.Y, startPoint.Z),
                new Point3d(startPoint.X + 0.5, startPoint.Y, startPoint.Z)
            );

            var baseLeft = new Line(
                new Point3d(baseBottom.StartPoint.X, baseBottom.StartPoint.Y, baseBottom.StartPoint.Z),
                new Point3d(baseBottom.StartPoint.X, baseBottom.StartPoint.Y + 2, baseBottom.StartPoint.Z)
            );

            var baseRight = new Line(
                new Point3d(baseBottom.EndPoint.X, baseBottom.EndPoint.Y, baseBottom.EndPoint.Z),
                new Point3d(baseBottom.EndPoint.X, baseBottom.EndPoint.Y + 2, baseBottom.EndPoint.Z)
            );

            return new List<Line>() { baseBottom, baseLeft, baseRight };
        }

        private List<Line> DrawTriangle(Point3d startPoint, double width, double height)
        {
            var bottom = new Line(
                new Point3d(startPoint.X - (width / 2), startPoint.Y, startPoint.Z), 
                new Point3d(startPoint.X + (width / 2), startPoint.Y, startPoint.Z)
            );

            var left = new Line(
                new Point3d(bottom.StartPoint.X, startPoint.Y, startPoint.Z), 
                new Point3d(startPoint.X, startPoint.Y + height, startPoint.Z)
            );

            var right = new Line(
                new Point3d(bottom.EndPoint.X, startPoint.Y, startPoint.Z),
                new Point3d(startPoint.X, startPoint.Y + height, startPoint.Z)     
            );

            return new List<Line>() { bottom, left, right };
        }

        private List<Line> DrawTriangles(Point3d startPoint, int count)
        {
            var lines = new List<Line>();

            var prevHeight = startPoint.Y;

            for (int i = 0; i < count; i++)
            {
                var width = (count - i + 1) * 2;
                var height = count - (Convert.ToDouble(i) + 1) / 3;

                var triangle = DrawTriangle(new Point3d(startPoint.X, prevHeight, startPoint.Z), width, height);

                prevHeight += height;

                lines.AddRange(triangle);
            }

            return lines;
        }


        [CommandMethod("thirdobject")]
        public void ThirdObject()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            var editor = doc.Editor;

            var startPointInput = new PromptPointOptions("\nSelect starting point:");
            startPointInput.AllowNone = false;
            Point3d startPoint = editor.GetPoint(startPointInput).Value;

            var triangleCountInput = new PromptIntegerOptions("\nNumber of branches:");
            triangleCountInput.AllowNone = false;
            int triangleCount = editor.GetInteger(triangleCountInput).Value;

            var transaction = db.TransactionManager.StartTransaction();

            using (transaction)
            {
                var treeBase = DrawBase(startPoint);

                var triangles = DrawTriangles(new Point3d(startPoint.X, startPoint.Y + 2, startPoint.Z), triangleCount);

                var record = (BlockTableRecord)transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                foreach (var line in treeBase)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in triangles)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }


                transaction.Commit();
            }
        }
    }
}
