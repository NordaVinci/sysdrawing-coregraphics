using System;
using System.Collections;
using System.Drawing;
//using System.Windows.Forms;

namespace Example4_2
{
    public class DataCollection
    {
        private ArrayList dataSeriesList;
        private int dataSeriesIndex = 0;

        public DataCollection()
        {
            dataSeriesList = new ArrayList();
        }

        public ArrayList DataSeriesList
        {
            get { return dataSeriesList; }
            set { dataSeriesList = value; }
        }
        public int DataSeriesIndex
        {
            get { return dataSeriesIndex; }
            set { dataSeriesIndex = value; }
        }

        public void Add(DataSeries ds)
        {
            dataSeriesList.Add(ds);
            if (ds.SeriesName == "")
            {
                ds.SeriesName = "DataSeries" + dataSeriesList.Count.ToString();
            }
        }

        public void Insert(int dataSeriesIndex, DataSeries ds)
        {
            dataSeriesList.Insert(dataSeriesIndex, ds);
            if (ds.SeriesName == "")
            {
                dataSeriesIndex = dataSeriesIndex + 1;
                ds.SeriesName = "DataSeries" + dataSeriesIndex.ToString();
            }
        }

        public void Remove(string dataSeriesName)
        {
            if (dataSeriesList != null)
            {
                for (int i = 0; i < dataSeriesList.Count; i++)
                {
                    DataSeries ds = (DataSeries)dataSeriesList[i];
                    if (ds.SeriesName == dataSeriesName)
                    {
                        dataSeriesList.RemoveAt(i);
                    }
                }
            }
        }

        public void RemoveAll()
        {
            dataSeriesList.Clear();
        }

