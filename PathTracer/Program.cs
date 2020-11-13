﻿using System;

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
            Vec dirLeft = (new Vec(goal.z, 0, -goal.x)).times(1.0 / w).normalize();

            // Cross-product to get the up vector
            Vec dirUp = new Vec(goal.y* dirLeft.z -goal.z * dirLeft.y,
                  goal.z* dirLeft.x - goal.x * dirLeft.z,
                  goal.x* dirLeft.y - goal.y * dirLeft.x);

            byte[] pixels = new byte[3 * w * h];
            for (int y = h; y > 0; --y) {
                for (int x = w; x > 0; --x) {
                    Vec color;
                    for (int p = samplesCount; p > 0; --p) {
                        color.plusM(PathTracer.trace(position, !(goal + left * (x - w / 2 + rnd.NextDouble()) + up * (y - h / 2 + rnd.NextDouble()))));
                    }

                    // Reinhard tone mapping
                    color.timesM(1.0/samplesCount);
                    color.plusAllM(14.0/241);
                    Vec o = color.plusAll(1.0);
                    color = (new Vec(color.x / o.x, color.y / o.y, color.z / o.z));
                    color.timesM(255.0);
                    int index = 3 * (w * y - w + x - 1);
                    pixels[index    ] = (byte)color.x;
                    pixels[index + 1] = (byte)color.y;
                    pixels[index + 2] = (byte)color.z;
                }
            }
            Output.createBMP(pixels, w, h, "cardCPP.bmp");

        }
    }
}
