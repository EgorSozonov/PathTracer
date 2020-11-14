using System;

namespace PathTracer {
    class Program {
        static void Main(string[] args) {            
            //byte[] testData = new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127,
            // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127
            // };
            //Output.createBMP(testData, 16, 9, "test.bmp");
            //return;


            //  int w = 960, h = 540, samplesCount = 16;
            int w = 240, h = 135, samplesCount = 8;
            Vec position = new Vec(-22, 5, 25);
            Vec goal = (new Vec(-3, 4, 0)).minus(position).normalize();
            Vec dirLeft = (new Vec(goal.z, 0, -goal.x)).normalize().times(1.0 / w);

            // Cross-product to get the up vector
            Vec dirUp = new Vec(goal.y*dirLeft.z - goal.z*dirLeft.y,
                                goal.z*dirLeft.x - goal.x*dirLeft.z,
                                goal.x*dirLeft.y - goal.y*dirLeft.x);

            byte[] pixels = new byte[3 * w * h];
            int countOutput = 0;
            for (int y = h; y > 0; --y) {
                for (int x = w; x > 0; --x) {
                    Vec color = new Vec(0, 0, 0);
                    for (int p = samplesCount; p > 0; --p) {
                        var randomizedDir = goal;
                        randomizedDir.plusM(dirLeft.times(x - w / 2 + PathTracer.rnd.NextDouble()));
                        randomizedDir.plusM(dirUp.times((y - h / 2 + PathTracer.rnd.NextDouble())));
                        randomizedDir.normalizeM();
                        Hit hType = Hit.None;
                        var incr = PathTracer.trace(ref position, randomizedDir);
                        if (countOutput++ < 1) {
                            Console.WriteLine($"{incr.x} {incr.y} {incr.z} {hType}");
                        }
                        color.plusM(incr);
                    }

                    // Reinhard tone mapping
                    color.timesM(241.0/samplesCount);
                    color = new Vec((color.x + 14.0) / (color.x + 255.0),
                                    (color.y + 14.0) / (color.y + 255.0),
                                    (color.z + 14.0) / (color.z + 255.0));
                    color.timesM(255.0);
                    int index = 3 * (w * y - w + x - 1);
                    pixels[index    ] = (byte)color.x;
                    pixels[index + 1] = (byte)color.y;
                    pixels[index + 2] = (byte)color.z;
                }
            }
            Output.createBMP(pixels, w, h, "card.bmp");
            Console.ReadKey();
        }
    }
}
