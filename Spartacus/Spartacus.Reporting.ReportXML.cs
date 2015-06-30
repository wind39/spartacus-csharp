/*
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
*/

using System;
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Report.
    /// Representa um relatório em PDF.
    /// </summary>
    public partial class Report
    {
        /// <summary>
        /// Lê o arquivo XML que define o relatório.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        private void ReadXml(string p_filename)
        {
            System.Xml.XmlReader v_reader, v_item;
            System.Xml.XmlReaderSettings v_settings;

            v_settings = new System.Xml.XmlReaderSettings();
            v_settings.IgnoreComments = true;
            v_settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;

            try
            {
                v_reader = System.Xml.XmlReader.Create(p_filename, v_settings);

                while (v_reader.Read())
                {
                    if (v_reader.IsStartElement())
                    {
                        switch(v_reader.Name)
                        {
                            case "connection":
                                v_item = v_reader.ReadSubtree();
                                this.ReadConnection(v_item);
                                v_item.Close();
                                break;
                            case "settings":
                                v_item = v_reader.ReadSubtree();
                                this.ReadSettings(v_item);
                                v_item.Close();
                                break;
                            case "command":
                                v_item = v_reader.ReadSubtree();
                                this.ReadCommand(v_item);
                                v_item.Close();
                                break;
                            case "header":
                                v_item = v_reader.ReadSubtree();
                                this.ReadHeader(v_item);
                                v_item.Close();
                                break;
                            case "footer":
                                v_item = v_reader.ReadSubtree();
                                this.ReadFooter(v_item);
                                v_item.Close();
                                break;
                            case "fields":
                                v_item = v_reader.ReadSubtree();
                                this.ReadFields(v_item);
                                v_item.Close();
                                break;
                            case "groups":
                                v_item = v_reader.ReadSubtree();
                                this.ReadGroups(v_item);
                                v_item.Close();
                                break;
                            default:
                                break;
                        }
                    }
                }

                v_reader.Close();
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Lê informações sobre a conexão.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadConnection(System.Xml.XmlReader p_reader)
        {
            string v_type = null;
            string v_provider = null;
            string v_host = null;
            string v_port = null;
            string v_service = null;
            string v_user = null;
            string v_password = null;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            v_type = p_reader.ReadString();
                            break;
                        case "provider":
                            v_provider = p_reader.ReadString();
                            break;
                        case "host":
                            v_host = p_reader.ReadString();
                            break;
                        case "port":
                            v_port = p_reader.ReadString();
                            break;
                        case "service":
                            v_service = p_reader.ReadString();
                            break;
                        case "user":
                            v_user = p_reader.ReadString();
                            break;
                        case "password":
                            v_password = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            switch (v_type.ToLower())
            {
                case "firebird":
                    this.v_database = new Spartacus.Database.Firebird(v_host, v_port, v_service, v_user, v_password);
                    break;
                case "firebirdembed":
                    this.v_database = new Spartacus.Database.FirebirdEmbed(v_service, v_user, v_password);
                    break;
                case "mysql":
                    this.v_database = new Spartacus.Database.Mysql(v_host, v_port, v_service, v_user, v_password);
                    break;
                case "odbc":
                    this.v_database = new Spartacus.Database.Odbc(v_service, v_user, v_password);
                    break;
                case "oledb":
                    this.v_database = new Spartacus.Database.Oledb(v_provider, v_host, v_port, v_service, v_user, v_password);
                    break;
                case "postgresql":
                    this.v_database = new Spartacus.Database.Postgresql(v_host, v_port, v_service, v_user, v_password);
                    break;
                case "sqlite":
                    this.v_database = new Spartacus.Database.Sqlite(v_service);
                    break;
                case "oracle":
                    this.v_database = new Spartacus.Database.Oracle(v_host, v_port, v_service, v_user, v_password);
                    break;
                case "memory":
                    this.v_database = new Spartacus.Database.Memory();
                    break;
                default:
                    this.v_database = null;
                    break;
            }
        }

        /// <summary>
        /// Lê informações sobre as configurações do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadSettings(System.Xml.XmlReader p_reader)
        {
            this.v_settings = new Spartacus.Reporting.Settings();
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "layout":
                            if (p_reader.ReadString() == "PORTRAIT")
                                this.v_settings.v_layout = Spartacus.Reporting.PageLayout.PORTRAIT;
                            else
                                this.v_settings.v_layout = Spartacus.Reporting.PageLayout.LANDSCAPE;
                            break;
                        case "topmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.TOP, p_reader.ReadString());
                            break;
                        case "bottommargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.BOTTOM, p_reader.ReadString());
                            break;
                        case "leftmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.LEFT, p_reader.ReadString());
                            break;
                        case "rightmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.RIGHT, p_reader.ReadString());
                            break;
                        case "dataheaderborder":
                            this.v_settings.v_dataheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "datafieldborder":
                            this.v_settings.v_datafieldborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "groupheaderborder":
                            this.v_settings.v_groupheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "groupfooterborder":
                            this.v_settings.v_groupfooterborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "reportheaderborder":
                            this.v_settings.v_reportheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "reportfooterborder":
                            this.v_settings.v_reportfooterborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "dataheadercolor":
                            this.v_settings.v_dataheadercolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "datafieldevencolor":
                            this.v_settings.v_datafieldevencolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "datafieldoddcolor":
                            this.v_settings.v_datafieldoddcolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupheaderevencolor":
                            this.v_settings.v_groupheaderevencolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupheaderoddcolor":
                            this.v_settings.v_groupheaderoddcolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupfooterevencolor":
                            this.v_settings.v_groupfooterevencolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupfooteroddcolor":
                            this.v_settings.v_groupfooteroddcolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "reportheaderfont":
                            this.v_settings.v_reportheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_reportheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "reportfooterfont":
                            this.v_settings.v_reportfooterfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_reportfooterfont, v_item);
                            v_item.Close();
                            break;
                        case "dataheaderfont":
                            this.v_settings.v_dataheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_dataheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "datafieldfont":
                            this.v_settings.v_datafieldfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_datafieldfont, v_item);
                            v_item.Close();
                            break;
                        case "groupheaderfont":
                            this.v_settings.v_groupheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_groupheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "groupfooterfont":
                            this.v_settings.v_groupfooterfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_groupfooterfont, v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê fonte.
        /// </summary>
        /// <param name="p_font">Objeto fonte, onde vai guardar as informações.</param>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFont(Spartacus.Reporting.Font p_font, System.Xml.XmlReader p_reader)
        {
            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "family":
                            p_font.SetFamily(p_reader.ReadString());
                            break;
                        case "size":
                            p_font.SetSize(p_reader.ReadString());
                            break;
                        case "bold":
                            if (p_reader.ReadString() == "TRUE")
                                p_font.v_bold = true;
                            else
                                p_font.v_bold = false;
                            break;
                        case "italic":
                            if (p_reader.ReadString() == "TRUE")
                                p_font.v_italic = true;
                            else
                                p_font.v_italic = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê comando.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadCommand(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            this.v_cmd = new Spartacus.Database.Command();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "sql":
                            this.v_cmd.v_text = p_reader.ReadString();
                            break;
                        case "parameter":
                            v_item = p_reader.ReadSubtree();
                            this.ReadParameter(v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê parâmetro.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadParameter(System.Xml.XmlReader p_reader)
        {
            string v_name = null;
            Spartacus.Database.Type v_type = Spartacus.Database.Type.STRING;
            Spartacus.Database.Locale v_locale = Spartacus.Database.Locale.EUROPEAN;
            string v_dateformat = null;
            string v_description = null;
            string v_lookup = null;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "name":
                            v_name = p_reader.ReadString();
                            break;
                        case "type":
                            switch(p_reader.ReadString())
                            {
                                case "INTEGER":
                                    v_type = Spartacus.Database.Type.INTEGER;
                                    break;
                                case "REAL":
                                    v_type = Spartacus.Database.Type.REAL;
                                    break;
                                case "BOOLEAN":
                                    v_type = Spartacus.Database.Type.BOOLEAN;
                                    break;
                                case "CHAR":
                                    v_type = Spartacus.Database.Type.CHAR;
                                    break;
                                case "DATE":
                                    v_type = Spartacus.Database.Type.DATE;
                                    break;
                                case "STRING":
                                    v_type = Spartacus.Database.Type.STRING;
                                    break;
                                case "QUOTEDSTRING":
                                    v_type = Spartacus.Database.Type.QUOTEDSTRING;
                                    break;
                                case "UNDEFINED":
                                    v_type = Spartacus.Database.Type.UNDEFINED;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "locale":
                            switch(p_reader.ReadString())
                            {
                                case "EUROPEAN":
                                    v_locale = Spartacus.Database.Locale.EUROPEAN;
                                    break;
                                case "AMERICAN":
                                    v_locale = Spartacus.Database.Locale.AMERICAN;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "dateformat":
                            v_dateformat = p_reader.ReadString();
                            break;
                        case "description":
                            v_description = p_reader.ReadString();
                            break;
                        case "lookup":
                            v_lookup = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_cmd.AddParameter(v_name, v_type);
            if (v_type == Spartacus.Database.Type.REAL)
                this.v_cmd.SetLocale(v_name, v_locale);
            else
                if (v_type == Spartacus.Database.Type.DATE)
                    this.v_cmd.SetDateFormat(v_name, v_dateformat);
            this.v_cmd.SetDescription(v_name, v_description);
            this.v_cmd.SetLookup(v_name, v_lookup);
        }

        /// <summary>
        /// Lê cabeçalho do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadHeader(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "height":
                            this.v_header.SetHeight(p_reader.ReadString());
                            break;
                        case "object":
                            v_item = p_reader.ReadSubtree();
                            this.ReadHeaderObject(v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_header.v_border = this.v_settings.v_reportheaderborder;
        }

        /// <summary>
        /// Lê objeto do cabeçalho do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadHeaderObject(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Object v_object;

            v_object = new Spartacus.Reporting.Object();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            switch (p_reader.ReadString())
                            {
                                case "IMAGE":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.IMAGE;
                                    break;
                                case "TEXT":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.TEXT;
                                    break;
                                case "PAGENUMBER":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.PAGENUMBER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "column":
                            v_object.v_column = p_reader.ReadString();
                            break;
                        case "posx":
                            v_object.SetPosX(p_reader.ReadString());
                            break;
                        case "posy":
                            v_object.SetPosY(p_reader.ReadString());
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_header.v_objects.Add(v_object);
        }

        /// <summary>
        /// Lê rodapé do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFooter(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "height":
                            this.v_footer.SetHeight(p_reader.ReadString());
                            break;
                        case "object":
                            v_item = p_reader.ReadSubtree();
                            this.ReadFooterObject(v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_footer.v_border = this.v_settings.v_reportfooterborder;
        }

        /// <summary>
        /// Lê objeto do rodapé do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFooterObject(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Object v_object;

            v_object = new Spartacus.Reporting.Object();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            switch (p_reader.ReadString())
                            {
                                case "IMAGE":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.IMAGE;
                                    break;
                                case "TEXT":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.TEXT;
                                    break;
                                case "PAGENUMBER":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.PAGENUMBER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "column":
                            v_object.v_column = p_reader.ReadString();
                            break;
                        case "posx":
                            v_object.SetPosX(p_reader.ReadString());
                            break;
                        case "posy":
                            v_object.SetPosY(p_reader.ReadString());
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_footer.v_objects.Add(v_object);
        }

        /// <summary>
        /// Lê campos do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFields(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            this.v_numrowsdetail = 1;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "field")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadField(v_item);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadField(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "format":
                            v_field.v_format = p_reader.ReadString();
                            break;
                        case "border":
                            v_field.v_border = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "blank":
                            v_field.v_blank = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > this.v_numrowsdetail)
                this.v_numrowsdetail = v_field.v_row + 1;

            this.v_fields.Add(v_field);
        }

        /// <summary>
        /// Lê grupos do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadGroups(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "group")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroup(v_item);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único grupo do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadGroup(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;
            Spartacus.Reporting.Group v_group;

            v_group = new Spartacus.Reporting.Group();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "level":
                            v_group.v_level = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "column":
                            v_group.v_column = p_reader.ReadString();
                            break;
                        case "sort":
                            v_group.v_sort = p_reader.ReadString();
                            break;
                        case "showheader":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showheader = false;
                            else
                                v_group.v_showheader = true;
                            break;
                        case "showfooter":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showfooter = false;
                            else
                                v_group.v_showfooter = true;
                            break;
                        case "headerfields":
                            v_item = p_reader.ReadSubtree();
                            this.ReadGroupHeaderFields(v_item, v_group);
                            v_item.Close();
                            break;
                        case "footerfields":
                            v_item = p_reader.ReadSubtree();
                            this.ReadGroupFooterFields(v_item, v_group);
                            v_item.Close();
                            break;
                        case "showheadertitles":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showheadertitles = false;
                            else
                                v_group.v_showheadertitles = true;
                            break;
                        case "showfootertitles":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showfootertitles = false;
                            else
                                v_group.v_showfootertitles = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_groups.Add(v_group);
        }

        /// <summary>
        /// Lê campos de um cabeçalho de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupHeaderFields(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "headerfield")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroupHeaderField(v_item, p_group);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo de um cabeçalho de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupHeaderField(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "groupedvalue":
                            if (p_reader.ReadString() == "FALSE")
                                v_field.v_groupedvalue = false;
                            else
                                v_field.v_groupedvalue = true;
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "format":
                            v_field.v_format = p_reader.ReadString();
                            break;
                        case "border":
                            v_field.v_border = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "blank":
                            v_field.v_blank = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > p_group.v_numrowsheader)
                p_group.v_numrowsheader = v_field.v_row + 1;

            p_group.v_headerfields.Add(v_field);
        }

        /// <summary>
        /// Lê campos de um rodapé de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupFooterFields(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "footerfield")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroupFooterField(v_item, p_group);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo de rodapé de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupFooterField(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "groupedvalue":
                            if (p_reader.ReadString() == "FALSE")
                                v_field.v_groupedvalue = false;
                            else
                                v_field.v_groupedvalue = true;
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "format":
                            v_field.v_format = p_reader.ReadString();
                            break;
                        case "border":
                            v_field.v_border = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "blank":
                            v_field.v_blank = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > p_group.v_numrowsfooter)
                p_group.v_numrowsfooter = v_field.v_row + 1;

            p_group.v_footerfields.Add(v_field);
        }
    }
}