        public void AddBars(Graphics g, ChartStyle cs, int numberOfDataSeries, int numberOfPoints)
        {
            // Draw bars:
            ArrayList temp = new ArrayList();
            float[] tempy = new float[numberOfPoints];
            PointF temppt = new PointF();
            int n = 0;
            foreach (DataSeries ds in DataSeriesList)
            {
                Pen aPen = new Pen(ds.BarStyle.BorderColor, ds.BarStyle.BorderThickness);
                SolidBrush aBrush = new SolidBrush(ds.BarStyle.FillColor);
                aPen.DashStyle = ds.BarStyle.BorderPattern;
                PointF[] pts = new PointF[4];
                PointF pt;
                float width;

                if (cs.BarType == ChartStyle.BarTypeEnum.Vertical)
                {
                    if (numberOfDataSeries == 1)
                    {
                        // Find the minumum and maximum y values:
                        float ymin = 0;
                        float ymax = 0;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            ymin = Math.Min(ymin, pt.Y);
                            ymax = Math.Max(ymax, pt.Y);
                        }

                        width = cs.XTick * ds.BarStyle.BarWidth;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            float x = pt.X - cs.XTick / 2;
                            pts[0] = cs.Point2D(new PointF(x - width / 2, 0));
                            pts[1] = cs.Point2D(new PointF(x + width / 2, 0));
                            pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y));
                            pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y));
                            if (ds.IsSingleColorMap)
                            {
                                DrawColorMap(g, pts, ds.CMap, ymin, ymax, pt.Y);
                            }
                            else if (ds.IsColorMap)
                            {
                                float dy = (ymax - ymin) / 100;
                                PointF[] points = new PointF[4];
                                for (int j = 0; j <= (int)Math.Round(100 * (pt.Y - ymin) / 
                                    (ymax - ymin)); j++)
                                {
                                    points[0] = cs.Point2D(new PointF(x - width / 2, (j - 1) * dy));
                                    points[1] = cs.Point2D(new PointF(x + width / 2, (j - 1) * dy));
                                    points[2] = cs.Point2D(new PointF(x + width / 2, j * dy));
                                    points[3] = cs.Point2D(new PointF(x - width / 2, j * dy));
                                    DrawColorMap(g, points, ds.CMap, ymin, ymax, j * dy);
                                }
                            }
                            else
                            {
                                g.FillPolygon(aBrush, pts);
                            }
                            g.DrawPolygon(aPen, pts);
                        }
                    }
                    else if (numberOfDataSeries > 1)
                    {
                        width = 0.7f * cs.XTick;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            float w1 = width / numberOfDataSeries;
                            float w = ds.BarStyle.BarWidth * w1;
                            float space = (w1 - w) / 2;
                            float x = pt.X - cs.XTick / 2;
                            pts[0] = cs.Point2D(new PointF(
                                x - width / 2 + space + n * w1, 0));
                            pts[1] = cs.Point2D(new PointF(
                                x - width / 2 + space + n * w1 + w, 0));
                            pts[2] = cs.Point2D(new PointF(
                                x - width / 2 + space + n * w1 + w, pt.Y));
                            pts[3] = cs.Point2D(new PointF(
                                x - width / 2 + space + n * w1, pt.Y));
                            g.FillPolygon(aBrush, pts);
                            g.DrawPolygon(aPen, pts);
                        }
                    }
                }
                else if (cs.BarType == ChartStyle.BarTypeEnum.VerticalOverlay 
                         && numberOfDataSeries >1)
                {
                    width = cs.XTick * ds.BarStyle.BarWidth;
                    width = width / (float)Math.Pow(2,n);
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        pt = (PointF)ds.PointList[i];
                        float x = pt.X - cs.XTick / 2;
                        pts[0] = cs.Point2D(new PointF(x - width / 2, 0));
                        pts[1] = cs.Point2D(new PointF(x + width / 2, 0));
                        pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y));
                        pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y));
                        g.FillPolygon(aBrush, pts);
                        g.DrawPolygon(aPen, pts);
                    }
                }
                else if (cs.BarType == ChartStyle.BarTypeEnum.VerticalStack
                    && numberOfDataSeries > 1)
                {
                    width = cs.XTick * ds.BarStyle.BarWidth;
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        pt = (PointF)ds.PointList[i];
                        if (temp.Count > 0)
                        {
                           tempy[i] = tempy[i] + ((PointF)temp[i]).Y;
                        }
                        float x = pt.X - cs.XTick / 2;
                        pts[0] = cs.Point2D(new PointF(x - width / 2, 0 + tempy[i]));
                        pts[1] = cs.Point2D(new PointF(x + width / 2, 0 + tempy[i]));
                        pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y + tempy[i]));
                        pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y + tempy[i]));

                        g.FillPolygon(aBrush, pts);
                        g.DrawPolygon(aPen, pts);
                    }
                    temp = ds.PointList;
                }

                else if (cs.BarType == ChartStyle.BarTypeEnum.Horizontal)
                {
                    if (numberOfDataSeries == 1)
                    {
                        width = cs.YTick * ds.BarStyle.BarWidth;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            float y = pt.Y - cs.YTick / 2;
                            pts[0] = cs.Point2D(new PointF(0, y - width / 2));
                            pts[1] = cs.Point2D(new PointF(0, y + width / 2));
                            pts[2] = cs.Point2D(new PointF(pt.X, y + width / 2));
                            pts[3] = cs.Point2D(new PointF(pt.X, y - width / 2));
                            g.FillPolygon(aBrush, pts);
                            g.DrawPolygon(aPen, pts);
                        }
                    }
                    else if (numberOfDataSeries > 1)
                    {
                        width = 0.7f * cs.YTick;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            float w1 = width / numberOfDataSeries;
                            float w = ds.BarStyle.BarWidth * w1;
                            float space = (w1 - w) / 2;
                            float y = pt.Y - cs.YTick / 2;
                            pts[0] = cs.Point2D(new PointF(0,
                                y - width / 2 + space + n * w1));
                            pts[1] = cs.Point2D(new PointF(0,
                                y - width / 2 + space + n * w1 + w));
                            pts[2] = cs.Point2D(new PointF(pt.X,
                                y - width / 2 + space + n * w1 + w));
                            pts[3] = cs.Point2D(new PointF(pt.X,
                                y - width / 2 + space + n * w1));
                            g.FillPolygon(aBrush, pts);
                            g.DrawPolygon(aPen, pts);
                        }
                    }
                }
                else if (cs.BarType == ChartStyle.BarTypeEnum.HorizontalOverlay &&
                         numberOfDataSeries > 1)
                {
                    width = cs.YTick * ds.BarStyle.BarWidth;
                    width = width / (float)Math.Pow(2, n);
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        temppt = (PointF)ds.PointList[i];
                        pt = new PointF(temppt.Y, temppt.X);
                        float y = pt.Y - cs.YTick / 2;
                        pts[0] = cs.Point2D(new PointF(0, y - width / 2));
                        pts[1] = cs.Point2D(new PointF(0, y + width / 2));
                        pts[2] = cs.Point2D(new PointF(pt.X, y + width / 2));
                        pts[3] = cs.Point2D(new PointF(pt.X, y - width / 2));
                        g.FillPolygon(aBrush, pts);
                        g.DrawPolygon(aPen, pts);
                    }
                }
                else if (cs.BarType == ChartStyle.BarTypeEnum.HorizontalStack &&
                         numberOfDataSeries > 1)
                {
                    {
                        width = cs.YTick * ds.BarStyle.BarWidth;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            if (temp.Count > 0)
                            {
                                temppt = (PointF)temp[i];
                                tempy[i] = tempy[i] + temppt.Y;
                            }
                            float y = pt.Y - cs.YTick / 2;
                            pts[0] = cs.Point2D(new PointF(0 + tempy[i], y - width / 2));
                            pts[1] = cs.Point2D(new PointF(0 + tempy[i], y + width / 2));
                            pts[2] = cs.Point2D(new PointF(pt.X + tempy[i], y + width / 2));
                            pts[3] = cs.Point2D(new PointF(pt.X + tempy[i], y - width / 2));

                            g.FillPolygon(aBrush, pts);
                            g.DrawPolygon(aPen, pts);
                        }
                        temp = ds.PointList;
                    }
                }
               n++;
                aPen.Dispose();
            }
        }

        private void DrawColorMap(Graphics g, PointF[] pts, int[,] cmap,
                                    float ymin, float ymax, float y)
        {
            int colorLength = cmap.GetLength(0);
            int cindex = (int)Math.Round((colorLength * (y - ymin) +
                        (ymax - y)) / (ymax - ymin));
            if (cindex < 1)
                cindex = 1;
            if (cindex > colorLength)
                cindex = colorLength;
            Color color = Color.FromArgb(cmap[cindex - 1, 0], cmap[cindex - 1, 1],
                                         cmap[cindex - 1, 2], cmap[cindex - 1, 3]);
            SolidBrush aBrush = new SolidBrush(color);
            g.FillPolygon(aBrush, pts);
        }
    }
}
