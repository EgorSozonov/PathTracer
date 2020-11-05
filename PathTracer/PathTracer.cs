using System;
using System.Collections.Generic;
using System.Text;

namespace PathTracer {
    public class PathTracer {
        protected Random rnd = new Random();
        protected static Vec room0 = new Vec(-30, -0.5, -30);
        protected static Vec room1 = new Vec(30, 18, 30);
        protected static Vec room2 = new Vec(-25, 17, -25);
        protected static Vec room3 = new Vec(25, 20, 25);
       
        public static double min(double a, double b) { return a < b ? a : b; }

        // Rectangle CSG equation. Returns minimum signed distance from
        // space carved by
        // lowerLeft vertex and opposite rectangle vertex upperRight.
        public static double boxText(Vec position, Vec lowerLeft, Vec upperRight) {
            Vec toLowerLeft = position.minus(lowerLeft);
            Vec fromUpperRight = upperRight.minus(position);
            return -min(
                        min(min(toLowerLeft.x, fromUpperRight.x), 
                            min(toLowerLeft.y, fromUpperRight.y)),
                        min(toLowerLeft.z, fromUpperRight.z));
        }

        public static double queryDatabase(Vec position, ref Hit hit) {
            double distance = 1e9;
            Vec f = position.times(1.0);
            f.z = 0.0;
            hit = Hit.Figure;
            double roomDist = min(
                -min(boxTest(position, room0, room1),
                    boxText(position, room2, room3)),

        }
    }

