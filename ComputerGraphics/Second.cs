using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ComputerGraphics
{
    public class Second
    {
        [CommandMethod("secondobject")]
        public void SecondObject()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            var editor = doc.Editor;

            var startPointInput = new PromptPointOptions("\nSelect starting point:");
            startPointInput.AllowNone = false;
            Point3d startPoint = editor.GetPoint(startPointInput).Value;

            var arrowAngleInput = new PromptAngleOptions("\nSpecify arrow angle:");
            arrowAngleInput.AllowNone = false;

            double arrowAngle = editor.GetAngle(arrowAngleInput).Value;


            var transaction = db.TransactionManager.StartTransaction();

            using (transaction)
            {
                var horizontalLine = new Line(new Point3d(startPoint.X - 1, startPoint.Y, startPoint.Z), new Point3d(startPoint.X + 1.5, startPoint.Y, startPoint.Z));
                var verticalLine = new Line(new Point3d(startPoint.X, startPoint.Y, startPoint.Z), new Point3d(startPoint.X, startPoint.Y - 1.5, startPoint.Z));

                var lineLeft = new Line(new Point3d(startPoint.X - 0.5, startPoint.Y - 0.25, startPoint.Z), new Point3d(startPoint.X - 0.5, startPoint.Y + 0.25, startPoint.Z));
                var lineRight = new Line(new Point3d(startPoint.X + 0.5, startPoint.Y - 0.25, startPoint.Z), new Point3d(startPoint.X + 0.5, startPoint.Y + 0.25, startPoint.Z));
                var lineBottom = new Line(new Point3d(startPoint.X - 0.25, startPoint.Y - 0.25, startPoint.Z), new Point3d(startPoint.X + 0.25, startPoint.Y - 0.25, startPoint.Z));

                var circle = new Circle(new Point3d(startPoint.X, startPoint.Y - 2.25, startPoint.Z), new Vector3d(0, 0, 1), 0.75);

                var innerCircle = new Circle(new Point3d(startPoint.X, startPoint.Y - 2.25, startPoint.Z), new Vector3d(0, 0, 1), 0.125);

                // Drawing arrow
                // Lower left line
                var startXLeft = innerCircle.Center.X - (Math.Cos(arrowAngle) * innerCircle.Radius);
                var startYLeft = innerCircle.Center.Y - (Math.Sin(arrowAngle) * innerCircle.Radius);
                var endXLeft = startXLeft - (Math.Cos(arrowAngle) * 0.5); // 0.5 - line length
                var endYLeft = startYLeft - (Math.Sin(arrowAngle) * 0.5);  

                var innerLineLeft = new Line(
                    new Point3d(startXLeft, startYLeft, innerCircle.Center.Z),
                    new Point3d(endXLeft, endYLeft, innerCircle.Center.Z)
                );
                
                // Upper right line
                var startXRight = innerCircle.Center.X + (Math.Cos(arrowAngle) * innerCircle.Radius);
                var startYRight = innerCircle.Center.Y + (Math.Sin(arrowAngle) * innerCircle.Radius);
                var endXRight = startXRight + (Math.Cos(arrowAngle) * 0.25);
                var endYRight = startYRight + (Math.Sin(arrowAngle) * 0.25);

                var innerLineRight = new Line(
                    new Point3d(startXRight, startYRight, innerCircle.Center.Z),
                    new Point3d(endXRight, endYRight, innerCircle.Center.Z) 
                );

                // Triangle
                var triangleLineLength = 0.25;
                var triangleHeight = Math.Sqrt(Math.Pow(triangleLineLength, 2) - Math.Pow(triangleLineLength / 2, 2));

                var innerTriangleLine1 = new Line(
                   new Point3d(
                       innerLineRight.EndPoint.X - (triangleLineLength / 2 * Math.Sin(arrowAngle)), 
                       innerLineRight.EndPoint.Y + (triangleLineLength / 2 * Math.Cos(arrowAngle)), 
                       innerLineRight.EndPoint.Z),
                   new Point3d(
                       innerLineRight.EndPoint.X + (triangleLineLength / 2 * Math.Sin(arrowAngle)), 
                       innerLineRight.EndPoint.Y - (triangleLineLength / 2 * Math.Cos(arrowAngle)), 
                       innerLineRight.EndPoint.Z)
                );

                var innerTriangleLine2 = new Line(
                    innerTriangleLine1.StartPoint,
                    new Point3d(
                        endXRight + (Math.Cos(arrowAngle) * triangleHeight),
                        endYRight + (Math.Sin(arrowAngle) * triangleHeight),
                        innerTriangleLine1.StartPoint.Z)
                );

                var innerTriangleLine3 = new Line(
                   innerTriangleLine1.EndPoint,
                   innerTriangleLine2.EndPoint
               );



                var record = (BlockTableRecord) transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                record.AppendEntity(horizontalLine);
                transaction.AddNewlyCreatedDBObject(horizontalLine, true);

                record.AppendEntity(verticalLine);
                transaction.AddNewlyCreatedDBObject(verticalLine, true);

                record.AppendEntity(lineLeft);
                record.AppendEntity(lineRight);
                record.AppendEntity(lineBottom);
                transaction.AddNewlyCreatedDBObject(lineLeft, true);
                transaction.AddNewlyCreatedDBObject(lineRight, true);
                transaction.AddNewlyCreatedDBObject(lineBottom, true);

                record.AppendEntity(circle);
                transaction.AddNewlyCreatedDBObject(circle, true);

                record.AppendEntity(innerCircle);
                record.AppendEntity(innerLineLeft);
                record.AppendEntity(innerLineRight);
                transaction.AddNewlyCreatedDBObject(innerCircle, true);
                transaction.AddNewlyCreatedDBObject(innerLineLeft, true);
                transaction.AddNewlyCreatedDBObject(innerLineRight, true);

                record.AppendEntity(innerTriangleLine1);
                record.AppendEntity(innerTriangleLine2);
                record.AppendEntity(innerTriangleLine3);
                transaction.AddNewlyCreatedDBObject(innerTriangleLine1, true);
                transaction.AddNewlyCreatedDBObject(innerTriangleLine2, true);
                transaction.AddNewlyCreatedDBObject(innerTriangleLine3, true);

                transaction.Commit();
            }

        }
    }
}
