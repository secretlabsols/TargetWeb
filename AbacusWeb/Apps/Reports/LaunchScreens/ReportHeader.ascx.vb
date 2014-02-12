Imports Target.Web.Apps
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.Reports.LaunchScreens

    Partial Public Class ReportHeader
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal reportID As Integer)
            Const SP_NAME As String = "spxWebReport_FetchHeaderInfo"

            Dim msg As ErrorMessage = Nothing
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(thePage.DbConnection, SP_NAME, False)
                spParams(0).Value = reportID
                reader = SqlHelper.ExecuteReader(thePage.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                Do While reader.Read
                    txtDescription.Text = reader("Description")
                    txtCategories.Text = reader("Categories")
                Loop

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            Finally
                SqlHelper.CloseReader(reader)
            End Try

            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

    End Class

End Namespace
