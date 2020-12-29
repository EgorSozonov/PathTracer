using System;
namespace PathTracerTest {
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PathTracer;

    [TestClass]
    public class GeometryTest {
        [TestMethod]
        public void BoxTest() {
            var box0 = new Vec(0, 0, 0);
            var box1 = new Vec(10, 20, 30);
            Assert.IsTrue(equalF(PathTracer.probeBox(new Vec(5, 5, 5), box0, box1), -5.0), "Inside box");
            Assert.IsTrue(equalF(PathTracer.probeBox(new Vec(5, 25, 5), box0, box1), 5.0), "Outside the box");
        }

        [TestMethod]
        public void CylTest() {
            var cyl0 = new Vec(-10, 6, 12);
            Assert.IsTrue(equalF(PathTracer.probeCylinder(new Vec(-10, 6, 13), cyl0, 3, 3), -1.0), "Inside cyl");
            Assert.IsTrue(equalF(PathTracer.probeCylinder(new Vec(-10, 6, 11), cyl0, 3, 3), 1.0), "Outside cyl");
            Assert.IsTrue(equalF(PathTracer.probeCylinder(new Vec(-10, 7, 17), cyl0, 3, 10), -2.0), "Inside cyl radial");
            Assert.IsTrue(equalF(PathTracer.probeCylinder(new Vec(-10, 10, 17), cyl0, 3, 10), 1.0), "Outside cyl radial");
        }

        public static bool equalF(double a, double b) {
            return Math.Abs(a - b) <= 0.001;
        }
    }
}
