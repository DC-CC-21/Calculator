using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Calculator
{
    struct Vector2
    {
        public double X;
        public double Y;
    }

    struct Vector3
    {
        public double X;
        public double Y;
        public double Z;
    }

    public class Graph
    {
        private Canvas canvas;
        private Label state;
        private Label output;

        private DateTime startTime;

        private Vector3 size = new()
        {
            X = 0,
            Y = 0,
            Z = 2
        };

        private Vector2 center;

        private double spacing = 1.0;

        private double X = 0.0;
        private string equation = "";

        public Graph(Canvas canvas, Label output, Label state)
        {
            this.canvas = canvas;
            this.output = output;
            this.state = state;
        }

        /// <summary>
        /// Update the canvas variables to match the current canvas dimensions
        /// </summary>
        private void UpdateCanvasValues()
        {
            size.X = canvas.ActualWidth;
            size.Y = canvas.ActualHeight;
            center = new()
            {
                X = size.X / 2.0,
                Y = size.Y / 2.0
            };

            DrawAxis();
        }

        /// <summary>
        /// Starts drawing the graph on the canvas
        /// </summary>
        /// <param name="equation"></param>
        public void StartGraph(string equation)
        {
            // Set the states to running
            Debug.WriteLine("Starting graph...");
            state.Content = "Running...";

            // Update the canvas values and set the default values
            UpdateCanvasValues();
            this.equation = equation;
            X = -center.X;
            startTime = DateTime.Now;
            CompositionTarget.Rendering += DrawPoint;
        }

        /// <summary>
        /// Draw the x and y axis
        /// </summary>
        public void DrawAxis()
        {
            // Clear the canvas to avoid drawing multiple axes
            canvas.Children.Clear();

            // Draw the x and y axis
            Line xAxis = new()
            {
                X1 = 0,
                Y1 = center.Y,
                X2 = size.X,
                Y2 = center.Y,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Line yAxis = new()
            {
                X1 = center.X,
                Y1 = 0,
                X2 = center.X,
                Y2 = size.Y,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);


            // Draw the ticks on the x axis
            int tickSpacing = 28;
            for (int i = 0; i < center.X; i += tickSpacing)
            {
                int offset = 10;

                // Positive Tick
                TextBlock xTick = new()
                {
                    Text = i.ToString()
                };
                canvas.Children.Add(xTick);
                Canvas.SetLeft(xTick, i + center.X - offset);
                Canvas.SetTop(xTick, center.Y + 2);

                // Negative Tick
                TextBlock nxTick = new()
                {
                    Text = (-i).ToString()
                };
                canvas.Children.Add(nxTick);
                Canvas.SetLeft(nxTick, -i + center.X - offset);
                Canvas.SetTop(nxTick, center.Y + 2);
            }

            // Draw the ticks on the y axis
            for (int i = 0; i < center.Y; i += Convert.ToInt32(tickSpacing*0.8))
            {
                int offset = 8;

                // Draw Positive Tick
                TextBlock yTick = new()
                {
                    Text = (i).ToString()
                };
                canvas.Children.Add(yTick);
                Canvas.SetLeft(yTick, center.X + 2);
                Canvas.SetTop(yTick, -i + center.Y - offset);

                // Draw Negative Tick
                TextBlock nyTick = new()
                {
                    Text = (-i).ToString()
                };
                canvas.Children.Add(nyTick);
                Canvas.SetLeft(nyTick, center.X + 2);
                Canvas.SetTop(nyTick, i + center.Y - offset);

            }
        }

        /// <summary>
        /// Stop drawing the graph
        /// </summary>
        public void Stop()
        {
            Debug.WriteLine("Stopping graph...");
            state.Content = "";
            CompositionTarget.Rendering -= DrawPoint;
        }

        /// <summary>
        /// Draws a point on the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawPoint(object? sender, EventArgs e)
        {
            // Replace all occurences of x with the current "X" value and evaluate the equation
            string eq = equation.Replace("x", X.ToString());
            double? Y = -Math.EvaluateMath(eq, output);

            if (Y is null)
            {
                X = 10000;
            }

            // Translate the point to the center of the canvas
            double pointX = X + center.X;
            double pointY = Convert.ToDouble(Y) + center.Y;

            // Draw the point to the canvas if it is within the canvas bounds
            if (InBounds(pointX, pointY))
            {
                Rectangle rect = new()
                {
                    Width = size.Z,
                    Height = size.Z,
                    Fill = new SolidColorBrush(Colors.Red)
                };
                Canvas.SetLeft(rect, pointX - size.Z / 2.0);
                Canvas.SetTop(rect, pointY - size.Z / 2.0);
                canvas.Children.Add(rect);
            }

            // Stop drawing if the point has crossed the entire width of the canvas or program has been drawing for 30 seconds
            TimeSpan drawTime = DateTime.Now - startTime;
            if (pointX > size.X || drawTime.TotalSeconds > 30)
            {
                Stop();
                return;
            }

            // Check if the next point is close to the current point
            double nextX = X + spacing;
            string nextEq = equation.Replace("x", nextX.ToString());
            double? nextY = -Math.EvaluateMath(nextEq, output);

            // Convert the currentY and nextY values to floats
            float floatY = Convert.ToSingle(Y);
            float floatY2 = Convert.ToSingle(nextY);

            // If the next point is close to the current point, increase the spacing
            if (MathF.Abs(floatY - floatY2) > 1)
            {
                X += spacing * 0.2;
            }
            else
            {
                X += spacing;
            }

            // If the point is out of bounds, call of the function again instead of wating for the next frame
            if (!InBounds(pointX, pointY))
            {
                DrawPoint(sender, e);
            }
        }

        /// <summary>
        /// Checks if the point is within the bounds of the canvas
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool InBounds(double x, double y)
        {
            return x >= 0 && y >= 0 && x <= size.X && y <= size.Y;
        }
    }
}
