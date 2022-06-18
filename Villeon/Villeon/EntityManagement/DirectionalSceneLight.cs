using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public class DirectionalSceneLight
    {
        private static Vector3[] _timeColors = new Vector3[24]
        {
            new (0f, 0.13f, 0.32f),         // 0    MIDNIGHTs
            new (0f, 0.2f, 0.4f),           // 1
            new (0f, 0.21f, 0.41f),         // 2
            new (0f, 0.25f, 0.42f),         // 3
            new (0f, 0.24f, 0.41f),         // 4
            new (0f, 0.36f, 0.53f),         // 5
            new (0.03f, 0.41f, 0.58f),      // 6    MORNING
            new (0.06f, 0.57f, 0.7f),       // 7
            new (0.42f, 0.8f, 0.79f),       // 8
            new (0.86f, 0.91f, 0.75f),      // 9
            new (0.97f, 0.88f, 0.37f),      // 10
            new (0.98f, 0.78f, 0.35f),      // 11
            new (1f, 0.7f, 0.43f),          // 12   MIDDLEDAY
            new (0.99f, 0.67f, 0.35f),      // 13
            new (0.99f, 0.64f, 0.35f),      // 14
            new (0.95f, 0.55f, 0.29f),      // 15
            new (0.95f, 0.45f, 0.47f),      // 16
            new (0.81f, 0.37f, 0.56f),      // 17
            new (0.42f, 0.2f, 0.53f),       // 18   EVENING
            new (0.24f, 0.11f, 0.48f),      // 19
            new (0.17f, 0.09f, 0.42f),      // 20
            new (0.12f, 0.15f, 0.39f),      // 21
            new (0.03f, 0.06f, 0.26f),      // 22
            new (0.01f, 0.05f, 0.24f),      // 23   NIGHT
        };

        public static Vector3 GetAmbientColor()
        {
            if (SceneLoader.CurrentScene.Name == "DungeonScene")
            {
                // Dungeon is completely dark
                return new Vector3(0.05f);
            }

            // In wich scene?
            // Dungeon -> Black
            int hourOfDay = (int)Time.CurrentDayTime;
            int nextHourOfDay = hourOfDay + 1;
            float lerpValue = 1 - (nextHourOfDay - Time.CurrentDayTime);
            if (nextHourOfDay >= 24)
                nextHourOfDay = 0;

            Vector3 lerpedColor = new Vector3(0f);
            lerpedColor.X = MathHelper.Lerp(_timeColors[hourOfDay].X, _timeColors[nextHourOfDay].X, lerpValue);
            lerpedColor.Y = MathHelper.Lerp(_timeColors[hourOfDay].Y, _timeColors[nextHourOfDay].Y, lerpValue);
            lerpedColor.Z = MathHelper.Lerp(_timeColors[hourOfDay].Z, _timeColors[nextHourOfDay].Z, lerpValue);
            return lerpedColor;
        }
    }
}
