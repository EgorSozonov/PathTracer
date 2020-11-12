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
            this.x += r.x;
            this.y += r.y;
            z += r.z;
            return this;
        }

        public Vec minus(Vec r) {
            this.x -= r.x;
            this.y -= r.y;
            this.z -= r.z;
            return this;
        }

        public Vec vecTimes(Vec r) {
            this.x *= r.x;
            this.y *= r.y;
            this.z *= r.z;
            return this;
        }

        public Vec times(double f) {
            this.x *= f;
            this.y *= f;
            this.z *= f;
            return this;
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
            x /= len;
            y /= len;
            z /= len;
            return this;
        }
    }
}
