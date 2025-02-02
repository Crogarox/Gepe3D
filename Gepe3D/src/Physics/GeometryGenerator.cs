
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;


namespace Gepe3D
{
    public class GeometryGenerator
    {

        private class IcoSphere
        {
            private static readonly float PHI = ( 1 + MathF.Sqrt(5) ) / 2;

            // initialized to the starting icosahedron
            public readonly List<Vector3> vertices = new List<Vector3>()
            {
                new Vector3(  -1,  PHI,    0 ),   new Vector3(    1,  PHI,    0 ),   new Vector3(   -1, -PHI,    0 ),
                new Vector3(   1, -PHI,    0 ),   new Vector3(    0,   -1,  PHI ),   new Vector3(    0,    1,  PHI ),
                new Vector3(   0,   -1, -PHI ),   new Vector3(    0,    1, -PHI ),   new Vector3(  PHI,    0,   -1 ),
                new Vector3( PHI,    0,    1 ),   new Vector3( -PHI,    0,   -1 ),   new Vector3( -PHI,    0,    1 )
            };

            // initialized to the starting icosahedron
            public List<Vector3i> triangles = new List<Vector3i>()
            {
                new Vector3i(  0, 11,  5 ),   new Vector3i(  0,  5,  1 ),   new Vector3i(  0,  1,  7 ),   new Vector3i(  0,  7, 10 ),
                new Vector3i(  0, 10, 11 ),   new Vector3i(  1,  5,  9 ),   new Vector3i(  5, 11,  4 ),   new Vector3i( 11, 10,  2 ),
                new Vector3i( 10,  7,  6 ),   new Vector3i(  7,  1,  8 ),   new Vector3i(  3,  9,  4 ),   new Vector3i(  3,  4,  2 ),
                new Vector3i(  3,  2,  6 ),   new Vector3i(  3,  6,  8 ),   new Vector3i(  3,  8,  9 ),   new Vector3i(  4,  9,  5 ),
                new Vector3i(  2,  4, 11 ),   new Vector3i(  6,  2, 10 ),   new Vector3i(  8,  6,  7 ),   new Vector3i(  9,  8,  1 )
            };

            Dictionary<long, int> existingMidPoints = new Dictionary<long, int>();

            public IcoSphere(float radius, int subdivisions)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = vertices[i].Normalized();
                }

                for (int i = 0; i < subdivisions; i++)
                {
                    // every iteration make a new list of triangles
                    List<Vector3i> newTriangles = new List<Vector3i>();

                    foreach (Vector3i tri in triangles)
                    {
                        // vertex index
                        int v1 = tri.X;
                        int v2 = tri.Y;
                        int v3 = tri.Z;

                        // replace triangle with 4 new triangles
                        int a = GenMidPointID(v1, v2);
                        int b = GenMidPointID(v2, v3);
                        int c = GenMidPointID(v3, v1);

                        newTriangles.Add( new Vector3i( v1, a, c ) );
                        newTriangles.Add( new Vector3i( v2, b, a ) );
                        newTriangles.Add( new Vector3i( v3, c, b ) );
                        newTriangles.Add( new Vector3i(  a, b, c ) );
                    }
                    triangles = newTriangles;
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = vertices[i] * radius;
                }
            }

            private int GenMidPointID(int v1, int v2)
            {
                // check if midpoint has already been generated by another triangle
                long id0 = MathHelper.Min(v1, v2);
                long id1 = MathHelper.Max(v1, v2);
                long uniqueCombination = (id0 << 32) + id1;

                if ( existingMidPoints.ContainsKey(uniqueCombination) )
                {
                    return existingMidPoints[uniqueCombination];
                }

                // generate new midpoint
                Vector3 midPoint = ( (vertices[v1] + vertices[v2]) / 2 ).Normalized();
                int newID = vertices.Count; 
                existingMidPoints.Add(uniqueCombination, newID);

                vertices.Add(midPoint);
                return newID;
            }
        }

        public static Geometry GenIcoSphere(float radius, int subdivisions)
        {
            IcoSphere icoSphere = new IcoSphere(radius, subdivisions);
            return new Geometry(icoSphere.vertices, icoSphere.triangles);
        }

        public static Geometry GenCube(float xLength, float yLength, float zLength)
        {
            Geometry cube = new Geometry();
            
            cube.AddVertex( -xLength / 2, -yLength / 2, -zLength / 2 );
            cube.AddVertex( -xLength / 2, -yLength / 2,  zLength / 2 );
            cube.AddVertex(  xLength / 2, -yLength / 2,  zLength / 2 );
            cube.AddVertex(  xLength / 2, -yLength / 2, -zLength / 2 );
            cube.AddVertex( -xLength / 2,  yLength / 2, -zLength / 2 );
            cube.AddVertex( -xLength / 2,  yLength / 2,  zLength / 2 );
            cube.AddVertex(  xLength / 2,  yLength / 2,  zLength / 2 );
            cube.AddVertex(  xLength / 2,  yLength / 2, -zLength / 2 );
            
            cube.AddTriangle(0, 2, 1);
            cube.AddTriangle(0, 3, 2);
            cube.AddTriangle(0, 1, 5);
            cube.AddTriangle(0, 5, 4);
            cube.AddTriangle(1, 2, 6);
            cube.AddTriangle(1, 6, 5);
            cube.AddTriangle(2, 3, 7);
            cube.AddTriangle(2, 7, 6);
            cube.AddTriangle(3, 0, 4);
            cube.AddTriangle(3, 4, 7);
            cube.AddTriangle(4, 5, 6);
            cube.AddTriangle(4, 6, 7);

            return cube;
        }
        
        public static Geometry GenQuad(float width, float height)
        {
            Geometry quad = new Geometry();
            quad.AddVertex(-width / 2, -height / 2, 0);
            quad.AddVertex( width / 2, -height / 2, 0);
            quad.AddVertex( width / 2,  height / 2, 0);
            quad.AddVertex(-width / 2,  height / 2, 0);
            quad.AddTriangle(0, 1, 2);
            quad.AddTriangle(0, 2, 3);
            return quad;
        }
    }
}