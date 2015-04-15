/**
 *  Chart.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Text;


namespace PDFjet.NET {
/**
 *  Used to create XY chart objects and draw them on a page.
 *
 *  Please see Example_09.
 */
public class Chart {

    private float w = 300f;
    private float h = 200f;

    private float x1;
    private float y1;

    private float x2;
    private float y2;

    private float x3;
    private float y3;

    private float x4;
    private float y4;

    private float x5;
    private float y5;

    private float x6;
    private float y6;

    private float x7;
    private float y7;

    private float x8;
    private float y8;

    private float x_max = System.Single.MinValue;
    private float x_min = System.Single.MaxValue;

    private float y_max = System.Single.MinValue;
    private float y_min = System.Single.MaxValue;

    private int x_axis_grid_lines = 0;
    private int y_axis_grid_lines = 0;

    private String title = "";
    private String x_axis_title = "";
    private String y_axis_title = "";

    private bool drawXAxisLabels = true;
    private bool drawYAxisLabels = true;

    private float h_grid_line_width = 0f;
    private float v_grid_line_width = 0f;

    private String h_grid_line_pattern = "[1 1] 0";
    private String v_grid_line_pattern = "[1 1] 0";

    private float chart_border_width = 0.3f;
    private float inner_border_width = 0.3f;

    private NumberFormat nf = null;
    private int minFractionDigits = 2;
    private int maxFractionDigits = 2;

    private Font f1 = null;
    private Font f2 = null;

    private List<List<Point>> chartData = null;


    /**
     *  Create a XY chart object.
     *
     *  @param f1 the font used for the chart title.
     *  @param f2 the font used for the X and Y axis titles.
     */
    public Chart(Font f1, Font f2) {
        this.f1 = f1;
        this.f2 = f2;
        nf = NumberFormat.getInstance();
    }


    /**
     *  Sets the title of the chart.
     *
     *  @param title the title text.
     */
    public void SetTitle(String title) {
        this.title = title;
    }


    /**
     *  Sets the title for the X axis.
     *
     *  @param title the X axis title.
     */
    public void SetXAxisTitle(String title) {
        this.x_axis_title = title;
    }


    /**
     *  Sets the title for the Y axis.
     *
     *  @param title the Y axis title.
     */
    public void SetYAxisTitle(String title) {
        this.y_axis_title = title;
    }


    /**
     *  Sets the data that will be used to draw this chart.
     *
     *  @param chartData the data.
     */
    public void SetData(List<List<Point>> chartData) {
        this.chartData = chartData;
    }


    /**
     *  Returns the chart data.
     *
     *  @return the chart data.
     */
    public List<List<Point>> GetData() {
        return chartData;
    }


