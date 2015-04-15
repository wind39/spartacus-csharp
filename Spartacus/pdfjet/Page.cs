/**
 *  Page.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and / or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;


namespace PDFjet.NET {
/**
 *  Used to create PDF page objects.
 *
 *  Please note:
 *  <pre>
 *  The coordinate (0.0f, 0.0f) is the top left corner of the page.
 *  The size of the pages are represented in points.
 *  1 point is 1/72 inches.
 *  </pre>
 *
 */
public class Page {

    internal PDF pdf;
    internal int objNumber;
    internal MemoryStream buf;
    internal float[] tm = new float[] {1f, 0f, 0f, 1f};
    internal int renderingMode = 0;
    internal float width;
    internal float height;
    internal List<Int32> contents;
    internal List<Annotation> annots;
    internal List<Destination> destinations;
    internal float[] cropBox = null;
    internal float[] bleedBox = null;
    internal float[] trimBox = null;
    internal float[] artBox = null;
    internal List<StructElem> structures = new List<StructElem>();

    private float[] pen = {0f, 0f, 0f};
    private float[] brush = {0f, 0f, 0f};
    private float pen_width = -1.0f;
    private int line_cap_style = 0;
    private int line_join_style = 0;
    private String linePattern = "[] 0";
    private Font font;
    private List<State> savedStates = new List<State>();
    private int mcid = 0;


    /**
     *  Creates page object and add it to the PDF document.
     *
     *  Please note:
     *  <pre>
     *  The coordinate (0.0, 0.0) is the top left corner of the page.
     *  The size of the pages are represented in points.
     *  1 point is 1/72 inches.
     *  </pre>
     *
     *  @param pdf the pdf object.
     *  @param pageSize the page size of this page.
     */
    public Page(PDF pdf, float[] pageSize) : this(pdf, pageSize, true) {
    }


    /**
     *  Creates page object and add it to the PDF document.
     *
     *  Please note:
     *  <pre>
     *  The coordinate (0.0, 0.0) is the top left corner of the page.
     *  The size of the pages are represented in points.
     *  1 point is 1/72 inches.
     *  </pre>
     *
     *  @param pdf the pdf object.
     *  @param pageSize the page size of this page.
     *  @param addPageToPDF bool flag.
     */
    public Page(PDF pdf, float[] pageSize, bool addPageToPDF) {
        this.pdf = pdf;
        contents = new List<Int32>();
        annots = new List<Annotation>();
        destinations = new List<Destination>();
        width = pageSize[0];
        height = pageSize[1];
        buf = new MemoryStream(8192);

        if (addPageToPDF) {
            pdf.AddPage(this);
        }
    }


    public byte[] GetContent() {
        return buf.ToArray();
    }


    /**
     *  Adds destination to this page.
     *
     *  @param name The destination name.
     *  @param yPosition The vertical position of the destination on this page.
     */
    public void AddDestination(String name, float yPosition) {
        destinations.Add(new Destination(name, height - yPosition));
    }


    /**
     *  Returns the width of this page.
     *
     *  @return the width of the page.
     */
    public float GetWidth() {
        return width;
    }


    /**
     *  Returns the height of this page.
     *
     *  @return the height of the page.
     */
    public float GetHeight() {
        return height;
    }


    /**
     *  Draws a line on the page, using the current color, between the points (x1, y1) and (x2, y2).
     *
     *  @param x1 the first point's x coordinate.
     *  @param y1 the first point's y coordinate.
     *  @param x2 the second point's x coordinate.
     *  @param y2 the second point's y coordinate.
     */
    public void DrawLine(
            double x1,
            double y1,
            double x2,
            double y2) {
        MoveTo(x1, y1);
        LineTo(x2, y2);
        StrokePath();
    }


