
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports Target.Web.Apps.Security

Namespace Apps.AmendReq.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.Admin.ChangeEditableFieldSettings
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to allow admin users to create custom editable fields settings for a user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	20/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ChangeEditableFieldSettings
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents cboSetting As System.Web.UI.WebControls.DropDownList

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim msg As ErrorMessage
            Dim pageTitle As String
            Dim pageOverview As String
            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("userID"))
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim externalFullname As String = Request.QueryString("fullName")

            ' change page text
            If userID = 0 Then
                pageTitle = "Change Default Editable Field Settings"
                pageOverview = "The list below displays the default editable field setting that apply to all external user account that do no have any customised settings. "
            Else
                pageTitle = String.Format("Customise Editable Field Settings for {0}", IIf(externalFullname Is Nothing OrElse externalFullname.Length = 0, "[Unknown]", externalFullname))
                pageOverview = "The list below displays the customised editable field settings for the external user account indicated above. "
            End If
            pageOverview &= "To change the setting for an individual editable field, click on the relevant option in the list below."
            litPageOverview.Text = pageOverview

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAmendReqAdminChangeEditableFieldSettings"), pageTitle)

            ' ensure custom settings exist for the specified user
            If userID > 0 Then
                msg = AmendReqBL.EnsureCustomEditableFieldSettings(Me.DbConnection, userID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If

            ' get the list of settings
            msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, userID, settings)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' load the repeater
            rptSettings.DataSource = settings
            rptSettings.DataBind()

        End Sub

        Private Sub rptSettings_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSettings.ItemCreated

            ' create a row in the table for each setting
            ' need to manually do it this way so we can create the dropdown list and hook up its event handler

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

                Dim row As HtmlTableRow
                Dim cell As HtmlTableCell
                Dim dataItem As WebAmendReqDataItem
                Dim dropdownItem As ListItem

                dataItem = DirectCast(e.Item.DataItem, WebAmendReqDataItem)

                ' create the row and first two cells
                row = New HtmlTableRow
                cell = New HtmlTableCell("td")
                cell.InnerText = dataItem.Entity
                row.Cells.Add(cell)

                cell = New HtmlTableCell("td")
                cell.InnerText = dataItem.Name
                row.Cells.Add(cell)

                ' create the third cell with the setting dropdown list
                cell = New HtmlTableCell("td")
                cboSetting = New DropDownList
                With cboSetting
                    AddHandler cboSetting.SelectedIndexChanged, AddressOf cboSetting_SelectedIndexChanged
                    .ID = String.Format("cboSetting{0}", dataItem.ID)
                    For Each setting As Byte In [Enum].GetValues(GetType(EditableStatus))
                        dropdownItem = New ListItem(Target.Library.Utils.SplitOnCapitals([Enum].GetName(GetType(EditableStatus), setting)), setting)
                        dropdownItem.Selected = (dropdownItem.Value = dataItem.EditMode)
                        .Items.Add(dropdownItem)
                    Next
                End With

                ' add the list to the cell and then to the row
                cell.Controls.Add(cboSetting)
                row.Cells.Add(cell)

                ' add the row to the repeater item for rendering
                e.Item.Controls.Add(row)

            End If

        End Sub

        Private Sub cboSetting_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

            ' on postback, fires for each setting dropdown that has been changed

            Dim msg As ErrorMessage
            Dim settingDropdown As DropDownList
            Dim dataItemID As Integer
            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("userID"))

            settingDropdown = DirectCast(sender, DropDownList)
            dataItemID = Target.Library.Utils.ToInt32(settingDropdown.ID.Replace("cboSetting", String.Empty))

            msg = AmendReqBL.SaveEditableFieldSetting(Me.DbConnection, dataItemID, userID, settingDropdown.SelectedValue)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

        End Sub

        Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Response.Redirect("ManageEditableFields.aspx")
        End Sub

    End Class


End Namespace

