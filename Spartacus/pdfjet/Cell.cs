/**
 *  Cell.cs
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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace PDFjet.NET {
/**
 *  Used to create table cell objects.
 *  See the Table class for more information.
 *
 */
public class Cell {

    internal Font font;
    internal Font fallbackFont;
    internal String text;
    internal Image image;
    internal Point point;
    internal CompositeTextLine compositeTextLine;

    internal float width = 70f;
    internal float top_padding = 2f;
    internal float bottom_padding = 2f;
    internal float left_padding = 2f;
    internal float right_padding = 2f;
    internal float lineWidth = 0.2f;

    private int background = -1;
    private int pen = Color.black;
    private int brush = Color.black;

    // Cell properties
    // Colspan:
    // bits 0 to 15
    // Border:
    // bit 16 - top
    // bit 17 - bottom
    // bit 18 - left
    // bit 19 - right
    // Text Alignment:
    // bit 20
    // bit 21
    // Text Decoration:
    // bit 22 - underline
    // bit 23 - strikeout
    // Future use:
    // bits 24 to 31
    private int properties = 0x000F0001;
    private String uri;


    /**
     *  Creates a cell object and sets the font.
     *
     *  @param font the font.
     */
    public Cell(Font font) {
        this.font = font;
    }


    /**
     *  Creates a cell object and sets the font and the cell text.
     *
     *  @param font the font.
     *  @param text the text.
     */
    public Cell(Font font, String text) {
        this.font = font;
        this.text = text;
    }


    /**
     *  Creates a cell object and sets the font, fallback font and the cell text.
     *
     *  @param font the font.
     *  @param fallbackFont the fallback font.
     *  @param text the text.
     */
    public Cell(Font font, Font fallbackFont, String text) {
        this.font = font;
        this.fallbackFont = fallbackFont;
        this.text = text;
    }


    /**
     *  Sets the font for this cell.
     *
     *  @param font the font.
     */
    public void SetFont(Font font) {
        this.font = font;
    }


    /**
     *  Sets the fallback font for this cell.
     *
     *  @param fallbackFont the fallback font.
     */
    public void SetFallbackFont(Font fallbackFont) {
        this.fallbackFont = fallbackFont;
    }


    /**
     *  Returns the font used by this cell.
     *
     *  @return the font.
     */
    public Font GetFont() {
        return this.font;
    }


    /**
     *  Returns the fallback font used by this cell.
     *
     *  @return the fallback font.
     */
    public Font GetFallbackFont() {
        return this.fallbackFont;
    }


    /**
     *  Sets the cell text.
     *
     *  @param text the cell text.
     */
    public void SetText(String text) {
        this.text = text;
    }


    /**
     *  Returns the cell text.
     *
     *  @return the cell text.
     */
    public String GetText() {
        return this.text;
    }


    /**
     *  Sets the image inside this cell.
     *
     *  @param image the image.
     */
    public void SetImage(Image image) {
        this.image = image;
    }


    /**
     *  Returns the cell image.
     *
     *  @return the image.
     */
    public Image GetImage() {
        return this.image;
    }


    /**
     *  Sets the point inside this cell.
     *  See the Point class and Example_09 for more information.
     *
     *  @param point the point.
     */
    public void SetPoint(Point point) {
        this.point = point;
    }


    /**
     *  Returns the cell point.
     *
     *  @return the point.
     */
    public Point GetPoint() {
        return this.point;
    }


    public void SetCompositeTextLine(CompositeTextLine compositeTextLine) {
        this.compositeTextLine = compositeTextLine;
    }


    public CompositeTextLine GetCompositeTextLine() {
        return this.compositeTextLine;
    }


    /**
     *  Sets the width of this cell.
     *
     *  @param width the specified width.
     */
    public void SetWidth(double width) {
        this.width = (float) width;
    }


    /**
     *  Returns the cell width.
     *
     *  @return the cell width.
     */
    public float GetWidth() {
        return this.width;
    }


    /**
     *  Sets the top padding of this cell.
     *
     *  @param padding the top padding.
     */
    public void SetTopPadding(float padding) {
        this.top_padding = padding;
    }


    /**
     *  Sets the bottom padding of this cell.
     *
     *  @param padding the bottom padding.
     */
    public void SetBottomPadding(float padding) {
        this.bottom_padding = padding;
    }