    /**
     *  Draws the text given by the specified string,
     *  using the specified Thai or Hebrew font and the current brush color.
     *  If the font is missing some glyphs - the fallback font is used.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font1 the Thai or Hebrew font.
     *  @param font2 the fallback font.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void DrawString(
            Font font1,
            Font font2,
            String str,
            float x,
            float y) {
        if (font2 == null) {
            DrawString(font1, str, x, y);
        }
        else {
            Font activeFont = font1;
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < str.Length; i++) {
                int ch = str[i];
                if ((font1.isCJK && ch >= 0x4E00 && ch <= 0x9FCC)
                        || (!font1.isCJK && font1.unicodeToGID[ch] != 0)) {
                    if (font1 != activeFont) {
                        String str2 = buf.ToString();
                        DrawString(activeFont, str2, x, y);
                        x += activeFont.StringWidth(str2);
                        buf.Length = 0;
                        activeFont = font1;
                    }
                }
                else {
                    if (font2 != activeFont) {
                        String str2 = buf.ToString();
                        DrawString(activeFont, str2, x, y);
                        x += activeFont.StringWidth(str2);
                        buf.Length = 0;
                        activeFont = font2;
                    }
                }
                buf.Append((char) ch);
            }
            DrawString(activeFont, buf.ToString(), x, y);
        }
    }


    /**
     *  Draws the text given by the specified string,
     *  using the specified font and the current brush color.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font the font to use.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void DrawString(
            Font font,
            String str,
            double x,
            double y) {
        DrawString(font, str, (float) x, (float) y);
    }


    /**
     *  Draws the text given by the specified string,
     *  using the specified font and the current brush color.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font the font to use.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void DrawString(
            Font font,
            String str,
            float x,
            float y) {

        if (str == null || str.Equals("")) {
            return;
        }

        // Append("q\n");
        Append("BT\n");

        if (font.fontID == null) {
            SetTextFont(font);
        }
        else {
            Append('/');
            Append(font.fontID);
            Append(' ');
            Append(font.size);
            Append(" Tf\n");
        }

        if (renderingMode != 0) {
            Append(renderingMode);
            Append(" Tr\n");
        }

        float skew = 0f;
        if (font.skew15 &&
                tm[0] == 1f &&
                tm[1] == 0f &&
                tm[2] == 0f &&
                tm[3] == 1f) {
            skew = 0.26f;
        }

        Append(tm[0]);
        Append(' ');
        Append(tm[1]);
        Append(' ');
        Append(tm[2] + skew);
        Append(' ');
        Append(tm[3]);
        Append(' ');
        Append(x);
        Append(' ');
        Append(height - y);
        // Append(" cm\n");
        Append(" Tm\n");

        Append("[ (");
        DrawString(font, str);
        Append(") ] TJ\n");
        Append("ET\n");

        // Append("Q\n");
    }


    private void DrawString(Font font, String str) {
        for (int i = 0; i < str.Length; i++) {
            int c1 = str[i];
            if (font.isComposite) {
                DrawTwoByteChar(c1, font);
            }
            else {
                DrawOneByteChar(c1, font, str, i);
            }
        }
    }


    private void DrawTwoByteChar(int c1, Font font) {
        if (c1 < font.firstChar || c1 > font.lastChar) {
            if (font.isCJK) {
                Append((byte) 0x0000);
                Append((byte) 0x0020);
            }
            else {
                Append((byte) font.unicodeToGID[0x0000]);
                Append((byte) font.unicodeToGID[0x0020]);
            }
        }
        else {
            byte hi;
            byte lo;
            if (font.isCJK) {
                hi = (byte) (c1 >> 8);
                lo = (byte) (c1);
            }
            else {
                int gid = font.unicodeToGID[c1];
                hi = (byte) (gid >> 8);
                lo = (byte) (gid);
            }
            if (hi == '(' || hi == ')' || hi == '\\') {
                Append((byte) '\\');
            }
            Append(hi);
    
            if (lo == '\r') {
                Append("\\015");
            }
            else {
                if (lo == '(' || lo == ')' || lo == '\\') {
                    Append((byte) '\\');
                }
                Append(lo);
            }
        }
    }


    public void DrawOneByteChar(int c1, Font font, String str, int i) {
        if (c1 < font.firstChar || c1 > font.lastChar) {
            c1 = font.MapUnicodeChar(c1);
        }
        if (c1 == '(' || c1 == ')' || c1 == '\\') {
            Append((byte) '\\');
        }
        Append((byte) c1);
    
        if (font.isStandard && font.kernPairs && i < (str.Length -1)) {
            c1 -= 32;
            int c2 = str[i + 1];
            if (c2 < font.firstChar || c2 > font.lastChar) {
                c2 = 32;
            }
            for (int j = 2; j < font.metrics[c1].Length; j += 2) {
                if (font.metrics[c1][j] == c2) {
                    Append(") ");
                    Append(-font.metrics[c1][j + 1]);
                    Append(" (");
                    break;
                }
            }
        }
    }


    /**
     * Sets the color for stroking operations.
     * The pen color is used when drawing lines and splines.
     *
     * @param r the red component is float value from 0.0 to 1.0.
     * @param g the green component is float value from 0.0 to 1.0.
     * @param b the blue component is float value from 0.0 to 1.0.
     */
    public void SetPenColor(
            double r, double g, double b) {
        SetPenColor((float) r, (float) g, (float) b);
    }


