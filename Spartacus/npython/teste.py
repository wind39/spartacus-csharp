import clr
clr.AddReference("System.Data")
clr.AddReference("System.Xml")
clr.AddReference("Spartacus")
import Spartacus

try:
    v_database = Spartacus.Database.Sqlite("basecadastro.db")
    v_table = v_database.Query("select * from paises", "PAISES")
    for r in v_table.Rows:
        print("{0} {1}".format(r["pais_st_codigo"], r["pais_st_nome"]))
except Spartacus.Database.Exception as exc:
    print("ERRO: {0}".format(exc.v_message))
