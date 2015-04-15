/**
 *  CompositeTextLine.cs
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

/*
 *  This class was designed and implemented by Jon T. Swanson, Ph.D.
 *  
 *  Refactored and integrated into the project by Eugene Dragoev - 2 June 2012.
 */

using System;
using System.Text;
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  Used to create composite text line objects.
 *
 *
 */
public class CompositeTextLine : IDrawable {

    private const int X = 0;
    private const int Y = 1;

    private List<TextLine> textLines = new List<TextLine>();

    private float[] position = new float[2];
    private float[] current  = new float[2];

    // Subscript and Superscript size factors
    private float subscript_size_factor    = 0.583f;
    private float superscript_size_factor  = 0.583f;
    
    // Subscript and Superscript positions in relation to the base font
    private float superscript_position = 0.350f;
    private float subscript_position   = 0.141f;
    
    private float fontSize = 12f;
    

    public CompositeTextLine(float x, float y) {
        position[X] = x;
        position[Y] = y;
        current[X]  = x;
        current[Y]  = y;
    }
    

    /**
     *  Sets the font size.
     *
     *  @param fontSize the font size.
     */
    public void SetFontSize(float fontSize) {
        this.fontSize = fontSize;
    }


    /**
     *  Gets the font size.
     *
     *  @return fontSize the font size.
     */
    public float GetFontSize() {
        return fontSize;
    }


    /**
     *  Sets the superscript factor for this composite text line.
     *
     *  @param superscript the superscript size factor.
     */
    public void SetSuperscriptFactor(float superscript) {
        this.superscript_size_factor = superscript;
    }
    
    
    /**
     *  Gets the superscript factor for this text line.
     *
     *  @return superscript the superscript size factor.
     */
    public float GetSuperscriptFactor() {
        return superscript_size_factor;
    }


    /**
     *  Sets the subscript factor for this composite text line.
     *
     *  @param subscript the subscript size factor.
     */
    public void SetSubscriptFactor(float subscript) {
        this.subscript_size_factor = subscript;
    }


    /**
     *  Gets the subscript factor for this text line.
     *
     *  @return subscript the subscript size factor.
     */
    public float GetSubscriptFactor() {
        return subscript_size_factor;
    }


    /**
     *  Sets the superscript position for this composite text line.
     *
     *  @param superscript_position the superscript position.
     */
    public void SetSuperscriptPosition(float superscript_position) {
        this.superscript_position = superscript_position;
    }
    
    
    /**
     *  Gets the superscript position for this text line.
     *
     *  @return superscript_position the superscript position.
     */
    public float GetSuperscriptPosition() {
        return superscript_position;
    }


    /**
     *  Sets the subscript position for this composite text line.
     *
     *  @param subscript_position the subscript position.
     */
    public void SetSubscriptPosition(float subscript_position) {
        this.subscript_position = subscript_position;
    }


    /**
     *  Gets the subscript position for this text line.
     *
     *  @return subscript_position the subscript position.
     */
    public float GetSubscriptPosition() {
        return subscript_position;
    }

    
    /**
     *  Add a new text line.
     *
     *  Find the current font, current size and effects (normal, super or subscript)
     *  Set the position of the component to the starting stored as current position
     *  Set the size and offset based on effects
     *  Set the new current position
     *
     *  @param component the component.
     */
    public void AddComponent(TextLine component) {
        if (component.GetTextEffect() == Effect.SUPERSCRIPT) {
            component.GetFont().SetSize(fontSize * superscript_size_factor);
            component.SetPosition(
                    current[X],
                    current[Y] - fontSize * superscript_position);
        }
        else if (component.GetTextEffect() == Effect.SUBSCRIPT) {
            component.GetFont().SetSize(fontSize * subscript_size_factor);
            component.SetPosition(
                    current[X],
                    current[Y] + fontSize * subscript_position);
        }
        else {
            component.GetFont().SetSize(fontSize);
            component.SetPosition(current[X], current[Y]);
        }
        current[X] += component.GetWidth();
        textLines.Add(component);
    }


    /**
     *  Loop through all the text lines and reset their position based on
     *  the new position set here.
     *
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void SetPosition(double x, double y) {
        SetLocation((float) x, (float) y);
    }


    /**
     *  Loop through all the text lines and reset their position based on
     *  the new position set here.
     *
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Loop through all the text lines and reset their location based on
     *  the new location set here.
     *
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
    public void SetLocation(float x, float y) {
        position[X] = x;
        position[Y] = y;
        current[X]  = x;
        current[Y]  = y;

        if (textLines == null)
            return;
        int size = textLines.Count;
        if (size == 0)
            return;

        foreach (TextLine component in textLines) {
            if (component.GetTextEffect() == Effect.SUPERSCRIPT) {
                component.SetPosition(
                        current[X], 
                        current[Y] - fontSize * superscript_position);
            }
            else if (component.GetTextEffect() == Effect.SUBSCRIPT) {
                component.SetPosition(
                        current[X], 
                        current[Y] + fontSize * subscript_position);
            }
            else {
                component.SetPosition(current[X], current[Y]);
            }
            current[X] += component.GetWidth();
        }
    }


    /**
     *  Return the position of this composite text line.
     *
     *  @return the position of this composite text line.
     */
    public float[] GetPosition() {
        return position;
    }


    /**
     *  Return the nth entry in the TextLine array.
     *
     *  @param index the index of the nth element.
     *  @return the text line at the specified index.
     */
    public TextLine Get(int index) {
        if (textLines == null)
            return null;
        int size = textLines.Count;
        if (size == 0)
            return null;
        if (index < 0 || index > size - 1)
            return null;
        return textLines[index];
    }


    /**
     *  Returns the number of text lines.
     *
     *  @return the number of text lines.
     */
    public int Size() {
       return textLines.Count;
    }


    /**
     *  Returns the vertical coordinates of the top left and bottom right corners
     *  of the bounding box of this composite text line.
     *
     *  @return the an array containing the vertical coordinates.
     */
    public float[] GetMinMax() {
        float min = position[Y];
        float max = position[Y];
        float cur;

        foreach (TextLine component in textLines) {
            if (component.GetTextEffect() == Effect.SUPERSCRIPT) {
                cur = (position[Y] - component.GetFont().ascent) - fontSize * superscript_position;
                if (cur < min)
                    min = cur;
            }
            else if (component.GetTextEffect() == Effect.SUBSCRIPT) {
                cur = (position[Y] - component.GetFont().descent) + fontSize * subscript_position;
                if (cur > max)
                    max = cur;
            }
            else {
                cur = position[Y] - component.GetFont().ascent;
                if (cur < min)
                    min = cur;
                cur = position[Y] - component.GetFont().descent;
                if (cur > max)
                    max = cur;
            }
        }

        return new float[] {min, max};
    }


    /**
     *  Returns the height of this CompositeTextLine.
     *
     *  @return the height.
     */
    public float GetHeight() {
        float[] yy = GetMinMax();
        return yy[1] - yy[0];
    }


    /**
     *  Returns the width of this CompositeTextLine.
     *
     *  @return the width.
     */
    public float GetWidth() {
        return (current[X] - position[X]);
    }


    /**
     *  Draws this line on the specified page.
     *
     *  @param page the page to draw this line on.
     */
    public void DrawOn(Page page) {
        // Loop through all the text lines and draw them on the page
        foreach (TextLine textLine in textLines) {
            textLine.DrawOn(page);
        }
    }

}   // End of CompositeTextLine.cs
}   // End of namespace PDFjet.NET