    /**
     * Sets the color for stroking operations.
     * The pen color is used when drawing lines and splines.
     *
     * @param r the red component is float value from 0.0f to 1.0f.
     * @param g the green component is float value from 0.0f to 1.0f.
     * @param b the blue component is float value from 0.0f to 1.0f.
     */
    public void SetPenColor(
            float r, float g, float b) {
        if (pen[0] != r ||
                pen[1] != g ||
                pen[2] != b) {
            SetColor(r, g, b);
            Append(" RG\n");
            pen[0] = r;
            pen[1] = g;
            pen[2] = b;
        }
    }


    /**
     * Sets the color for brush operations.
     * This is the color used when drawing regular text and filling shapes.
     *
     * @param r the red component is float value from 0.0 to 1.0.
     * @param g the green component is float value from 0.0 to 1.0.
     * @param b the blue component is float value from 0.0 to 1.0.
     */
    public void SetBrushColor(double r, double g, double b) {
        SetBrushColor((float) r, (float) g, (float) b);
    }


    /**
     * Sets the color for brush operations.
     * This is the color used when drawing regular text and filling shapes.
     *
     * @param r the red component is float value from 0.0f to 1.0f.
     * @param g the green component is float value from 0.0f to 1.0f.
     * @param b the blue component is float value from 0.0f to 1.0f.
     */
    public void SetBrushColor(
            float r, float g, float b) {
        if (brush[0] != r ||
                brush[1] != g ||
                brush[2] != b) {
            SetColor(r, g, b);
            Append(" rg\n");
            brush[0] = r;
            brush[1] = g;
            brush[2] = b;
        }
    }


    /**
     * Sets the color for brush operations.
     * 
     * @param color the color.
     * @throws IOException
     */
    public void SetBrushColor(float[] color) {
        SetBrushColor(color[0], color[1], color[2]);
    }


    /**
     * Returns the brush color.
     * 
     * @return the brush color.
     */
    public float[] GetBrushColor() {
        return brush;
    }


    private void SetColor(
            float r, float g, float b) {
        Append(r);
        Append(' ');
        Append(g);
        Append(' ');
        Append(b);
    }


    /**
     * Sets the pen color.
     * 
     * @param color the color. See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
     * @throws IOException
     */
    public void SetPenColor(int color) {
        float r = ((color >> 16) & 0xff)/255.0f;
        float g = ((color >>  8) & 0xff)/255.0f;
        float b = ((color)       & 0xff)/255.0f;
        SetPenColor(r, g, b);
    }


    /**
     * Sets the brush color.
     * 
     * @param color the color. See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
     * @throws IOException
     */
    public void SetBrushColor(int color) {
        float r = ((color >> 16) & 0xff)/255.0f;
        float g = ((color >>  8) & 0xff)/255.0f;
        float b = ((color)       & 0xff)/255.0f;
        SetBrushColor(r, g, b);
    }


    /**
     *  Sets the line width to the default.
     *  The default is the finest line width.
     */
    public void SetDefaultLineWidth() {
        if (pen_width != 0f) {
            pen_width = 0f;
            Append(pen_width);
            Append(" w\n");
        }
    }


    /**
     *  The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
     *  It is specified by a dash array and a dash phase.
     *  The elements of the dash array are positive numbers that specify the lengths of
     *  alternating dashes and gaps.
     *  The dash phase specifies the distance into the dash pattern at which to start the dash.
     *  The elements of both the dash array and the dash phase are expressed in user space units.
     *  <pre>
     *  Examples of line dash patterns:
     *
     *      "[Array] Phase"     Appearance          Description
     *      _______________     _________________   ____________________________________
     *
     *      "[] 0"              -----------------   Solid line
     *      "[3] 0"             ---   ---   ---     3 units on, 3 units off, ...
     *      "[2] 1"             -  --  --  --  --   1 on, 2 off, 2 on, 2 off, ...
     *      "[2 1] 0"           -- -- -- -- -- --   2 on, 1 off, 2 on, 1 off, ...
     *      "[3 5] 6"             ---     ---       2 off, 3 on, 5 off, 3 on, 5 off, ...
     *      "[2 3] 11"          -   --   --   --    1 on, 3 off, 2 on, 3 off, 2 on, ...
     *  </pre>
     *
     *  @param pattern the line dash pattern.
     */
    public void SetLinePattern(String pattern) {
        if (!pattern.Equals(linePattern)) {
            linePattern = pattern;
            Append(linePattern);
            Append(" d\n");
        }
    }


