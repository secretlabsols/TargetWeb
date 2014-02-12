
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.SavedWizardSelections.WebSvc

    ''' <summary>
    ''' Web service to provide services to the saved wizard selections functionlaity.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  21/07/2009  D11651 - created.
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Apps/SavedWizardSelections")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class Selections
        Inherits System.Web.Services.WebService

#Region " FetchSelectionsList "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSelectionsList(ByVal page As Integer, _
                                            ByVal selectedID As Integer, _
                                            ByVal listFilterName As String, _
                                            ByVal listFilterScreen As String, _
                                            ByVal listFilterOwner As String) As FetchSelectionsListResult

            Const SP_NAME As String = "spxvwWebSavedWizardSelection_FetchListWithPaging"

            Dim msg As ErrorMessage
            Dim result As FetchSelectionsListResult = New FetchSelectionsListResult()
            Dim conn As SqlConnection = Nothing
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()
            Dim settings As SystemSettings
            Dim applicationID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim currentUser As WebSecurityUser
            Dim canManageGlobals As Boolean, canManageOtherUsers As Boolean

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get settings
                settings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                applicationID = Convert.ToInt32(settings.CurrentApplicationID)
                ' get current user
                currentUser = SecurityBL.GetCurrentUser()
                ' get access rights
                canManageGlobals = _
                    SecurityBL.UserHasMenuItemCommand( _
                        conn, _
                        currentUser.ID, _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            String.Format("{0}.WebNavMenuItemCommand.SavedWizardSelectionsEdit.ManageGlobal", settings.CurrentApplicationID) _
                        ), _
                        applicationID _
                )
                canManageOtherUsers = _
                    SecurityBL.UserHasMenuItemCommand( _
                        conn, _
                        currentUser.ID, _
                        Target.Library.Web.ConstantsManager.GetConstant( _
                            String.Format("{0}.WebNavMenuItemCommand.SavedWizardSelectionsEdit.ManageOtherUsers", settings.CurrentApplicationID) _
                        ), _
                        applicationID _
                )

                ' call SPX
                spParams = SqlHelperParameterCache.GetSpParameterSet(conn, SP_NAME, False)
                spParams(0).Direction = ParameterDirection.InputOutput
                spParams(0).Value = page
                spParams(1).Value = pageSize
                spParams(2).Value = applicationID
                If selectedID > 0 Then spParams(3).Value = selectedID
                If listFilterName.Length > 0 Then spParams(4).Value = listFilterName.Replace("*", "%")
                If listFilterScreen.Length > 0 Then spParams(5).Value = listFilterScreen.Replace("*", "%")
                If listFilterOwner.Length > 0 Then spParams(6).Value = listFilterOwner.Replace("*", "%")
                spParams(7).Value = canManageGlobals
                spParams(8).Value = canManageOtherUsers
                spParams(9).Value = currentUser.ID
                spParams(10).Direction = ParameterDirection.InputOutput

                reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, SP_NAME, spParams)

                result.Selections = New List(Of ViewableSavedWebSelection)
                While reader.Read()
                    result.Selections.Add( _
                        New ViewableSavedWebSelection( _
                            reader("ID"), _
                            reader("Name"), _
                            reader("ScreenName"), _
                            reader("OwnerName"), _
                            reader("GlobalSelection") _
                        ) _
                    )
                End While
                SqlHelper.CloseReader(reader)

                ' get record count
                totalRecords = Target.Library.Utils.ToInt32(spParams(10).Value)
                ' get selected page
                page = Target.Library.Utils.ToInt32(spParams(0).Value)

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchSelectionList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                SqlHelper.CloseReader(reader)
                SqlHelper.CloseConnection(conn)
            End Try

            Return result

        End Function

#End Region

    End Class

#Region " FetchSelectionsListResult "

    Public Class FetchSelectionsListResult
        Public ErrMsg As ErrorMessage
        Public Selections As List(Of ViewableSavedWebSelection)
        Public PagingLinks As String
    End Class

#End Region

End Namespace
