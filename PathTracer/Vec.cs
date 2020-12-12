using System;
using System.Collections.Generic;
using System.Text;

namespace PathTracer {
    public struct Vec {
        public double x;
        public double y;
        public double z;

        public Vec(double _a, double _b, double _c) {
            x = _a;
            y = _b;
            z = _c;
        }

        public Vec plus(Vec r) {
            return new Vec(x + r.x, y + r.y, z + r.z);
        }

        public void plusM(Vec r) {
            x += r.x;
            y += r.y;
            z += r.z;
        }

        public Vec plusAll(double d) {
            return new Vec(x + d, y + d, z + d);
        }

        public void plusAllM(double d) {
            x += d;
            y += d;
            z += d;
        }

        public Vec minus(Vec r) {
            return new Vec(x - r.x, y - r.y, z - r.z);
        }

        public void minusM(Vec r) {
            this.x -= r.x;
            this.y -= r.y;
            this.z -= r.z;
        }

        public void vecTimesM(Vec r) {
            this.x *= r.x;
            this.y *= r.y;
            this.z *= r.z;
        }

        public Vec times(double f) {
            return new Vec(x * f, y * f, z * f);
        }

        public void timesM(double f) {
            this.x *= f;
            this.y *= f;
            this.z *= f;
        }

        public double dot(Vec r) {
            return this.x * r.x + this.y * r.y + this.z * r.z;
        }

        public double length() {
            return Math.Sqrt(x*x + y*y + z*z);
        }

        public Vec normalize() {
            double len = Math.Sqrt(x * x + y * y + z * z);
            if (len == 0.0) return this;
            return new Vec(x / len, y / len, z / len);
        }

        public void normalizeM() {
            double len = Math.Sqrt(x * x + y * y + z * z);
            if (len == 0.0) return;
            x /= len;
            y /= len;
            z /= len;
        }

        public override string ToString() {
            return $"{x} {y} {z}";
        }
    }
}
