using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MaxRev.Gdal.Core;
using OSGeo.OGR;
using OxyPlot;
using OxyPlot.Series;

namespace ShapeFileVertices.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var myModel = new PlotModel();

            GetPlots();

            var points = GetPlots();
            var s1 = new ScatterSeries
            {
                MarkerType = MarkerType.Circle, 
                MarkerSize = 1, 
                MarkerStroke = OxyColors.Black
            };

            foreach (var point in points)
                s1.Points.Add(new ScatterPoint(point.lon, point.lat));

            myModel.Series.Add(s1);
            myModel.Title = $"{points.Count} Points";
            plot1.Model = myModel;
        }

        private List<(double lon, double lat)> GetPlots()
        {
            GdalBase.ConfigureAll();
            double[] pointList = {0, 0, 0};
            var points = new List<(double lon, double lat)>();

            foreach (var file in Directory.GetFiles("files", "*.shp"))
            {
                var dataSource = Ogr.Open(file, 0);
                var layer = dataSource.GetLayerByIndex(0);

                var envelop = new Envelope();
                layer.GetExtent(envelop, 0);

                for (var i = 0; i < layer.GetFeatureCount(0); ++i)
                {
                    var feature = layer.GetFeature(i);
                    var geo = feature.GetGeometryRef();
                    for (var j = 0; j < geo.GetGeometryCount(); ++j)
                    {
                        var inGeo = geo.GetGeometryRef(j);

                        for (var k = 0; k < inGeo.GetGeometryCount(); ++k)
                        {
                            var ring = inGeo.GetGeometryRef(k);

                            var pointCount = ring.GetPointCount();

                            for (var l = 0; l < pointCount; ++l)
                            {
                                ring.GetPoint(l, pointList);
                                points.Add((pointList[0], pointList[1]));
                                // pointList[0] is the Longitude.
                                // pointList[1] is the Latitude.
                                // pointList[2] is the Altitude. 
                            }
                        }
                    }
                }
            }

            return points;
        }
    }
}