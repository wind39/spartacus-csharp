<!--
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

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
-->

<%@ Page Language="C#" %>
<!DOCTYPE html>
<html>
<head runat="server">
 <title>Download</title>
 <script runat="server">
    public void Page_Load()
    {
        System.IO.FileInfo v_file;

        v_file = new System.IO.FileInfo(this.Session["FILENAME"].ToString());

        this.Response.ContentType = "application/pdf";
        this.Response.AddHeader("content-disposition", "attachment; filename='report.pdf'");
        this.Response.AddHeader("content-length", v_file.Length.ToString());
        this.Response.TransmitFile("tmp/" + v_file.Name);
        this.Response.End();
    }
  </script>
</head>
<body>
 <form id="form1" runat="server">
 </form>
</body>
</html>