    /**
     *  Sets the default line dash pattern - solid line.
     */
    public void SetDefaultLinePattern() {
        Append("[] 0");
        Append(" d\n");
    }


    /**
     *  Sets the pen width that will be used to draw lines and splines on this page.
     *
     *  @param width the pen width.
     */
    public void SetPenWidth(double width) {
        SetPenWidth((float) width);
    }


    /**
     *  Sets the pen width that will be used to draw lines and splines on this page.
     *
     *  @param width the pen width.
     */
    public void SetPenWidth(float width) {
        if (pen_width != width) {
            pen_width = width;
            Append(pen_width);
            Append(" w\n");
        }
    }


    /**
     *  Sets the current line cap style.
     *
     *  @param style the cap style of the current line. Supported values: Cap.BUTT, Cap.ROUND and Cap.PROJECTING_SQUARE
     */
    public void SetLineCapStyle(int style) {
        if (line_cap_style != style) {
            line_cap_style = style;
            Append(line_cap_style);
            Append(" J\n");
        }
    }


    /**
     *  Sets the line join style.
     *
     *  @param style the line join style code. Supported values: Join.MITER, Join.ROUND and Join.BEVEL
     */
    public void SetLineJoinStyle(int style) {
        if (line_join_style != style) {
            line_join_style = style;
            Append(line_join_style);
            Append(" j\n");
        }
    }


    /**
     *  Moves the pen to the point with coordinates (x, y) on the page.
     *
     *  @param x the x coordinate of new pen position.
     *  @param y the y coordinate of new pen position.
     */
    public void MoveTo(double x, double y) {
        MoveTo((float) x, (float) y);
    }


    /**
     *  Moves the pen to the point with coordinates (x, y) on the page.
     *
     *  @param x the x coordinate of new pen position.
     *  @param y the y coordinate of new pen position.
     */
    public void MoveTo(float x, float y) {
        Append(x);
        Append(' ');
        Append(height - y);
        Append(" m\n");
    }


    /**
     *  Draws a line from the current pen position to the point with coordinates (x, y),
     *  using the current pen width and stroke color.
     */
    public void LineTo(double x, double y) {
        LineTo((float) x, (float) y);
    }


    /**
     *  Draws a line from the current pen position to the point with coordinates (x, y),
     *  using the current pen width and stroke color.
     */
    public void LineTo(float x, float y) {
        Append(x);
        Append(' ');
        Append(height - y);
        Append(" l\n");
    }


    /**
     *  Draws the path using the current pen color.
     */
    public void StrokePath() {
        Append("S\n");
    }


    /**
     *  Closes the path and draws it using the current pen color.
     */
    public void ClosePath() {
        Append("s\n");
    }


    /**
     *  Closes and fills the path with the current brush color.
     */
    public void FillPath() {
        Append("f\n");
    }


    /**
     *  Draws the outline of the specified rectangle on the page.
     *  The left and right edges of the rectangle are at x and x + w.
     *  The top and bottom edges are at y and y + h.
     *  The rectangle is drawn using the current pen color.
     *
     *  @param x the x coordinate of the rectangle to be drawn.
     *  @param y the y coordinate of the rectangle to be drawn.
     *  @param w the width of the rectangle to be drawn.
     *  @param h the height of the rectangle to be drawn.
     */
    public void DrawRect(double x, double y, double w, double h) {
        MoveTo(x, y);
        LineTo(x+w, y);
        LineTo(x+w, y+h);
        LineTo(x, y+h);
        ClosePath();
    }


    /**
     *  Fills the specified rectangle on the page.
     *  The left and right edges of the rectangle are at x and x + w.
     *  The top and bottom edges are at y and y + h.
     *  The rectangle is drawn using the current pen color.
     *
     *  @param x the x coordinate of the rectangle to be drawn.
     *  @param y the y coordinate of the rectangle to be drawn.
     *  @param w the width of the rectangle to be drawn.
     *  @param h the height of the rectangle to be drawn.
     */
    public void FillRect(double x, double y, double w, double h) {
        MoveTo(x, y);
        LineTo(x+w, y);
        LineTo(x+w, y+h);
        LineTo(x, y+h);
        FillPath();
    }


