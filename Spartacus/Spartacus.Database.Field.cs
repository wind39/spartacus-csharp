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

namespace Spartacus.Database
{
	public class Field
	{
		public string v_name;
		public Spartacus.Database.Type v_type;

		public Field()
		{
			this.v_name = "";
			this.v_type = Spartacus.Database.Type.UNDEFINED;
		}

		public Field(string p_name, Spartacus.Database.Type p_type)
		{
			this.v_name = p_name;
			this.v_type = p_type;
		}

		public Field(string p_name, System.Type p_type)
		{
			this.v_name = p_name;

			if (p_type == typeof(System.Boolean))
				this.v_type = Spartacus.Database.Type.BOOLEAN;
			else if (p_type == typeof(System.Char))
				this.v_type = Spartacus.Database.Type.CHAR;
			else if (p_type == typeof(System.DateTime) || p_type == typeof(System.TimeSpan))
				this.v_type = Spartacus.Database.Type.DATETIME;
			else if (p_type == typeof(System.Decimal))
				this.v_type = Spartacus.Database.Type.DECIMAL;
			else if (p_type == typeof(System.Double))
				this.v_type = Spartacus.Database.Type.DOUBLE;
			else if (p_type == typeof(System.Single))
				this.v_type = Spartacus.Database.Type.FLOAT;
			else if (p_type == typeof(System.Int16) || p_type == typeof(System.UInt16))
				this.v_type = Spartacus.Database.Type.SMALLINTEGER;
			else if (p_type == typeof(System.Int32) || p_type == typeof(System.UInt32))
				this.v_type = Spartacus.Database.Type.INTEGER;
			else if (p_type == typeof(System.Int64) || p_type == typeof(System.UInt64))
				this.v_type = Spartacus.Database.Type.DOUBLE;
			else if (p_type == typeof(System.String) || p_type == typeof(System.Guid))
				this.v_type = Spartacus.Database.Type.STRING;
			else if (p_type == typeof(System.Byte) || p_type == typeof(System.SByte))
				this.v_type = Spartacus.Database.Type.BYTE;
			else
				this.v_type = Spartacus.Database.Type.UNDEFINED;
		}

		public Field(string p_name, System.Type p_type, bool p_generic)
		{
			this.v_name = p_name;

			if (p_generic)
			{
				if (p_type == typeof(System.Boolean))
					this.v_type = Spartacus.Database.Type.BOOLEAN;
				else if (p_type == typeof(System.Char))
					this.v_type = Spartacus.Database.Type.CHAR;
				else if (p_type == typeof(System.DateTime) || p_type == typeof(System.TimeSpan))
					this.v_type = Spartacus.Database.Type.DATETIME;
				else if (p_type == typeof(System.Decimal) ||
				         p_type == typeof(System.Double) ||
				         p_type == typeof(System.Single))
					this.v_type = Spartacus.Database.Type.REAL;
				else if (p_type == typeof(System.Int16) || p_type == typeof(System.UInt16) ||
				         p_type == typeof(System.Int32) || p_type == typeof(System.UInt32) ||
				         p_type == typeof(System.Int64) || p_type == typeof(System.UInt64))
					this.v_type = Spartacus.Database.Type.INTEGER;
				else if (p_type == typeof(System.String) || p_type == typeof(System.Guid))
					this.v_type = Spartacus.Database.Type.STRING;
				else if (p_type == typeof(System.Byte) || p_type == typeof(System.SByte))
					this.v_type = Spartacus.Database.Type.BYTE;
				else
					this.v_type = Spartacus.Database.Type.UNDEFINED;
			}
			else
			{
				if (p_type == typeof(System.Boolean))
					this.v_type = Spartacus.Database.Type.BOOLEAN;
				else if (p_type == typeof(System.Char))
					this.v_type = Spartacus.Database.Type.CHAR;
				else if (p_type == typeof(System.DateTime) || p_type == typeof(System.TimeSpan))
					this.v_type = Spartacus.Database.Type.DATE;
				else if (p_type == typeof(System.Decimal))
					this.v_type = Spartacus.Database.Type.DECIMAL;
				else if (p_type == typeof(System.Double))
					this.v_type = Spartacus.Database.Type.DOUBLE;
				else if (p_type == typeof(System.Single))
					this.v_type = Spartacus.Database.Type.FLOAT;
				else if (p_type == typeof(System.Int16) || p_type == typeof(System.UInt16))
					this.v_type = Spartacus.Database.Type.SMALLINTEGER;
				else if (p_type == typeof(System.Int32) || p_type == typeof(System.UInt32))
					this.v_type = Spartacus.Database.Type.INTEGER;
				else if (p_type == typeof(System.Int64) || p_type == typeof(System.UInt64))
					this.v_type = Spartacus.Database.Type.DOUBLE;
				else if (p_type == typeof(System.String) || p_type == typeof(System.Guid))
					this.v_type = Spartacus.Database.Type.STRING;
				else if (p_type == typeof(System.Byte) || p_type == typeof(System.SByte))
					this.v_type = Spartacus.Database.Type.BYTE;
				else
					this.v_type = Spartacus.Database.Type.UNDEFINED;
			}
		}
	}
}
