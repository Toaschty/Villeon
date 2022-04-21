using OpenTK.Mathematics;
using System;

namespace Villeon.Systems
{
    public static class Camera
    {
        private static Matrix4 _cameraMatrix = Matrix4.Identity;
        private static Matrix4 _translationMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
        private static Matrix4 _rotationMatrix = Matrix4.CreateRotationZ(0.0f);
        private static Matrix4 _scaleMatrix = Matrix4.CreateScale(1/9.0f); // World: Height 9, Width = 16
        private static Matrix4 _shiftOriginMatrix = Matrix4.CreateTranslation(-1.0f, -1.0f, 0.0f);
        private static Matrix4 _aspectRationMatrix = Matrix4.Identity;
        
        public static void Translate(float x, float y)
        {
            _translationMatrix = Matrix4.CreateTranslation(x, y, 0.0f);
        }

        public static void Translate(Vector2 translation)
        {
            _translationMatrix = Matrix4.CreateTranslation(translation.X, translation.Y, 0.0f);
        }

        public static void RotateDegrees(float degrees)
        {
            _rotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(degrees));
        }

        public static void RotateRadiants(float radiants)
        {
            _rotationMatrix = Matrix4.CreateRotationZ(radiants);
        }

        public static void Resize(int width, int height)
        {
            float aspectRatio = height / (float)width;
            _aspectRationMatrix = Matrix4.CreateScale(aspectRatio, 1.0f, 1.0f);
        }

        public static void Scale(float x, float y)
        {
            _scaleMatrix = Matrix4.CreateScale(x, y, 1.0f);
        }

        public static void Scale(Vector2 scale)
        {
            _scaleMatrix = Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
        }

        public static Matrix4 GetMatrix()
        {
            _cameraMatrix = _translationMatrix * _rotationMatrix * _scaleMatrix * _aspectRationMatrix;
            _cameraMatrix *= _shiftOriginMatrix;
            return _cameraMatrix;
        }

        public static void Update()
        {
            
        }
    }
}