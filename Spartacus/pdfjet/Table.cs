/**
 *  Table.cs
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace PDFjet.NET {
/**
 *  Used to create table objects and draw them on a page.
 *
 *  Please see Example_08.
 */
public class Table {

    public static readonly int DATA_HAS_0_HEADER_ROWS = 0;
    public static readonly int DATA_HAS_1_HEADER_ROWS = 1;
    public static readonly int DATA_HAS_2_HEADER_ROWS = 2;
    public static readonly int DATA_HAS_3_HEADER_ROWS = 3;
    public static readonly int DATA_HAS_4_HEADER_ROWS = 4;
    public static readonly int DATA_HAS_5_HEADER_ROWS = 5;
    public static readonly int DATA_HAS_6_HEADER_ROWS = 6;
    public static readonly int DATA_HAS_7_HEADER_ROWS = 7;
    public static readonly int DATA_HAS_8_HEADER_ROWS = 8;
    public static readonly int DATA_HAS_9_HEADER_ROWS = 9;

    private int rendered = 0;
    private int numOfPages;

    private List<List<Cell>> tableData = null;
    private int numOfHeaderRows = 0;

    private float x1;
    private float y1;

    private float bottom_margin = 30f;


    /**
     *  Create a table object.
     *
     */
    public Table() {
        tableData = new List<List<Cell>>();
    }


