Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Administration page used to maintain project codes
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW   07/06/2010  A4WA#6329 - Added support for deletion.
    ''' ColinD  06/05/2010  D11756 - created
    ''' </history>
    Partial Public Class ProjectCodes
        Inherits BasePage

#Region "Fields"

        Private _CurrentUser As WebSecurityUser     ' use property _CurrentUser to access this variable

#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim stdBut As StdButtonsBase

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant( _
                            "AbacusIntranet.WebNavMenuItem.Commitments.ReferenceData.ProjectCodes"), _
                            "Project Codes")

            stdBut = stdButtons1

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand( _
                                Target.Library.Web.ConstantsManager.GetConstant( _
                                    "AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceData.ProjectCodes.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand( _
                                Target.Library.Web.ConstantsManager.GetConstant( _
                                    "AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceData.ProjectCodes.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand( _
                                Target.Library.Web.ConstantsManager.GetConstant( _
                                    "AbacusIntranet.WebNavMenuItemCommand.Commitments.ReferenceData.ProjectCodes.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .AuditLogTableNames.Add("ProjectCode")
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ProjectCode
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant( _
                            "AbacusIntranet.WebReport.Commitments.ReferenceData")
            End With

            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf EditClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.NewClicked, AddressOf NewClicked

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim code As ProjectCode

            code = GetNewProjectCodeInstance()

            If e.ItemID > 0 Then
                ' if we have an project code to work with
                With code
                    msg = .Fetch(e.ItemID)
                    If msg.Success Then
                        ' if found a matching project code, display values
                        DisplayProjectCode(code)
                    Else
                        ' else not found project code so display error
                        WebUtils.DisplayError(msg)
                    End If
                End With
            Else
                ' else no project code, setup as new
                DisplayEmptyProjectCode(False)
            End If

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim code As ProjectCode

            code = GetNewProjectCodeInstance()

            msg = code.Fetch(e.ItemID)

            If msg.Success Then
                DisplayProjectCode(code)
            Else
                WebUtils.DisplayError(msg)
            End If

        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = ProjectCode.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub


        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            DisplayEmptyProjectCode(True)
            txtCode.SetFocus = True

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim code As ProjectCode = Nothing
            Dim codeCollection As New ProjectCodeCollection()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then

                code = GetNewProjectCodeInstance()

                If e.ItemID > 0 Then
                    ' existing project code so fetch from db

                    With code

                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    End With

                Else
                    ' check for existing ProjectCodes with the same code

                    ProjectCode.FetchList(conn:=DbConnection, _
                                   auditLogTitle:=AuditLogTitle, _
                                   auditUserName:=AuditLogUserName, _
                                   code:=txtCode.Text, _
                                   list:=codeCollection)

                    code.Code = txtCode.Text

                End If

                If codeCollection.Count = 0 Then
                    ' no duplicate code exist

                    With code
                        .Description = txtDescription.Text
                        .Redundant = chkRedundant.CheckBox.Checked
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        e.ItemID = .ID
                    End With

                    DisplayProjectCode(code)

                Else
                    ' duplicate code exists so advise user

                    lblError.Text = String.Format("The code '{0}' is already in use, please specify another code.", _
                                                  txtCode.Text)
                    e.Cancel = True

                End If

            Else

                e.Cancel = True

            End If

        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Displays an empty project code i.e. a code with no values
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DisplayEmptyProjectCode(ByVal codeEntryEnabled As Boolean)

            txtCode.Text = String.Empty
            If codeEntryEnabled Then
                txtCode.TextBox.Attributes.Remove("disabled")
            Else
                txtCode.TextBox.Attributes.Add("disabled", "true")
            End If
            txtDescription.Text = String.Empty
            chkRedundant.CheckBox.Checked = False

        End Sub

        ''' <summary>
        ''' Displays a project code
        ''' </summary>
        ''' <param name="code">The code to display</param>
        ''' <remarks></remarks>
        Private Sub DisplayProjectCode(ByVal code As ProjectCode)

            If Not code Is Nothing AndAlso code.ID > 0 Then
                ' if we have a code to work with

                txtCode.Text = code.Code
                txtCode.TextBox.Attributes.Add("disabled", "true")
                txtCode.SetFocus = False
                txtDescription.Text = code.Description
                txtDescription.SetFocus = True
                chkRedundant.CheckBox.Checked = code.Redundant

            Else
                ' else display an empty project code 

                DisplayEmptyProjectCode(True)

            End If

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Gets an instance of ProjectCode object
        ''' </summary>
        ''' <returns>An instance of ProjectCode object</returns>
        ''' <remarks></remarks>
        Private Function GetNewProjectCodeInstance() As ProjectCode

            Return New ProjectCode(conn:=DbConnection, _
                                        AuditLogTitle:=AuditLogTitle, _
                                        auditUserName:=AuditLogUserName)

        End Function

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current user
        ''' </summary>
        ''' <value></value>
        ''' <returns>The current user</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

        ''' <summary>
        ''' Gets the audit log title
        ''' </summary>
        ''' <value></value>
        ''' <returns>The audit log title</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
            End Get
        End Property

        ''' <summary>
        ''' Gets the audit log user name
        ''' </summary>
        ''' <value></value>
        ''' <returns>The audit log user name</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

#End Region

    End Class

End Namespace
