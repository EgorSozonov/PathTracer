using System;
using System.Collections.Generic;
using System.Text;

namespace PathTracer {
    public class PathTracer {
        public static Random rnd = new Random();
        protected static Vec room0 = new Vec(-30, -0.5, -30);
        protected static Vec room1 = new Vec(30, 18, 30);
        protected static Vec room2 = new Vec(-25, 17, -25);
        protected static Vec room3 = new Vec(25, 20, 25);
        protected static Vec plank0 = new Vec(1.5, 18.5, -25);
        protected static Vec plank1 = new Vec(6.5, 20, 25);
        protected static readonly Vec dX = new Vec(0.01, 0, 0);
        protected static readonly Vec dY = new Vec(0, 0.01, 0);
        protected static readonly Vec dZ = new Vec(0, 0, 0.01);
        protected static readonly Vec lightDirection = (new Vec(0.6, 0.6, 1.0)).normalize();
        protected static readonly Vec colorSun = new Vec(50, 80, 100);
        protected static readonly Vec colorWall = new Vec(500, 400, 100);

        public static double min(double a, double b) { return a < b ? a : b; }

        /// Rectangle CSG equation. Returns minimum signed distance from
        /// space carved by lowerLeft vertex and opposite rectangle vertex upperRight.
        /// Negative return value if point is inside, positive if outside.
        public static double boxTest(Vec position, Vec lowerLeft, Vec upperRight) {
            Vec fromLowerLeft = position.minus(lowerLeft);
            Vec toUpperRight = upperRight.minus(position);
            return -min(
                        min(min(fromLowerLeft.x, toUpperRight.x),
                            min(fromLowerLeft.y, toUpperRight.y)),
                        min(fromLowerLeft.z, toUpperRight.z));
        }

        /// Cylinder CSG equation. Returns minimum signed distance from
        /// a cylinder oriented along the z axis.
        /// 'proximalCenter' = coordinates of the center of the closest cylinder base.
        double cylinderTest(Vec position, Vec proximalCenter, double radius, double height) {
            double distAxial = min(position.z - proximalCenter.z, proximalCenter.z + height - position.z);
            double distRadial = radius 
                                - Math.Sqrt(Math.Pow(position.x - proximalCenter.x, 2) + Math.Pow(position.y - proximalCenter.y, 2));
            return -min(distAxial, distRadial);
        }

        /// Cylinder CSG equation. Returns minimum signed distance from
        /// a cylinder oriented along the z axis and cut in half by the zy plane.
        /// 'proximalCenter' = coordinates of the center of the closest cylinder base.
        double halfCylinderTest(Vec position, Vec proximalCenter, double radius, double height) {
            double distAxial = min(position.z - proximalCenter.z, proximalCenter.z + height - position.z);
            double distRadial = radius
                                - Math.Sqrt(Math.Pow(position.x - proximalCenter.x, 2) + Math.Pow(position.y - proximalCenter.y, 2));
            return -min(distAxial, distRadial);
        }

        public static double queryDatabase(Vec position, ref Hit hit) {
            double distance = 1e9;            

            hit = Hit.Figure;
            Vec plankedPosition = new Vec(Math.Abs(position.x) % 8.0, position.y, position.z);
            double roomDist = min(
                -min(boxTest(position, room0, room1),
                    boxTest(position, room2, room3)),
                boxTest(plankedPosition, plank0, plank1));
            if (roomDist < distance) {
                distance = roomDist;
                hit = Hit.Wall;
            }
            double sun = 19.9 - position.y;
            if (sun < distance) {
                hit = Hit.Sun;
                return sun;
            }
            return distance;
        }

        public static Hit rayMarching(Vec origin, Vec direction, ref Vec hitPos, ref Vec hitNorm) {
            Hit hitType = Hit.None;
            int noHitCount = 0;
            double d = 0.0; // distance from the closest object in the world.
            for (double totalD = 0; totalD < 100; totalD += d) {
                hitPos = origin.plus(direction.times(totalD));
                d = queryDatabase(hitPos, ref hitType);
                if (d < 0.01 || ++noHitCount > 99) {
                    Hit temp = Hit.None;
                    double normX = queryDatabase(hitPos.plus(dX), ref temp) - d;
                    double normY = queryDatabase(hitPos.plus(dY), ref temp) - d;
                    double normZ = queryDatabase(hitPos.plus(dZ), ref temp) - d;
                    hitNorm = (new Vec(normX, normY, normZ)).normalize();
                    return hitType;
                }
            }
            return Hit.None;
        }

        public static Vec trace(ref Vec origin, Vec direction) {
            var hitPoint = new Vec(0, 0, 0);
            var newDirection = direction;
            var normal = new Vec(0, 0, 0);
            var color = new Vec(0, 0, 0);
            double attenuation = 1.0;
            for (int bounceCount = 3; bounceCount > 0; --bounceCount) {
                Hit hitType = rayMarching(origin, direction, ref hitPoint, ref normal);
                if (hitType == Hit.None) {
                    break;
                }
                if (hitType == Hit.Figure) { // Specular bounce on a letter. No color acc.
                    newDirection.minusM(normal.times(normal.dot(direction) * 2.0));
                    origin = hitPoint.plus(newDirection.times(0.1));
                    attenuation *= 0.2;
                } else if (hitType == Hit.Wall) {
                    double incidence = normal.dot(lightDirection);
                    double p = 6.283185 * rnd.NextDouble();
                    double c = rnd.NextDouble();
                    double s = Math.Sqrt(1.0 - c);
                    double g = normal.z < 0 ? -1 : 1;
                    double u = -1 / (g + normal.z);
                    double v = normal.x * normal.y * u;
                    Vec a = new Vec(v, g + normal.y * normal.y * u, -normal.y);
                    a.timesM(s * Math.Cos(p));
                    Vec b = new Vec(1 + g * normal.x * normal.x * u, g * v, -g * normal.x);
                    b.timesM(s * Math.Sin(p));

                    newDirection = a;
                    newDirection.plusM(b);
                    newDirection.plusM(normal.times(Math.Sqrt(c)));
                    origin = hitPoint.plus(newDirection.times(0.1));
                    attenuation *= 0.2;
                    var ptAbove = hitPoint.plus(normal.times(0.1));
                    if (incidence > 0) {
                        var tmp = rayMarching(ptAbove, lightDirection, ref hitPoint, ref normal);
                        if (tmp == Hit.Sun) {
                            color.plusM(colorWall.times(attenuation * incidence));
                        }
                    }
                } else if (hitType == Hit.Sun) {
                    color.plusM(colorSun.times(attenuation));
                    break;
                }
            }
            
            return color;
        }
    }
    public enum Hit { Wall, Sun, Figure, None, }
}
