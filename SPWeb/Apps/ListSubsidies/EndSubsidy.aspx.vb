Imports Target.Library.Web
Imports System.text
Imports System.Data.SqlClient
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.ListSubsidies
    Partial Class EndSubsidy
        Inherits Target.Web.Apps.BasePage

        Const SP_NAME As String = "spxSPSubsidy_Fetch"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Dim subsidyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim msg As ErrorMessage = New ErrorMessage
            Dim dropdownItem As ListItem

            Const SP_ENDREASON As String = "pr_FetchSPSubsidyEndReasons"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSubsidyEnd"), "End Subsidy")

            strStyle.Append("label.label { float:left; width:19em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("span.label { float:left; width:9em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("ul.list { margin:0em;list-style:none; }")
            strStyle.Append(".content { float:left; width:60%; }")
            strStyle.Append(".content1 { float:left; width:30%; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/ListSubsidies/EndSubsidy.js"))

            If IsPostBack Then
                Me.Validate()
                If Me.IsValid Then
                    ' create a SU notif
                    msg = CreateNewNotif(Me.DbConnection)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    Response.Redirect("../SUNotif/ListSUNotif.aspx")
                End If
            End If

            'Populate End Reasons
            Try
                ' execute
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_ENDREASON)

                With cboEndReason.Items
                    dropdownItem = New ListItem("", 0)
                    .Add(dropdownItem)
                    While reader.Read
                        dropdownItem = New ListItem(reader("Description"), reader("ID"))
                        .Add(dropdownItem)
                    End While
                End With
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_ENDREASON, "EndSubsidy.Page_load")     ' error reading data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
            End Try

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = subsidyID

                ' execute
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                While reader.Read
                    lblProvider.Text = WebUtils.EncodeOutput(reader("ProviderReference")) & " - " & WebUtils.EncodeOutput(reader("ProviderName"))
                    lblService.Text = WebUtils.EncodeOutput(reader("ServiceReference")) & " - " & WebUtils.EncodeOutput(reader("ServiceName"))
                    lblPrimaryServiceUser.Text = WebUtils.EncodeOutput(reader("PrimaryServiceUserRef")) & " - " & WebUtils.EncodeOutput(reader("PrimaryServiceUserName"))
                    If Not IsDBNull(reader("SecondaryServiceUserRef")) Then
                        lblSecondaryServiceUser.Text = WebUtils.EncodeOutput(reader("SecondaryServiceUserRef")) & " - " & WebUtils.EncodeOutput(reader("SecondaryServiceUserName"))
                    End If
                    lblDateFrom.Text = WebUtils.EncodeOutput(reader("DateFrom"))
                    lblSubsidy.Text = String.Format("{0:C}", reader("Subsidy"))
                    If WebUtils.EncodeOutput(reader("DateTo")) <> "31/12/2079" Then
                        txtEndDate.Text = WebUtils.EncodeOutput(reader("DateTo"))
                    End If

                    If Not IsDBNull(reader("SubsidyEndReasonID")) Then
                        cboEndReason.Value = reader("SubsidyEndReasonID")
                    End If

                End While

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "EndSubsidy.Page_load")     ' error reading data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
            End Try
        End Sub

        Private Function CreateNewNotif(ByVal conn As SqlConnection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim notif As WebSPSUNotif
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim subsidyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim subsidyDT As DataTable

            Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
            spParams(0).Value = subsidyID

            ' execute
            subsidyDT = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)

            notif = New WebSPSUNotif(conn)
            With notif
                .TypeID = SUNotifType.EndNotification
                .StatusID = SUNotifStatus.Submitted
                .CreatedDate = Now
                .SubmittedDate = Now
                .RequestedByUserID = user.ID
                .PrimaryFirstNames = WebUtils.EncodeOutput(subsidyDT.Rows(0)("PrimaryServiceUserFirstNames"), False)
                .PrimaryLastName = WebUtils.EncodeOutput(subsidyDT.Rows(0)("PrimaryServiceUserLastName"), False)
                .SecondaryFirstNames = WebUtils.EncodeOutput(subsidyDT.Rows(0)("SecondaryServiceUserFirstNames"), False)
                .SecondaryLastName = WebUtils.EncodeOutput(subsidyDT.Rows(0)("SecondaryServiceUserLastName"), False)
                .SPProviderID = subsidyDT.Rows(0)("ProviderID")
                .SPServiceID = subsidyDT.Rows(0)("ServiceID")
                .SPSubsidyAgreementID = subsidyID
                .SubsidyEndDate = txtEndDate.Text
                .SubsidyEndReasonID = cboEndReason.Items(cboEndReason.SelectedIndex).Value
                msg = .Save()
                If Not msg.Success Then Return msg
            End With

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

    End Class
End Namespace
