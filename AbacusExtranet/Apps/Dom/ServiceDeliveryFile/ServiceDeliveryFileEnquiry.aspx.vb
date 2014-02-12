Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Constants = Target.Library.Web




Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Screen to allow a user to search for a view uploaded service delivery files.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ServiceDeliveryFileEnquiry
        Inherits Target.Web.Apps.BasePage

#Region " Fields "
        Private Const _WebCmdUploadServiceFileKey As String = "AbacusExtranet.WebNavMenuItemCommand.UploadServiceDeliveryFile"
#End Region

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can add new records.
        ''' </summary>
        ''' <value><c>true</c> if user can upload Service delivery file , otherwise <c>false</c>.</value>
        Private ReadOnly Property UserHasUploadFileCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdUploadServiceFileKey))
            End Get
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceDeliveryFileEnquiry"), "Service Delivery Files")

            IntialiseJsVariables()


            Dim filterStep As ServiceDeliveryFileFilterStep = New ServiceDeliveryFileFilterStep
            Dim resultStep As ServiceDeliveryFileEnquiryResultsStep = New ServiceDeliveryFileEnquiryResultsStep
            Dim fileupload As Boolean = UserHasUploadFileCommand
            resultStep.UserHasUploadServiceFileCommand = fileupload

            With SelectorWizard1
                .BackButton.Style.Add("width", "7em;")
                .FinishButton.Style.Add("width", "7em")
                .NewButton.Style.Add("width", "7em")
                .Steps.Add(filterStep)
                .Steps.Add(resultStep)
                .InitControl()
            End With
        End Sub


        Private Sub IntialiseJsVariables()


            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

         
        End Sub

    End Class

End Namespace