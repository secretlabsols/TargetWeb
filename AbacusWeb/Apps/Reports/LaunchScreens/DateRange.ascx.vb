Imports Target.Web.Apps
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.Reports.LaunchScreens

    Partial Public Class DateRange
        Inherits System.Web.UI.UserControl

        Public Property DateFrom() As Date
            Get
                Return dteDateFrom.Text
            End Get
            Set(ByVal value As Date)
                dteDateFrom.Text = value
            End Set
        End Property

        Public Property DateTo() As Date
            Get
                Return dteDateTo.Text
            End Get
            Set(ByVal value As Date)
                dteDateTo.Text = value
            End Set
        End Property

        Public Sub InitControl(ByVal thePage As BasePage, ByVal defaultToFinancialYear As Boolean)

            If defaultToFinancialYear Then
                If Date.Now.Month = 1 Or Date.Now.Month = 2 Or Date.Now.Month = 3 Then
                    dteDateFrom.Text = String.Format("01/04/{0}", Date.Now.Year - 1)
                    dteDateTo.Text = String.Format("31/03/{0}", Date.Now.Year)
                Else
                    dteDateFrom.Text = String.Format("01/04/{0}", Date.Now.Year)
                    dteDateTo.Text = String.Format("31/03/{0}", Date.Now.Year + 1)
                End If

            End If

        End Sub

    End Class

End Namespace