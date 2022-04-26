﻿using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon
{
    public static class Camera
    {
        private static Matrix4 _cameraMatrix = Matrix4.Identity;
        private static Matrix4 _shiftOriginMatrix = Matrix4.CreateTranslation(-1.0f, -1.0f, 0.0f);
        private static Matrix4 _aspectRatioMatrix = Matrix4.Identity;
        private static Matrix4 _inverseViewportMatrix = Matrix4.Identity;
        private static Vector2 _trackingPosition = Vector2.Zero;

        private static Vector2 _cameraCenter = new (1f, 1f);
        private static float _cameraRotation;
        private static float _cameraScale = 9.0f;

        public static Matrix4 InverseViewportMatrix
        {
            get { return _inverseViewportMatrix; }
            set { _inverseViewportMatrix = value; }
        }

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

            // Window to World converstoin matrix
            Matrix4 translate = Translate(-1f, 1f); // Top left <- ^
            Matrix4 scale = Scale(2f / (width - 1), -2f / (height - 1));
            _inverseViewportMatrix = scale * translate;
        }

        public static Matrix4 Scale(float scale)
        {
            return Matrix4.CreateScale(scale);
        }

        public static Matrix4 Scale(float x, float y)
        {
            return Matrix4.CreateScale(x, y, 1f);
        }

        public static Matrix4 GetMatrix()
        {
            Matrix4 translation = Translate(-_cameraCenter);
            Matrix4 rotation = RotateDegrees(-_cameraRotation);
            Matrix4 scale = Scale(1 / _cameraScale);
            _cameraMatrix = translation * rotation * scale * _aspectRatioMatrix;
            return _cameraMatrix;
        }

        public static Matrix4 GetInverseMatrix()
        {
            Matrix4 inverseMatrix = GetMatrix().Inverted();
            return inverseMatrix;
        }

        public static Vector2 Transform(this Vector2 input, Matrix4 transformation)
        {
            return Vector4.TransformRow(new Vector4(input.X, input.Y, 0f, 1f), transformation).Xy;
        }

        public static void SetRotation(float rotation)
        {
            _cameraRotation = rotation;
        }

        public static void SetTracker(Vector2 position)
        {
            _trackingPosition = position;
        }

        public static void Update()
        {
            // Set Camera to player
            _cameraCenter = -_trackingPosition;

            // Mouse Wheel Camera Scaling
            _cameraScale += -MouseHandler.WheelChanged();
            if (_cameraScale < 1f)
                _cameraScale = 1f;
        }
    }
}