/**
 *  TextColumn.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.
*/

using System;
using System.Text;
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  Used to create text column objects and draw them on a page.
 *
 *  Please see Example_10 and Example_29.
 */
public class TextColumn {

    internal int alignment = Align.LEFT;
    internal int rotate;

    private float x;    // This variable keeps it's original value after being initialized.
    private float y;    // This variable keeps it's original value after being initialized.
    private float w;
    private float h;

    private float x1;
    private float y1;
    private float line_height;

    private float space_between_lines = 1.0f;
    private float space_between_paragraphs = 2.0f;

    private List<Paragraph> paragraphs;

    private bool lineBetweenParagraphs = false;


    /**
     *  Create a text column object.
     *
     */
    public TextColumn() {
        this.paragraphs = new List<Paragraph>();
    }


    /**
     *  Create a text column object and set the rotation angle.
     *
     *  @param rotateByDegrees the specified rotation angle in degrees.
     */
    public TextColumn(int rotateByDegrees) {
        this.rotate = rotateByDegrees;
        if (rotate == 0 || rotate == 90 || rotate == 270) {
        }
        else {
            throw new Exception(
                    "Invalid rotation angle. Please use 0, 90 or 270 degrees.");
        }
        this.paragraphs = new List<Paragraph>();
    }


    /**
     *  Sets the lineBetweenParagraphs private variable value.
     *  If the value is set to true - an empty line will be inserted between the current and next paragraphs.
     *
     *  @param lineBetweenParagraphs the specified bool value.
     */
    public void SetLineBetweenParagraphs(bool lineBetweenParagraphs) {
        this.lineBetweenParagraphs = lineBetweenParagraphs;
    }


    public void SetSpaceBetweenLines(float space_between_lines) {
        this.space_between_lines = space_between_lines;
    }


    public void SetSpaceBetweenParagraphs(float space_between_paragraphs) {
        this.space_between_paragraphs = space_between_paragraphs;
    }