    /**
     *  Sets the left padding of this cell.
     *
     *  @param padding the left padding.
     */
    public void SetLeftPadding(float padding) {
        this.left_padding = padding;
    }


    /**
     *  Sets the right padding of this cell.
     *
     *  @param padding the right padding.
     */
    public void SetRightPadding(float padding) {
        this.right_padding = padding;
    }


    /**
     *  Returns the cell height.
     *
     *  @return the cell height.
     */
    public float GetHeight() {
        if (image != null) {
            return image.GetHeight() + top_padding + bottom_padding;
        }
        return font.body_height + top_padding + bottom_padding;
    }


    public void SetLineWidth(float lineWidth) {
        this.lineWidth = lineWidth;
    }


    public float GetLineWidth() {
        return this.lineWidth;
    }


    /**
     *  Sets the background to the specified color.
     *
     *  @param color the color specified as an integer.
     */
    public void SetBgColor(int color) {
        this.background = color;
    }


    /**
     *  Returns the background color of this cell.
     *
     */
    public int GetBgColor() {
        return this.background;
    }


    /**
     *  Sets the pen color.
     *
     *  @param color the color specified as an integer.
     */
    public void SetPenColor(int color) {
        this.pen = color;
    }


    /**
     *  Returns the pen color.
     *
     */
    public int GetPenColor() {
        return pen;
    }


    /**
     *  Sets the brush color.
     *
     *  @param color the color specified as an integer.
     */
    public void SetBrushColor(int color) {
        this.brush = color;
    }


    /**
     *  Returns the brush color.
     *
     */
    public int GetBrushColor() {
        return brush;
    }


    /**
     *  Sets the pen and brush colors to the specified color.
     *
     *  @param color the color specified as an integer.
     */
    public void SetFgColor(int color) {
        this.pen = color;
        this.brush = color;
    }


    internal void SetProperties(int properties) {
        this.properties = properties;
    }


    internal int GetProperties() {
        return this.properties;
    }


    /**
     *  Sets the column span private variable.
     *
     *  @param colspan the specified column span value.
     */
    public void SetColSpan(int colspan) {
        this.properties &= 0x00FF0000;
        this.properties |= (colspan & 0x0000FFFF);
    }


    /**
     *  Returns the column span private variable value.
     *
     *  @return the column span value.
     */
    public int GetColSpan() {
        return (this.properties & 0x0000FFFF);
    }


    /**
     *  Sets the cell border object.
     *
     *  @param border the border object.
     */
    public void SetBorder(int border, bool visible) {
        if (visible) {
            this.properties |= border;
        }
        else {
            this.properties &= (~border & 0x00FFFFFF);
        }
    }


    /**
     *  Returns the cell border object.
     *
     *  @return the cell border object.
     */
    public bool GetBorder(int border) {
        return (this.properties & border) != 0;
    }


    /**
     *  Sets all border object parameters to false.
     *  This cell will have no borders when drawn on the page.
     */
    public void SetNoBorders() {
        this.properties &= 0x00F0FFFF;
    }


    /**
     *  Sets the cell text alignment.
     *
     *  @param alignment the alignment code.
     *  Supported values: Align.LEFT, Align.RIGHT and Align.CENTER.
     */
    public void SetTextAlignment(int alignment) {
        this.properties &= 0x00CFFFFF;
        this.properties |= (alignment & 0x00300000);
    }


    /**
     *  Returns the text alignment.
     *
     */
    public int GetTextAlignment() {
        return (this.properties & 0x00300000);
    }


    /**
     *  Sets the underline variable.
     *  If the value of the underline variable is 'true' - the text is underlined.
     *
     *  @param underline the underline flag.
     */
    public void SetUnderline(bool underline) {
        if (underline) {
            this.properties |= 0x00400000;
        }
        else {
            this.properties &= 0x00BFFFFF;
        }
    }


    public bool GetUnderline() {
        return (properties & 0x00400000) != 0;
    }


    public void SetStrikeout(bool strikeout) {
        if (strikeout) {
            this.properties |= 0x00800000;
        }
        else {
            this.properties &= 0x007FFFFF;
        }
    }


    public bool GetStrikeout() {
        return (properties & 0x00800000) != 0;
    }


    public void SetURIAction(String uri) {
        this.uri = uri;
    }


