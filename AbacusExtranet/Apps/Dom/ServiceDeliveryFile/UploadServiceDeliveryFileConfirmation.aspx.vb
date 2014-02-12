Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Screen to ask the user for confirmation that the existing file should be overwritten.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   22/03/2011  D12083
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  24/11/2008  A4WA#5033 - security fixes.
    ''' </history>
    Partial Class UploadServiceDeliveryFileConfirmation
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.JsLinks.Add("UploadServiceDeliveryFileConfirmation.js")
            Dim interfaceFileID As Integer = Utils.ToInt32(Session("DomProviderInterfaceFileID"))
            Dim validationCondition As String = Request.QueryString("valcon")
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceDeliveryFileEnquiry"), "Upload Service Delivery File Confirmation")
            Dim interfaceFile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(Me.DbConnection)
            Dim msg As ErrorMessage
            Dim strStyle As New StringBuilder
            Const SP_NAME_CONFIRMATION As String = "spxServiceDeliveryFileUploadConfirmation_Fetch"
            Dim reader As SqlDataReader = Nothing

            strStyle.Append("label.label, label.label2 { float:left; font-weight:bold; }")
            strStyle.Append("label.label2 { width:12em }")
            Me.AddExtraCssStyle(strStyle.ToString)
            'fetch interfacefile details
            msg = interfaceFile.Fetch(interfaceFileID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            'output file details to the page
            lblFileRef.Text = interfaceFile.Reference
            lblFileDesc.Text = interfaceFile.Description
            lblFileCreatedDate.Text = WebUtils.EncodeOutput(interfaceFile.FileCreatedDate)

            'Populate list of contracts included in file
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_CONFIRMATION, False)
                spParams(0).Value = interfaceFileID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_CONFIRMATION, spParams)
                rptProviderContracts.DataSource = reader
                rptProviderContracts.DataBind()
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
            End Try

            'Decide to show confirmation panel or not!
            'onclick=";"
            chkConfirmation.CheckBox.Attributes.Add("onclick", _
                                                    String.Format("return onChanged(""{0}_{1}"",""{2}"")", _
                                                                  chkConfirmation.ClientID, _
                                                                  chkConfirmation.CheckBox.ClientID, _
                                                                  btnNext.ClientID))
            If validationCondition = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET Or validationCondition = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET Then
                grpConfirmation.Visible = True
                btnNext.Disabled = True
                lblConfirmationText.Text = String.Format("A file with the reference ({0}) has already been uploaded. {1}{2}{3} {4}", _
                                                         interfaceFile.Reference, _
                                                         "As none of the pro forma invoices associated with this file have been ", _
                                                         "verified the previously submitted file and its contents will be deleted ", _
                                                         "prior to uploading the current file. <br /><br />", _
                                                         "Please confirm that you understand and accept this action.")
            Else
                grpConfirmation.Visible = False
            End If

        End Sub

        Private Sub btnNext_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.ServerClick
            Dim interfaceFileID As Integer = Utils.ToInt32(Session("DomProviderInterfaceFileID"))
            Dim validationCondition As String = Request.QueryString("valcon")
            Dim queryString As String = ""

            If chkConfirmation.CheckBox.Checked <> True And grpConfirmation.Visible Then
                lblError.Text = "Confirmation is needed before you can continue."
            Else
                queryString = String.Format("?valcon={0}", validationCondition)
                Response.Redirect(String.Format("{0}{1}", "UploadServiceDeliveryFileCompletion.aspx", queryString))
            End If
        End Sub
    End Class
End Namespace