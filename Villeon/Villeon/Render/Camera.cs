using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Render
{
    public static class Camera
    {
        private static Matrix4 _shiftOriginMatrix = Matrix4.CreateTranslation(-1.0f, -1.0f, 0.0f);
        private static Matrix4 _aspectRatioMatrixY = Matrix4.Identity;

        private static Matrix4 _inverseViewportMatrix = Matrix4.Identity;
        private static Vector2 _trackingPosition = Vector2.Zero;

        private static Vector2 _cameraCenter = new (1f, 1f);
        private static float _cameraRotation;
        private static float _cameraScale = 10.0f;
        private static float _screenWidth;
        private static float _screenHeight;

        public static Matrix4 InverseViewportMatrix
        {
            get { return _inverseViewportMatrix; }
            set { _inverseViewportMatrix = value; }
        }

        public static Matrix4 Translate(float x, float y) => Matrix4.CreateTranslation(x, y, 0.0f);

        public static Matrix4 Translate(Vector2 translation) => Matrix4.CreateTranslation(-translation.X, -translation.Y, 0.0f);

        public static Matrix4 RotateDegrees(float degrees) => Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(degrees));

        public static Matrix4 RotateRadiants(float radiants) => Matrix4.CreateRotationZ(radiants);

        public static void Resize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;

            float aspectRatioY = width / (float)height;
            _aspectRatioMatrixY = Matrix4.CreateScale(1.0f, aspectRatioY, 1.0f);

            // Window to World conversions matrix
            Matrix4 translate = Translate(-1f, 1f); // Top left <- ^
            Matrix4 scale = Scale(2f / (width - 1), -2f / (height - 1));
            _inverseViewportMatrix = scale * translate;
        }

        public static Matrix4 Scale(float scale) => Matrix4.CreateScale(scale);

        public static Matrix4 Scale(float x, float y) => Matrix4.CreateScale(x, y, 1f);

        public static Matrix4 GetMatrix()
        {
            Matrix4 translation = Translate(-_cameraCenter);
            Matrix4 rotation = RotateDegrees(-_cameraRotation);
            Matrix4 scale = Scale(1 / _cameraScale);
            Matrix4 cameraMatrix = translation * rotation * scale * _aspectRatioMatrixY;
            return cameraMatrix;
        }

        public static Matrix4 GetInverseMatrix() => GetMatrix().Inverted();

        public static Vector2 Transform(this Vector2 input, Matrix4 transformation) => Vector4.TransformRow(new Vector4(input.X, input.Y, 0f, 1f), transformation).Xy;

        public static void SetRotation(float rotation) => _cameraRotation = rotation;

        public static void SetTracker(Vector2 position) => _trackingPosition = position;

        public static void Update()
        {
            // Set Camera to player
            _cameraCenter = -_trackingPosition;

            // Mouse Wheel Camera Scaling

            if (StateManager.IsPlaying)
                _cameraScale += -MouseHandler.WheelChanged();
            if (_cameraScale < 1f)
                _cameraScale = 1f;
            //if (_cameraScale > 20f)
             //   _cameraScale = 20f;

        }

        public static Matrix4 GetScreenMatrix()
        {
            float scaling = 10f;
            float aspect = 16f / 9f;
            float screenAspect = _screenWidth / (float)_screenHeight;
            Matrix4 screenMatrix = Matrix4.Identity;
            Matrix4 scaleMatrix = Scale(1 / scaling);
            if (aspect <= screenAspect)
            {
                screenMatrix = Matrix4.CreateScale(aspect / screenAspect, aspect, 1f);
            }
            else
            {
                screenMatrix = Matrix4.CreateScale(1f, (screenAspect / aspect) * aspect, 1f);
            }

            return scaleMatrix * screenMatrix;
        }
    }
}