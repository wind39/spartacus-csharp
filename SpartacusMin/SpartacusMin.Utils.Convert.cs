/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace SpartacusMin.Utils
{
    public static class Convert
    {
        public static System.Collections.Generic.List<T> DataTableToList<T>(System.Data.DataTable p_table)
        {
            System.Collections.Generic.List<T> v_list;
            System.Type v_type;
            T v_obj;
            System.Reflection.PropertyInfo v_prop;

            v_list = new System.Collections.Generic.List<T>();
            v_type = typeof(T);

            foreach (System.Data.DataRow r in p_table.Rows)
            {
                v_obj = System.Activator.CreateInstance<T>();

                foreach (System.Data.DataColumn c in p_table.Columns)
                {
                    v_prop = v_type.GetProperty(c.ColumnName);

                    if (v_prop != null && c.ColumnName == v_prop.Name && r[c].ToString() != "")
                        v_prop.SetValue(v_obj, System.Convert.ChangeType(r[c], v_prop.PropertyType), null);
                }

                v_list.Add(v_obj);
            }

            return v_list;
        }
    }
}
