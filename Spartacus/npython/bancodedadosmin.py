import clr
clr.AddReference("System.Data")
clr.AddReference("System.Xml")
clr.AddReference("SpartacusMin")
import SpartacusMin

try:
    v_database = SpartacusMin.Database.Sqlite("basecadastro.db")
    v_table = v_database.Query("select * from paises", "PAISES")
    for r in v_table.Rows:
        print("{0} {1}".format(r["pais_st_codigo"], r["pais_st_nome"]))
except SpartacusMin.Database.Exception as exc:
    print("ERRO: {0}".format(exc.v_message))

