Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Notes

Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to manage notes
    ''' </summary>
    ''' <history>
    ''' MoTahir  D11971 Created 08/04/2011
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Notes")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class Notes
        Inherits System.Web.Services.WebService

#Region "Fields"

        ' fields
        Private Shared _ConnectionString As String = Nothing

        ' constants
        Private Const _ConnectionStringKey As String = "Abacus"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the connection string.
        ''' </summary>
        ''' <value>The connection string.</value>
        Private Shared ReadOnly Property ConnectionString() As String
            Get
                If String.IsNullOrEmpty(_ConnectionString) OrElse _ConnectionString.Trim().Length = 0 Then
                    ' if we havent already fetched the connection string then do so
                    _ConnectionString = ConnectionStrings(_ConnectionStringKey).ConnectionString
                End If
                Return _ConnectionString
            End Get
        End Property

#End Region

#Region "Functions"

        ''' <summary>
        ''' Gets the notes.
        ''' </summary>
        ''' <param name="page">The page to fetch</param>
        ''' <param name="pageSize">The size of the page to fetch</param>
        ''' <param name="noteCategoryID">The note category id of the note.</param>
        ''' <param name="noteTypeID">The note type of the note</param>
        ''' <param name="noteTypeChildID">The note type child id</param>
        ''' <param name="selectedID">The selected id</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Get Note records By NoteTypeChildID and NoteCategoryID"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Shared Function GetPagedNotes(ByVal page As Integer, _
                                             ByVal pageSize As Integer, _
                                             ByVal noteCategoryID As Integer, _
                                             ByVal noteTypeID As NoteTypes, _
                                             ByVal noteTypeChildID As Integer, _
                                             ByVal selectedID As Integer) _
                                             As Notes_GetPagedNotesResult

            Dim connection As SqlConnection = Nothing
            Dim msg As New ErrorMessage()
            Dim result As New Notes_GetPagedNotesResult()

            Try

                Dim totalRecords As Integer = 0
                Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

                ' check the requesting user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                ' get the connection
                connection = SqlHelper.GetConnection(ConnectionString)

                ' get the list of notes and throw error if not succeeded
                msg = NoteBL.GetPagedNotes(connection, page, pageSize, totalRecords, _
                                           noteCategoryID, noteTypeID, noteTypeChildID, selectedID, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage()
                    .ErrorMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:NotesSelector_FetchNotes({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                ' catch and wrap the unexpected error

                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            Finally
                ' always close the connection

                SqlHelper.CloseConnection(connection)

            End Try

            Return result

        End Function


#Region " FetchNoteCategoriesList "

        ''' <summary>
        ''' Gets the note categories.
        ''' </summary>
        ''' <param name="noteTypeID">The note type of the note</param>
        ''' <param name="noteTypeChildID">The note type child id</param>
        ''' <returns></returns>
        <WebMethod(EnableSession:=True, Description:="Get NoteCategory records By NoteType and NoteTypeChildID"), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchNoteCategoriesList(ByVal noteTypeID As NoteTypes, _
                                                ByVal noteTypeChildID As Integer) As FetchNoteCategoriesListResult

            Dim msg As ErrorMessage
            Dim result As FetchNoteCategoriesListResult = New FetchNoteCategoriesListResult
            Dim conn As SqlConnection = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of note categories
                msg = NoteBL.FetchListOfViewableNoteCategoriesByNoteTypeAndChildID(conn, noteTypeID, noteTypeChildID, result.Items)
                If Not msg.Success Then
                    result.ErrorMsg = msg
                    Return result
                End If

                With result
                    .ErrorMsg = New ErrorMessage
                    .ErrorMsg.Success = True
                End With

            Catch ex As Exception
                result.ErrorMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#End Region

    End Class

End Namespace