    /**
     *  Sets the position of this chart on the page.
     *
     *  @param x the x coordinate of the top left corner of this chart when drawn on the page.
     *  @param y the y coordinate of the top left corner of this chart when drawn on the page.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position of this chart on the page.
     *
     *  @param x the x coordinate of the top left corner of this chart when drawn on the page.
     *  @param y the y coordinate of the top left corner of this chart when drawn on the page.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location of this chart on the page.
     *
     *  @param x the x coordinate of the top left corner of this chart when drawn on the page.
     *  @param y the y coordinate of the top left corner of this chart when drawn on the page.
     */
    public void SetLocation(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Sets the size of this chart.
     *
     *  @param w the width of this chart.
     *  @param h the height of this chart.
     */
    public void SetSize(double w, double h) {
        SetSize((float) w, (float) h);
    }


    /**
     *  Sets the size of this chart.
     *
     *  @param w the width of this chart.
     *  @param h the height of this chart.
     */
    public void SetSize(float w, float h) {
        this.w = w;
        this.h = h;
    }


    /**
     *  Sets the minimum number of fractions digits do display for the X and Y axis labels.
     *
     *  @param minFractionDigits the minimum number of fraction digits.
     */
    public void SetMinimumFractionDigits(int minFractionDigits) {
        this.minFractionDigits = minFractionDigits;
    }


    /**
     *  Sets the maximum number of fractions digits do display for the X and Y axis labels.
     *
     *  @param maxFractionDigits the maximum number of fraction digits.
     */
    public void SetMaximumFractionDigits(int maxFractionDigits) {
        this.maxFractionDigits = maxFractionDigits;
    }


    /**
     *  Calculates the Slope of a trend line given a list of points.
     *  See Example_09.
     *
     *  @param points the list of points.
     *  @return the Slope float value.
     */
    public float Slope(List<Point> points) {
        return (Covar(points) / Devsq(points) * (points.Count - 1));
    }


    /**
     *  Calculates the Intercept of a trend line given a list of points.
     *  See Example_09.
     *
     *  @param points the list of points.
     *  @return the Intercept float value.
     */
    public float Intercept(List<Point> points, double slope) {
        return Intercept(points, (float) slope);
    }


    /**
     *  Calculates the Intercept of a trend line given a list of points.
     *  See Example_09.
     *
     *  @param points the list of points.
     *  @return the Intercept float value.
     */
    public float Intercept(List<Point> points, float slope) {
        float[] _mean = Mean(points);
        return (_mean[1] - slope * _mean[0]);
    }


    public void SetDrawXAxisLabels(bool drawXAxisLabels) {
        this.drawXAxisLabels = drawXAxisLabels;
    }


    public void SetDrawYAxisLabels(bool drawYAxisLabels) {
        this.drawYAxisLabels = drawYAxisLabels;
    }

    
    /**
     *  Draws this chart on the specified page.
     *
     *  @param page the page to draw this chart on.
     */
    public void DrawOn(Page page) {
        nf.SetMinimumFractionDigits(minFractionDigits);
        nf.SetMaximumFractionDigits(maxFractionDigits);

        x2 = x1 + w;
        y2 = y1;

        x3 = x2;
        y3 = y1 + h;

        x4 = x1;
        y4 = y3;

        SetMinAndMaxChartValues();
        RoundXAxisMinAndMaxValues();
        RoundYAxisMinAndMaxValues();

        // Draw chart title
        page.DrawString(
                f1,
                title,
                x1 + ((w - f1.StringWidth(title)) / 2),
                y1 + 1.5f * f1.body_height);

        float top_margin = 2.5f * f1.body_height;
        float left_margin = GetLongestAxisYLabelWidth() + 2f * f2.body_height;
        float right_margin = 2f * f2.body_height;
        float bottom_margin = 2.5f * f2.body_height;

        x5 = x1 + left_margin;
        y5 = y1 + top_margin;

        x6 = x2 - right_margin;
        y6 = y5;

        x7 = x6;
        y7 = y3 - bottom_margin;

        x8 = x5;
        y8 = y7;

        DrawChartBorder(page);
        DrawInnerBorder(page);

        DrawHorizontalGridLines(page);
        DrawVerticalGridLines(page);

        if (drawXAxisLabels) {
            DrawXAxisLabels(page);
        }
        if (drawYAxisLabels) {
            DrawYAxisLabels(page);
        }

        // Translate the point coordinates
        for (int i = 0; i < chartData.Count; i++) {
            List<Point> points = chartData[i];
            for (int j = 0; j < points.Count; j++) {
                Point point = points[j];
                point.x = x5 + (point.x - x_min) * (x6 - x5)
                        / (x_max - x_min);
                point.y = y8 - (point.y - y_min) * (y8 - y5)
                        / (y_max - y_min);
                if (point.GetURIAction() != null) {
                    page.AddAnnotation(new Annotation(
                            point.GetURIAction(),
                            null,
                            point.x - point.r,
                            page.height - (point.y - point.r),
                            point.x + point.r,
                            page.height - (point.y + point.r),
                            null,
                            null,
                            null));
                }
            }
        }

        DrawPathsAndPoints(page, chartData);

        // Draw the Y axis title
        page.SetBrushColor(Color.black);
        page.SetTextDirection(90);
        page.DrawString(
                f2,
                y_axis_title,
                x1 + f2.body_height,
                y8 - ((y8 - y5) - f2.StringWidth(y_axis_title)) / 2);

        // Draw the X axis title
        page.SetTextDirection(0);
        page.DrawString(
                f2,
                x_axis_title,
                x5 + ((x6 - x5) - f2.StringWidth(x_axis_title)) / 2,
                y4 - f2.body_height / 2);

        page.SetDefaultLineWidth();
        page.SetDefaultLinePattern();
        page.SetPenColor(Color.black);
    }


    private float GetLongestAxisYLabelWidth() {
        float min_label_width =
                f2.StringWidth(nf.Format(y_min) + "0");
        float max_label_width =
                f2.StringWidth(nf.Format(y_max) + "0");
        if (max_label_width > min_label_width) {
            return max_label_width;
        }
        return min_label_width;
    }


    private void SetMinAndMaxChartValues() {
        for (int i = 0; i < chartData.Count; i++) {
            List<Point> points = chartData[i];
            for (int j = 0; j < points.Count; j++) {
                Point point = points[j];
                if (point.x < x_min) {
                    x_min = point.x;
                }
                if (point.x > x_max) {
                    x_max = point.x;
                }
                if (point.y < y_min) {
                    y_min = point.y;
                }
                if (point.y > y_max) {
                    y_max = point.y;
                }
            }
        }
    }


    private void RoundXAxisMinAndMaxValues() {
        Round round = RoundMaxAndMinValues(x_max, x_min);
        x_max = round.max_value;
        x_min = round.min_value;
        x_axis_grid_lines = round.num_of_grid_lines;
    }


    private void RoundYAxisMinAndMaxValues() {
        Round round = RoundMaxAndMinValues(y_max, y_min);
        y_max = round.max_value;
        y_min = round.min_value;
        y_axis_grid_lines = round.num_of_grid_lines;
    }


    private void DrawChartBorder(Page page) {
        page.SetPenWidth(chart_border_width);
        page.SetPenColor(Color.black);
        page.MoveTo(x1, y1);
        page.LineTo(x2, y2);
        page.LineTo(x3, y3);
        page.LineTo(x4, y4);
        page.ClosePath();
        page.StrokePath();
    }


    private void DrawInnerBorder(Page page) {
        page.SetPenWidth(inner_border_width);
        page.SetPenColor(Color.black);
        page.MoveTo(x5, y5);
        page.LineTo(x6, y6);
        page.LineTo(x7, y7);
        page.LineTo(x8, y8);
        page.ClosePath();
        page.StrokePath();
    }


    private void DrawHorizontalGridLines(Page page) {
        page.SetPenWidth(h_grid_line_width);
        page.SetPenColor(Color.black);
        page.SetLinePattern(h_grid_line_pattern);
        float x = x8;
        float y = y8;
        float step = (y8 - y5) / y_axis_grid_lines;
        for (int i = 0; i < y_axis_grid_lines; i++) {
            page.DrawLine(x, y, x6, y);
            y -= step;
        }
    }


    private void DrawVerticalGridLines(Page page) {
        page.SetPenWidth(v_grid_line_width);
        page.SetPenColor(Color.black);
        page.SetLinePattern(v_grid_line_pattern);
        float x = x5;
        float y = y5;
        float step = (x6 - x5) / x_axis_grid_lines;
        for (int i = 0; i < x_axis_grid_lines; i++) {
            page.DrawLine(x, y, x, y8);
            x += step;
        }
    }


    private void DrawXAxisLabels(Page page) {
        float x = x5;
        float y = y8 + f2.body_height;
        float step = (x6 - x5) / x_axis_grid_lines;
        page.SetBrushColor(Color.black);
        for (int i = 0; i < (x_axis_grid_lines + 1); i++) {
            String label = nf.Format(
                    x_min + (x_max - x_min) / x_axis_grid_lines * i);
            page.DrawString(
                    f2, label, x - (f2.StringWidth(label) / 2), y);
            x += step;
        }
    }


    private void DrawYAxisLabels(Page page) {
        float x = x5 - GetLongestAxisYLabelWidth();
        float y = y8 + f2.ascent / 3;
        float step = (y8 - y5) / y_axis_grid_lines;
        page.SetBrushColor(Color.black);
        for (int i = 0; i < (y_axis_grid_lines + 1); i++) {
            String label = nf.Format(
                    y_min + (y_max - y_min) / y_axis_grid_lines * i);
            page.DrawString(f2, label, x, y);
            y -= step;
        }
    }


    private void DrawPathsAndPoints(
            Page page, List<List<Point>> chartData) {
        for (int i = 0; i < chartData.Count; i++) {
            List<Point> points = chartData[i];
            Point point = points[0];
            if (point.isStartOfPath) {
                page.SetPenColor(point.color);
                page.SetPenWidth(point.lineWidth);
                page.SetLinePattern(point.linePattern);
                page.DrawPath(points, Operation.STROKE);
                if (point.GetText() != null) {
                    page.SetBrushColor(point.GetTextColor());
                    page.SetTextDirection(point.GetTextDirection());
                    page.DrawString(f2, point.GetText(), point.x, point.y);
                }
            }
            for (int j = 0; j < points.Count; j++) {
                point = points[j];
                if (point.GetShape() != Point.INVISIBLE) {
                    page.SetPenWidth(point.lineWidth);
                    page.SetLinePattern(point.linePattern);
                    page.SetPenColor(point.color);
                    page.SetBrushColor(point.color);
                    page.DrawPoint(point);
                }
            }
        }
    }


    private Round RoundMaxAndMinValues(float max_value, float min_value) {

        int max_exponent = (int) Math.Floor(Math.Log(max_value) / Math.Log(10));
        max_value *= (float) Math.Pow(10, -max_exponent);

        if      (max_value > 9.00f) { max_value = 10.0f; }
        else if (max_value > 8.00f) { max_value = 9.00f; }
        else if (max_value > 7.00f) { max_value = 8.00f; }
        else if (max_value > 6.00f) { max_value = 7.00f; }
        else if (max_value > 5.00f) { max_value = 6.00f; }
        else if (max_value > 4.00f) { max_value = 5.00f; }
        else if (max_value > 3.50f) { max_value = 4.00f; }
        else if (max_value > 3.00f) { max_value = 3.50f; }
        else if (max_value > 2.50f) { max_value = 3.00f; }
        else if (max_value > 2.00f) { max_value = 2.50f; }
        else if (max_value > 1.75f) { max_value = 2.00f; }
        else if (max_value > 1.50f) { max_value = 1.75f; }
        else if (max_value > 1.25f) { max_value = 1.50f; }
        else if (max_value > 1.00f) { max_value = 1.25f; }
        else                        { max_value = 1.00f; }

        Round round = new Round();

        if      (max_value == 10.0f) { round.num_of_grid_lines = 10; }
        else if (max_value == 9.00f) { round.num_of_grid_lines =  9; }
        else if (max_value == 8.00f) { round.num_of_grid_lines =  8; }
        else if (max_value == 7.00f) { round.num_of_grid_lines =  7; }
        else if (max_value == 6.00f) { round.num_of_grid_lines =  6; }
        else if (max_value == 5.00f) { round.num_of_grid_lines =  5; }
        else if (max_value == 4.00f) { round.num_of_grid_lines =  8; }
        else if (max_value == 3.50f) { round.num_of_grid_lines =  7; }
        else if (max_value == 3.00f) { round.num_of_grid_lines =  6; }
        else if (max_value == 2.50f) { round.num_of_grid_lines =  5; }
        else if (max_value == 2.00f) { round.num_of_grid_lines =  8; }
        else if (max_value == 1.75f) { round.num_of_grid_lines =  7; }
        else if (max_value == 1.50f) { round.num_of_grid_lines =  6; }
        else if (max_value == 1.25f) { round.num_of_grid_lines =  5; }
        else if (max_value == 1.00f) { round.num_of_grid_lines = 10; }

        round.max_value = max_value * ((float) Math.Pow(10, max_exponent));
        float step = round.max_value / round.num_of_grid_lines;
        float temp = round.max_value;
        round.num_of_grid_lines = 0;
        for (;;) {
            round.num_of_grid_lines++;
            temp -= step;
            if (temp <= min_value) {
                round.min_value = temp;
                break;
            }
        }

        return round;
    }


    private float[] Mean(List<Point> points) {
        float[] _mean = new float[2];
        for (int i = 0; i < points.Count; i++) {
            Point point = points[i];
            _mean[0] += point.x;
            _mean[1] += point.y;
        }
        _mean[0] /= points.Count - 1;
        _mean[1] /= points.Count - 1;
        return _mean;
    }


    private float Covar(List<Point> points) {
        float covariance = 0f;
        float[] _mean = Mean(points);
        for (int i = 0; i < points.Count; i++) {
            Point point = points[i];
            covariance += (point.x - _mean[0]) * (point.y - _mean[1]);
        }
        return (covariance / (points.Count - 1));
    }


    /**
     * Devsq() returns the sum of squares of deviations.
     *
     */
    private float Devsq(List<Point> points) {
        float _devsq = 0f;
        float[] _mean = Mean(points);
        for (int i = 0; i < points.Count; i++) {
            Point point = points[i];
            _devsq += (float) Math.Pow((point.x - _mean[0]), 2);
        }
        return _devsq;
    }

}   // End of Chart.cs
}   // End of namespace PDFjet.NET
