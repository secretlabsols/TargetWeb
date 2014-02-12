Imports Target.Abacus.Library.eInvoice
Imports System.Collections.Generic




'Imports System.Data.SqlClient
'Imports System.Text
Imports Target.Abacus.Library
'Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
'Imports Target.Library.Web.Controls
'Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
'Imports Target.Web.Apps
'Imports Target.Web.Apps.Security
'Imports Target.Abacus.Library.DomProviderInvoice


Partial Public Class ManualEnterVisitGrid
    Inherits System.Web.UI.UserControl


#Region " Constants "
    Const DeleteCommandName As String = "Delete"
#End Region

#Region " Private Variables "


   
#End Region



    Protected Function PreRoundedDurationClaimedText(ByVal preRoundedValue As String) As String
        If preRoundedValue.Length > 8 Then
            Dim minDate As New Date(1800, 1, 1)
            Dim preRoundedDate As Date = Utils.ToDateTime(preRoundedValue)
            Dim tSpan As TimeSpan
            tSpan = New TimeSpan(TimeSpan.FromMinutes(DateDiff(DateInterval.Minute, minDate, preRoundedDate)).Ticks)
            Return String.Format("[{0}:{1}]", tSpan.Hours.ToString("00"), tSpan.Minutes.ToString("00"))
            'Return String.Format("[{0}]", New TimeSpan.ToString())
        Else
            Return String.Empty
        End If
    End Function

   
End Class