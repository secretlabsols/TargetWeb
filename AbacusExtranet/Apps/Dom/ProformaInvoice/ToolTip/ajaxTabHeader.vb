Imports System.Web.UI
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.eInvoice


Public Class ajaxTabHeader
    Implements System.Web.UI.ITemplate

    Private mLabel As Label
    Private pnl As New Panel
    Public Sub New(ByVal headerText As String, ByVal toolTipText As String)

        mLabel = New Label()
        mLabel.Text = headerText


        mLabel.ToolTip = toolTipText
    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn

        Dim ph As New Panel()

        pnl = New Panel()
        pnl.Controls.Add(mLabel)
        pnl.Style.Add("float", "left")
        container.Controls.Add(pnl)

        pnl = New Panel()
        pnl.CssClass = "clearer"
        container.Controls.Add(pnl)

    End Sub



End Class

Public Class ajaxTabHeaderEx
    Implements System.Web.UI.ITemplate

    Private mLabel As Label
    Private pnl As New Panel
    Public imgBtnDelete As ImageButton
    Public imgBtnEdit As HtmlImage


    Public Sub New(ByVal cp As CareProvider, _
                   ByVal TabPanelId As String)

        mLabel = New Label()
        mLabel.Text = cp.CareProviderName & "&nbsp;"
        mLabel.ToolTip = cp.CareProviderName

        imgBtnDelete = New ImageButton()
        imgBtnDelete.ID = "btnDeleteCareWorder"
        imgBtnDelete.ImageUrl = WebUtils.GetVirtualPath("Images/delete.png")
        imgBtnDelete.ToolTip = "Delete care worker along with associated visits"
        imgBtnDelete.CommandName = "DELETE"
        imgBtnDelete.CommandArgument = cp.CareProviderID & "_" & cp.CareProviderName & "_" & cp.ObjectIndex


        imgBtnEdit = New HtmlImage()
        imgBtnEdit.ID = "btnEditCareWorker"
        imgBtnEdit.Alt = "Amend care worker"
        imgBtnEdit.Src = WebUtils.GetVirtualPath("Images/EditTable.png")
        Dim script As String = String.Format("javascript:EditCareProvider('{0}','{1}','{2}','{3}','{4}');", _
                                             cp.ExistingCareWorkers, _
                                             cp.ObjectIndex, _
                                             cp.CareProviderID, _
                                             cp.CareProviderName, _
                                             cp.Reference)
        imgBtnEdit.Attributes.Add("onClick", script)

    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn

        Dim ph As New Panel()

        pnl = New Panel()
        pnl.Controls.Add(mLabel)
        pnl.Style.Add("margin-right", "3px")
        pnl.Style.Add("float", "left")
        container.Controls.Add(pnl)

        pnl = New Panel()
        pnl.Controls.Add(imgBtnDelete)
        pnl.Style.Add("margin-right", "3px")
        pnl.Style.Add("float", "left")
        container.Controls.Add(pnl)

        pnl = New Panel()
        pnl.Controls.Add(imgBtnEdit)
        pnl.Style.Add("float", "left")
        container.Controls.Add(pnl)

        pnl = New Panel()
        pnl.CssClass = "clearer"
        container.Controls.Add(pnl)

    End Sub



End Class
