using System.Drawing;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace PaintItBlack
{
    static class FunctionClass
    {
        public static void Fill(Point p, Color fillColor, ref ZoomPictureBox PicBox)
        {
            int x = p.X;
            int y = p.Y;
            Color temp;
            temp = GetPx(x, y, ref PicBox);
            int countLeft = x;
            int countRight = x;

            while (countLeft - 1 > 0 && GetPx(countLeft - 1, y, ref PicBox).ToArgb() == temp.ToArgb())
            {
                countLeft--;
            }
            while (countRight + 1 < PicBox.image.Width && GetPx(countRight + 1, y, ref PicBox).ToArgb() == temp.ToArgb())
            {
                countRight++;
            }

            for (int i = countLeft; i <= countRight; i++)
            {
                PicBox.image.SetPixel(i, y, fillColor);
            }

            for (int i = countLeft; i <= countRight; i++)
            {
                if (GetPx(i, y - 1, ref PicBox).ToArgb() == temp.ToArgb() && y - 1 > 0)
                {
                    Fill(new Point(i, y - 1), fillColor, ref PicBox);
                }

                if (GetPx(i, y + 1, ref PicBox).ToArgb() == temp.ToArgb() && y + 1 < PicBox.image.Height - 1)
                {
                    Fill(new Point(i, y + 1), fillColor, ref PicBox);
                }
            }
        }
        public static Color GetPx(int x, int y, ref ZoomPictureBox PicBox)
        {
            return PicBox.image.GetPixel(x, y);
        }

        public static Rectangle CreateRect(Point xy, Point xy2)
        {
            int temp;
            Point StartPoint = xy;
            Point NowPoint = xy2;
            Size size;
            if (StartPoint.X > NowPoint.X)
            {
                temp = StartPoint.X;
                StartPoint.X = NowPoint.X;
                NowPoint.X = temp;
            }
            if (StartPoint.Y > NowPoint.Y)
            {
                temp = StartPoint.Y;
                StartPoint.Y = NowPoint.Y;
                NowPoint.Y = temp;
            }

            //рисование прямоугольника и эллипса с shift
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (Math.Abs(NowPoint.X - StartPoint.X) > Math.Abs(xy.Y - xy2.Y))
                {
                    size = new Size(Math.Abs(NowPoint.X - StartPoint.X), Math.Abs(NowPoint.X - StartPoint.X));
                    if (xy.X > xy2.X)
                    { StartPoint.X = xy.X - Math.Abs(NowPoint.X - StartPoint.X); }
                    if (xy.Y > xy2.Y)
                    { StartPoint.Y = xy.Y - Math.Abs(NowPoint.X - StartPoint.X); }
                }
                else if (Math.Abs(NowPoint.X - StartPoint.X) < Math.Abs(xy.Y - xy2.Y))
                {
                    size = new Size(Math.Abs(xy.Y - xy2.Y), Math.Abs(xy.Y - xy2.Y));
                    if (xy.X > xy2.X)
                    { StartPoint.X = xy.X - Math.Abs(xy.Y - xy2.Y); }
                    if (xy.Y > xy2.Y)
                    { StartPoint.Y = xy.Y - Math.Abs(xy.Y - xy2.Y); }
                }
                else
                { size = new Size(Math.Abs(NowPoint.X - StartPoint.X), Math.Abs(xy.Y - xy2.Y)); }
            }
            else
            {
                size = new Size(Math.Abs(NowPoint.X - StartPoint.X), Math.Abs(xy.Y - xy2.Y));
            }
            return new Rectangle(StartPoint, size);
        }
        public static void DrawLine(Point point1, Point point2, Graphics e, Pen p)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (Math.Abs(point2.X - point1.X) > Math.Abs(point2.Y - point1.Y))
                {
                    point2.Y = point1.Y;
                }
                else
                {
                    point2.X = point1.X;
                }
            }
            e.DrawLine(p, point1, point2);
        }

    }
}
