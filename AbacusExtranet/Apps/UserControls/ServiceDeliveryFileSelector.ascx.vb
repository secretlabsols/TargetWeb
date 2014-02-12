Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.UserControls

    ''' <summary>
    ''' User control to allow the listing/selection of service delivery files.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   22/03/2011  D12083
    ''' ColinD  05/04/2010  D11755 - implemented ability to filter by failed uploads
    ''' MikeVO  15/10/2009  D11546 - fixes/chnages to the SubmittedBy filter. 
    ''' </history>
    Partial Class ServiceDeliveryFileSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                                ByVal submittedByUserID As Integer, _
                                ByVal dateFrom As Date, _
                                ByVal dateTo As Date, _
                                ByVal deleted As Boolean, _
                                ByVal processed As Boolean, _
                                ByVal awaitingProcessing As Boolean, _
                                ByVal workInProgress As Boolean, _
                                ByVal failed As Boolean, _
                                ByVal userHasServiceFileUploadCommand As Boolean _
                                )

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ServiceDeliveryFileSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.ServiceDeliveryFile))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.ServiceDeliveryFileSelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("currentPage={0};dateFrom={1};dateTo={2};deleted={3};processed={4};workInProgress={5};awaitingProcessing={6};submittedByUserID={7};failed={8};userHasServiceFileUploadCommand={9};Init();", _
                        currentPage, _
                        IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
                        IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
                        deleted.ToString.ToLower, _
                        processed.ToString.ToLower, _
                        workInProgress.ToString.ToLower, _
                        awaitingProcessing.ToString.ToLower, _
                        submittedByUserID, _
                        failed.ToString.ToLower, _
                        userHasServiceFileUploadCommand.ToString.ToLower _
                    ) _
                ) _
            )

        End Sub

    End Class

End Namespace