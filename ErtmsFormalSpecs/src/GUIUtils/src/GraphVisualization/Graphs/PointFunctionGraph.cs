﻿using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization.Functions;

namespace GUIUtils.GraphVisualization.Graphs
{
    public class PointFunctionGraph : Graph
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphVisualizer"></param>
        /// <param name="function"></param>
        public PointFunctionGraph(GraphVisualizer graphVisualizer, PointFunction function)
            : base(graphVisualizer)
        {
            Function = function;
            Data.MarkerSize = 8;
        }

        /// <summary>
        /// Handles the display
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        protected override void HandleDisplay(double maxDistance, double minDistance, double height)
        {
            PointFunction pointFunction = Function as PointFunction;
            if (pointFunction != null)
            {
                Data.ChartType = SeriesChartType.Point;
                if (GraphVisualizer.RecordPreviousValuesInTSM)
                {
                    DisplayPreviousData();
                }
                if (pointFunction.Point != null && !double.IsNaN(pointFunction.Point.Distance))
                {
                    AddPoint(new DataPoint(pointFunction.Point.Distance, pointFunction.Point.Speed));
                }
                else if (pointFunction.SimulatedValues.Count != 0)
                {
                    Data.ChartType = SeriesChartType.Line;
                    DataPoint dataPoint;
                    foreach (SpeedDistanceProfile profile in pointFunction.SimulatedValues)
                    {
                        dataPoint = new DataPoint(0, 0);
                        dataPoint.IsEmpty = true;
                        AddPoint(dataPoint);
                        foreach (SpeedDistancePoint point in profile.Points)
                        {
                            dataPoint = new DataPoint(point.Distance, point.Speed);
                            AddPoint(dataPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Nothing to save
        /// </summary>
        protected override void SaveSettings()
        {
        }
    }
}