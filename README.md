# PortScanner
Port Scanner VB.NET

To use class:

Dim PS As New PortScan("[IP-ADDRESS]", 1, 1000)

Dim MyDT As DataTable = PS.Scan

PS.SaveToFile(My.Application.Info.DirectoryPath, PS.IP & ".scan.txt")
