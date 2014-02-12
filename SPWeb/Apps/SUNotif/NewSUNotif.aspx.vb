
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.SP.Web.Apps.UserControls
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.SP.Library
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Security

Namespace Apps.SUNotif

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.SUNotif.NewSUNotif
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Wizard screen that allows a user to enter the detail of the new service user
    '''     that has been taken into a particular service.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      05/03/2007  Added Provider Ref, Service Level & Unit Cost.
    '''                             Made DoB and NINo optional.
    '''                             Allow user to navigate back to enter details step once notif has been created.
    ''' 	[Mikevo]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class NewSUNotif
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_SUNOTIFID As String = "suNotifID"
        Const STEP_INDEX_ENTER_DETAILS As Integer = 2
        Const STEP_INDEX_PRINT_DOC As Integer = 3
        Const STEP_INDEX_ATTACH_DOC As Integer = 4
        Const STEP_INDEX_COMPLETE As Integer = 5

        Private _enterDetailsStep As NewSUNotifEnterDetailsStep

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSUNotifsNew"), "New Service User Notification")

            Dim suNotifID As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_SUNOTIFID))
            Dim currentStep As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_CURRENTSTEP))
            Dim provStep As ProviderStep
            Dim svcStep As ServiceStep

            ' setup the steps
            provStep = New ProviderStep
            provStep.Required = True
            svcStep = New ServiceStep
            svcStep.Required = True
            _enterDetailsStep = New NewSUNotifEnterDetailsStep

            With SelectorWizard1
                ' add the steps
                .Steps.Add(provStep)
                .Steps.Add(svcStep)
                .Steps.Add(_enterDetailsStep)
                .Steps.Add(New NewSUNotifPrintDocStep)
                .Steps.Add(New NewSUNotifAttachDocStep)
                .Steps.Add(New NewSUNotifCompleteStep)
                ' set the display of the new button
                .NewButton.Value = "New Notification"
                .NewButton.Attributes("title") = "Click here to start a new notification."
                ' hide the back button & header controls if we are returning to a previously saved notif
                If suNotifID > 0 Then
                    .ShowHeaderControls = False
                    If currentStep = STEP_INDEX_COMPLETE Then .BackButton.Visible = False
                End If
                ' hide the finish button when not on the attach doc step
                If currentStep <> STEP_INDEX_ATTACH_DOC Then .FinishButton.Visible = False
                .InitControl()
            End With

            If currentStep = STEP_INDEX_ATTACH_DOC Then Me.Form.Enctype = "multipart/form-data"

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim suNotifID As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_SUNOTIFID))
            Dim currentStep As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_CURRENTSTEP))
            Dim notif As WebSPSUNotif
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean
            Dim enterDetailsControl As NewSUNotifEnterDetails = _enterDetailsStep.EnterDetailsControl

            If suNotifID > 0 Then
                ' check the notif exists
                notif = New WebSPSUNotif(Me.DbConnection)
                msg = notif.Fetch(suNotifID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' check the user can see the notif
                msg = SPClassesBL.UserCanViewSUNotif(Me.DbConnection, user.ExternalUserID, suNotifID, canView)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If Not canView Then
                    Response.Redirect("~/Library/Errors/AccessDenied.aspx")
                End If
                ' populate the screen
                If Not IsPostBack AndAlso currentStep = STEP_INDEX_ENTER_DETAILS Then
                    With enterDetailsControl
                        .ExpectedStartDate = notif.ServiceStartDate
                        .TenancySupportAgreement = notif.TenancyServiceAgreement
                        .YourReference = notif.YourReference
                        .ServiceLevel = notif.ServiceLevel
                        If notif.UnitCost > 0 Then .UnitCost = notif.UnitCost.ToString("#.##")
                        .PrimaryTitle = notif.PrimaryTitleID
                        .PrimaryFirstNames = notif.PrimaryFirstNames
                        .PrimarySurname = notif.PrimaryLastName
                        .PrimaryNINo = notif.PrimaryNINo
                        If Target.Library.Utils.IsDate(notif.PrimaryBirthDate) Then .PrimaryDoB = notif.PrimaryBirthDate
                        .Address = notif.Address
                        .Postcode = notif.Postcode
                        .SecondaryTitle = notif.SecondaryTitleID
                        .SecondaryFirstNames = notif.SecondaryFirstNames
                        .SecondarySurname = notif.SecondaryLastName
                        .SecondaryNINo = notif.SecondaryNINo
                        If Target.Library.Utils.IsDate(notif.SecondaryBirthDate) Then .PrimaryDoB = notif.SecondaryBirthDate
                    End With
                End If
            End If
            ' ensure we can't go back too far in the wizard if we do have a notif
            ' ensure we can't go forward too far in the wizard without a notif
            If (currentStep >= STEP_INDEX_PRINT_DOC AndAlso suNotifID <= 0) OrElse (currentStep < STEP_INDEX_ENTER_DETAILS AndAlso suNotifID > 0) Then
                Throw New InvalidOperationException("Invalid current step/SU notification ID combination.")
            End If

            ' upon submission
            If IsPostBack Then
                Validate()
                If IsValid Then
                    Select Case currentStep
                        Case STEP_INDEX_ENTER_DETAILS
                            ' save the new notification
                            msg = SaveNotif(Me.DbConnection, suNotifID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            MyRedirect(STEP_INDEX_PRINT_DOC, suNotifID)

                        Case STEP_INDEX_ATTACH_DOC
                            ' upload the scanned doc (if specified) and submit for processing
                            msg = UploadScannedDoc(Me.DbConnection, suNotifID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            MyRedirect(STEP_INDEX_COMPLETE, suNotifID)

                    End Select
                End If
            End If

        End Sub

        Private Sub MyRedirect(ByVal stepID As Integer, ByVal suNotifID As Integer)
            Dim newQS As NameValueCollection = New NameValueCollection(Request.QueryString)
            newQS.Remove(QS_CURRENTSTEP)
            newQS.Add(QS_CURRENTSTEP, stepID)
            newQS.Remove(QS_SUNOTIFID)
            newQS.Add(QS_SUNOTIFID, suNotifID)
            Response.Redirect(String.Format("NewSUNotif.aspx?{0}", WebUtils.GetNameValueCollectionAsString(newQS)))
        End Sub

        Private Function SaveNotif(ByVal conn As SqlConnection, ByRef suNotifID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim notif As WebSPSUNotif
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("providerID"))
            Dim serviceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("serviceID"))
            Dim enterDetailsControl As NewSUNotifEnterDetails = _enterDetailsStep.EnterDetailsControl

            notif = New WebSPSUNotif(conn)
            If suNotifID > 0 Then
                msg = notif.Fetch(suNotifID)
                If Not msg.Success Then Return msg
            End If
            With notif
                If suNotifID = 0 Then
                    .TypeID = SUNotifType.NewNotification
                    .StatusID = SUNotifStatus.Provisional
                    .CreatedDate = Now
                    .RequestedByUserID = user.ID
                    .SPProviderID = providerID
                    .SPServiceID = serviceID
                End If
                .ServiceStartDate = enterDetailsControl.ExpectedStartDate
                .TenancyServiceAgreement = enterDetailsControl.TenancySupportAgreement
                .PrimaryTitleID = enterDetailsControl.PrimaryTitle
                .PrimaryFirstNames = enterDetailsControl.PrimaryFirstNames
                .PrimaryLastName = enterDetailsControl.PrimarySurname
                .PrimaryNINo = enterDetailsControl.PrimaryNINo
                If Target.Library.Utils.IsDate(enterDetailsControl.PrimaryDoB) Then .PrimaryBirthDate = enterDetailsControl.PrimaryDoB
                .Address = enterDetailsControl.Address
                .Postcode = enterDetailsControl.Postcode
                .SecondaryTitleID = enterDetailsControl.SecondaryTitle
                .SecondaryFirstNames = enterDetailsControl.SecondaryFirstNames
                .SecondaryLastName = enterDetailsControl.SecondarySurname
                .SecondaryNINo = enterDetailsControl.SecondaryNINo
                If Target.Library.Utils.IsDate(enterDetailsControl.SecondaryDoB) Then .SecondaryBirthDate = enterDetailsControl.SecondaryDoB
                If enterDetailsControl.YourReference.Length > 0 Then .YourReference = enterDetailsControl.YourReference
                If enterDetailsControl.ServiceLevel.Length > 0 Then .ServiceLevel = enterDetailsControl.ServiceLevel
                If enterDetailsControl.UnitCost.Length > 0 Then .UnitCost = Convert.ToDecimal(enterDetailsControl.UnitCost)
                msg = .Save()
                If Not msg.Success Then Return msg
                suNotifID = .ID
            End With

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

        Private Function UploadScannedDoc(ByVal conn As SqlConnection, ByVal suNotifID As Integer) As ErrorMessage

            Dim msg As ErrorMessage = Nothing
            Dim scannedDocIDs As Integer()
            Dim newDataFileID As Integer
            Dim notif As WebSPSUNotif
            Dim trans As SqlTransaction = Nothing

            Try
                trans = SqlHelper.GetTransaction(conn)

                ' get the uploaded doc ID
                scannedDocIDs = FileStoreBL.GetUploadedTempFileIDs()

                ' move the uploaded doc to proper storage
                If scannedDocIDs.Length > 0 Then
                    msg = FileStoreBL.MoveFromTempStoreToDataStore(trans, scannedDocIDs(0), newDataFileID)
                    If Not msg.Success Then Return msg
                End If

                ' update the notif
                notif = New WebSPSUNotif(trans)
                msg = notif.Fetch(suNotifID)
                If Not msg.Success Then Return msg
                With notif
                    .StatusID = SUNotifStatus.Submitted
                    .SubmittedDate = Now
                    If scannedDocIDs.Length > 0 Then .WebFileStoreDataID = newDataFileID
                    msg = .Save
                    If Not msg.Success Then Return msg
                End With

                ' commit
                trans.Commit()

                msg = New ErrorMessage
                msg.Success = True

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0502", "WebSPSUNotif")   ' error whilst saving data
            Finally
                If Not msg.Success AndAlso Not trans Is Nothing AndAlso Not trans.Connection Is Nothing Then trans.Rollback()
            End Try

            Return msg

        End Function

    End Class

End Namespace

