Imports Target.Abacus.Library.Notes
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library

Namespace Apps.ServiceUsers.Enquiry

    ''' <summary>
    ''' Screen used to hold NotesSelector user controls.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     19/04/2011  D11971 - Sds Generic Creditor Notes
    ''' </history>
    Partial Public Class Notes
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim clientId As Integer
            Dim noteType As NoteTypes
            Me.JsLinks.Add("Notes.js")

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            'get the variables reguired to set the notes selector properties 
            clientId = Target.Library.Utils.ToInt32(Request.QueryString("clientid"))
            noteType = Target.Library.Utils.ToInt32(Request.QueryString("notetype"))

            'load the notes control
            With CType(Notes1, Target.Abacus.Web.Apps.UserControls.NotesSelector)
                .FilterNoteType = noteType
                .FilterNoteTypeChildID = clientId
                .ViewNoteInNewWindow = True
                .InitControl(Me.Page)
            End With
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
        End Sub
    End Class
End Namespace