    /**
     *  Draws or fills the specified path using the current pen or brush.
     *
     *  @param path the path.
     *  @param operation specifies 'stroke' or 'fill' operation.
     */
    public void DrawPath(
            List<Point> path, char operation) {
        if (path.Count < 2) {
            throw new Exception(
                    "The Path object must contain at least 2 points");
        }
        Point point = path[0];
        MoveTo(point.x, point.y);
        bool curve = false;
        for (int i = 1; i < path.Count; i++) {
            point = path[i];
            if (point.isControlPoint) {
                curve = true;
                Append(point);
            }
            else {
                if (curve) {
                    curve = false;
                    Append(point);
                    Append("c\n");
                }
                else {
                    LineTo(point.x, point.y);
                }
            }
        }

        Append(operation);
        Append('\n');
    }


    /**
     * Strokes a bezier curve and draws it using the current pen.
     * @deprecated  As of v4.00 replaced by {@link #drawPath(List, char)}
     *
     * @param list the list of points that define the bezier curve.
     */
    public void DrawBezierCurve(List<Point> list) {
        DrawBezierCurve(list, Operation.STROKE);
    }
    

    /**
     * Draws a bezier curve and fills it using the current brush.
     * @deprecated  As of v4.00 replaced by {@link #drawPath(List, char)}
     *
     * @param list the list of points that define the bezier curve.
     * @param operation must be Operation.STROKE or Operation.FILL.
     */
    public void DrawBezierCurve(
            List<Point> list, char operation) {
        Point point = list[0];
        MoveTo(point.x, point.y);
        for (int i = 1; i < list.Count; i++) {
            point = list[i];
            Append(point);
            if (i % 3 == 0) {
                Append("c\n");
            }
        }

        Append(operation);
        Append('\n');
    }


    /**
     *  Draws a circle on the page.
     *
     *  The outline of the circle is drawn using the current pen color.
     *
     *  @param x the x coordinate of the center of the circle to be drawn.
     *  @param y the y coordinate of the center of the circle to be drawn.
     *  @param r the radius of the circle to be drawn.
     */
    public void DrawCircle(
            double x,
            double y,
            double r) {
        DrawEllipse((float) x, (float) y, (float) r, (float) r, Operation.STROKE);
    }


    /**
     *  Draws the specified circle on the page and fills it with the current brush color.
     *
     *  @param x the x coordinate of the center of the circle to be drawn.
     *  @param y the y coordinate of the center of the circle to be drawn.
     *  @param r the radius of the circle to be drawn.
     *  @param operation Operation.STROKE or Operation.FILL
     */
    public void DrawCircle(
            double x,
            double y,
            double r,
            char operation) {
        DrawEllipse((float) x, (float) y, (float) r, (float) r, operation);
    }


    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     *  @param operation must be: Operation.FILL
     */
    public void DrawEllipse(
            double x,
            double y,
            double r1,
            double r2) {
        DrawEllipse((float) x, (float) y, (float) r1, (float) r2, Operation.STROKE);
    }


    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     *  @param operation must be: Operation.FILL
     */
    public void DrawEllipse(
            float x,
            float y,
            float r1,
            float r2) {
        DrawEllipse((float) x, (float) y, (float) r1, (float) r2, Operation.STROKE);
    }


    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     */
    public void FillEllipse(
            double x,
            double y,
            double r1,
            double r2) {
        DrawEllipse((float) x, (float) y, (float) r1, (float) r2, Operation.FILL);
    }


    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     */
    public void FillEllipse(
            float x,
            float y,
            float r1,
            float r2) {
        DrawEllipse(x, y, r1, r2, Operation.FILL);
    }


    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     *  @param operation the operation.
     */
    private void DrawEllipse(
            float x,
            float y,
            float r1,
            float r2,
            char operation) {
        // The best 4-spline magic number
        float m4 = 0.551784f;

        // Starting point
        MoveTo(x, y - r2);

        AppendPointXY(x + m4*r1, y - r2);
        AppendPointXY(x + r1, y - m4*r2);
        AppendPointXY(x + r1, y);
        Append("c\n");

        AppendPointXY(x + r1, y + m4*r2);
        AppendPointXY(x + m4*r1, y + r2);
        AppendPointXY(x, y + r2);
        Append("c\n");

        AppendPointXY(x - m4*r1, y + r2);
        AppendPointXY(x - r1, y + m4*r2);
        AppendPointXY(x - r1, y);
        Append("c\n");

        AppendPointXY(x - r1, y - m4*r2);
        AppendPointXY(x - m4*r1, y - r2);
        AppendPointXY(x, y - r2);
        Append("c\n");

        Append(operation);
        Append('\n');
    }


