using OpenTK.Mathematics;
using System;

using Villeon.Helper;
using Villeon.Components;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Villeon.Systems
{
    public static class Camera
    {
        private static Matrix4 _cameraMatrix = Matrix4.Identity;
        private static Matrix4 _shiftOriginMatrix = Matrix4.CreateTranslation(-1.0f, -1.0f, 0.0f);
        private static Matrix4 _aspectRatioMatrix = Matrix4.Identity;
        
        public static Matrix4 Translate(float x, float y)
        {
            return Matrix4.CreateTranslation(x, y, 0.0f);
        }

        public static Matrix4 Translate(Vector2 translation)
        {
            return Matrix4.CreateTranslation(-translation.X, -translation.Y, 0.0f);
        }

        public static Matrix4 RotateDegrees(float degrees)
        {
            return Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(degrees));
        }

        public static Matrix4 RotateRadiants(float radiants)
        {
            return Matrix4.CreateRotationZ(radiants);
        }

        public static void Resize(int width, int height)
        {
            float aspectRatio = height / (float)width;
            _aspectRatioMatrix = Matrix4.CreateScale(aspectRatio, 1.0f, 1.0f);
        }

        public static Matrix4 Scale(float scale)
        {
            return Matrix4.CreateScale(scale);
        }

        public static Matrix4 GetMatrix()
        {
            Matrix4 translation = Translate(-CameraCenter);
            Matrix4 rotation = RotateDegrees(-CameraRotation);
            Matrix4 scale = Scale(1 / CameraScale);
            _cameraMatrix = translation * rotation * scale * _aspectRatioMatrix;
            return _cameraMatrix;
        }

        private static Vector2 CameraCenter { get; set; } = new(1f, 1f);
        private static float CameraRotation { get; set; }
        private static float CameraScale { get; set; } = 9.0f;

        public static void Update(IEntity player)
        {
            // Set Camera to player
            Collider collider = player.GetComponent<Collider>();
            Vector2 position = collider.Position + new Vector2(collider.Width/2, collider.Height/2);
            CameraCenter = -position;

            // Mouse Wheel Camera Scaling
            CameraScale += -MouseHandler.WheelChanged();
            if (CameraScale < 1f)
                CameraScale = 1f;
        }
    }
}