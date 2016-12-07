import clr
import System
from System import Console

v_list = System.Collections.Generic.List[System.String]()
v_list.Add("john")
v_list.Add("pat")
v_list.Add("gary")
v_list.Add("michael")

for a in v_list:
	print "Esse eh meu amigo " + a

