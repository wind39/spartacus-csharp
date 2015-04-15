/**
 *  Text.cs
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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


/**
 *  Please see Example_45
 */
namespace PDFjet.NET {
public class Text {

    private List<Paragraph> paragraphs;
    private Font font;
    private Font fallbackFont;
    private float x;
    private float y;
    private float w;
    private float x_text;
    private float y_text;
    private float leading;
    private float paragraphLeading;
    private List<float[]> beginParagraphPoints;
    private List<float[]> endParagraphPoints;
    private float spaceBetweenTextLines;


    public Text(List<Paragraph> paragraphs) {
        this.paragraphs = paragraphs;
        this.font = paragraphs[0].list[0].GetFont();
        this.fallbackFont = paragraphs[0].list[0].GetFallbackFont();
        this.leading = font.GetBodyHeight();
        this.paragraphLeading = 2*leading;
        this.beginParagraphPoints = new List<float[]>();
        this.endParagraphPoints = new List<float[]>();
        this.spaceBetweenTextLines = font.StringWidth(fallbackFont, " ");
    }


    public Text SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }


    public Text SetWidth(float w) {
        this.w = w;
        return this;
    }


    public Text SetLeading(float leading) {
        this.leading = leading;
        return this;
    }


    public Text SetParagraphLeading(float paragraphLeading) {
        this.paragraphLeading = paragraphLeading;
        return this;
    }


    public List<float[]> GetBeginParagraphPoints() {
        return this.beginParagraphPoints;
    }


    public List<float[]> GetEndParagraphPoints() {
        return this.endParagraphPoints;
    }


    public Text SetSpaceBetweenTextLines(float spaceBetweenTextLines) {
        this.spaceBetweenTextLines = spaceBetweenTextLines;
        return this;
    }


    public float[] DrawOn(Page page) {
        return DrawOn(page, true);
    }


    public float[] DrawOn(Page page, bool draw) {
        this.x_text = x;
        this.y_text = y + font.GetAscent();
        foreach (Paragraph paragraph in paragraphs) {
            int numberOfTextLines = paragraph.list.Count;
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < numberOfTextLines; i++) {
                TextLine textLine = paragraph.list[i];
                buf.Append(textLine.GetText());
            }
            for (int i = 0; i < numberOfTextLines; i++) {
                TextLine textLine = paragraph.list[i];
                if (i == 0) {
                    beginParagraphPoints.Add(new float[] { x_text, y_text });
                }
                textLine.SetAltDescription((i == 0) ? buf.ToString() : Single.space);
                textLine.SetActualText((i == 0) ? buf.ToString() : Single.space);
                float[] point = DrawTextLine(page, x_text, y_text, textLine, draw);
                if (i == (numberOfTextLines - 1)) {
                    endParagraphPoints.Add(new float[] { point[0], point[1] });
                }
                x_text = point[0] + spaceBetweenTextLines;
                y_text = point[1];
            }
            x_text = x;
            y_text += paragraphLeading;
        }
        return new float[] { x_text, y_text + font.GetDescent() };
    }


    public float[] DrawTextLine(
            Page page, float x_text, float y_text, TextLine textLine, bool draw) {

        Font font = textLine.GetFont();
        Font fallbackFont = textLine.GetFallbackFont();
        int color = textLine.GetColor();

        StringBuilder buf = new StringBuilder();
        String[] tokens = Regex.Split(textLine.GetText(), "\\s+");
        bool firstTextSegment = true;
        for (int i = 0; i < tokens.Length; i++) {
            String token = (i == 0) ? tokens[i] : (" " + tokens[i]);
            if (font.StringWidth(fallbackFont, token) < (this.w - (x_text - x))) {
                buf.Append(token);
                x_text += font.StringWidth(fallbackFont, token);
            }
            else {
                if (draw) {
                    new TextLine(font, buf.ToString())
                            .SetFallbackFont(textLine.GetFallbackFont())
                            .SetLocation(x_text - font.StringWidth(fallbackFont, buf.ToString()), y_text)
                            .SetColor(color)
                            .SetUnderline(textLine.GetUnderline())
                            .SetStrikeout(textLine.GetStrikeout())
                            .SetLanguage(textLine.GetLanguage())
                            .SetAltDescription(firstTextSegment ? textLine.GetAltDescription() : Single.space)
                            .SetActualText(firstTextSegment ? textLine.GetActualText() : Single.space)
                            .DrawOn(page);
                    firstTextSegment = false;
                }
                x_text = x + font.StringWidth(fallbackFont, tokens[i]);
                y_text += leading;
                buf.Length = 0;
                buf.Append(tokens[i]);
            }
        }
        if (draw) {
            new TextLine(font, buf.ToString())
                    .SetFallbackFont(textLine.GetFallbackFont())
                    .SetLocation(x_text - font.StringWidth(fallbackFont, buf.ToString()), y_text)
                    .SetColor(color)
                    .SetUnderline(textLine.GetUnderline())
                    .SetStrikeout(textLine.GetStrikeout())
                    .SetLanguage(textLine.GetLanguage())
                    .SetAltDescription(firstTextSegment ? textLine.GetAltDescription() : Single.space)
                    .SetActualText(firstTextSegment ? textLine.GetActualText() : Single.space)
                    .DrawOn(page);
            firstTextSegment = false;
        }

        return new float[] { x_text, y_text };
    }

}   // End of Text.cs
}   // End of namespace PDFjet.NET
