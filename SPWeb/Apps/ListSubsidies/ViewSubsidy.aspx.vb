
Imports System.Configuration.ConfigurationManager
Imports Target.Library.Web
Imports System.Data.SqlClient
Imports Target.Library
Imports System.text
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports Target.Web.Apps.Security
Imports Target.SP.Library
Imports Target.SP.Library.Collections

Namespace Apps.ListSubsidies

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ListSubsidies.ViewSubsidy
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to view combined subsidy/service agreement details.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      08/03/2007  Hide Amend Req buttons based on security item.
    '''     MikeVO      27/11/2006  Removed external factors section.
    '''                             Check user can view the subsidy.
    '''     MikeVO      24/11/2006  Links to View Service USer pass property ID.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewSubsidy
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Dim subsidyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim msg As ErrorMessage = New ErrorMessage
            Dim editMode As Boolean = Convert.ToBoolean(Target.Library.Utils.ToInt32(Request.QueryString("editMode")))
            Dim processedFieldCount As Integer = Target.Library.Utils.ToInt32(Request.QueryString("processedFieldCount"))
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim serviceID As Integer
            Dim canView As Boolean, canViewAmendReqs As Boolean

            Const SP_NAME As String = "spxSPSubsidy_Fetch"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSubsidyView"), "View Subsidy Details")

            strStyle.Append("label.label { float:left; width:19em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("span.label { float:left; width:9em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("ul.list { margin:0em;list-style:none; }")
            strStyle.Append(".content { float:left; width:60%; }")
            strStyle.Append(".content1 { float:left; width:60%; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            canViewAmendReqs = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"))

            ' setup edit buttons
            If canViewAmendReqs Then
                btnEdit.Visible = Not editMode
                btnCancel.Visible = editMode
                btnSubmit.Style.Add("float", "left")
                btnSubmit.Visible = editMode
                If processedFieldCount > 0 Then
                    lblAmendReq.Visible = True
                End If
            Else
                btnEdit.Visible = False
                btnCancel.Visible = False
                btnSubmit.Visible = False
            End If

            ' enable editable fields
            If Not IsPostBack Then
                If editMode Then
                    ' get settings
                    msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    ' enable editable fields
                    AmendReqBL.EnableEditableFields(Page.Controls, settings)
                End If
                Try
                    Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = subsidyID

                    ' execute
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    While reader.Read
                        serviceID = Target.Library.Utils.ToInt32(reader("ServiceID"))
                        lblProvider.Text = WebUtils.EncodeOutput(reader("ProviderReference")) & " - " & WebUtils.EncodeOutput(reader("ProviderName"))
                        lblService.Text = WebUtils.EncodeOutput(reader("ServiceReference")) & " - " & WebUtils.EncodeOutput(reader("ServiceName"))
                        lblPrimaryServiceUser.Text = WebUtils.EncodeOutput(reader("PrimaryServiceUserRef")) & " - " & WebUtils.EncodeOutput(reader("PrimaryServiceUserName"))
                        btnPrimaryClient.Attributes.Add("onclick", String.Format("document.location.href='../ViewServiceUser.aspx?id={0}&propertyID={1}';", reader("PrimaryClientID"), Target.Library.Utils.ToInt32(reader("PropertyID"))))
                        If Not IsDBNull(reader("SecondaryServiceUserRef")) Then
                            btnSecondaryClient.Attributes.Add("onclick", String.Format("document.location.href='../ViewServiceUser.aspx?id={0}&propertyID={1}';", reader("SecondaryClientID"), Target.Library.Utils.ToInt32(reader("PropertyID"))))
                            btnSecondaryClient.Disabled = False
                            lblSecondaryServiceUser.Text = WebUtils.EncodeOutput(reader("SecondaryServiceUserRef")) & " - " & WebUtils.EncodeOutput(reader("SecondaryServiceUserName"))
                        Else
                            btnSecondaryClient.Disabled = True
                        End If
                        lblDateFrom.Text = WebUtils.EncodeOutput(reader("DateFrom"))
                        If WebUtils.EncodeOutput(reader("DateTo")) = "31/12/2079" Then
                            lblDateTo.Text = "Open Ended"
                        Else
                            lblDateTo.Text = WebUtils.EncodeOutput(reader("DateTo"))
                        End If
                        lblEndReason.Text = WebUtils.EncodeOutput(reader("EndReason"))
                        lblReviewDate.Text = WebUtils.EncodeOutput(reader("ReviewDate"))
                        txtProviderRef.Text = WebUtils.EncodeOutput(reader("SAProviderReference"), txtProviderRef.IsReadOnly)
                        txtProviderRef.RecordID = Target.Library.Utils.ToInt32(reader("ServiceAgreementID"))
                        lblSubsidy.Text = String.Format("{0:C}", reader("Subsidy"))
                        lblVAT.Text = String.Format("{0:C}", reader("VAT"))
                        lblServiceUserContribution.Text = String.Format("{0:C}", reader("ClientContribution"))
                        lblLevel.Text = WebUtils.EncodeOutput(reader("THBServiceLevel"))
                        lblUnitCost.Text = String.Format("{0:C}", reader("UnitCost"))
                        Select Case Convert.ToInt32(reader("Status"))
                            Case 1
                                lblStatus.Text = "Active"
                            Case 2
                                lblStatus.Text = "Cancelled"
                            Case 4
                                lblStatus.Text = "Provisional"
                            Case 8
                                lblStatus.Text = "Documentary"
                            Case 16
                                lblStatus.Text = "Suspended"
                        End Select
                        lblHBReference.Text = WebUtils.EncodeOutput(reader("HBReference"))
                        lblHBAppliedOn.Text = WebUtils.EncodeOutput(reader("HBAppliedOn"))
                        lblHBStatus.Text = WebUtils.EncodeOutput(reader("HBStatus"))
                        lblHBStatusDate.Text = WebUtils.EncodeOutput(reader("HBAwardStatusDate"))
                        lblDPWaiver.Text = IIf(reader("HBDPWaiver"), "Yes", "No")
                        lblFCReference.Text = WebUtils.EncodeOutput(reader("FCReference"))
                        lblFCAppliedOn.Text = WebUtils.EncodeOutput(reader("FCAppliedOn"))
                        lblFCStatus.Text = WebUtils.EncodeOutput(reader("FCStatus"))
                        lblFCStatusDate.Text = WebUtils.EncodeOutput(reader("FCAssessmentStatusDate"))
                    End While

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "ViewSubsidy.Page_load")     ' error reading data
                    Target.Library.Web.Utils.DisplayError(msg)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
                End Try

                ' ensure user can view the service to which this subsidy relates
                ' should ideally do this further up but it achieves the same end...
                msg = SPClassesBL.UserCanViewService(Me.DbConnection, user.ExternalUserID, serviceID, canView)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If Not canView Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            End If
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

            Dim msg As ErrorMessage
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim processedFieldCount As Integer
            Dim subsidyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            If IsValid Then
                ' get the edtiable field settings
                msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' create amendment requests
                msg = AmendReqBL.ProcessEditableFields(Page.Controls, settings, Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), user.ExternalUsername, user.ID, processedFieldCount)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                Response.Redirect(String.Format("ViewSubsidy.aspx?id={0}&processedFieldCount={1}", subsidyID, processedFieldCount))
            End If

        End Sub

    End Class
End Namespace