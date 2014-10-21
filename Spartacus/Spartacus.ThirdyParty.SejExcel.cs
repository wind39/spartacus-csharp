// SejExcel, by Jose Segarra
// Website: http://www.codeproject.com/Tips/829389/Fast-Excel-import-and-export
// Github: https://github.com/jsegarra1971/SejExcelExport
// Version: 1.1 (July 16, 2014)
//


using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

namespace Spartacus.ThirdParty.SejExcel
{
    public class OoXmlStringsStream:MemoryStream
    {
        public Dictionary<string, int> dStrings;
        public int _BLANK;
        public int _ZERO;
        public bool ready;
        public OoXmlStringsStream(List<string> words)
            : base()
        {
            dStrings = new Dictionary<string, int>();
            for (int i = 0; i < words.Count; i++) AddString(words[i]);
            _BLANK = AddString("");
            _ZERO = AddString("0");
        }

        public int AddString(string s)
        {
            int index;
            if (!dStrings.TryGetValue(s, out index))
            {
                index = dStrings.Count;
                dStrings.Add(s, index);
            }
            return index;
        }

        public void Initialize()
        {
            StreamWriter sw = new StreamWriter(this, new UTF8Encoding());
            int nl=dStrings.Count;
            sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><sst xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" count=\""+nl+"\" uniqueCount=\""+nl+"\">\n");
            string[] v = new string[nl];
            foreach (KeyValuePair<string, int> p in dStrings) v[p.Value] = p.Key;           // Get the dictionary sorted (it should already be sorted, but just in case)
            for (int i = 0; i < v.Length; i++)
            {
                string s = "<si><t><![CDATA[" + v[i] + "]]></t></si>\n";
                sw.Write(s);
            }
            sw.Write("</sst>");
            sw.Flush();
            Position = 0;
            ready=true;
        }

