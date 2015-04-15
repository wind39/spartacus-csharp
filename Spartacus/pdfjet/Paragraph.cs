/**
 *  Paragraph.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.
*/

using System;
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  Used to create paragraph objects.
 *  See the TextColumn class for more information.
 *
 */
public class Paragraph {

    internal List<TextLine> list = null;
    internal int alignment = Align.LEFT;


    /**
     *  Constructor for creating paragraph objects.
     *
     */
    public Paragraph() {
        list = new List<TextLine>();
    }


    /**
     *  Adds a text line to this paragraph.
     *
     *  @param text the text line to add to this paragraph.
     */
    public Paragraph Add(TextLine text) {
        list.Add(text);
        return this;
    }


    /**
     *  Removes the last text line added to this paragraph.
     *
     */
    public void removeLastTextLine() {
        if (list.Count >= 1) {
            list.RemoveAt(list.Count - 1);
        }
    }


    /**
     *  Sets the alignment of the text in this paragraph.
     *
     *  @param alignment the alignment code.
     *
     *  <pre>Supported values: Align.LEFT, Align.RIGHT, Align.CENTER and Align.JUSTIFY.</pre>
     */
    public void SetAlignment(int alignment) {
        this.alignment = alignment;
    }

}   // End of Paragraph.cs
}   // End of namespace PDFjet.NET