    /**
     *  Draws the point, text and borders of this cell.
     *
     */
    internal virtual void Paint(
            Page page,
            float x,
            float y,
            float w,
            float h) {
        if (background != -1) {
            DrawBackground(page, x, y, w, h);
        }
        if (image != null) {
            image.SetLocation(x + left_padding, y + top_padding);
            image.DrawOn(page);
        }
        DrawBorders(page, x, y, w, h);
        if (text != null) {
            DrawText(page, x, y, w);
        }
        if (point != null) {
            if (point.align == Align.LEFT) {
                point.x = x + 2*point.r;
            }
            else if (point.align == Align.RIGHT) {
                point.x = (x + w) - this.right_padding/2;
            }
            point.y = y + h/2;
            page.SetBrushColor(point.GetColor());
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
            page.DrawPoint(point);
        }
    }

    /**
     *  Draws the point, text and borders of this cell.
     *
     */
    internal virtual void ImprovedPaint(
            Page page,
            float x,
            float y,
            float w,
            float h,
            string p_text) {
        if (background != -1) {
            DrawBackground(page, x, y, w, h);
        }
        if (image != null) {
            image.SetLocation(x + left_padding, y + top_padding);
            image.DrawOn(page);
        }
        DrawBorders(page, x, y, w, h);
        if (p_text != null) {
            ImprovedDrawText(page, x, y, w, p_text);
        }
        if (point != null) {
            if (point.align == Align.LEFT) {
                point.x = x + 2*point.r;
            }
            else if (point.align == Align.RIGHT) {
                point.x = (x + w) - this.right_padding/2;
            }
            point.y = y + h/2;
            page.SetBrushColor(point.GetColor());
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
            page.DrawPoint(point);
        }
    }    

    private void DrawBackground(
            Page page,
            float x,
            float y,
            float cell_w,
            float cell_h) {
        page.SetBrushColor(background);
        page.FillRect(x, y, cell_w, cell_h);
    }


