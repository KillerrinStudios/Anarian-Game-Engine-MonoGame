using Anarian.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles
{
    public static class ParticleHelpers
    {
        public static int RandomBetween(int min, int max)
        {
            return AnarianConsts.Random.Next(min, max);
        }

        public static double RandomBetween(double min, double max)
        {
            if (min == max) return max;
            return min + (AnarianConsts.Random.NextDouble() * (max - min));
        }

        public static float RandomBetween(float min, float max)
        {
            if (min == max) return max;
            return min + (float)AnarianConsts.Random.NextDouble() * (max - min);
        }

        public static Vector2 PickRandomDirection()
        {
            float angle = RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public static Vector3 CreateRandomDirection()
        {
            Vector3 DirVect;
            DirVect.X = RandomBetween(-1, 1);
            DirVect.Y = RandomBetween(-1, 1);
            DirVect.Z = RandomBetween(-1, 1);

            DirVect.Normalize();

            return DirVect;
        }

        #region Linear Interpolate
        public static byte LinearInterpolate(byte a, byte b, double t)
        {
            return (byte)(a * (1 - t) + b * t);
        }
        public static float LinearInterpolate(float a, float b, double t)
        {
            return (float)(a * (1 - t) + b * t);
        }
        public static Vector2 LinearInterpolate(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t));
        }
        public static Vector4 LinearInterpolate(Vector4 a, Vector4 b, double t)
        {
            return new Vector4(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t), LinearInterpolate(a.Z, b.Z, t), LinearInterpolate(a.W, b.W, t));
        }
        public static Color LinearInterpolate(Color a, Color b, double t)
        {
            return new Color(LinearInterpolate(a.R, b.R, t), LinearInterpolate(a.G, b.G, t), LinearInterpolate(a.B, b.B, t), LinearInterpolate(a.A, b.A, t));
        }
        #endregion
    }
}