    /**
     *  Draws a point on the page using the current pen color.
     *
     *  @param p the point.
     */
    public void DrawPoint(Point p) {
        if (p.shape != Point.INVISIBLE) {
            List<Point> list;
    
            if (p.shape == Point.CIRCLE) {
                if (p.fillShape) {
                    DrawCircle(p.x, p.y, p.r, 'f');
                }
                else {
                    DrawCircle(p.x, p.y, p.r, 'S');
                }
            }
            else if (p.shape == Point.DIAMOND) {
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y));
                list.Add(new Point(p.x, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.BOX) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.PLUS) {
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            }
            else if (p.shape == Point.UP_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.DOWN_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                list.Add(new Point(p.x, p.y + p.r));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.LEFT_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.RIGHT_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }
                else {
                    DrawPath(list, 's');
                }
            }
            else if (p.shape == Point.H_DASH) {
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
            }
            else if (p.shape == Point.V_DASH) {
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            }
            else if (p.shape == Point.X_MARK) {
                DrawLine(p.x - p.r, p.y - p.r, p.x + p.r, p.y + p.r);
                DrawLine(p.x - p.r, p.y + p.r, p.x + p.r, p.y - p.r);
            }
            else if (p.shape == Point.MULTIPLY) {
                DrawLine(p.x - p.r, p.y - p.r, p.x + p.r, p.y + p.r);
                DrawLine(p.x - p.r, p.y + p.r, p.x + p.r, p.y - p.r);
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            }
            else if (p.shape == Point.STAR) {
                float angle = (float) Math.PI / 10;
                float sin18 = (float) Math.Sin(angle);
                float cos18 = (float) Math.Cos(angle);
                float a = p.r * cos18;
                float b = p.r * sin18;
                float c = 2 * a * sin18;
                float d = 2 * a * cos18 - p.r;
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + c, p.y + d));
                list.Add(new Point(p.x - a, p.y - b));
                list.Add(new Point(p.x + a, p.y - b));
                list.Add(new Point(p.x - c, p.y + d));
                if (p.fillShape) {
                    DrawPath(list, 'f');
                }   
                else {
                    DrawPath(list, 's');
                }
            }
        }
    }


    /**
     *  Sets the text rendering mode.
     *
     *  @param mode the rendering mode.
     */
    public void SetTextRenderingMode(int mode) {
        if (mode >= 0 && mode <= 7) {
            this.renderingMode = mode;
        }
        else {
            throw new Exception("Invalid text rendering mode: " + mode);
        }
    }


    /**
     *  Sets the text direction.
     *
     *  @param degrees the angle.
     */
    public void SetTextDirection(int degrees) {
        if (degrees > 360) degrees %= 360;
        if (degrees == 0) {
            tm = new float[] { 1f,  0f,  0f,  1f};
        }
        else if (degrees == 90) {
            tm = new float[] { 0f,  1f, -1f,  0f};
        }
        else if (degrees == 180) {
            tm = new float[] {-1f,  0f,  0f, -1f};
        }
        else if (degrees == 270) {
            tm = new float[] { 0f, -1f,  1f,  0f};
        }
        else if (degrees == 360) {
            tm = new float[] { 1f,  0f,  0f,  1f};
        }
        else {
            float sinOfAngle = (float) Math.Sin(degrees * (Math.PI / 180));
            float cosOfAngle = (float) Math.Cos(degrees * (Math.PI / 180));
            tm = new float[] {cosOfAngle, sinOfAngle, -sinOfAngle, cosOfAngle};
        }
    }


    /**
     *  Draws a bezier curve starting from the current point.
     *  <strong>Please note:</strong> You must call the StrokePath, ClosePath or FillPath methods after the last BezierCurveTo call.
     *  <p><i>Author:</i> <strong>Pieter Libin</strong>, pieter@emweb.be</p>
     *
     *  @param p1 this first control point.
     *  @param p2 this second control point.
     *  @param p3 this end point.
     */
    public void BezierCurveTo(Point p1, Point p2, Point p3) {
    	Append(p1);
    	Append(p2);
    	Append(p3);
    	Append("c\n");
    }


    public void SetTextStart() {
        Append("BT\n");
    }


    /**
     *  Sets the text location.
     *
     *  @param x the x coordinate of new text location.
     *  @param y the y coordinate of new text location.
     */
    public void SetTextLocation(float x, float y) {
        Append(x);
        Append(' ');
        Append(height - y);
        Append(" Td\n");
    }


    public void SetTextBegin(float x, float y) {
        Append("BT\n");
        Append(x);
        Append(' ');
        Append(height - y);
        Append(" Td\n");
    }


    /**
     *  Sets the text leading.
     *
     *  @param leading the leading.
     */
    public void SetTextLeading(float leading) {
        Append(leading);
        Append(" TL\n");
    }


    public void SetCharSpacing(float spacing) {
        Append(spacing);
        Append(" Tc\n");
    }


    public void SetWordSpacing(float spacing) {
        Append(spacing);
        Append(" Tw\n");
    }


    public void SetTextScaling(float scaling) {
        Append(scaling);
        Append(" Tz\n");
    }


    public void SetTextRise(float rise) {
        Append(rise);
        Append(" Ts\n");
    }


    public void SetTextFont(Font font) {
        this.font = font;
        Append("/F");
        Append(font.objNumber);
        Append(' ');
        Append(font.size);
        Append(" Tf\n");
    }


    public void Println(String str) {
        Print(str);
        Println();
    }


    public void Print(String str) {
        Append('(');
        if (font != null && font.isComposite) {
            for (int i = 0; i < str.Length; i++) {
                int c1 = str[i];
                DrawTwoByteChar(c1, font);
            }
        }
        else {
            for (int i = 0; i < str.Length; i++) {
                int ch = str[i];
                if (ch == '(' || ch == ')' || ch == '\\') {
                    Append('\\');
                    Append((byte) ch);
                }
                else if (ch == '\t') {
                    Append(' ');
                    Append(' ');
                    Append(' ');
                    Append(' ');
                }
                else {
                    Append((byte) ch);
                }
            }
        }
        Append(") Tj\n");
    }


    public void Println() {
        Append("T*\n");
    }


    public void SetTextEnd() {
        Append("ET\n");
    }


    // Code provided by:
    // Dominique Andre Gunia <contact@dgunia.de>
    // <<
    public void DrawRectRoundCorners(
            float x, float y, float w, float h, float r1, float r2, char operation) {

        // The best 4-spline magic number
        float m4 = 0.551784f;

        List<Point> list = new List<Point>();

        // Starting point
        list.Add(new Point(x + w - r1, y));
        list.Add(new Point(x + w - r1 + m4*r1, y, Point.CONTROL_POINT));
        list.Add(new Point(x + w, y + r2 - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + w, y + r2));

        list.Add(new Point(x + w, y + h - r2));
        list.Add(new Point(x + w, y + h - r2 + m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + w - m4*r1, y + h, Point.CONTROL_POINT));
        list.Add(new Point(x + w - r1, y + h));

        list.Add(new Point(x + r1, y + h));
        list.Add(new Point(x + r1 - m4*r1, y + h, Point.CONTROL_POINT));
        list.Add(new Point(x, y + h - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x, y + h - r2));

        list.Add(new Point(x, y + r2));
        list.Add(new Point(x, y + r2 - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + m4*r1, y, Point.CONTROL_POINT));
        list.Add(new Point(x + r1, y));
        list.Add(new Point(x + w - r1, y));

        DrawPath(list, operation);
    }


    /**
     *  Clips the path.
     */
    public void ClipPath() {
        Append("W\n");
        Append("n\n");  // Close the path without painting it.
    }


    public void ClipRect(float x, float y, float w, float h) {
        MoveTo(x, y);
        LineTo(x + w, y);
        LineTo(x + w, y + h);
        LineTo(x, y + h);
        ClipPath();
    }


    public void Save() {
        Append("q\n");
        savedStates.Add(new State(
                pen, brush, pen_width, line_cap_style, line_join_style, linePattern));
    }


    public void Restore() {
        Append("Q\n");
        if (savedStates.Count > 0) {
            savedStates.RemoveAt(savedStates.Count - 1);
            State savedState = savedStates[savedStates.Count - 1];
            pen = savedState.GetPen();
            brush = savedState.GetBrush();
            pen_width = savedState.GetPenWidth();
            line_cap_style = savedState.GetLineCapStyle();
            line_join_style = savedState.GetLineJoinStyle();
            linePattern = savedState.GetLinePattern();
        }
    }
    // <<


    /**
     * Sets the page CropBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the CropBox.
     * @param upperLeftY the top left Y coordinate of the CropBox.
     * @param lowerRightX the bottom right X coordinate of the CropBox.
     * @param lowerRightY the bottom right Y coordinate of the CropBox.
     */
    public void SetCropBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.cropBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }


    /**
     * Sets the page BleedBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the BleedBox.
     * @param upperLeftY the top left Y coordinate of the BleedBox.
     * @param lowerRightX the bottom right X coordinate of the BleedBox.
     * @param lowerRightY the bottom right Y coordinate of the BleedBox.
     */
    public void SetBleedBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.bleedBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }


    /**
     * Sets the page TrimBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the TrimBox.
     * @param upperLeftY the top left Y coordinate of the TrimBox.
     * @param lowerRightX the bottom right X coordinate of the TrimBox.
     * @param lowerRightY the bottom right Y coordinate of the TrimBox.
     */
    public void SetTrimBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.trimBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }


    /**
     * Sets the page ArtBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the ArtBox.
     * @param upperLeftY the top left Y coordinate of the ArtBox.
     * @param lowerRightX the bottom right X coordinate of the ArtBox.
     * @param lowerRightY the bottom right Y coordinate of the ArtBox.
     */
    public void SetArtBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.artBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }


    private void AppendPointXY(float x, float y) {
        Append(x);
        Append(' ');
        Append(height - y);
        Append(' ');
    }


    private void Append(Point point) {
        Append(point.x);
        Append(' ');
        Append(height - point.y);
        Append(' ');
    }


    internal void Append(String str) {
        for (int i = 0; i < str.Length; i++) {
            buf.WriteByte((byte) str[i]);
        }
    }


    internal void Append(int num) {
        Append(num.ToString());
    }


    internal void Append(float val) {
        Append(val.ToString("0.###", PDF.culture_en_us));
    }


    internal void Append(char ch) {
        buf.WriteByte((byte) ch);
    }


    internal void Append(byte b) {
        buf.WriteByte(b);
    }


    /**
     *  Appends the specified array of bytes to the page.
     */
    public void Append(byte[] buffer) {
        buf.Write(buffer, 0, buffer.Length);
    }


    internal void DrawString(
            Font font,
            String str,
            float x,
            float y,
            Dictionary<String, Int32> colors) {
        SetTextBegin(x, y);
        SetTextFont(font);

        StringBuilder buf1 = new StringBuilder();
        StringBuilder buf2 = new StringBuilder();
        for (int i = 0; i < str.Length; i++) {
            char ch = str[i];
            if (Char.IsLetterOrDigit(ch)) {
                PrintBuffer(buf2, colors);
                buf1.Append(ch);
            }
            else {
                PrintBuffer(buf1, colors);
                buf2.Append(ch);
            }
        }
        PrintBuffer(buf1, colors);
        PrintBuffer(buf2, colors);

        SetTextEnd();
    }


    private void PrintBuffer(
            StringBuilder buf,
            Dictionary<String, Int32> colors) {
        String str = buf.ToString();
        if (str.Length > 0) {
            if (colors.ContainsKey(str)) {
                SetBrushColor(colors[str]);
            }
            else {
                SetBrushColor(Color.black);
            }
        }
        Print(str);
        buf.Length = 0;
    }


    internal void SetStructElementsPageObjNumber(int pageObjNumber) {
        foreach (StructElem element in structures) {
            element.pageObjNumber = pageObjNumber;
        }
    }


    public void AddBMC(
            String structure,
            String altDescription,
            String actualText) {
        AddBMC(structure, null, altDescription, actualText);
    }


    public void AddBMC(
            String structure,
            String language,
            String altDescription,
            String actualText) {
        if (pdf.compliance == Compliance.PDF_UA) {
            StructElem element = new StructElem();
            element.structure = structure;
            element.mcid = mcid;
            element.language = language;
            element.altDescription = altDescription;
            element.actualText = actualText;
            structures.Add(element);
    
            Append("/");
            Append(structure);
            Append(" <</MCID ");
            Append(mcid++);
            Append(">>\n");
            Append("BDC\n");
        }
    }
 

    public void AddEMC() {
        if (pdf.compliance == Compliance.PDF_UA) {
            Append("EMC\n");
        }
    }


    internal void AddAnnotation(Annotation annotation) {
        annots.Add(annotation);
        if (pdf.compliance == Compliance.PDF_UA) {
            StructElem element = new StructElem();
            element.structure = StructElem.LINK;
            element.language = annotation.language;
            element.altDescription = annotation.altDescription;
            element.actualText = annotation.actualText;
            element.annotation = annotation;
            structures.Add(element);
        }
    }

}   // End of Page.cs
}   // End of namespace PDFjet.NET
