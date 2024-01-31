using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Painting
{
    struct Point3d
    {
        public int X;
        public int Y;
        public int Z;

        public Point3d(int x, int y, int z = 0)
        {
            X = x; Y = y; Z = z;
        }

        public string ToString() => X + ", " + Y + ", " + Z;
    }
    
    class ImageConversion
    {
        static int[,] map;
        private static List<Point3d> way = new List<Point3d>();
        private static int pixelNum = 0;

        public static Boolean CreateMapFromImg(ref WriteableBitmap img, string filename)
        {
            pixelNum = 0;

            img = BitmapFactory.FromContent(filename);
            int width = (int)img.Width;
            int height = (int)img.Height;

            map = new int[height, width]; // создаем размер массива

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (img.GetPixel(j, i) == Color.FromRgb(0, 0, 255))
                    {
                        map[i, j] = 1;
                        pixelNum++;       // Счетчик единичных элементов, т.е. счетчик нарисованных пикселей
                    }
                    else map[i, j] = 0;

            return true;
        }

        public static Point3d FindStartPoint()
        {
            Point3d startpoint = new Point3d();

            for (int y = 1; y < map.GetLength(0) - 1; ++y)
            {
                for (int x = 1; x < map.GetLength(1) - 1; ++x)
                {
                    if (map[y, x] == 1)
                    {
                        startpoint = new Point3d(x, y);
                        map[y, x] = -1;

                        return startpoint;
                    }
                }
            }

            return startpoint;
        }

        public static List<Point3d> CreateWay(Point3d startpoint)
        {
            way.Clear();
            int step = 1;  // количество шагов. Соответствует количеству пикселей в матрице

            // с этой клетки начинается отчет пути
            int row = startpoint.Y;
            int col = startpoint.X;
            way.Add(new Point3d(col, row));

            while (step <= pixelNum - 1)
            {
                if (map[row - 1, col - 1] == 1)
                {
                    map[row - 1, col - 1] = step + 1;
                    row = row - 1;
                    col = col - 1;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row - 1, col] == 1)
                {
                    map[row - 1, col] = step + 1;
                    row = row - 1;
                    col = col;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row - 1, col + 1] == 1)
                {
                    map[row - 1, col + 1] = step + 1;
                    row = row - 1;
                    col = col + 1;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row, col + 1] == 1)
                {
                    map[row, col + 1] = step + 1;
                    row = row;
                    col = col + 1;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row + 1, col + 1] == 1)
                {
                    map[row + 1, col + 1] = step + 1;
                    row = row + 1;
                    col = col + 1;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row + 1, col] == 1)
                {
                    map[row + 1, col] = step + 1;
                    row = row + 1;
                    col = col;
                    way.Add(new Point3d(col, row));
                } else

                if (map[row + 1, col - 1] == 1)
                {
                    map[row + 1, col - 1] = step + 1;
                    row = row + 1;
                    col = col - 1;
                    way.Add(new Point3d(col, row));
                } else 

                if (map[row, col - 1] == 1)
                {
                    map[row, col - 1] = step + 1;
                    row = row;
                    col = col - 1;
                    way.Add(new Point3d(col, row));
                }

                step++;
            }

            return way;
        }
    }
}
