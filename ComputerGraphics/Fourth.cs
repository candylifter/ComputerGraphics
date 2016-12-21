using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ComputerGraphics
{
    public class Fourth
    {
        private List<Line> DrawLeg(Point3d startPoint)
        {
            var bottom = new Line(
                startPoint,
                new Point3d(startPoint.X + 3, startPoint.Y, startPoint.Z) 
            );

            var left = new Line(
                new Point3d(bottom.StartPoint.X, startPoint.Y, startPoint.Z),
                new Point3d(bottom.StartPoint.X, startPoint.Y + 10, startPoint.Z)
            );

            var right = new Line(
                new Point3d(bottom.EndPoint.X, startPoint.Y, startPoint.Z),
                new Point3d(bottom.EndPoint.X, startPoint.Y + 10, startPoint.Z)
            );

            return new List<Line>() { bottom, left, right };
        }

        private List<Line> DrawSurface(Point3d startPoint)
        {
            var bottom = new Line(
                startPoint,
                new Point3d(startPoint.X + 13, startPoint.Y, startPoint.Z)
            );

            var left = new Line(
                startPoint,
                new Point3d(startPoint.X, startPoint.Y + 3, startPoint.Z)
            );

            var right = new Line(
                new Point3d(bottom.EndPoint.X, startPoint.Y, startPoint.Z),
                new Point3d(bottom.EndPoint.X, startPoint.Y + 3, startPoint.Z)
            );

            var top = new Line(
                left.EndPoint,
                right.EndPoint
            );

            return new List<Line>() { bottom, left, right, top };
        }

        private List<Line> DrawRest(Point3d startPoint)
        {
            var left = new Line(
                startPoint,
                new Point3d(startPoint.X, startPoint.Y + 10, startPoint.Z)
            );

            var right = new Line(
                new Point3d(startPoint.X + 3, startPoint.Y, startPoint.Z),
                new Point3d(startPoint.X + 3, startPoint.Y + 10, startPoint.Z) 
            );

            var top = new Line(
                left.EndPoint,
                right.EndPoint
            );

            return new List<Line>() { left, right, top };
        }

        [CommandMethod("fourthobject")]
        public void FourthObject()
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
                var leg1 = DrawLeg(startPoint);
                var leg2 = DrawLeg(new Point3d(startPoint.X + 10, startPoint.Y, startPoint.Z));

                var surface = DrawSurface(new Point3d(startPoint.X, startPoint.Y + 10, startPoint.Z));

                var rest = DrawRest(new Point3d(startPoint.X + 10, startPoint.Y + 13, startPoint.Z));

                var record = (BlockTableRecord)transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                foreach (var line in leg1)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in leg2)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in surface)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                foreach (var line in rest)
                {
                    record.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                }

                transaction.Commit();
            }
        }
    }
}
