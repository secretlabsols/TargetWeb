
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.CMS.Controls

Namespace Apps.CMS.Admin

    ''' <summary>
    ''' Screen to allow the selection of a CMS page.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class SelectItem
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS: Select Item")

            With tree
                .TreeMode = Target.Library.Utils.ToInt32(Request.QueryString("treeMode"))
                .CurrentFolderID = Target.Library.Utils.ToInt32(Request.QueryString("currentFolderID"))
            End With

        End Sub

    End Class

End Namespace