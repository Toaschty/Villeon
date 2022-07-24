using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Zenseless.OpenTK;
using Buffer = System.Buffer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Villeon.Systems.Update
{
    public class RaytracingSystem : System, IUpdateSystem
    {
        private const int RESOLUTION_X = 256;
        private const int RESOLUTION_Y = 144;

        private static float[,] _renderLightmap = new float[RESOLUTION_X, RESOLUTION_Y];
        private static Mutex _mutex = new Mutex();

        private List<IEntity> _colliders = new List<IEntity>();
        private List<IEntity> _lights = new List<IEntity>();

        private Running _running = new Running();

        private Texture2D? _texture;

        public RaytracingSystem(string name)
            : base(name)
        {
            Signature.IncludeOR(typeof(Light), typeof(Collider), typeof(DynamicCollider));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Collider collider = entity.GetComponent<Collider>();
            Light light = entity.GetComponent<Light>();

            if (light is not null)
                _lights.Add(entity);
            else if (collider is not null)
                _colliders.Add(entity);
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            Collider collider = entity.GetComponent<Collider>();
            Light light = entity.GetComponent<Light>();

            if (light is not null)
                _lights.Remove(entity);
            else if (collider is not null)
                _colliders.Remove(entity);
        }

        public void Update(float time)
        {
            if (!StateManager.RayTracingEnabled)
                return;

            if (!_running.IsRunning)
            {
                _running.IsRunning = true;
                _texture = new Texture2D(RESOLUTION_X, RESOLUTION_Y, OpenTK.Graphics.OpenGL4.SizedInternalFormat.Rgba32f);
                RayTracer rayTracer = new RayTracer(_colliders, _lights, _running);
                Thread thread = new Thread(rayTracer.Raytracing);
                thread.IsBackground = true;
                thread.Start();
            }

            ExportLightMap();
            GL.Finish();
        }

        private void ExportLightMap()
        {
            _mutex.WaitOne();
            Bitmap bitmap = new Bitmap(_renderLightmap.GetLength(0), _renderLightmap.GetLength(1));
            for (int x = 0; x < _renderLightmap.GetLength(0); x++)
            {
                for (int y = 0; y < _renderLightmap.GetLength(1); y++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(255, (int)(_renderLightmap[x, y] * 255), (int)(_renderLightmap[x, y] * 255), (int)(_renderLightmap[x, y] * 255)));
                }
            }

            _mutex.ReleaseMutex();

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            GL.TextureSubImage2D(_texture!.Handle, 0, 0, 0, _renderLightmap.GetLength(0), _renderLightmap.GetLength(1), OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            GL.ActiveTexture(TextureUnit.Texture8);
            _texture.Bind();
        }

        private class RayTracer
        {
            private const int N_RAYS = 180;
            private const int RENDER_DISTANCE = 40;

            private float[,] _lightmap = new float[1, 1];

            private List<IEntity> _colliders;
            private List<IEntity> _lights;

            private Matrix4 _rayTracingMatrix;
            private List<Line> _horizontalLines = new List<Line>();
            private List<Line> _verticalLines = new List<Line>();
            private List<RayCaster> _rayCasters = new List<RayCaster>();

            private Running _running;

            public RayTracer(List<IEntity> colliders, List<IEntity> lights, Running running)
            {
                _colliders = colliders;
                _lights = lights;
                _running = running;
            }

            public void Raytracing()
            {
                while (StateManager.InDungeon)
                {
                    _horizontalLines.Clear();
                    _verticalLines.Clear();
                    _rayCasters.Clear();

                    _lightmap = new float[RESOLUTION_X, RESOLUTION_Y];
                    Camera.Update();
                    _rayTracingMatrix = Camera.GetRaytracingMatrix(RESOLUTION_X, RESOLUTION_Y);

                    Vector4 min = new Vector4(0 - RENDER_DISTANCE, 0 - RENDER_DISTANCE, 1, 1);
                    min *= _rayTracingMatrix.Inverted();
                    Vector4 max = new Vector4(RESOLUTION_X + RENDER_DISTANCE, RESOLUTION_Y + RENDER_DISTANCE, 1, 1);
                    max *= _rayTracingMatrix.Inverted();

                    CreateLinesInView(min, max);
                    CreateLightsInView(min, max);

                    foreach (RayCaster rayCaster in _rayCasters)
                    {
                        Vector4 lightPosition = new Vector4(rayCaster.Position.X, rayCaster.Position.Y, 1, 1);
                        lightPosition *= _rayTracingMatrix;

                        for (int i = 0; i < N_RAYS; i++)
                        {
                            double radian = (i * (180.0f / N_RAYS)) / (180.0f / Math.PI);
                            float slope = 0;
                            if (Math.Cos(radian) != 0)
                                slope = (float)(Math.Sin(radian) / Math.Cos(radian));

                            float b = rayCaster.Position.Y - (slope * rayCaster.Position.X);
                            Vector2 point1 = new Vector2(-1, -1);
                            Vector2 point2 = new Vector2(-1, -1);
                            float p1Distance = float.PositiveInfinity;
                            float p2Distance = float.PositiveInfinity;

                            HorizontalIntersects(rayCaster, slope, b, ref point1, ref point2, ref p1Distance, ref p2Distance);
                            VerticalInterects(rayCaster, slope, b, ref point1, ref point2, ref p1Distance, ref p2Distance);

                            //add closest point
                            if (point1.X != -1)
                                AddTracedRay(rayCaster, point1);
                            if (point2.X != -1)
                                AddTracedRay(rayCaster, point2);
                        }
                    }

                    CopyLightmap();
                }

                _running.IsRunning = false;
            }

            private void CopyLightmap()
            {
                _mutex.WaitOne();
                _renderLightmap = (float[,])_lightmap.Clone();
                _mutex.ReleaseMutex();
            }

            private void VerticalInterects(RayCaster raycaster, float slope, float b, ref Vector2 point1, ref Vector2 point2, ref float p1Distance, ref float p2Distance)
            {
                foreach (Line line in _verticalLines)
                {
                    float y = (slope * line.P1.X) + b;
                    if (y > line.P1.Y && y < line.P2.Y)
                    {
                        Vector2 newPoint = new Vector2(line.P1.X, y);
                        float newDistance = (raycaster.Position - newPoint).Length;
                        if (newDistance < p1Distance && line.P1.X > raycaster.Position.X)
                        {
                            point1 = newPoint;
                            p1Distance = (raycaster.Position - point1).Length;
                        }
                        else if (newDistance < p2Distance && line.P1.X <= raycaster.Position.X)
                        {
                            point2 = newPoint;
                            p2Distance = (raycaster.Position - point2).Length;
                        }
                    }
                }
            }

            private void HorizontalIntersects(RayCaster rayCaster, float slope, float b, ref Vector2 point1, ref Vector2 point2, ref float p1Distance, ref float p2Distance)
            {
                foreach (Line line in _horizontalLines)
                {
                    float x = (line.P1.Y - b) / slope;
                    if (x > line.P1.X && x < line.P2.X)
                    {
                        Vector2 newPoint = new Vector2(x, line.P1.Y);
                        float newDistance = (rayCaster.Position - newPoint).Length;
                        if (newDistance < p1Distance && x > rayCaster.Position.X)
                        {
                            point1 = newPoint;
                            p1Distance = (rayCaster.Position - point1).Length;
                        }
                        else if (newDistance < p2Distance && x <= rayCaster.Position.X)
                        {
                            point2 = newPoint;
                            p2Distance = (rayCaster.Position - point2).Length;
                        }
                    }
                }
            }

            private void AddTracedRay(RayCaster rayCaster, Vector2 point)
            {
                Vector4 mappedLight = new Vector4(rayCaster.Position.X, rayCaster.Position.Y, 1, 1);
                Vector4 mappedPoint = new Vector4(point.X, point.Y, 1, 1);
                mappedLight *= _rayTracingMatrix;
                mappedPoint *= _rayTracingMatrix;

                foreach (Vector2 pixel in FindLine(new Vector2(mappedLight.X, mappedLight.Y), new Vector2(mappedPoint.X, mappedPoint.Y)))
                    AddTracedPoint(pixel, rayCaster.Intensity);
            }

            private void AddTracedPoint(Vector2 point, float intensity)
            {
                if (point.X >= 0 && point.X < _lightmap.GetLength(0) &&
                    point.Y >= 0 && point.Y < _lightmap.GetLength(1))
                    _lightmap[(int)point.X, (int)point.Y] = Math.Min(_lightmap[(int)point.X, (int)point.Y] + (0.015f * intensity), 1.0f);
            }

            private void CreateLightsInView(Vector4 min, Vector4 max)
            {
                foreach (IEntity entity in _lights)
                {
                    Transform transform = entity.GetComponent<Transform>();
                    Light light = entity.GetComponent<Light>();
                    if (transform.Position.X > min.X && transform.Position.X < max.X &&
                       transform.Position.Y > min.Y && transform.Position.Y < max.Y)
                        _rayCasters.Add(new RayCaster(transform.Position + light.Offset, light.LightAmbientIntensity));
                }
            }

            private void CreateLinesInView(Vector4 min, Vector4 max)
            {
                // Add borders
                _horizontalLines.Add(new Line(new Vector2(min.X, min.Y), new Vector2(max.X, min.Y)));
                _horizontalLines.Add(new Line(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y)));
                _verticalLines.Add(new Line(new Vector2(min.X, min.Y), new Vector2(max.X, max.Y)));
                _verticalLines.Add(new Line(new Vector2(max.X, min.Y), new Vector2(max.X, max.Y)));

                foreach (IEntity entity in _colliders)
                {
                    Collider collider = entity.GetComponent<Collider>();
                    Vector2[] corners = collider.GetPolygon();
                    if (corners[0].X > min.X && corners[0].X < max.X)
                        _verticalLines.Add(new Line(corners[0], corners[3]));
                    if (corners[1].X > min.X && corners[1].X < max.X)
                        _verticalLines.Add(new Line(corners[1], corners[2]));
                    if (corners[0].Y > min.Y && corners[0].Y < max.Y)
                        _horizontalLines.Add(new Line(corners[0], corners[1]));
                    if (corners[3].Y > min.Y && corners[3].Y < max.Y)
                        _horizontalLines.Add(new Line(corners[3], corners[2]));
                }
            }

            private List<Vector2> FindLine(Vector2 p1, Vector2 p2)
            {
                int x0 = (int)p1.X;
                int y0 = (int)p1.Y;
                int x1 = (int)p2.X;
                int y1 = (int)p2.Y;

                List<Vector2> line = new List<Vector2>();

                int dx = Math.Abs(x1 - x0);
                int dy = Math.Abs(y1 - y0);

                int sx = x0 < x1 ? 1 : -1;
                int sy = y0 < y1 ? 1 : -1;

                int err = dx - dy;
                int e2;

                while (true)
                {
                    line.Add(new Vector2(x0, y0));

                    if (x0 == x1 && y0 == y1)
                        break;

                    e2 = 2 * err;
                    if (e2 > -dy)
                    {
                        err = err - dy;
                        x0 = x0 + sx;
                    }

                    if (e2 < dx)
                    {
                        err = err + dx;
                        y0 = y0 + sy;
                    }
                }

                return line;
            }

            private void SaveLightmap()
            {
                Bitmap bitmap = new Bitmap(_lightmap.GetLength(0), _lightmap.GetLength(1));
                for (int x = 0; x < _lightmap.GetLength(0); x++)
                {
                    for (int y = 0; y < _lightmap.GetLength(1); y++)
                    {
                        int count = 1;
                        float lightValue = GetLightValue(x, y);
                        int radius = 2;
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            for (int dy = -radius; dy <= radius; dy++)
                            {
                                float neighbourLightValue = GetLightValue(x + dx, y + dy);
                                if (neighbourLightValue != -1)
                                {
                                    lightValue += neighbourLightValue;
                                    count++;
                                }
                            }
                        }

                        lightValue /= count;
                        bitmap.SetPixel(x, _lightmap.GetLength(1) - 1 - y, Color.FromArgb(255, (int)(lightValue * 255), (int)(lightValue * 255), (int)(lightValue * 255)));
                    }
                }

                Bitmap resized = new Bitmap(bitmap, new Size((int)Camera.ScreenWidth, (int)Camera.ScreenHeight));
                resized.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.villeon/rt.png");
            }

            private float GetLightValue(int x, int y)
            {
                if (x >= 0 && x < _lightmap.GetLength(0) &&
                    y >= 0 && y < _lightmap.GetLength(1))
                    return _lightmap[x, y];
                else
                    return -1;
            }

            private struct Line
            {
                public Vector2 P1;

                public Vector2 P2;

                public Line(Vector2 p1, Vector2 p2)
                {
                    P1 = p1;
                    P2 = p2;
                }
            }

            private struct RayCaster
            {
                public Vector2 Position;
                public float Intensity;

                public RayCaster(Vector2 position, float intensity)
                {
                    Position = position;
                    Intensity = intensity;
                }
            }
        }

        private class Running
        {
            public bool IsRunning { get; set; } = false;
        }
    }
}
