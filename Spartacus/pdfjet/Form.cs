/**
 *  Form.cs
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
public class Form {

    private List<Field> fields;
    private float x;
    private float y;
    private Font f1;
    private float labelFontSize = 8f;
    private Font f2;
    private float valueFontSize = 10f;
    private int numberOfRows;
    private float rowLength = 500f;
    private float rowHeight = 12f;
    private int labelColor = Color.black;
    private int valueColor = Color.blue;
    private List<float[]> endOfLinePoints;


    public Form(List<Field> fields) {
        this.fields = fields;
        this.endOfLinePoints = new List<float[]>();
        foreach (Field field in fields) {
            if (field.x == 0f) {
                numberOfRows += field.values.Length;
            }
        }
    }


    public Form SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }


    public Form SetRowLength(float rowLength) {
        this.rowLength = rowLength;
        return this;
    }


    public Form SetRowHeight(float rowHeight) {
        this.rowHeight = rowHeight;
        return this;
    }


    public Form SetLabelFont(Font f1) {
        this.f1 = f1;
        return this;
    }


    public Form SetLabelFontSize(float labelFontSize) {
        this.labelFontSize = labelFontSize;
        return this;
    }


    public Form SetValueFont(Font f2) {
        this.f2 = f2;
        return this;
    }


    public Form SetValueFontSize(float valueFontSize) {
        this.valueFontSize = valueFontSize;
        return this;
    }


    public Form SetLabelColor(int labelColor) {
        this.labelColor = labelColor;
        return this;
    }


    public Form SetValueColor(int valueColor) {
        this.valueColor = valueColor;
        return this;
    }


    public float[] DrawOn(Page page) {
        if (numberOfRows == 0) {
            return new float[] { x, y };
        }

        float boxHeight = rowHeight*numberOfRows;
        Box box = new Box();
        box.SetLocation(x, y);
        box.SetSize(rowLength, boxHeight);
        box.DrawOn(page);

        float field_y = 0f;
        int row_span = 1;
        float row_y = 0;
        foreach (Field field in fields) {
            if (field.x == 0f) {
                row_y += row_span*rowHeight;
                row_span = field.values.Length;
            }
            field_y = row_y;
            for (int i = 0; i < field.values.Length; i++) {
                Font font = (i == 0) ? f1 : f2;
                float fontSize = (i == 0) ? labelFontSize : valueFontSize;
                int color = (i == 0) ? labelColor : valueColor;
                new TextLine(font, field.values[i])
                        .SetFontSize(fontSize)
                        .SetColor(color)
                        .PlaceIn(box, field.x + f1.GetDescent(), field_y - font.GetDescent())
                        .SetAltDescription((i == 0) ? field.altDescription[i] : (field.altDescription[i] + ","))
                        .SetActualText((i == 0) ? field.actualText[i] : (field.actualText[i] + ","))
                        .DrawOn(page);
                endOfLinePoints.Add(new float[] {
                        field.x + f1.GetDescent() + font.StringWidth(field.values[i]),
                        field_y - font.GetDescent(),
                });
                if (i == (field.values.Length - 1)) {
                    new Line(0f, 0f, rowLength, 0f)
                            .PlaceIn(box, 0f, field_y)
                            .DrawOn(page);
                    if (field.x != 0f) {
                        new Line(0f, -(field.values.Length-1)*rowHeight, 0f, 0f)
                                .PlaceIn(box, field.x, field_y)
                                .DrawOn(page);
                    }
                }
                field_y += rowHeight;
            }
        }

        return new float[] { x + rowLength, y + boxHeight };
    }

}   // End of Form.cs
}   // End of namespace PDFjet.NET