    /**
     *  Sets the position of this text column on the page.
     *
     *  @param x the x coordinate of the top left corner of this text column when drawn on the page.
     *  @param y the y coordinate of the top left corner of this text column when drawn on the page.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position of this text column on the page.
     *
     *  @param x the x coordinate of the top left corner of this text column when drawn on the page.
     *  @param y the y coordinate of the top left corner of this text column when drawn on the page.
     */
    public void SetPosition(float x, float y) {
        this.x = x;
        this.y = y;
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Sets the location of this text column on the page.
     *
     *  @param x the x coordinate of the top left corner.
     *  @param y the y coordinate of the top left corner.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Sets the size of this text column.
     *
     *  @param w the width of this text column.
     *  @param h the height of this text column.
     */
    public void SetSize(double w, double h) {
        SetSize((float) w, (float) h);
    }


    /**
     *  Sets the size of this text column.
     *
     *  @param w the width of this text column.
     *  @param h the height of this text column.
     */
    public void SetSize(float w, float h) {
        this.w = w;
        this.h = h;
    }


    /**
     *  Sets the desired width of this text column.
     *
     *  @param w the width of this text column.
     */
    public void SetWidth(float w) {
        this.w = w;
    }


    /**
     *  Sets the text alignment.
     *
     *  @param alignment the specified alignment code. Supported values: Align.LEFT, Align.RIGHT. Align.CENTER and Align.JUSTIFY
     */
    public void SetAlignment(int alignment) {
        this.alignment = alignment;
    }


    /**
     *  Sets the spacing between the lines in this text column.
     *
     *  @param spacing the specified spacing value.
     */
    public void SetLineSpacing(double spacing) {
        this.space_between_lines = (float) space_between_lines;
    }


    /**
     *  Sets the spacing between the lines in this text column.
     *
     *  @param spacing the specified spacing value.
     */
    public void SetLineSpacing(float spacing) {
        this.space_between_lines = spacing;
    }


    /**
     *  Adds a new paragraph to this text column.
     *
     *  @param paragraph the new paragraph object.
     */
    public void AddParagraph(Paragraph paragraph) {
        this.paragraphs.Add(paragraph);
    }


    /**
     *  Removes the last paragraph added to this text column.
     *
     */
    public void RemoveLastParagraph() {
        if (this.paragraphs.Count >= 1) {
            this.paragraphs.RemoveAt(this.paragraphs.Count - 1);
        }
    }


    /**
     *  Returns dimension object containing the width and height of this component.
     *  Please see Example_29.
     *
     *  @Return dimension object containing the width and height of this component.
     */
    public Dimension GetSize() {
        Point point = DrawOn(null, false);
        return new Dimension(this.w, point.y - this.y);
    }


    /**
     *  Draws this text column on the specified page.
     *
     *  @param page the page to draw this text column on.
     *  @return the point with x and y coordinates of the location where to draw the next component.
     */
    public Point DrawOn(Page page) {
        return DrawOn(page, true);
    }


    /**
     *  Draws this text column on the specified page if the 'draw' boolean value is 'true'.
     *
     *  @param page the page to draw this text column on.
     *  @param draw the boolean value that specified if the text column should actually be drawn on the page.
     *  @return the point with x and y coordinates of the location where to draw the next component.
     */
    public Point DrawOn(Page page, bool draw) {
        Point point = null;
        for (int i = 0; i < paragraphs.Count; i++) {
            Paragraph paragraph = paragraphs[i];
            this.alignment = paragraph.alignment;
            point = DrawParagraphOn(page, paragraph, draw);
        }
        // Restore the original location
        SetLocation(this.x, this.y);
        return point;
    }


    private Point DrawParagraphOn(
            Page page, Paragraph paragraph, bool draw) {

        List<TextLine> list = new List<TextLine>();
        float run_length = 0f;
        for (int i = 0; i < paragraph.list.Count; i++) {
            TextLine line = paragraph.list[i];
            if (i == 0) {
                line_height = line.font.body_height + space_between_lines;
                if (rotate == 0) {
                    y1 += line.font.ascent;
                }
                else if (rotate == 90) {
                    x1 += line.font.ascent;
                }
                else if (rotate == 270) {
                    x1 -= line.font.ascent;
                }
            }

            String[] tokens = line.str.Split(new Char[] {' ', '\r', '\n', '\t'});
            TextLine text = null;
            for (int j = 0; j < tokens.Length; j++) {
                String str = tokens[j];
                text = new TextLine(line.font, str);
                text.SetColor(line.GetColor());
                text.SetUnderline(line.GetUnderline());
                text.SetStrikeout(line.GetStrikeout());
                text.SetURIAction(line.GetURIAction());
                text.SetGoToAction(line.GetGoToAction());
                text.SetFallbackFont(line.GetFallbackFont());
                run_length += line.font.StringWidth(line.GetFallbackFont(), str);
                if (run_length >= w) {
                    DrawLineOfText(page, list, draw);
                    MoveToNextLine();
                    list.Clear();
                    list.Add(text);
                    run_length = line.font.StringWidth(line.GetFallbackFont(), str + " ");
                }
                else {
                    list.Add(text);
                    run_length += line.font.StringWidth(line.GetFallbackFont(), " ");
                }
            }
        }
        DrawNonJustifiedLine(page, list, draw);

        if (lineBetweenParagraphs) {
            return MoveToNextLine();
        }

        return MoveToNextParagraph(this.space_between_paragraphs);
    }


    private Point MoveToNextLine() {
        if (rotate == 0) {
            x1 = x;
            y1 += line_height;
        }
        else if (rotate == 90) {
            x1 += line_height;
            y1 = y;
        }
        else if (rotate == 270) {
            x1 -= line_height;
            y1 = y;
        }
        return new Point(x1, y1);
    }


    private Point MoveToNextParagraph(float space_between_paragraphs) {
        if (rotate == 0) {
            x1 = x;
            y1 += space_between_paragraphs;
        }
        else if (rotate == 90) {
            x1 += space_between_paragraphs;
            y1 = y;
        }
        else if (rotate == 270) {
            x1 -= space_between_paragraphs;
            y1 = y;
        }
        return new Point(x1, y1);
    }


    private void DrawLineOfText(
            Page page, List<TextLine> list, bool draw) {
        if (alignment == Align.JUSTIFY) {
            float sum_of_word_widths = 0f;
            for (int i = 0; i < list.Count; i++) {
                TextLine text = list[i];
                sum_of_word_widths += text.font.StringWidth(text.GetFallbackFont(), text.str);
            }
            float dx = (w - sum_of_word_widths) / (list.Count - 1);
            for (int i = 0; i < list.Count; i++) {
                TextLine text = list[i];
                text.SetPosition(x1, y1);

                if (text.GetGoToAction() != null) {
                    page.AddAnnotation(new Annotation(
                            null,                   // The URI
                            text.GetGoToAction(),   // The destination name
                            x,
                            page.height - (y - text.font.ascent),
                            x + text.font.StringWidth(text.GetFallbackFont(), text.GetText()),
                            page.height - (y - text.font.descent),
                            null,
                            null,
                            null));
                }

                if (rotate == 0) {
                    text.SetTextDirection(0);
                    text.DrawOn(page, draw);
                    x1 += text.font.StringWidth(text.GetFallbackFont(), text.str) + dx;
                }
                else if (rotate == 90) {
                    text.SetTextDirection(90);
                    text.DrawOn(page, draw);
                    y1 -= text.font.StringWidth(text.GetFallbackFont(), text.str) + dx;
                }
                else if (rotate == 270) {
                    text.SetTextDirection(270);
                    text.DrawOn(page, draw);
                    y1 += text.font.StringWidth(text.GetFallbackFont(), text.str) + dx;
                }
            }
        }
        else {
            DrawNonJustifiedLine(page, list, draw);
        }
    }


    private void DrawNonJustifiedLine(
            Page page, List<TextLine> list, bool draw) {
        float run_length = 0f;
        for (int i = 0; i < list.Count; i++) {
            TextLine text = list[i];
            if (i < (list.Count - 1)) {
                text.str += " ";
            }
            run_length += text.font.StringWidth(text.GetFallbackFont(), text.str);
        }

        if (alignment == Align.CENTER) {
            if (rotate == 0) {
                x1 = x + ((w - run_length) / 2);
            }
            else if (rotate == 90) {
                y1 = y - ((w - run_length) / 2);
            }
            else if (rotate == 270) {
                y1 = y + ((w - run_length) / 2);
            }
        }
        else if (alignment == Align.RIGHT) {
            if (rotate == 0) {
                x1 = x + (w - run_length);
            }
            else if (rotate == 90) {
                y1 = y - (w - run_length);
            }
            else if (rotate == 270) {
                y1 = y + (w - run_length);
            }
        }

        for (int i = 0; i < list.Count; i++) {
            TextLine text = list[i];
            text.SetPosition(x1, y1);

            if (text.GetGoToAction() != null) {
                page.AddAnnotation(new Annotation(
                        null,                   // The URI
                        text.GetGoToAction(),   // The destination name
                        x,
                        page.height - (y - text.font.ascent),
                        x + text.font.StringWidth(text.GetFallbackFont(), text.GetText()),
                        page.height - (y - text.font.descent),
                        null,
                        null,
                        null));
            }

            if (rotate == 0) {
                text.SetTextDirection(0);
                text.DrawOn(page, draw);
                x1 += text.font.StringWidth(text.GetFallbackFont(), text.str);
            }
            else if (rotate == 90) {
                text.SetTextDirection(90);
                text.DrawOn(page, draw);
                y1 -= text.font.StringWidth(text.GetFallbackFont(), text.str);
            }
            else if (rotate == 270) {
                text.SetTextDirection(270);
                text.DrawOn(page, draw);
                y1 += text.font.StringWidth(text.GetFallbackFont(), text.str);
            } 
        }
    }


    public void AddChineseParagraph(Font font, String chinese) {
        Paragraph p = null;
        StringBuilder buf = new StringBuilder();
        for (int i = 0; i < chinese.Length; i++) {
            char ch = chinese[i];
            if (font.StringWidth(buf.ToString() + ch) > w) {
                p = new Paragraph();
                p.Add(new TextLine(font, buf.ToString()));
                AddParagraph(p);
                buf.Length = 0;
            }
            buf.Append(ch);
        }
        p = new Paragraph();
        p.Add(new TextLine(font, buf.ToString()));
        AddParagraph(p);
    }

}   // End of TextColumn.cs
}   // End of namespace PDFjet.NET
