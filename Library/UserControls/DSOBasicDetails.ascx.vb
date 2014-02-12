Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library

Namespace Library.UserControls
    ''' <summary>
    ''' Control to display header information about the Service Order.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Paul Wheaver    25/10/2012   D12199 Service Order Funding Amendments
    ''' </history>
    Partial Public Class DSOBasicDetails
        Inherits System.Web.UI.UserControl

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' add the web service type
            AjaxPro.Utility.RegisterTypeForAjax(GetType(ServiceOrder.ServiceOrderService))

            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("Library/UserControls/DSOBasicDetails.js"))

        End Sub
    End Class
End Namespace