    /**
     *  Sets the position (x, y) of the top left corner of this table on the page.
     *
     *  @param x the x coordinate of the top left point of the table.
     *  @param y the y coordinate of the top left point of the table.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position (x, y) of the top left corner of this table on the page.
     *
     *  @param x the x coordinate of the top left point of the table.
     *  @param y the y coordinate of the top left point of the table.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location (x, y) of the top left corner of this table on the page.
     *
     *  @param x the x coordinate of the top left point of the table.
     *  @param y the y coordinate of the top left point of the table.
     */
    public void SetLocation(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Sets the bottom margin for this table.
     *
     *  @param bottom_margin the margin.
     */
    public void SetBottomMargin(double bottom_margin) {
        this.bottom_margin = (float) bottom_margin;
    }


    /**
     *  Sets the bottom margin for this table.
     *
     *  @param bottom_margin the margin.
     */
    public void SetBottomMargin(float bottom_margin) {
        this.bottom_margin = bottom_margin;
    }


    /**
     *  Sets the table data.
     *
     *  @param tableData the table data.
     */
    public void SetData(
            List<List<Cell>> tableData) {
        this.tableData = tableData;
        this.numOfHeaderRows = 0;
        this.rendered = numOfHeaderRows;
    }


    /**
     *  Sets the table data and specifies the number of header rows in this data.
     *
     *  @param tableData the table data.
     *  @param numOfHeaderRows the number of header rows in this data.
     */
    public void SetData(
            List<List<Cell>> tableData, int numOfHeaderRows) {
        this.tableData = tableData;
        this.numOfHeaderRows = numOfHeaderRows;
        this.rendered = numOfHeaderRows;
    }


    /**
     *  Auto adjusts the widths of all columns so that they are just wide enough to hold the text without truncation.
     */
    public void AutoAdjustColumnWidths() {
        // Find the maximum text width for each column
        float[] max_col_widths = new float[tableData[0].Count];
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                Cell cell = row[j];
                if (cell.GetColSpan() == 1) {
                    float cellWidth = 0f;
                    if (cell.image != null) {
                        cellWidth = cell.image.GetWidth();
                    }
                    if (cell.text != null) {
                        if (cell.font.StringWidth(cell.text) > cellWidth) {
                            cellWidth = cell.font.StringWidth(cell.text);
                        }
                    }
                    cell.SetWidth(cellWidth + cell.left_padding + cell.right_padding);
                    if (max_col_widths[j] == 0f ||
                            cell.GetWidth() > max_col_widths[j]) {
                        max_col_widths[j] = cell.GetWidth();
                    }
                }
            }
        }

        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                Cell cell = row[j];
                cell.SetWidth(max_col_widths[j]);
            }
        }
    }


    /**
     *  Sets the alignment of the numbers to the right.
     */
    public void RightAlignNumbers() {
        for (int i = numOfHeaderRows; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                Cell cell = row[j];
                if (cell.text != null) {
                    String str = cell.text;
                    int len = str.Length;
                    bool isNumber = true;
                    int k = 0;
                    while (k < len) {
                        char ch = str[k++];
                        if (!Char.IsNumber(ch)
                                && ch != '('
                                && ch != ')'
                                && ch != '-'
                                && ch != '.'
                                && ch != ','
                                && ch != '\'') {
                            isNumber = false;
                        }
                    }
                    if (isNumber) {
                        cell.SetTextAlignment(Align.RIGHT);
                    }
                }
            }
        }
    }


    /**
     *  Removes the horizontal lines between the rows from index1 to index2.
     */
    public void RemoveLineBetweenRows(
            int index1, int index2) {
        for (int j = index1; j < index2; j++) {
            List<Cell> row = tableData[j];
            for (int i = 0; i < row.Count; i++) {
                Cell cell = row[i];
                cell.SetBorder(Border.BOTTOM, false);
            }
            row = tableData[j + 1];
            for (int i = 0; i < row.Count; i++) {
                Cell cell = row[i];
                cell.SetBorder(Border.TOP, false);
            }
        }
    }


    /**
     *  Sets the text alignment in the specified column.
     *
     *  @param index the index of the specified column.
     *  @param alignment the specified alignment. Supported values: Align.LEFT, Align.RIGHT, Align.CENTER and Align.JUSTIFY.
     */
    public void SetTextAlignInColumn(
            int index, int alignment) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            if (index < row.Count) {
                row[index].SetTextAlignment(alignment);
            }
        }
    }


    /**
     *  Sets the color of the text in the specified column.
     *
     *  @param index the index of the specified column.
     *  @param color the color specified as an integer.
     */
    public void SetTextColorInColumn(int index, int color) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            if (index < row.Count) {
                row[index].SetBrushColor(color);
            }
        }
    }


    /**
     *  Sets the font for the specified column.
     *
     *  @param index the column index.
     *  @param font the font.
     */
    public void SetFontInColumn(int index, Font font) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            if (index < row.Count) {
                row[index].font = font;
            }
        }
    }


    /**
     *  Sets the color of the text in the specified row.
     *
     *  @param index the index of the specified row.
     *  @param color the color specified as an integer.
     */
    public void SetTextColorInRow(int index, int color) {
        List<Cell> row = tableData[index];
        for (int i = 0; i < row.Count; i++) {
            row[i].SetBrushColor(color);
        }
    }


    /**
     *  Sets the font for the specified row.
     *
     *  @param index the row index.
     *  @param font the font.
     */
    public void SetFontInRow(int index, Font font) {
        List<Cell> row = tableData[index];
        for (int i = 0; i < row.Count; i++) {
            row[i].font = font;
        }
    }


    /**
     *  Sets the width of the column with the specified index.
     *
     *  @param index the index of specified column.
     *  @param width the specified width.
     */
    public void SetColumnWidth(
            int index, double width) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            if (index < row.Count) {
                row[index].SetWidth(width);
            }
        }
    }


    /**
     *  Returns the column width of the column at the specified index.
     *
     *  @param index the index of the column.
     *  @return the width of the column.
     */
    public float GetColumnWidth(int index) {
        return GetCellAtRowColumn(0, index).GetWidth(); 
    }


    /**
     *  Returns the cell at the specified row and column.
     *
     *  @param row the specified row.
     *  @param col the specified column.
     *
     *  @return the cell at the specified row and column.
     */
    public Cell GetCellAt(
            int row, int col) {
        if (row >= 0) {
            return tableData[row][col];
        }
        return tableData[tableData.Count + row][col];
    }


    /**
     *  Returns the cell at the specified row and column.
     *
     *  @param row the specified row.
     *  @param col the specified column.
     *
     *  @return the cell at the specified row and column.
     */
    public Cell GetCellAtRowColumn(int row, int col) {
        return GetCellAt(row, col);
    }


    /**
     *  Returns a list of cell for the specified row.
     *
     *  @param index the index of the specified row.
     *
     *  @return the list of cells.
     */
    public List<Cell> GetRow(int index) {
        return tableData[index];
    }


    public List<Cell> GetRowAtIndex(int index) {
        return GetRow(index);
    }


    /**
     *  Returns a list of cell for the specified column.
     *
     *  @param index the index of the specified column.
     *
     *  @return the list of cells.
     */
    public List<Cell> GetColumn(int index) {
        List<Cell> column = new List<Cell>();
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            if (index < row.Count) {
                column.Add(row[index]);
            }
        }
        return column;
    }


    /**
     *  Returns the total number of pages that are required to draw this table on.
     *
     *  @param page the type of pages we are drawing this table on.
     *
     *  @return the number of pages.
     */
    public int GetNumberOfPages(Page page) {
        numOfPages = 1;
        while (HasMoreData()) {
            DrawOn(page, false);
        }
        ResetRenderedPagesCount();
        return numOfPages;
    }


    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw if false - do not draw the table. Use to only find out where the table ends.
     *
     *  @return Point the point on the page where to draw the next component.
     */
    public Point DrawOn(Page page) {
        return DrawOn(page, true);
    }


    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw if false - do not draw the table. Use to only find out where the table ends.
     *  @param p_textlist Lista de listas de strings a serem renderizadas.
     *
     *  @return Point the point on the page where to draw the next component.
     */
    public Point ImprovedDrawOn(Page page, List<List<string>> p_textlist) {
        return ImprovedDrawOn(page, true, p_textlist);
    }


    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw if false - do not draw the table. Use to only find out where the table ends.
     *  @param p_file Arquivo de onde ler as strings a serem renderizadas.
     *
     *  @return Point the point on the page where to draw the next component.
     */
    public Point ImprovedDrawOn(Page page, System.IO.StreamReader p_file) {
        return ImprovedDrawOn(page, true, p_file);
    }
    
    
    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw
     *
     *  @return Point the point on the page where to draw the next component.
     */
    private Point DrawOn(Page page, bool draw) {
        return DrawTableRows(page, draw, DrawHeaderRows(page, draw));
    }

    
    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw
     *  @param p_textlist Lista de listas de strings a serem renderizadas.
     *
     *  @return Point the point on the page where to draw the next component.
     */
    private Point ImprovedDrawOn(Page page, bool draw, List<List<string>> p_textlist) {
        return ImprovedDrawTableRows(page, draw, DrawHeaderRows(page, draw), p_textlist);
    }
    

    /**
     *  Draws this table on the specified page.
     *
     *  @param page the page to draw this table on.
     *  @param draw
     *  @param p_file Arquivo de onde ler as strings a serem renderizadas.
     *
     *  @return Point the point on the page where to draw the next component.
     */
    private Point ImprovedDrawOn(Page page, bool draw, System.IO.StreamReader p_file) {
        return ImprovedDrawTableRows(page, draw, DrawHeaderRows(page, draw), p_file);
    }


    private float[] DrawHeaderRows(Page page, bool draw) {
        float x = x1;
        float y = y1;
        float cell_w = 0f;
        float cell_h = 0f;

        for (int i = 0; i < numOfHeaderRows; i++) {
            List<Cell> dataRow = tableData[i];
            cell_h = GetMaxCellHeight(dataRow);

            for (int j = 0; j < dataRow.Count; j++) {
                Cell cell = dataRow[j];
                float cellHeight = cell.GetHeight();
                if (cellHeight > cell_h) {
                    cell_h = cellHeight;
                }
                cell_w = cell.GetWidth();
                for (int k = 1; k < cell.GetColSpan(); k++) {
                    cell_w += dataRow[++j].GetWidth();
                }

                if (draw) {
                    page.SetBrushColor(cell.GetBrushColor());
                    cell.Paint(page, x, y, cell_w, cell_h);
                }

                x += cell_w;
            }
            x = x1;
            y += cell_h;
        }

        return new float[] {x, y, cell_w, cell_h};
    }


    private Point DrawTableRows(Page page, bool draw, float[] parameter) {
        float x = parameter[0];
        float y = parameter[1];
        float cell_w = parameter[2];
        float cell_h = parameter[3];

        for (int i = rendered; i < tableData.Count; i++) {
            List<Cell> dataRow = tableData[i];
            cell_h = GetMaxCellHeight(dataRow);

            for (int j = 0; j < dataRow.Count; j++) {
                Cell cell = dataRow[j];
                float cellHeight = cell.GetHeight();
                if (cellHeight > cell_h) {
                    cell_h = cellHeight;
                }
                cell_w = cell.GetWidth();
                for (int k = 1; k < cell.GetColSpan(); k++) {
                    cell_w += dataRow[++j].GetWidth();
                }

                if (draw) {
                    page.SetBrushColor(cell.GetBrushColor());
                    cell.Paint(page, x, y, cell_w, cell_h);
                }

                x += cell_w;
            }
            x = x1;
            y += cell_h;

            // Consider the height of the next row when checking if we must go to a new page
            if (i < (tableData.Count - 1)) {
                List<Cell> nextRow = tableData[i + 1];
                for (int j = 0; j < nextRow.Count; j++) {
                    Cell cell = nextRow[j];
                    float cellHeight = cell.GetHeight();
                    if (cellHeight > cell_h) {
                        cell_h = cellHeight;
                    }
                }
            }

            if ((y + cell_h) > (page.height - bottom_margin)) {
                if (i == tableData.Count - 1) {
                    rendered = -1;
                }
                else {
                    rendered = i + 1;
                    numOfPages++;
                }
                return new Point(x, y);
            }
        }
        rendered = -1;

        return new Point(x, y);
    }

    
    private Point ImprovedDrawTableRows(Page page, bool draw, float[] parameter, List<List<string>> p_textlist) {
        float x = parameter[0];
        float y = parameter[1];
        float cell_w = parameter[2];
        float cell_h = parameter[3];

        for (int i = rendered; i < tableData.Count; i++) {
            List<Cell> dataRow = tableData[i];
            cell_h = GetMaxCellHeight(dataRow);

            for (int j = 0; j < dataRow.Count; j++) {
                Cell cell = dataRow[j];
                float cellHeight = cell.GetHeight();
                if (cellHeight > cell_h) {
                    cell_h = cellHeight;
                }
                cell_w = cell.GetWidth();
                for (int k = 1; k < cell.GetColSpan(); k++) {
                    cell_w += dataRow[++j].GetWidth();
                }

                if (draw) {
                    page.SetBrushColor(cell.GetBrushColor());
                    cell.ImprovedPaint(page, x, y, cell_w, cell_h, p_textlist[i][j]);
                }

                x += cell_w;
            }
            x = x1;
            y += cell_h;

            // Consider the height of the next row when checking if we must go to a new page
            if (i < (tableData.Count - 1)) {
                List<Cell> nextRow = tableData[i + 1];
                for (int j = 0; j < nextRow.Count; j++) {
                    Cell cell = nextRow[j];
                    float cellHeight = cell.GetHeight();
                    if (cellHeight > cell_h) {
                        cell_h = cellHeight;
                    }
                }
            }

            if ((y + cell_h) > (page.height - bottom_margin)) {
                if (i == tableData.Count - 1) {
                    rendered = -1;
                }
                else {
                    rendered = i + 1;
                    numOfPages++;
                }
                return new Point(x, y);
            }
        }
        rendered = -1;

        return new Point(x, y);
    }
    

    private Point ImprovedDrawTableRows(Page page, bool draw, float[] parameter, System.IO.StreamReader p_file) {
        float x = parameter[0];
        float y = parameter[1];
        float cell_w = parameter[2];
        float cell_h = parameter[3];
        string[] v_line;

        for (int i = rendered; i < tableData.Count; i++) {
            List<Cell> dataRow = tableData[i];
            cell_h = GetMaxCellHeight(dataRow);
            
            v_line = p_file.ReadLine().Split(';');

            for (int j = 0; j < dataRow.Count; j++) {
                Cell cell = dataRow[j];
                float cellHeight = cell.GetHeight();
                if (cellHeight > cell_h) {
                    cell_h = cellHeight;
                }
                cell_w = cell.GetWidth();
                for (int k = 1; k < cell.GetColSpan(); k++) {
                    cell_w += dataRow[++j].GetWidth();
                }

                if (draw) {
                    page.SetBrushColor(cell.GetBrushColor());
                    cell.ImprovedPaint(page, x, y, cell_w, cell_h, v_line[j]);
                }

                x += cell_w;
            }
            x = x1;
            y += cell_h;

            // Consider the height of the next row when checking if we must go to a new page
            if (i < (tableData.Count - 1)) {
                List<Cell> nextRow = tableData[i + 1];
                for (int j = 0; j < nextRow.Count; j++) {
                    Cell cell = nextRow[j];
                    float cellHeight = cell.GetHeight();
                    if (cellHeight > cell_h) {
                        cell_h = cellHeight;
                    }
                }
            }

            if ((y + cell_h) > (page.height - bottom_margin)) {
                if (i == tableData.Count - 1) {
                    rendered = -1;
                }
                else {
                    rendered = i + 1;
                    numOfPages++;
                }
                return new Point(x, y);
            }
        }
        rendered = -1;

        return new Point(x, y);
    }


    private float GetMaxCellHeight(List<Cell> row) {
        float max_cell_height = 0f;
        for (int j = 0; j < row.Count; j++) {
            Cell cell = row[j];
            if (cell.GetHeight() > max_cell_height) {
                max_cell_height = cell.GetHeight();
            }
        }
        return max_cell_height;
    }


    /**
     *  Returns true if the table contains more data that needs to be drawn on a page.
     */
    public bool HasMoreData() {
        return rendered != -1;
    }


    /**
     *  Returns the width of this table when drawn on a page.
     *
     *  @return the width of this table.
     */
    public float GetWidth() {
        float table_width = 0f;
        List<Cell> row = tableData[0];
        for (int i = 0; i < row.Count; i++) {
            table_width += row[i].GetWidth();
        }
        return table_width;
    }


    /**
     *  Returns the number of data rows that have been rendered so far.
     *
     *  @return the number of data rows that have been rendered so far.
     */
    public int GetRowsRendered() {
        return rendered == -1 ? rendered : rendered - numOfHeaderRows;
    }


    /**
     *  Wraps around the text in all cells so it fits the column width.
     *  This method should be called after all calls to SetColumnWidth and AutoAdjustColumnWidths.
     *
     */
    public void WrapAroundCellText() {
        List<List<Cell>> tableData2 = new List<List<Cell>>();

        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            int maxNumVerCells = 1;
            for (int j = 0; j < row.Count; j++) {
                Cell cell = row[j];
                int colspan = cell.GetColSpan();
                for (int n = 1; n < colspan; n++) {
                    Cell next = row[j + n];
                    cell.SetWidth(cell.GetWidth() + next.GetWidth());
                    next.SetWidth(0f);
                }
                int numVerCells = cell.GetNumVerCells();
                if (numVerCells > maxNumVerCells) {
                    maxNumVerCells = numVerCells;
                }
            }

            for (int j = 0; j < maxNumVerCells; j++) {
                List<Cell> row2 = new List<Cell>();
                for (int k = 0; k < row.Count; k++) {
                    Cell cell = row[k];

                    Cell cell2 = new Cell(cell.GetFont(), "");
                    cell2.SetFallbackFont(cell.GetFallbackFont());
                    cell2.SetPoint(cell.GetPoint());
                    cell2.SetCompositeTextLine(cell.GetCompositeTextLine());
                    cell2.SetWidth(cell.GetWidth());
                    if (j == 0) {
                        cell2.SetTopPadding(cell.top_padding);
                    }
                    if (j == (maxNumVerCells - 1)) {
                        cell2.SetBottomPadding(cell.bottom_padding);
                    }
                    cell2.SetLeftPadding(cell.left_padding);
                    cell2.SetRightPadding(cell.right_padding);
                    cell2.SetLineWidth(cell.GetLineWidth());
                    cell2.SetBgColor(cell.GetBgColor());
                    cell2.SetPenColor(cell.GetPenColor());
                    cell2.SetBrushColor(cell.GetBrushColor());
                    cell2.SetProperties(cell.GetProperties());
                    cell2.SetImage(cell.GetImage());

                    if (j == 0) {
                        cell2.SetText(cell.GetText());
                        if (maxNumVerCells > 1) {
                            cell2.SetBorder(Border.BOTTOM, false);
                        }
                    }
                    else {
                        cell2.SetBorder(Border.TOP, false);
                        if (j < (maxNumVerCells - 1)) {
                            cell2.SetBorder(Border.BOTTOM, false);
                        }
                    }
                    row2.Add(cell2);
                }
                tableData2.Add(row2);
            }
        }

        for (int i = 0; i < tableData2.Count; i++) {
            List<Cell> row = tableData2[i];
            for (int j = 0; j < row.Count; j++) {
                Cell cell = row[j];
                if (cell.text != null) {
                    int n = 0;
                    String[] tokens = Regex.Split(cell.GetText(), @"\s+");
                    StringBuilder sb = new StringBuilder();
                    if (tokens.Length == 1) {
                        sb.Append(tokens[0]);
                    }
                    else {
                        for (int k = 0; k < tokens.Length; k++) {
                            String token = tokens[k];
                            if (cell.font.StringWidth(sb.ToString() + " " + token) >
                                    (cell.GetWidth() - (cell.left_padding + cell.right_padding))) {
                                tableData2[i + n][j].SetText(sb.ToString());
                                sb = new StringBuilder(token);
                                n++;
                            }
                            else {
                                if (k > 0) {
                                    sb.Append(" ");
                                }
                                sb.Append(token);
                            }
                        }
                    }
                    tableData2[i + n][j].SetText(sb.ToString());
                }
            }
        }

        tableData = tableData2;
    }


    /**
     *  Sets all table cells borders to <strong>false</strong>.
     *
     */
    public void SetNoCellBorders() {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                tableData[i][j].SetNoBorders();
            }
        }
    }


    /**
     *  Sets the color of the cell border lines.
     *
     *  @param color the color of the cell border lines.
     */
    public void SetCellBordersColor(int color) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                tableData[i][j].SetPenColor(color);
            }
        }
    }


    /**
     *  Sets the width of the cell border lines.
     *
     *  @param width the width of the cell border lines.
     */
    public void SetCellBordersWidth(float width) {
        for (int i = 0; i < tableData.Count; i++) {
            List<Cell> row = tableData[i];
            for (int j = 0; j < row.Count; j++) {
                tableData[i][j].SetLineWidth(width);
            }
        }
    }


    /**
     * Resets the rendered pages count.
     * Call this method if you have to draw this table more than one time.
     */
    public void ResetRenderedPagesCount() {
        this.rendered = numOfHeaderRows;
    }

}   // End of Table.cs
}   // End of namespace PDFjet.NET
