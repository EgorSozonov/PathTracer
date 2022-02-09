using System;
using System.Collections.Generic;
using System.Text;

namespace PathTracer {
    public class PathTracer {
        public static Random rnd = new Random();
        protected static Vec room0 = new Vec(-30, -0.5, -30);
        protected static Vec room1 = new Vec(30, 15, 30);
        protected static Vec room2 = new Vec(-20, 14, -20);
        protected static Vec room3 = new Vec(20, 20, 20);
        protected static Vec plank0 = new Vec(1.5, 18.5, -25);
        protected static Vec plank1 = new Vec(6.5, 20, 25);
        protected static readonly Vec dX = new Vec(0.01, 0, 0);
        protected static readonly Vec dY = new Vec(0, 0.01, 0);
        protected static readonly Vec dZ = new Vec(0, 0, 0.01);
        protected static readonly Vec lightDirection = (new Vec(0.3, 0.6, 0.4)).normalize();
        protected static readonly Vec colorSun = new Vec(50, 80, 100);
        protected static readonly Vec colorWall = new Vec(500, 400, 100);

        public static double min(double a, double b) { return a < b ? a : b; }

        public static double carveOut(double a, double b) {
            if (b < 0) return -b;
            return Math.Min(a, b);
        }

        /// Rectangle CSG equation. Returns minimum signed distance from
        /// space carved by lowerLeft vertex and opposite rectangle vertex upperRight.
        /// Negative return value if point is inside, positive if outside.
        public static double probeBox(Vec position, Vec lowerLeft, Vec upperRight) {
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
        public static double probeCylinder(Vec position, Vec proximalCenter, double radius, double height) {
            double distAxial = -min(position.z - proximalCenter.z, proximalCenter.z + height - position.z);
            double diffX = position.x - proximalCenter.x;
            double diffY = position.y - proximalCenter.y;
            double distRadial = -radius 
                                + Math.Sqrt(diffX*diffX + diffY*diffY);
            if (distAxial > 0 && distRadial > 0) return Math.Min(distAxial, distRadial);
            return Math.Max(distRadial, distAxial);
        }

        /// Cylinder CSG equation. Returns minimum signed distance from
        /// a cylinder oriented along the z axis and cut in half by the zy plane.
        /// 'proximalCenter' = coordinates of the center of the closest cylinder base.
        double halfCylinderTest(Vec position, Vec proximalCenter, double radius, double height) {
            double distAxial = min(position.z - proximalCenter.z, proximalCenter.z + height - position.z);
            double distRadial = radius
                                - Math.Sqrt(Math.Pow(position.x - proximalCenter.x, 2) + Math.Pow(position.y - proximalCenter.y, 2));
            return Math.Max(distRadial, distAxial);
        }

        public static double queryDatabase(Vec position, ref Hit hit) {
            double distance = 1e9;
            //distance = min(distance,
            //    boxTest(position, new Vec(-10, 5, -4), new Vec(-8, 10, 6))
            //    );
            //var inner = probeCylinder(position, new Vec(-18, 6, 12), 1, 3);
            var outer = probeCylinder(position, new Vec(-10, 6, 12), 3, 3);

            //distance = min(distance, carveOut(outer, inner));
            distance = min(distance, outer);
            distance = Math.Max(distance, position.x) - 0.5;
            hit = Hit.Figure;
            Vec plankedPosition = new Vec(Math.Abs(position.x) % 8.0, position.y, position.z);
            double roomDist = min(
                -min(probeBox(position, room0, room1),
                    probeBox(position, room2, room3)),
                probeBox(plankedPosition, plank0, plank1));
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

        public static Vec trace(Vec origin, Vec direction) {
            var hitPoint = new Vec(0, 0, 0);
            var normal = new Vec(0, 0, 0);
            var result = new Vec(0, 0, 0);
            double attenuation = 1.0;
            var newDirection = new Vec(direction.x, direction.y, direction.z);
            var newOrigin = new Vec(origin.x, origin.y, origin.z);
            for (int bounceCount = 3; bounceCount > 0; --bounceCount) {
                Hit hitType = rayMarching(newOrigin, newDirection, ref hitPoint, ref normal);
                if (hitType == Hit.None) {
                    break;
                }
                if (hitType == Hit.Figure) { // Specular bounce on a letter. No color acc.

                    newDirection.minusM(normal.times(normal.dot(newDirection) * 2.0));
                    newOrigin = hitPoint.plus(newDirection.times(0.1));
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
                    newOrigin = hitPoint.plus(newDirection.times(0.1));
                    attenuation *= 0.2;
                    var ptAbove = hitPoint.plus(normal.times(0.1));
                    if (incidence > 0) {
                        var tmp = rayMarching(ptAbove, lightDirection, ref hitPoint, ref normal);
                        if (tmp == Hit.Sun) {
                            result.plusM(colorWall.times(attenuation * incidence));
                        }
                    }
                } else if (hitType == Hit.Sun) {
                    result.plusM(colorSun.times(attenuation));
                    break;
                }
            }            
            return result;
        }

        public void run(Vec position, Vec dirObserver, int samplesCount, int w, int h) {
            Vec dirLeft = (new Vec(dirObserver.z, 0, -dirObserver.x)).normalize();
            //dirLeft.timesM(1.0 / w);
            dirLeft.timesM(1.0 / h);
            // Cross-product to get the up vector

            Vec dirUp = new Vec(dirObserver.y * dirLeft.z - dirObserver.z * dirLeft.y,
                                dirObserver.z * dirLeft.x - dirObserver.x * dirLeft.z,
                                dirObserver.x * dirLeft.y - dirObserver.y * dirLeft.x);
            dirUp.normalizeM();
            dirUp.timesM(1.0 / h);
            byte[] pixels = new byte[3 * w * h];

            Parallel.For (1, h - 1, (y) => {
                for (int x = w; x > 0; --x) {
                    Vec color = new Vec(0, 0, 0);
                    for (int p = samplesCount; p > 0; --p) {
                        var randomLeft = dirLeft.times(x - w / 2 + rnd.NextDouble());
                        var randomUp = dirUp.times((y - h / 2 + rnd.NextDouble()));
                        var randomizedDir = new Vec(dirObserver.x, dirObserver.y, dirObserver.z);
                        randomizedDir.plusM(randomLeft);
                        randomizedDir.plusM(randomUp);
                        randomizedDir.normalizeM();
                        //Hit hType = Hit.None;
                        var incr = trace(position, randomizedDir);
                        if (y < h/2) {
                            ;
                        }
                        color.plusM(incr);
                    }

                    // Reinhard tone mapping
                    color.timesM(241.0 / samplesCount);
                    color = new Vec((color.x + 14.0) / (color.x + 255.0),
                                    (color.y + 14.0) / (color.y + 255.0),
                                    (color.z + 14.0) / (color.z + 255.0));
                    color.timesM(255.0);
                    int index = 3 * (w * y - w + x - 1);
                    pixels[index    ] = (byte)color.x;
                    pixels[index + 1] = (byte)color.y;
                    pixels[index + 2] = (byte)color.z;
                }
            });
            Output.createBMP(pixels, w, h, "card.bmp");
        }
    }

    public enum Hit { Wall, Sun, Figure, None, }
}
