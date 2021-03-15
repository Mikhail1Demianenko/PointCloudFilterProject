using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.UI;
using PointCloudFilterWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterService.Helpers
{
    public class FaceServices
    {
        private UIDocument _uidoc { get; set; }


        public FaceServices(UIDocument uidoc)
        {
            _uidoc = uidoc;
        }
        public void Initialize()
        {


            Face face = PickFace();
            PCFilterDTO.FinalDistance = CalculateDistances(face).Where(dist => Math.Abs(dist)<PCFilterDTO.NeglectFarther).Average();

        }

        private Face PickFace()
        {
            if(PCFilterDTO.PointCloud == null)
            {
                TaskDialog.Show("Error", "No Point Cloud Selected");
                return null;
            }
            else
            {
                Reference pickedFace = _uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face, "Pick Face");

                Element element = _uidoc.Document.GetElement(pickedFace.ElementId);

                Face face = element.GetGeometryObjectFromReference(pickedFace) as Face;

                return face;
            }
        }

        private List<double> CalculateDistances(Face face)
        {

            List<Plane> boundingPlanes = new List<Plane>();

            // constructs bounding planes for whatever type of plane
            boundingPlanes = ConstructPlanesSwitch(face);

            // point collection is needed to extract points that passed the filter and to filter the pointcloud according to the filter
            PointCollection pointCollection = PointCloudServices.FilterPointCloud(boundingPlanes);

            List<double> distances = ProjectOnFace(face, pointCollection);


            return distances;
        }



        private List<Plane> ConstructPlanesSwitch(Face face)
        {
            List<Plane> boundingPlanes = new List<Plane>();
            // Construct planes
            switch (face.ToString())
            {
                case "Autodesk.Revit.DB.PlanarFace":
                    boundingPlanes = ConstructPlanes(face as PlanarFace);
                    break;
                case "Autodesk.Revit.DB.CylindricalFace":
                    boundingPlanes = ConstructPlanes(face as CylindricalFace);
                    break;
                case "Autodesk.Revit.DB.RuledFace":
                    boundingPlanes = ConstructPlanes(face as RuledFace);
                    break;
                default:
                    TaskDialog.Show("Error", "Face type not supported");
                    break;
            }

            return boundingPlanes;
        }

        private List<double> ProjectOnFace(Face face, PointCollection pointCollection)
        {
            double scale = PCFilterDTO.PointCloudScale;
            List<double> distances = new List<double>();

            

            foreach (XYZ point in pointCollection)
            {
                IntersectionResult iResult = face.Project(point * scale);
                
                if (iResult != null)
                {
                    XYZ normal = face.ComputeNormal(iResult.UVPoint);
                    XYZ origin = iResult.XYZPoint;

                    

                    double sign = normal.DotProduct(point.Normalize()-origin.Normalize());

                    
                    if (sign < 0)
                    {
                        sign = -1;
                    }
                    else
                    {
                        sign = 1;
                    }


                    distances.Add(iResult.Distance * sign);
                }

            }
            return distances;
        }
        private List<XYZ> GetBoundingPoints(Face inputFace)
        {
            Face face = inputFace as Face;

            BoundingBoxUV bBox = face.GetBoundingBox();

            UV min = bBox.Min;
            UV max = bBox.Max;


            UV I = min;
            UV II = new UV(min.U, max.V);
            UV III = max;
            UV IV = new UV(max.U, min.V);

            List<XYZ> points = new List<XYZ>() { inputFace.Evaluate(I), inputFace.Evaluate(II), inputFace.Evaluate(III), inputFace.Evaluate(IV) };

            return points;
        }

        #region ConstructPlanes overloads
        private List<Plane> ConstructPlanes(PlanarFace planarFace)
        {

            List<XYZ> points = GetBoundingPoints(planarFace);
            XYZ normal = Plane.CreateByThreePoints(points[0], points[1], points[2]).Normal;
            double offset = PCFilterDTO.NeglectFarther;

            List<Plane> planes = new List<Plane>()
            {
                // 4 planes around bounding plane
                Plane.CreateByThreePoints(points[0], points[1], points[0] - normal),
                Plane.CreateByThreePoints(points[1], points[2], points[1] - normal),
                Plane.CreateByThreePoints(points[2], points[3], points[2] - normal),
                Plane.CreateByThreePoints(points[3], points[0], points[3] - normal),

                //top and bottom planes
                Plane.CreateByThreePoints(points[0] + normal * offset, points[2] + normal * offset, points[1] + normal * offset),
                Plane.CreateByThreePoints(points[0] - normal * offset, points[1] - normal * offset, points[2] - normal * offset)

            };

            return planes;

        }
        private List<Plane> ConstructPlanes(CylindricalFace cylindricalFace)
        {
            List<XYZ> points = GetBoundingPoints(cylindricalFace);
            XYZ normal = Plane.CreateByThreePoints(points[0], points[1], points[2]).Normal;
            double offset = PCFilterDTO.NeglectFarther;//MainWindow.NeglectFarther;

            XYZ radius = cylindricalFace.get_Radius(0);


            Plane auxPlane1 = Plane.CreateByThreePoints(points[0], points[1], points[0] - normal);
            Plane auxPlane2 = Plane.CreateByThreePoints(points[2], points[3], points[2] - normal);

            List<Plane> planes = new List<Plane>()
            {
                // 4 planes around bounding plane
                Plane.CreateByNormalAndOrigin(auxPlane1.Normal, points[0] - auxPlane1.Normal * offset),
                Plane.CreateByThreePoints(points[1], points[2], points[1] - normal),
                Plane.CreateByNormalAndOrigin(auxPlane2.Normal, points[2] - auxPlane2.Normal * offset), //<-- side plane
                Plane.CreateByThreePoints(points[3], points[0], points[3] - normal),

                //top and bottom planes
                Plane.CreateByThreePoints(points[0] + normal * offset , points[2] + normal * offset, points[1] + normal * offset),
                Plane.CreateByThreePoints(points[0] - normal * offset + radius, points[1] - normal * offset + radius, points[2] - normal * offset + radius)

            };

            return planes;
        }
        private List<Plane> ConstructPlanes(RuledFace ruledFace)
        {
            List<XYZ> points = GetBoundingPoints(ruledFace);

            XYZ normal = ruledFace.ComputeNormal(new UV(0.5, 0.5));

            List<Plane> fourPlanes = new List<Plane>() {                 
                
                // 4 planes around bounding plane
                Plane.CreateByThreePoints(points[1], points[0], points[0] - normal),
                Plane.CreateByThreePoints(points[2], points[1], points[1] - normal),
                Plane.CreateByThreePoints(points[3], points[2], points[2] - normal),
                Plane.CreateByThreePoints(points[0], points[3], points[3] - normal)

            };


            double offset = PCFilterDTO.NeglectFarther;// MainWindow.NeglectFarther;

            List<Plane> planes = new List<Plane>();
            planes.AddRange(fourPlanes);

            List<Plane> twoplanes = PickFarthestPlanes(ruledFace, fourPlanes[0], fourPlanes[1]);
            planes.Add(Plane.CreateByNormalAndOrigin(twoplanes[0].Normal * offset, twoplanes[0].Origin));
            planes.Add(Plane.CreateByNormalAndOrigin(twoplanes[1].Normal * offset, twoplanes[1].Origin));

            return planes;

        }
        private List<Plane> PickFarthestPlanes(RuledFace ruledFace, Plane plane1, Plane plane2)
        {

            // normal of future plane should be parallel to the rest 4 planes
            XYZ normal = plane1.Normal.CrossProduct(plane2.Normal);

            // get each point of the ruled face
            List<XYZ> facePoints = new List<XYZ>();
      
            foreach (Curve curve in ruledFace.GetEdgesAsCurveLoops()[0])
            {
                List<XYZ> pts = new List<XYZ>() {
                    curve.GetEndPoint(0),
                    curve.GetEndPoint(1)
                };

                foreach (XYZ point in pts)
                {
                    if(!facePoints.Contains(point))
                    {
                        facePoints.Add(point);
                    }
                }
            }
            facePoints = facePoints.Distinct().ToList();
            Dictionary<double, XYZ> distances = new Dictionary<double, XYZ>();

            foreach (XYZ point in facePoints)
            {
                double distance = normal.DotProduct(ruledFace.Evaluate(new UV(0.5, 0.5)).Subtract(point));

                if(!distances.ContainsKey(distance))
                {
                    distances.Add(distance, point);
                }
                
            }

            Plane minPlane = Plane.CreateByNormalAndOrigin(-normal, distances[distances.Keys.Min()]);
            Plane maxPlane = Plane.CreateByNormalAndOrigin(normal, distances[distances.Keys.Max()]);

            return new List<Plane>() { maxPlane, minPlane };
        }
        #endregion
    }
}

