using System;

namespace Common.Entities.Pathfinding
{

    [Serializable]
    public class Point2D
    {
        private double[] _coordinates = new double[3];

        public Point2D(double[] coordinates)
        {
            if (coordinates == null) throw new ArgumentNullException();
            if (coordinates.Length != 3)
                throw new ArgumentException("The Coordinates' array must contain exactly 3 elements.");
            X = coordinates[0];
            Y = coordinates[1];
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Accede to coordinates by indexes.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Index must belong to [0;2].</exception>
        public double this[int coordinateIndex]
        {
            get => _coordinates[coordinateIndex];
            set => _coordinates[coordinateIndex] = value;
        }

        /// <summary>
        /// Gets/Set X coordinate.
        /// </summary>
        public double X
        {
            set => _coordinates[0] = value;
            get => _coordinates[0];
        }

        /// <summary>
        /// Gets/Set Y coordinate.
        /// </summary>
        public double Y
        {
            set => _coordinates[1] = value;
            get => _coordinates[1];
        }


        /// <summary>
        /// Returns the distance between two points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <returns>Distance value.</returns>
        public static double DistanceBetween(Point2D p1, Point2D p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }


        /// <summary>
        /// Object.Equals override.
        /// Tells if two points are equal by comparing coordinates.
        /// </summary>
        /// <exception cref="ArgumentException">Cannot compare Point2D with another type.</exception>
        /// <param name="point">The other 3DPoint to compare with.</param>
        /// <returns>'true' if points are equal.</returns>
        public override bool Equals(object point)
        {
            Point2D p = (Point2D) point;
            return X == p.X && Y == p.Y;
        }

        /// <summary>
        /// Object.GetHashCode override.
        /// </summary>
        /// <returns>HashCode value.</returns>
        public override int GetHashCode()
        {
            double res = X + 45.47543278721;
            res = 31 * res + Y;
            //res = 37 * res + 0;
            return res.GetHashCode();
        }

        /// <summary>
        /// Object.GetHashCode override.
        /// Returns a textual description of the point.
        /// </summary>
        /// <returns>String describing this point.</returns>
        public override string ToString()
        {
            return $"{{{X};{Y}}}";
        }
    }
}