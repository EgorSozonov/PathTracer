using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PathTracer {
    public class Output {
        public static int createBMP(byte[] data, int w, int h, string fName) {
            int fileSize = 54 + 3 * w * h;
            byte[] img = new byte[3 * w * h];
            for (int i = 0; i < w; ++i) {
                for (int j = 0; j < h; ++j) {
                    int indSource = 3 * (j * w + i);
                    int indTarget = 3 * ((h - j - 1) * w + i);
                    img[indTarget    ] = data[indSource + 2];
                    img[indTarget + 1] = data[indSource + 1];
                    img[indTarget + 2] = data[indSource    ];

                }
            }
            byte[] bmpHeader = new byte[14] { Convert.ToByte('B'), Convert.ToByte('M'), 0, 0,  0, 0, 0, 0,  0, 0, 54, 0,  0, 0, };
            bmpHeader[2] = (byte)(fileSize);
            bmpHeader[3] = (byte)(fileSize >> 8);
            bmpHeader[4] = (byte)(fileSize >> 16);
            bmpHeader[5] = (byte)(fileSize >> 24);

            byte[] bmpInfoHeader = new byte[40] {
                40, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  1, 0, 24, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,
                0, 0, 0, 0,  0, 0, 0, 0,
            };
            bmpInfoHeader[ 4] = (byte)(w      );
            bmpInfoHeader[ 5] = (byte)(w >>  8);
            bmpInfoHeader[ 6] = (byte)(w >> 16);
            bmpInfoHeader[ 7] = (byte)(w >> 24);
            bmpInfoHeader[ 8] = (byte)(h      );
            bmpInfoHeader[ 9] = (byte)(h >>  8);
            bmpInfoHeader[10] = (byte)(h >> 16);
            bmpInfoHeader[11] = (byte)(h >> 24);

            byte[] bmpPad = new byte[3] { 0, 0, 0, };


            using (var f = new FileStream(fName, FileMode.Create, FileAccess.Write)) {
                f.Write(bmpHeader, 0, 14);
                f.Write(bmpInfoHeader, 0, 40);
                int lenPad = (4 - (w * 3) % 4) % 4;
                if (lenPad > 0) {
                    for (int i = 0; i < h; ++i) {
                        f.Write(img, 3 * i * w, 3*w);
                        f.Write(bmpPad, 0, 3);
                    }
                } else {
                    for (int i = 0; i < h; ++i) {
                        f.Write(img, 3 * i * w, 3*w);
                    }
                }
            }
            return 0;
        }
    }
}
