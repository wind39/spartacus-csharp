// Most of the important code for this control was 
// taken from a control written by Nishant Sivakumar.
// http://www.codeproject.com/cs/combobox/DotNetMultiColumnComboBox.asp


using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Spartacus.ThirdParty
{
    public class MultiColumnComboBox : ComboBox
    {
        private bool     _AutoComplete;
        private bool     _AutoDropdown;
        private Color    _BackColorEven = Color.White;
        private Color    _BackColorOdd = Color.White;
        private string   _ColumnNameString = "";
        private int      _ColumnWidthDefault = 75;
        private string   _ColumnWidthString = "";
        private int      _LinkedColumnIndex;
        private TextBox  _LinkedTextBox;
        private int      _TotalWidth = 0;
        private int      _ValueMemberColumnIndex = 0;

        private Collection<string> _ColumnNames  = new Collection<string>();
        private Collection<int>    _ColumnWidths = new Collection<int>();

        public MultiColumnComboBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;

            // If all of your boxes will be RightToLeft, uncomment 
            // the following line to make RTL the default.
            //RightToLeft = RightToLeft.Yes;

            // Remove the Context Menu to disable pasting 
            ContextMenu = new ContextMenu();
        }

        public event System.EventHandler OpenSearchForm;

        public bool AutoComplete
        {
            get
            {
                return _AutoComplete;
            }
            set
            {
                _AutoComplete = value;
            }
        }

        public bool AutoDropdown
        {
            get
            {
                return _AutoDropdown;
            }
            set
            {
                _AutoDropdown = value;
            }
        }

        public Color BackColorEven
        {
            get
            {
                return _BackColorEven;
            }
            set
            {
                _BackColorEven = value;
            }
        }

        public Color BackColorOdd
        {
            get
            {
                return _BackColorOdd;
            }
            set
            {
                _BackColorOdd = value;
            }
        }

        public Collection<string> ColumnNameCollection
        {
            get
            {
                return _ColumnNames;
            }
        }

        public string ColumnNames
        {
            get
            {
                return _ColumnNameString;
            }

            set
            {
                // If the column string is blank, leave it blank.
                // The default width will be used for all columns.
                if (! Convert.ToBoolean(value.Trim().Length))
                {
                    _ColumnNameString = "";
                }
                else if (value != null)
                {
                    char[] delimiterChars = { ',', ';', ':' };
                    string[] columnNames = value.Split(delimiterChars);

                    if (!DesignMode)
                    {
                        _ColumnNames.Clear();
                    }

                    // After splitting the string into an array, iterate
                    // through the strings and check that they're all valid.
                    foreach (string s in columnNames)
                    {
                        // Does it have length?
                        if (Convert.ToBoolean(s.Trim().Length))
                        {
                            if (!DesignMode)
                            {
                                _ColumnNames.Add(s.Trim());
                            }
                        }
                        else // The value is blank
                        {
                            throw new NotSupportedException("Column names can not be blank.");
                        }
                    }
                    _ColumnNameString = value;
                }
            }
        }

        public Collection<int> ColumnWidthCollection
        {
            get
            {
                return _ColumnWidths;
            }
        }

        public int ColumnWidthDefault
        {
            get
            {
                return _ColumnWidthDefault;
            }
            set
            {
                _ColumnWidthDefault = value;
            }
        }

        public string ColumnWidths
        {
            get
            {
                return _ColumnWidthString;
            }

            set
            {
                // If the column string is blank, leave it blank.
                // The default width will be used for all columns.
                if (! Convert.ToBoolean(value.Trim().Length))
                {
                    _ColumnWidthString = "";
                }
                else if (value != null)
                {
                    char[] delimiterChars = { ',', ';', ':' };
                    string[] columnWidths = value.Split(delimiterChars);
                    string invalidValue = "";
                    int invalidIndex = -1;
                    int idx = 1;
                    int intValue;

                    // After splitting the string into an array, iterate
                    // through the strings and check that they're all integers
                    // or blanks
                    foreach (string s in columnWidths)
                    {
                        // If it has length, test if it's an integer
                        if (Convert.ToBoolean(s.Trim().Length))
                        {
                            // It's not an integer. Flag the offending value.
                            if (!int.TryParse(s, out intValue))
                            {
                                invalidIndex = idx;
                                invalidValue = s;
                            }
                            else // The value was okay. Increment the item index.
                            {
                                idx++;
                            }
                        }
                        else // The value is a space. Use the default width.
                        {
                            idx++;
                        }
                    }

                    // If an invalid value was found, raise an exception.
                    if (invalidIndex > -1)
                    {
                        string errMsg;

                        errMsg = "Invalid column width '" + invalidValue + "' located at column " + invalidIndex.ToString();
                        throw new ArgumentOutOfRangeException(errMsg);
                    }
                    else // The string is fine
                    {
                        _ColumnWidthString = value;

                        // Only set the values of the collections at runtime.
                        // Setting them at design time doesn't accomplish 
                        // anything and causes errors since the collections 
                        // don't exist at design time.
                        if (!DesignMode)
                        {
                            _ColumnWidths.Clear();
                            foreach (string s in columnWidths)
                            {
                                // Initialize a column width to an integer
                                if (Convert.ToBoolean(s.Trim().Length))
                                {
                                    _ColumnWidths.Add(Convert.ToInt32(s));
                                }
                                else // Initialize the column to the default
                                {
                                    _ColumnWidths.Add(_ColumnWidthDefault);
                                }
                            }

                            // If the column is bound to data, set the column widths
                            // for any columns that aren't explicitly set by the 
                            // string value entered by the programmer
                            if (DataManager != null)
                            {
                                InitializeColumns();
                            }
                        }
                    }
                }
            }
        }

        public new DrawMode DrawMode 
        { 
            get
            {
                return base.DrawMode;
            } 
            set
            {
                if (value != DrawMode.OwnerDrawVariable)
                {
                    throw new NotSupportedException("Needs to be DrawMode.OwnerDrawVariable");
                }
                base.DrawMode = value;
            }
        }

        public new ComboBoxStyle DropDownStyle
        { 
            get
            {
                return base.DropDownStyle;
            } 
            set
            {
                if (value != ComboBoxStyle.DropDown)
                {
                    throw new NotSupportedException("ComboBoxStyle.DropDown is the only supported style");
                }
                base.DropDownStyle = value;
            } 
        }

        public int LinkedColumnIndex
        {
            get 
            { 
                return _LinkedColumnIndex; 
            }
            set 
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("A column index can not be negative");
                }
                _LinkedColumnIndex = value; 
            }
        }

        public TextBox LinkedTextBox
        {
            get 
            { 
                return _LinkedTextBox; 
            }
            set 
            { 
                _LinkedTextBox = value;

                if (_LinkedTextBox != null)
                {
                    // Set any default properties of the Linked Textbox here
                    _LinkedTextBox.ReadOnly = true;
                    _LinkedTextBox.TabStop = false;
                }
            }
        }

        public int TotalWidth
        {
            get
            {
                return _TotalWidth;
            }
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            InitializeColumns();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (DesignMode)
                return;

            e.DrawBackground();

            Rectangle boundsRect = e.Bounds;
            int lastRight = 0;

            Color brushForeColor;
            if ((e.State & DrawItemState.Selected) == 0)
            {   
                // Item is not selected. Use BackColorOdd & BackColorEven
                Color backColor;
                backColor = Convert.ToBoolean(e.Index % 2) ? _BackColorOdd : _BackColorEven;
                using (SolidBrush brushBackColor = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brushBackColor, e.Bounds);
                }
                brushForeColor = Color.Black;
            }
            else
            {
                // Item is selected. Use ForeColor = White
                brushForeColor = Color.White;
            }

            using (Pen linePen = new Pen(SystemColors.GrayText))
            {
                using (SolidBrush brush = new SolidBrush(brushForeColor))
                {
                    if (! Convert.ToBoolean(_ColumnNames.Count))
                    {
                        e.Graphics.DrawString(Convert.ToString(Items[e.Index]), Font, brush, boundsRect);
                    }
                    else
                    {
                        // If the ComboBox is displaying a RightToLeft language, draw it this way.
                        if (RightToLeft.Equals(RightToLeft.Yes))
                        {
                            // Define a StringFormat object to make the string display RTL.
                            StringFormat rtl = new StringFormat();
                            rtl.Alignment = StringAlignment.Near;
                            rtl.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                            // Draw the strings in reverse order from high column index to zero column index.
                            for (int colIndex = _ColumnNames.Count - 1; colIndex >= 0; colIndex--)
                            {
                                if (Convert.ToBoolean(_ColumnWidths[colIndex]))
                                {
                                    string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], _ColumnNames[colIndex]));

                                    boundsRect.X = lastRight;
                                    boundsRect.Width = (int)_ColumnWidths[colIndex];
                                    lastRight = boundsRect.Right;

                                    // Draw the string with the RTL object.
                                    e.Graphics.DrawString(item, Font, brush, boundsRect, rtl);

                                    if (colIndex > 0)
                                    {
                                        e.Graphics.DrawLine(linePen, boundsRect.Right, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
                                    }
                                }
                            }
                        }
                        // If the ComboBox is displaying a LeftToRight language, draw it this way.
                        else
                        {
                            // Display the strings in ascending order from zero to the highest column.
                            for (int colIndex = 0; colIndex < _ColumnNames.Count; colIndex++)
                            {
                                if (Convert.ToBoolean(_ColumnWidths[colIndex]))
                                {
                                    string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], _ColumnNames[colIndex]));

                                    boundsRect.X = lastRight;
                                    boundsRect.Width = (int)_ColumnWidths[colIndex];
                                    lastRight = boundsRect.Right;
                                    e.Graphics.DrawString(item, Font, brush, boundsRect);

                                    if (colIndex < _ColumnNames.Count - 1)
                                    {
                                        e.Graphics.DrawLine(linePen, boundsRect.Right, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            e.DrawFocusRectangle();
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);

            if (_TotalWidth > 0)
            {
                if (Items.Count > MaxDropDownItems)
                {
                    // The vertical scrollbar is present. Add its width to the total.
                    // If you don't then RightToLeft languages will have a few characters obscured.
                    this.DropDownWidth = _TotalWidth + SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    this.DropDownWidth = _TotalWidth;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Use the Delete or Escape Key to blank out the ComboBox and
            // allow the user to type in a new value
            if ((e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Escape))
            {
                SelectedIndex = -1;
                Text = "";
                if (_LinkedTextBox != null)
                {
                    _LinkedTextBox.Text = "";
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                // Fire the OpenSearchForm Event
                if (OpenSearchForm != null)
                {
                    OpenSearchForm(this, System.EventArgs.Empty);
                }
            }
        }

        // Some of the code for OnKeyPress was derived from some VB.NET code  
        // posted by Laurent Muller as a suggested improvement for another control.
        // http://www.codeproject.com/vb/net/autocomplete_combobox.asp?df=100&forumid=3716&select=579095#xx579095xx
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            int idx = -1;
            string toFind;

            DroppedDown = _AutoDropdown;
            if (!Char.IsControl(e.KeyChar))
            {
                if (_AutoComplete)
                {
                    toFind = Text.Substring(0, SelectionStart) + e.KeyChar;
                    idx = FindStringExact(toFind);

                    if (idx == -1)
                    {
                        // An exact match for the whole string was not found
                        // Find a substring instead.
                        idx = FindString(toFind);
                    }
                    else
                    {
                        // An exact match was found. Close the dropdown.
                        DroppedDown = false;
                    }

                    if (idx != -1) // The substring was found.
                    {
                        SelectedIndex = idx;
                        SelectionStart = toFind.Length;
                        SelectionLength = Text.Length - SelectionStart;
                    }
                    else // The last keystroke did not create a valid substring.
                    {
                        // If the substring is not found, cancel the keypress
                        e.KeyChar = (char)0;
                    }
                }
                else // AutoComplete = false. Treat it like a DropDownList by finding the
                    // KeyChar that was struck starting from the current index
                {
                    idx = FindString(e.KeyChar.ToString(), SelectedIndex);

                    if (idx != -1)
                    {
                        SelectedIndex = idx;
                    }
                }
            }

            // Do no allow the user to backspace over characters. Treat it like
            // a left arrow instead. The user must not be allowed to change the 
            // value in the ComboBox. 
            if ((e.KeyChar == (char)(Keys.Back)) &&  // A Backspace Key is hit
                (_AutoComplete) &&                   // AutoComplete = true
                (Convert.ToBoolean(SelectionStart))) // And the SelectionStart is positive
            {
                // Find a substring that is one character less the the current selection.
                // This mimicks moving back one space with an arrow key. This substring should
                // always exist since we don't allow invalid selections to be typed. If you're
                // on the 3rd character of a valid code, then the first two characters have to 
                // be valid. Moving back to them and finding the 1st occurrence should never fail.
                toFind = Text.Substring(0, SelectionStart - 1);
                idx = FindString(toFind);

                if (idx != -1)
                {
                    SelectedIndex = idx;
                    SelectionStart = toFind.Length;
                    SelectionLength = Text.Length - SelectionStart;
                }
            }

            // e.Handled is always true. We handle every keystroke programatically.
            e.Handled = true;
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e); //Added after version 1.3 on 01/31/2008

            if (_LinkedTextBox != null)
            {
                if (_LinkedColumnIndex < _ColumnNames.Count)
                {
                    _LinkedTextBox.Text = Convert.ToString(FilterItemOnProperty(SelectedItem, _ColumnNames[_LinkedColumnIndex]));
                }
            }
        }

        protected override void OnValueMemberChanged(EventArgs e)
        {
            base.OnValueMemberChanged(e);

            InitializeValueMemberColumn();
        }

        private void InitializeColumns()
        {
            if (!Convert.ToBoolean(_ColumnNameString.Length))
            {
                PropertyDescriptorCollection propertyDescriptorCollection = DataManager.GetItemProperties();

                _TotalWidth = 0;
                _ColumnNames.Clear();

                for (int colIndex = 0; colIndex < propertyDescriptorCollection.Count; colIndex++)
                {
                    _ColumnNames.Add(propertyDescriptorCollection[colIndex].Name);

                    // If the index is greater than the collection of explicitly
                    // set column widths, set any additional columns to the default
                    if (colIndex >= _ColumnWidths.Count)
                    {
                        _ColumnWidths.Add(_ColumnWidthDefault);
                    }
                    _TotalWidth += _ColumnWidths[colIndex];
                }
            }
            else
            {
                _TotalWidth = 0;

                for (int colIndex = 0; colIndex < _ColumnNames.Count; colIndex++)
                {
                    // If the index is greater than the collection of explicitly
                    // set column widths, set any additional columns to the default
                    if (colIndex >= _ColumnWidths.Count)
                    {
                        _ColumnWidths.Add(_ColumnWidthDefault);
                    }
                    _TotalWidth += _ColumnWidths[colIndex];
                }

            }

            // Check to see if the programmer is trying to display a column
            // in the linked textbox that is greater than the columns in the 
            // ComboBox. I handle this error by resetting it to zero.
            if (_LinkedColumnIndex >= _ColumnNames.Count)
            {
                _LinkedColumnIndex = 0; // Or replace this with an OutOfBounds Exception
            }

            // Alteração William (27/08/2014)
            DropDownWidth = _TotalWidth;
            // Fim da Alteração
        }

        private void InitializeValueMemberColumn()
        {
            int colIndex = 0;
            foreach (String columnName in _ColumnNames)
            {
                if (String.Compare(columnName, ValueMember, true, CultureInfo.CurrentUICulture) == 0)
                {
                    _ValueMemberColumnIndex = colIndex;
                    break;
                }
                colIndex++;
            }
        }
    }
}