        public override int Read(byte[] buffer, int start, int count)
        {
            if (!ready) Initialize();
            return base.Read(buffer, start, count);
        }
    }

    public class OoXml
    {
        internal ZipStorer zip = null;                                                                                          // The zip storer holding the main XLSX file stream
        internal List<gStream> streams = new List<gStream>();                                                                   // The streams that make the Excel template
        internal Dictionary<string, Stream> sources = new Dictionary<string, Stream>();                                         // Sources of data for new sheets, used when saving to inject data     

        public   Dictionary<string, gSheet> sheets = new Dictionary<string, gSheet>();                                          // The Sheets with data
        public   List<string> words = new List<string>();                                                                       // The XLSX dictionary
        public   OoXml(string template_filename)                                                                                // Constructor from a file name
        {
            zip = ZipStorer.Open(template_filename, FileAccess.Read);                                                           // Open the template
            foreach (ZipStorer.ZipFileEntry l in zip.ReadCentralDir()) streams.Add(new gStream(this, zip, l));                  // Get the streams that make up the template and add them
            SetStructure();                                                                                                     // Analyzes the Sheets structure
        }

        public void Close()                                                                                                     // Close the excel file
        {
            if (zip != null) { zip.Close(); zip = null; }
        }

        public void Save(string file_name)
        {
            ZipStorer result = ZipStorer.Create(file_name, "");                                                                 // Create the output filename
            Stream data;                                                                                                        // This will be out stream
            gStream sst = null;
            foreach (gStream g in streams)                                                                                      // Loop all the streams in the template    
            {
                string name = g.zfe.FilenameInZip;                                                                              // This is the stream name        
                if (name != "xl/sharedStrings.xml")                                                                             // If this is not the shared strings stream
                {
                    if (!sources.TryGetValue(name, out data)) data = g.ReadAsMemory();                                          // Get data stream either from memory or from Sources        
                    result.AddStream(ZipStorer.Compression.Deflate, name, data, DateTime.Now, "");                              // Add to our ZIP file
                }
                else sst = g;
            }
            if (!sources.TryGetValue("xl/sharedStrings.xml", out data)) data = sst.ReadAsMemory();                              // Get data stream either from memory or from Sources        
            result.AddStream(ZipStorer.Compression.Deflate, "xl/sharedStrings.xml", data, DateTime.Now, "");                    // Add to our ZIP file
            result.Close();                                                                                                     // Close the ZIP file
        }

        /*
    
        public bool SaveStream(string StreamName, string FileName)
        {
            gStream f = null;
            foreach (gStream s in streams) if (s.zfe.FilenameInZip == StreamName) f = s;
            if (f == null) return false;
            f.SaveToFile(FileName);
            return true;
        }
         * */

        private void SetStructure()                                                                                             // Initializes all the elements of a valid XLSX file
        {
            XmlDocument rd = null, sd = null, st = null;
            foreach (gStream s in streams)                                                                                      // Loop all the streams and get
            {   
                if (s.zfe.FilenameInZip == "xl/_rels/workbook.xml.rels") rd = s.ReadAsXml();                                    //      Stream for Relations
                if (s.zfe.FilenameInZip == "xl/workbook.xml") sd = s.ReadAsXml();                                               //      Stream for structure
                if (s.zfe.FilenameInZip == "xl/sharedStrings.xml") st = s.ReadAsXml();                                          //      Stream for strings 
            }
            if ((rd == null) || (sd == null) || (st == null)) throw new Exception("Bad WorkBook");                              // If ay of them could not be found, then raise an error
            XmlNode tn = st.FirstChild.NextSibling.FirstChild;                                                                  // This is the first node with strings
            while (tn != null) { words.Add(tn.FirstChild.InnerText); tn = tn.NextSibling; }                                     // Add all strings to dictionary        

            XmlNamespaceManager nsmgr0 = new System.Xml.XmlNamespaceManager(sd.NameTable);                                      // Add namespaces   
            nsmgr0.AddNamespace("main", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            nsmgr0.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            XmlNamespaceManager nsmgr1 = new System.Xml.XmlNamespaceManager(rd.NameTable);
            nsmgr1.AddNamespace("rels", "http://schemas.openxmlformats.org/package/2006/relationships");

            foreach (XmlNode n in sd.SelectNodes("main:workbook/main:sheets/main:sheet", nsmgr0))                               // For each sheet   
            {
                string rId = n.Attributes["r:id"].Value;                                                                        // Get its RelationID
                XmlNode r = rd.SelectSingleNode("rels:Relationships/rels:Relationship[@Id='" + rId + "']", nsmgr1);             // Use RelationID to find it in Relations 
                if (r != null)                                                                                                  // If found, 
                {
                    gStream sr = streams.Find(element => element.zfe.FilenameInZip== "xl/" + r.Attributes["Target"].Value);     // Use Target to find the stream implementing the relation   
                    if (sr == null) throw new Exception("Could not find [" + r.Attributes["Target"].Value + "] in streams");    // Raise and error if fail
                    sr.Sheet = new gSheet(this, Int32.Parse(n.Attributes["sheetId"].Value), n.Attributes["name"].Value, sr);    // Create the stream structure
                    sheets[sr.Sheet.Name] = sr.Sheet;                                                                           // Add it   
                }
            }
        }
    }

    // This is a stream inside a Excel ZIP
    public class gStream
    {
        internal ZipStorer.ZipFileEntry zfe;                                                                                    // This is the ZipFileEntry where the stream is stored
        public  OoXml   Document;                                                                                               // This is the document OoXMl document this stream belongs to
        public  gSheet Sheet = null;                                                                                            // This links to a data sheet if his stream implements a data sheet

        public gStream(OoXml doc, ZipStorer zip, ZipStorer.ZipFileEntry z)                                                      // This constructor is called when creating the stream from the source template
        {
            Document = doc;                                                                                                     // Save a reference to the document  
            zfe = z;                                                                                                            // Store the ZipFileEntry  
        }

        internal XmlDocument ReadAsXml()
        {
            Stream Content = new MemoryStream((int)zfe.FileSize);                                                               // Create a memory stream that will hold the data
            if (!Document.zip.ExtractFile(zfe, Content)) throw new Exception("Error reading template stream");                  // Extract the data to this memory stream
            Content.Position = 0;                                                                                               // Go to the beggining of the stream
            XmlDocument node = new XmlDocument();                                                                               // Create a XML document
            node.Load(Content);                                                                                                 // And parse the extracted data as XML
            return node;                                                                                                        // Return the node
        }

        /*
        internal void SaveToFile(string fname)
        {
            using (FileStream fs = new FileStream(fname, FileMode.Append, FileAccess.Write))
            {
                if (!Document.zip.ExtractFile(zfe, fs)) throw new Exception("Error extracting to file");                       // Fail if cannot unzip
            }
        }*/

        internal MemoryStream ReadAsMemory()
        {
            MemoryStream Content = new MemoryStream((int)zfe.FileSize);                                                         // Unzip the stream from the template
            if (!Document.zip.ExtractFile(zfe, Content)) throw new Exception("Error reading template stream");                  // Fail if cannot unzip
            Content.Position = 0;
            return Content;
        }
    }

    public class gSheet
    {
        public OoXml Document;
        public gStream Stream;
        public int Index;
        public string Name;
        private OoXmlStringsStream sst= null;
        private OoXmlDataStream sdata = null;

        internal gSheet(OoXml d, int i, string n, gStream s)
        {
            Document = d; Index = i;
            Name = n;
            Stream = s;
        }


        public Stream GetStream()
        {
            return Stream.Document.zip.GetStream(Stream.zfe);
        }
        // If a sheet is loaded in memory, then it is read and parsed, created as a whole XML document
        internal Dictionary<int, XmlNode> rows = null;

        public void LoadInMemory()
        {
            rows = new Dictionary<int, XmlNode>();
            XmlDocument doc = Stream.ReadAsXml();
            XmlNode n = doc.FirstChild.NextSibling.FirstChild;
            while (n != null && n.Name != "sheetData") n = n.NextSibling;
            if (n == null) return; else n = n.FirstChild;
            while (n != null)
            {
                if (n.Name == "row" && HasCellsWithData(n)) rows[Int32.Parse(n.Attributes["r"].Value)] = n;
                n = n.NextSibling;
            }
        }

        public void SetSource(OnNewRow ondata, int keep)
        {
            Stream data;
            if (!Document.sources.TryGetValue("xl/sharedStrings.xml", out data))
            {
                data=new OoXmlStringsStream(Document.words);
                Document.sources["xl/sharedStrings.xml"] = data;
            }
            sst = (OoXmlStringsStream)data;
            sdata = new OoXmlDataStream(this, keep, ondata);
            Document.sources[Stream.zfe.FilenameInZip] = sdata;
        }

        public void BeginRow(int i)                   { sdata.BeginRow(i);}
        public void EndRow()                          { sdata.EndRow();}
        public void WriteCell(int cell, int value)    { sdata.WriteCell(cell, value);}
        public void WriteCell(int cell, double value) { sdata.WriteCell(cell, value); }
        public void WriteCell(int cell, string value) 
        { 
            sdata.WriteSSTCell(cell, sst.AddString(value));
        }

        public void WriteCell(int cell, object value)
        {
            if (    value is float  || value is double   || value is decimal) { WriteCell(cell, (double)value); return; }
            if (    value is sbyte  || value is byte     || value is short ||
                value is ushort || value is int      || value is uint  || 
                value is long   || value is ulong) { WriteCell(cell, (int)value); return; }
            WriteCell(cell, (string)value);                                          
        }

        /*
        public void SaveToFile(string name)
        {
            Stream.SaveToFile(name);
        }
         * */

        // Get string from SST table
        string StrIndex(string k)
        {
            return Document.words[Int32.Parse(k)];
        }

        // COnvert a CELL Ref to ColumnIndex: ie:  A23 -> 1 ; A1 ->1 ; B4-> 2 ; Z201 -> 25 ; AA10 -> 26
        int CellRefToColIndex(string c)
        {
            int i = 0, sum = 0;
            while (c[i] >= 'A' && c[i] <= 'Z') { sum *= 26; sum += (c[i] - 'A' + 1); i++; }
            return sum;
        }

        // Get MAX column Index for this row
        int MaxColumnIndex(XmlNode cell)
        {
            int mc = 0;                                                                                                 // Set MC=0
            while (cell != null)                                                                                         // Loop all cells
            {
                int tc = CellRefToColIndex(cell.Attributes["r"].Value);                                                 // Get column index for this cell
                if (tc > mc) mc = tc;                                                                                   // If this cell index is greater than MC
                cell = cell.NextSibling;                                                                                // Move to next cell
            }
            return mc;                                                                                                  // Return MC
        }

        // Fills in a string[] with contents for row I
        public string[] Row(int rowindex)
        {
            XmlNode r;                                                                                                  // Get row from dictionary
            if (rows == null) LoadInMemory();                                                                           // If data has not been parsed, do it
            if (!rows.TryGetValue(rowindex + 1, out r)) return null;                                                    // If row is not in list of rows return null
            r = r.FirstChild;                                                                                           // This is the first cell
            string[] v = new string[MaxColumnIndex(r)];                                                                 // Create our output array
            for (int i = 0; i < v.Length; i++) v[i] = "";                                                               // Reset all values    
            while (r != null)                                                                                           // Loop all cells
            {
                XmlNode r1 = GetVNode(r);                                                                               // Get V node    
                if (r1 != null)                                                                                         // If there is a V node
                {
                    int t = CellRefToColIndex(r.Attributes["r"].Value) - 1;                                             // Get its column index            
                    if (r.Attributes["t"].Value == "s") v[t] = StrIndex(r1.InnerText); else v[t] = r1.InnerText;        // And set its value
                }
                r = r.NextSibling;                                                                                      // Move to next value
            }
            return v;                                                                                                   // Return the node
        }

        XmlNode GetVNode(XmlNode n)
        {
            XmlNode n1 = n.FirstChild;
            while (n1 != null) if (n1.Name == "v") return n1; else n1 = n1.NextSibling;
            return null;
        }

        bool HasCellsWithData(XmlNode n)
        {
            n = n.FirstChild;
            while (n != null) if (GetVNode(n) != null) return true; else n = n.NextSibling;
            return false;
        }
    }

    static class Extension
    {
        public static string Attribute(this XmlNode n, string name)
        {
            XmlAttribute b = n.Attributes[name];
            if (b == null) return ""; else return b.Value;
        }
    }


    public delegate void OnNewRow(gSheet sheet);

    // http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.cell(v=office.14).aspx
    class Column
    {
        internal byte[] part1, part2, part3;
        internal Column(XmlNode n)
        {
            string p="      <c";                                                                                                // This is the cell name    
            foreach(XmlAttribute a in n.Attributes) if (a.Name!="r" && a.Name!="t") p=p+" "+a.Name+"=\""+a.Value+"\"";          // Get all attributes except r and t
            string ci=new String(n.Attribute("r").Where(c => (c > '9')).ToArray());                                              // Get col Index as [r] without numeric chars
            part1=ASCIIEncoding.ASCII.GetBytes(p+" r=\""+ci);
            part2=ASCIIEncoding.ASCII.GetBytes("\"><v>");
            part3=ASCIIEncoding.ASCII.GetBytes("</v></c>\n");
        }
    }


    // This Stream implements Writing Rows of eBilling Data To Sheet Stream in Excel file.
    // On creation it creates Byte[] holding the common part of the received sheet
    class OoXmlDataStream : MemoryStream
    {
        List<Column> columns = new List<Column>();                                                                              // Format of columns to be replaces
        byte[]      header;                                                                                                     // Xml before data
        byte[]      footer;                                                                                                     // Xml after data
        byte[]      rowopen1 ,rowopen2,rowclose;                                                                                // Call back on writing row
        OnNewRow    onrow;
        gSheet      srsheet;
        bool        datadone = false;

        int         nbytes;
        byte[]      nbuffer;
        byte[]      thisrow;
        byte[]      sstpart;
        public OoXmlDataStream(gSheet sheet, int Keep,OnNewRow dr)
            : base()
        {
            string separator = Guid.NewGuid().ToString();
            sheet.LoadInMemory();                                                                                               // Make sure that the sheet is loaded in memory
            XmlDocument d = sheet.Stream.ReadAsXml();                                                                           // Load as XML
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(d.NameTable);                                                   // Set the name space    
            nsmgr.AddNamespace("aa", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");                              // To Open XML
            XmlNode ws = d.SelectSingleNode("//aa:worksheet/aa:sheetData", nsmgr);                                              // Locate data NODE
            if (ws == null) throw new Exception("Could not find sheet data");                                                   // Error if not
            XmlNode r = ws.FirstChild; while (Keep > 0 && r != null) { r = r.NextSibling; Keep--; }                             // Skip rows to keep
            if (r != null) InitializeColumns(r);                                                                                // if present use first row NOT to keep to initialize columns
            ws.AppendChild(d.CreateNode(XmlNodeType.Comment, "SEPARATOR", null)).InnerText = separator;                         // Add separator mark up
            InitializeHeaderAndFooter(d.OuterXml, separator);                                                                   // Get header and footer using separator
            rowopen1= ASCIIEncoding.ASCII.GetBytes("<row r=\"");                                                                // <row r="
            sstpart  = ASCIIEncoding.ASCII.GetBytes("\" t=\"s");                                                                // " t="s 
            rowopen2 = ASCIIEncoding.ASCII.GetBytes("\">\n");                                                                   // ">
            rowclose = ASCIIEncoding.ASCII.GetBytes("</row>\n");                                                                // </row>

            srsheet = sheet;                                                                                                    // Sheet this stream is linked to
            onrow = dr;                                                                                                         // On data callback
        }

        private void InitializeColumns(XmlNode r)
        {
            XmlNode c = r.FirstChild;                                                                                           // This is the first column
            while (c != null) { if (c.Name == "c") columns.Add(new Column(c)); c = c.NextSibling; }                             // Loop all columns an initialize their format
            while (r.NextSibling != null) r.ParentNode.RemoveChild(r.NextSibling);                                              // Remove all rows after r
            r.ParentNode.RemoveChild(r);                                                                                        // Remove r
        }

        private void InitializeHeaderAndFooter(string s, string r)                                                              // Initialize global header & footer
        {
            int i= s.IndexOf(r);                                                                                                // Use separator to split XML
            while (s[i] != '<') i--;header=ASCIIEncoding.ASCII.GetBytes(s.Substring(0, i));                                     // First part will be header
            while (s[i] != '>') i++;footer=ASCIIEncoding.ASCII.GetBytes(s.Substring(i+1));                                      // Second part will be footer            
        }

        public override int Read(byte[] buffer, int start, int count)                                                           // This one is called by the writer    
        {
            byte[] d;
            if (header != null) { d = header; header = null; return WriteBytes(d, buffer, 0); }                                 // If header has not been written. Write it and return
            if (!datadone)                                                                                                      // If all data has not been written    
            {
                nbytes = 0; nbuffer = buffer;                                                                                   // Reset the number of bytes written
                onrow(srsheet);                                                                                                 // Ask the host to write the data
                if (nbytes>0) return nbytes;                                                                                    // If host writted something then return it
            }
            datadone = true;                                                                                                    // If we ever get here, all data has already been written
            if (footer != null) { d = footer; footer = null; return WriteBytes(d, buffer, 0); }                                 // If footer has not been written. Write it and return
            return 0;
        }

        private int WriteBytes(byte[] w, byte[] buffer,int pos)
        {
            int j = w.Length;
            if (buffer.Length < j) throw new Exception("Partial buffer writing NOT implemented");
            System.Buffer.BlockCopy(w, 0, buffer, pos, j);
            return j;
        }

        private int WriteInt(int k, byte[] buffer,int pos)
        {
            byte[] w = ASCIIEncoding.ASCII.GetBytes(k.ToString());
            int j = w.Length;
            if (buffer.Length > j) { } else throw new Exception("Partial buffer writing NOT implemented");
            System.Buffer.BlockCopy(w, 0, buffer, pos, j);
            return j;
        }

        public void BeginRow(int RowNumber) 
        {
            nbytes = nbytes + WriteBytes(rowopen1, nbuffer, nbytes);
            thisrow= ASCIIEncoding.ASCII.GetBytes(RowNumber.ToString());
            nbytes = nbytes + WriteBytes(thisrow, nbuffer, nbytes); 
            nbytes = nbytes + WriteBytes(rowopen2, nbuffer, nbytes);                // Write row TAG

        }

        public void EndRow() 
        {
            nbytes = nbytes + WriteBytes(rowclose, nbuffer, nbytes);
        }

        public void WriteCell(int cell, int value)
        {
            if (cell >= columns.Count) return;
            Column c=columns[cell];
            nbytes = nbytes + WriteBytes(c.part1,nbuffer,nbytes);
            nbytes = nbytes + WriteBytes(thisrow, nbuffer, nbytes); 
            nbytes = nbytes + WriteBytes(c.part2,nbuffer,nbytes);
            nbytes = nbytes + WriteInt(value, nbuffer, nbytes); 
            nbytes = nbytes + WriteBytes(c.part3,nbuffer,nbytes);
        }

        public void WriteSSTCell(int cell, int value)
        {
            if (cell >= columns.Count) return;
            Column c = columns[cell];
            nbytes = nbytes + WriteBytes(c.part1, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(thisrow, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(sstpart, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(c.part2, nbuffer, nbytes);
            nbytes = nbytes + WriteInt(value, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(c.part3, nbuffer, nbytes);
        }

        public void WriteCell(int cell, double value) 
        {
            if (cell >= columns.Count) return;
            Column c = columns[cell];
            nbytes = nbytes + WriteBytes(c.part1, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(thisrow, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(c.part2, nbuffer, nbytes);
            byte[] w = ASCIIEncoding.ASCII.GetBytes(value.ToString());
            nbytes = nbytes + WriteBytes(w, nbuffer, nbytes);
            nbytes = nbytes + WriteBytes(c.part3, nbuffer, nbytes);
        }


    }
}