    public enum Hit {
        Wall, Sun, Figure, None,
    }


}
/*

// Sample the world using Signed Distance Fields.
float QueryDatabase(Vec position, int &hitType) {
  

  float roomDist ;
  roomDist = min(// min(A,B) = Union with Constructive solid geometry
               //-min carves an empty space
                -min(// Lower room
                     BoxTest(position, Vec(-30, -.5, -30), Vec(30, 18, 30)),
                     // Upper room
                     BoxTest(position, Vec(-25, 17, -25), Vec(25, 20, 25))
                ),
                BoxTest( // Ceiling "planks" spaced 8 units apart.
                  Vec(fmodf(fabsf(position.x), 8),
                      position.y,
                      position.z),
                  Vec(1.5, 18.5, -25),
                  Vec(6.5, 20, 25)
                )
  );  
  if (roomDist < distance) distance = roomDist, hitType = HIT_WALL;

  float sun = 19.9 - position.y ; // Everything above 19.9 is light source.
  if (sun < distance)distance = sun, hitType = HIT_SUN;

  return distance;
}

// Perform signed sphere marching
// Returns hitType 0, 1, 2, or 3 and update hit position/normal
int RayMarching(Vec origin, Vec direction, Vec &hitPos, Vec &hitNorm) {
  int hitType = HIT_NONE;
  int noHitCount = 0;
  float d; // distance from closest object in world.

  // Signed distance marching
  for (float total_d=0; total_d < 100; total_d += d)
    if ((d = QueryDatabase(hitPos = origin + direction * total_d, hitType)) < .01
            || ++noHitCount > 99)
      return hitNorm =
         !Vec(QueryDatabase(hitPos + Vec(.01, 0), noHitCount) - d,
              QueryDatabase(hitPos + Vec(0, .01), noHitCount) - d,
              QueryDatabase(hitPos + Vec(0, 0, .01), noHitCount) - d)
         , hitType; // Weird return statement where a variable is also updated.
  return 0;
}

Vec Trace(Vec origin, Vec direction) {
  Vec sampledPosition, normal, color, attenuation = 1;
  Vec lightDirection(!Vec(.6, .6, 1)); // Directional light

  for (int bounceCount = 3; bounceCount--;) {
    int hitType = RayMarching(origin, direction, sampledPosition, normal);
    if (hitType == HIT_NONE) break; // No hit. This is over, return color.
    if (hitType == HIT_LETTER) { // Specular bounce on a letter. No color acc.
      direction = direction + normal * ( normal % direction * -2);
      origin = sampledPosition + direction * 0.1;
      attenuation = attenuation * 0.2; // Attenuation via distance traveled.
    }
    if (hitType == HIT_WALL) { // Wall hit uses color yellow?
      float incidence = normal % lightDirection;
      float p = 6.283185 * randomVal();
      float c = randomVal();
      float s = sqrtf(1 - c);
      float g = normal.z < 0 ? -1 : 1;
      float u = -1 / (g + normal.z);
      float v = normal.x * normal.y * u;
      direction = Vec(v,
                      g + normal.y * normal.y * u,
                      -normal.y) * (cosf(p) * s)
                  +
                  Vec(1 + g * normal.x * normal.x * u,
                      g * v,
                      -g * normal.x) * (sinf(p) * s) + normal * sqrtf(c);
      origin = sampledPosition + direction * .1;
      attenuation = attenuation * 0.2;
      if (incidence > 0 &&
          RayMarching(sampledPosition + normal * .1,
                      lightDirection,
                      sampledPosition,
                      normal) == HIT_SUN)
        color = color + attenuation * Vec(500, 400, 100) * incidence;
    }
    if (hitType == HIT_SUN) { //
      color = color + attenuation * Vec(50, 80, 100); break; // Sun Color
    }
  }
  return color;
}

void createBMP(byte data[], int w, int h, char* fName) {
    FILE *f;
    int filesize = 54 + 3*w*h;

    byte *img = new byte[3*w*h];
    //memset(img, 0, 3*w*h);

    for(int i = 0; i < w; ++i) {
        for(int j = 0; j < h; ++j) {
            int indSource = (j*w + i)*3;
            int indTarget = (w*j + w - i - 1)*3;

            img[indTarget    ] = data[indSource + 2];
            img[indTarget + 1] = data[indSource + 1];
            img[indTarget + 2] = data[indSource    ];
        }
    }

    byte bmpfileheader[14] = { 'B', 'M', 0,0,0,0, 0,0, 0,0, 54, 0, 0,  0};
    byte bmpinfoheader[40] = { 40,0,0,0, 0,0,0,0, 0,0, 0,0, 1,  0, 24, 0};
    byte bmppad[3] = {0, 0, 0};

    bmpfileheader[ 2] = (byte)(filesize    );
    bmpfileheader[ 3] = (byte)(filesize>> 8);
    bmpfileheader[ 4] = (byte)(filesize>>16);
    bmpfileheader[ 5] = (byte)(filesize>>24);

    bmpinfoheader[ 4] = (byte)(w    );
    bmpinfoheader[ 5] = (byte)(w>> 8);
    bmpinfoheader[ 6] = (byte)(w>>16);
    bmpinfoheader[ 7] = (byte)(w>>24);
    bmpinfoheader[ 8] = (byte)(h    );
    bmpinfoheader[ 9] = (byte)(h>> 8);
    bmpinfoheader[10] = (byte)(h>>16);
    bmpinfoheader[11] = (byte)(h>>24);

    f = fopen(fName,"wb");
    fwrite(bmpfileheader, 1, 14, f);
    fwrite(bmpinfoheader, 1, 40, f);
    int lenPad = (4 - (w*3)%4)%4;
    if (lenPad > 0) {
        for(int i = 0; i < h; i++) {
            fwrite(img + w*i*3, 3, w, f);
            fwrite(bmppad, 1, lenPad, f);
        }
    } else {
        for(int i = 0; i < h; i++) {
            fwrite(img + (w*i*3), 3, w, f);
        }
    }   

    delete[] img;
    fclose(f);
}

int main() {
  // byte testData[] = { 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 
  // 255, 0, 0, 255, 0, 0, 255, 0, 0, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127
  // };
  // createBMP(testData, 16, 9, "test.bmp");
  // return 0;

//  int w = 960, h = 540, samplesCount = 16;
  int w = 240, h = 135, samplesCount = 8;
  Vec position(-22, 5, 25);
  Vec goal = !(Vec(-3, 4, 0) + position * -1);
  Vec left = !Vec(goal.z, 0, -goal.x) * (1. / w);

  // Cross-product to get the up vector
  Vec up(goal.y * left.z - goal.z * left.y,
      goal.z * left.x - goal.x * left.z,
      goal.x * left.y - goal.y * left.x);

  //printf("P6 %d %d 255 ", w, h);
  byte* pixels = new byte[3*w*h];
  for (int y = h; y > 0; --y) {
    for (int x = w; x > 0; --x) {
      Vec color;
      for (int p = samplesCount; p--;) {
        color = color + Trace(position, !(goal + left * (x - w / 2 + randomVal()) + up * (y - h / 2 + randomVal())));
      }

      // Reinhard tone mapping
      color = color * (1. / samplesCount) + 14. / 241;
      Vec o = color + 1;
      color = Vec(color.x / o.x, color.y / o.y, color.z / o.z) * 255;
      int index = 3*(w*y - w + x - 1);
      pixels[index    ] = (byte)color.x;
      pixels[index + 1] = (byte)color.y;
      pixels[index + 2] = (byte)color.z;
      //if (x < w/2 && y < h/2) printf("%d %d \n", x, y);
      //printf("%c%c%c", (int) color.x, (int) color.y, (int) color.z);
    }
  }
  createBMP(pixels, w, h, "cardCPP.bmp");
}

float CylTest(Vec position, Vec lowerLeft, Vec upperRight) {
  lowerLeft = position + lowerLeft * -1;
  upperRight = upperRight + position * -1;
  return -min(
          min(
                  min(lowerLeft.x, upperRight.x),
                  min(lowerLeft.y, upperRight.y)),
          min(lowerLeft.z, upperRight.z));
}
*/
