Imports System.Text
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Extensions.AjaxControlToolkit


Namespace Apps.ServiceUsers.Enquiry

    ''' <summary>
    ''' Screen used to maintain a service user addresses.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Mo Tahir      19/04/2011  D11971 - SDS Generic Creditor Notes
    '''     ColinD        23/12/2010  D11852C - Added sds contributions tab, reordered tabs at same time. Changes to enable auto selection of sds contributions tab and auto expand all panels inside tab.
    '''     Mo Tahir      27/08/2010  D11814 - Service User Enquiry.
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _clientID As Integer


#Region " Properties"
        Public ReadOnly Property ServiceUserID() As Integer
            Get
                Return _clientID
            End Get
        End Property
#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientID")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            End If

            tabDocuments.Visible = SecurityBL.UserHasMenuItem(Me.DbConnection, user.ID, _
                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                   "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                   Settings.CurrentApplicationID)

            ' setup buttons
            With _stdBut
                .EditableControls.Add(tabAdministration.Controls)
                .EditableControls.Add(tabAddresses.Controls)
                .EditableControls.Add(tabFinance.Controls)
                .EditableControls.Add(tabServices.Controls)
                .EditableControls.Add(tabDocuments.Controls)
                .EditableControls.Add(tabNotes.Controls)
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUser.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowBack = True
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUser.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUser.Delete"))
                .AuditLogTableNames.Add("ClientDetail")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            With CType(serviceUserHeader1, Target.Abacus.Web.Apps.UserControls.ServiceUserHeader)
                .InitControl(Me.Page, Me.DbConnection, _clientID)
            End With
        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)



        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)


        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)


        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)



        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            
        End Sub

#End Region

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Const SCRIPT_STARTUP As String = "Startup"
            Dim expandChildrenPanels As Boolean = False
            Dim showSdsContributionsTab As Boolean = False
            Dim tmpBoolean As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("expandChildren"))
            Dim tabLoadJs As String = ""

            If Not tmpBoolean Is Nothing AndAlso tmpBoolean.HasValue Then
                expandChildrenPanels = tmpBoolean.Value
            End If

            tmpBoolean = Utils.ToBoolean(Request.QueryString("showSdsContribs"))

            If Not tmpBoolean Is Nothing AndAlso tmpBoolean.HasValue Then
                showSdsContributionsTab = tmpBoolean.Value
            End If

            If showSdsContributionsTab Then
                tabStrip.SetActiveTabByHeaderText("SDS Contributions")
                tabLoadJs = "LoadSdsContribsTab();"
            End If

            ' output javascript
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
            Me.JsLinks.Add("Edit.js")
            Me.UseJQuery = True

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("clientID='{0}'; expandChildrenPanels = {1}; {2}", _clientID, expandChildrenPanels.ToString().ToLower(), tabLoadJs), True)
            End If
        End Sub
    End Class

End Namespace