    private void DrawBorders(
            Page page,
            float x,
            float y,
            float cell_w,
            float cell_h) {

        page.SetPenColor(pen);
        page.SetPenWidth(lineWidth);

        if (GetBorder(Border.TOP) &&
                GetBorder(Border.BOTTOM) &&
                GetBorder(Border.LEFT) &&
                GetBorder(Border.RIGHT)) {
            page.AddBMC(StructElem.SPAN, Single.space, Single.space);
            page.DrawRect(x, y, cell_w, cell_h);
            page.AddEMC();
        }
        else {
            if (GetBorder(Border.TOP)) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.MoveTo(x, y);
                page.LineTo(x + cell_w, y);
                page.StrokePath();
                page.AddEMC();
            }
            if (GetBorder(Border.BOTTOM)) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.MoveTo(x, y + cell_h);
                page.LineTo(x + cell_w, y + cell_h);
                page.StrokePath();
                page.AddEMC();
            }
            if (GetBorder(Border.LEFT)) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.MoveTo(x, y);
                page.LineTo(x, y + cell_h);
                page.StrokePath();
                page.AddEMC();
            }
            if (GetBorder(Border.RIGHT)) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.MoveTo(x + cell_w, y);
                page.LineTo(x + cell_w, y + cell_h);
                page.StrokePath();
                page.AddEMC();
            }
        }

    }


    private void DrawText(
            Page page,
            float x,
            float y,
            float cell_w) {

        float x_text;
        float y_text = y + font.ascent + this.top_padding;

        page.SetPenColor(pen);
        page.SetBrushColor(brush);

        if (GetTextAlignment() == Align.RIGHT) {
            if (compositeTextLine == null) {
                x_text = (x + cell_w) - (font.StringWidth(text) + this.right_padding);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, text, x_text, y_text);
                }
            }
            else {
                x_text = (x + cell_w) - (compositeTextLine.GetWidth() + this.right_padding);
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else if (GetTextAlignment() == Align.CENTER) {
            if (compositeTextLine == null) {
                x_text = x + this.left_padding +
                        (((cell_w - (left_padding + right_padding)) - font.StringWidth(text)) / 2);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, text, x_text, y_text);
                }
            }
            else {
                x_text = x + this.left_padding +
                        (((cell_w - (left_padding + right_padding)) - compositeTextLine.GetWidth()) / 2);
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else if (GetTextAlignment() == Align.LEFT) {
            x_text = x + this.left_padding;
            if (compositeTextLine == null) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, text, x_text, y_text);
                }
            }
            else {
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else {
            throw new Exception("Invalid Text Alignment!");
        }

        if (uri != null) {
            float w = (compositeTextLine != null) ?
                    compositeTextLine.GetWidth() : font.StringWidth(text);
            // Please note: The font descent is a negative number.
            page.AddAnnotation(new Annotation(
                    uri,
                    null,
                    x_text,
                    (page.height - y_text) + font.descent,
                    x_text + w,
                    (page.height - y_text) + font.ascent,
                    null,
                    null,
                    null));
        }
    }

    private void ImprovedDrawText(
            Page page,
            float x,
            float y,
            float cell_w,
            string p_text) {

        float x_text;
        float y_text = y + font.ascent + this.top_padding;

        page.SetPenColor(pen);
        page.SetBrushColor(brush);

        if (GetTextAlignment() == Align.RIGHT) {
            if (compositeTextLine == null) {
                x_text = (x + cell_w) - (font.StringWidth(p_text) + this.right_padding);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, p_text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, p_text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, p_text, x_text, y_text);
                }
            }
            else {
                x_text = (x + cell_w) - (compositeTextLine.GetWidth() + this.right_padding);
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else if (GetTextAlignment() == Align.CENTER) {
            if (compositeTextLine == null) {
                x_text = x + this.left_padding +
                        (((cell_w - (left_padding + right_padding)) - font.StringWidth(p_text)) / 2);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, p_text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, p_text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, p_text, x_text, y_text);
                }
            }
            else {
                x_text = x + this.left_padding +
                        (((cell_w - (left_padding + right_padding)) - compositeTextLine.GetWidth()) / 2);
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else if (GetTextAlignment() == Align.LEFT) {
            x_text = x + this.left_padding;
            if (compositeTextLine == null) {
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                page.DrawString(font, fallbackFont, p_text, x_text, y_text);
                page.AddEMC();
                if (GetUnderline()) {
                    UnderlineText(page, font, p_text, x_text, y_text);
                }
                if (GetStrikeout()) {
                    StrikeoutText(page, font, p_text, x_text, y_text);
                }
            }
            else {
                compositeTextLine.SetPosition(x_text, y_text);
                page.AddBMC(StructElem.SPAN, Single.space, Single.space);
                compositeTextLine.DrawOn(page);
                page.AddEMC();
            }
        }
        else {
            throw new Exception("Invalid Text Alignment!");
        }

        if (uri != null) {
            float w = (compositeTextLine != null) ?
                    compositeTextLine.GetWidth() : font.StringWidth(p_text);
            // Please note: The font descent is a negative number.
            page.AddAnnotation(new Annotation(
                    uri,
                    null,
                    x_text,
                    (page.height - y_text) + font.descent,
                    x_text + w,
                    (page.height - y_text) + font.ascent,
                    null,
                    null,
                    null));
        }
    }
    

    private void UnderlineText(
            Page page, Font font, String text, float x, float y) {
        float descent = font.GetDescent();
        page.AddBMC(StructElem.SPAN, Single.space, Single.space);
        page.SetPenWidth(font.underlineThickness);
        page.MoveTo(x, y + descent);
        page.LineTo(x + font.StringWidth(text), y + descent);
        page.StrokePath();
        page.AddEMC();
    }


    private void StrikeoutText(
            Page page, Font font, String text, float x, float y) {
        page.AddBMC(StructElem.SPAN, Single.space, Single.space);
        page.SetPenWidth(font.underlineThickness);
        page.MoveTo(x, y - font.GetAscent()/3f);
        page.LineTo(x + font.StringWidth(text), y - font.GetAscent()/3f);
        page.StrokePath();
        page.AddEMC();
    }


    internal int GetNumVerCells() {
        int n = 1;
        String[] tokens = Regex.Split(text, @"\s+");
        if (tokens.Length == 1) {
            return n;
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < tokens.Length; i++) {
            String token = tokens[i];
            if (font.StringWidth(sb.ToString() + " " + token) >
                    (GetWidth() - (this.left_padding + this.right_padding))) {
                sb = new StringBuilder(token);
                n++;
            }
            else {
                if (i > 0) {
                    sb.Append(" ");
                }
                sb.Append(token);
            }
        }
        return n;
    }

}   // End of Cell.cs
}   // End of namespace PDFjet.NET
