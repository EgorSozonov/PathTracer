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
            return new Vec(this.x + r.x, this.y + r.y, this.z + r.z);
        }

        public Vec minus(Vec r) {
            return new Vec(this.x - r.x, this.y - r.y, this.z - r.z);
        }

        public Vec vecTimes(Vec r) {
            return new Vec(this.x * r.x, this.y * r.y, this.z * r.z);
        }

        public Vec times(double f) {
            return new Vec(this.x * f, this.y * f, this.z * f);
        }

        public double dot(Vec r) {
            return this.x * r.x + this.y * r.y + this.z * r.z;
        }

        public double length() {
            return Math.Sqrt(x*x + y*y + z*z);
        }

        public Vec normalize() {
            double len = Math.Sqrt(x * x + y * y + z * z);
            return len > 0.0 ? new Vec(this.x / len, this.y / len, this.z / len) : this;
        }
    }
}
