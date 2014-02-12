
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.SP.Library
Imports Target.Web.Apps.Security

Namespace Apps.SUNotif.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.SUNotif.Admin.ProcessSUNotif
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allows admin user to process service user notifications.
    '''     Note that this only sets the status of the notification for the purpose
    '''     of giving feedback to the provider.  The actually steps that the admin user 
    '''     needs to go through in the SP EXE must be done manually.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      05/03/2007  Added Provider Ref, Service Level & Unit Cost.
    '''                             Made DoB and NINo optional.
    ''' 	[Mikevo]	24/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ProcessSUNotif
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Const SP_NAME As String = "spxSPSubsidy_Fetch"
            Const SP_ENDREASON As String = "pr_FetchSPSubsidyEndReasons"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSUNotifsProcess"), "Process Service User Notification")

            Dim suNotifID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("suNotifID"))
            Dim msg As ErrorMessage
            Dim notif As ViewableSUNotif = Nothing
            Dim accept As Boolean
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim subsidyDT As DataTable, endReasonsDT As DataTable
            Dim spParams As SqlParameter()

            Me.AddExtraCssStyle(".content { float:left; } fieldset.group { margin-bottom:1em; }")
            Me.JsLinks.Add("ProcessSUNotif.js")

            ' fetch the notif
            msg = SPClassesBL.FetchViewableSUNotif(Me.DbConnection, suNotifID, notif)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If IsPostBack Then
                ' update the notification
                accept = Convert.ToBoolean(hidDecision.Value)
                With notif
                    .CompletedDate = Now
                    .ProcessedByUserID = user.ID
                    .Comment = txtComment.Text
                    .StatusID = IIf(accept, SUNotifStatus.Accepted, SUNotifStatus.Declined)
                    msg = .Save
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    Response.Redirect("../ListSUNotif.aspx")
                End With
            End If

            ' check the status
            If notif.StatusID <> SUNotifStatus.Submitted Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            With notif
                txtReference.Text = .Reference
                txtType.Text = Target.Library.Utils.SplitOnCapitals(.TypeDesc)
                txtCreated.Text = .CreatedDate
                txtSubmitted.Text = .SubmittedDate
                txtRequestedBy.Text = .RequestedBy
                If .TypeID = SUNotifType.NewNotification Then
                    grpEnd.Visible = False
                    txtNewPrimaryName.Text = String.Format("{0} {1} {2}", .PrimaryTitle, .PrimaryFirstNames, .PrimaryLastName)
                    txtNewPrimaryNINO.Text = .PrimaryNINo
                    txtNewPrimaryBirthDate.Text = IIf(Target.Library.Utils.IsDate(.PrimaryBirthDate), .PrimaryBirthDate, "")
                    txtNewSecondaryName.Text = String.Format("{0} {1} {2}", .SecondaryTitle, .SecondaryFirstNames, .SecondaryLastName)
                    txtNewSecondaryNINO.Text = .SecondaryNINo
                    If Target.Library.Utils.IsDate(.SecondaryBirthDate) Then txtNewSecondaryBirthDate.Text = .SecondaryBirthDate
                    txtNewAddress.Text = String.Format("{0}{1}{2}", .Address, vbCrLf, .Postcode).Replace(vbCrLf, "<br />")
                    txtNewProvider.Text = String.Format("{0}: {1}", .ProviderRef, .ProviderName)
                    txtNewService.Text = String.Format("{0}: {1}", .ServiceRef, .ServiceName)
                    txtExpectedStartDate.Text = .ServiceStartDate
                    txtAgreement.Text = IIf(.TenancyServiceAgreement, "Yes", "No")
                    txtYourReference.Text = .YourReference
                    txtServiceLevel.Text = .ServiceLevel
                    txtUnitCost.Text = .UnitCost.ToString("c")
                End If
                If .TypeID = SUNotifType.EndNotification Then
                    ' get the subsidy details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = .SPSubsidyAgreementID
                    subsidyDT = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)
                    ' get the end reasons
                    endReasonsDT = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_ENDREASON).Tables(0)

                    grpNew.Visible = False
                    txtEndProvider.Text = String.Format("{0}: {1}", .ProviderRef, .ProviderName)
                    txtEndService.Text = String.Format("{0}: {1}", .ServiceRef, .ServiceName)
                    txtEndPrimaryName.Text = String.Format("{0} {1}", .PrimaryFirstNames, .PrimaryLastName)
                    txtEndSecondaryName.Text = String.Format("{0} {1}", .SecondaryFirstNames, .SecondaryLastName)
                    txtEndDateFrom.Text = subsidyDT.Rows(0)("DateFrom")
                    txtEndSubsidy.Text = Convert.ToDecimal(subsidyDT.Rows(0)("Subsidy")).ToString("C")
                    txtEndDateTo.Text = .SubsidyEndDate
                    txtEndReason.Text = endReasonsDT.Select(String.Format("ID = {0}", .SubsidyEndReasonID))(0)("Description")

                End If
            End With

            With txtComment.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 5
            End With

        End Sub

    End Class

End Namespace



