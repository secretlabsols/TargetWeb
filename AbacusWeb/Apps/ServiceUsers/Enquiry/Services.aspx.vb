Imports Target.Web.Apps
Imports Target.Library

Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    ''' Screen used to maintain service user services details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir 24/10/2012  D12399 Copy Function For Direct Payment Contracts.
    '''     Waqas         14/02/2011  D12009 - Passes default value for SDS filter 
    '''     ColinD        22/12/2010  D11852C - Added Spend Plans control, originally located in Finance tab
    '''     MikeVO        02/12/2010  SDS issues #267/268
    '''     Mo Tahir      27/08/2010  D11814 - Service User Enquiry.
    ''' </history>
    Partial Public Class Services
        Inherits BasePage
        Private _clientID As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Me.UseJQuery = True
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            'get the controls
            spendPlans1 = cpSpendPlans.FindControl("spendPlans1")
            serviceOrderSelector1 = cpCommissionedService.FindControl("serviceOrderSelector1")
            dpConractSelector1 = cpDirectPaymentContracts.FindControl("dpConractSelector1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientid")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientid"))
            End If

            With CType(serviceOrderSelector1, Target.Abacus.Web.Apps.UserControls.ServiceOrderSelector)
                .InitControl(Me.Page, Nothing, Nothing, Nothing, Nothing, _clientID, Nothing, Nothing, Nothing, True, True, True, True)
            End With


            With CType(dpConractSelector1, Target.Abacus.Web.Apps.UserControls.DPContractSelector)
                .InitControl(Me.Page, _clientID, Nothing, Nothing, Nothing, True, True, False, False, Nothing, True, True, True, TriState.UseDefault)
            End With

            With CType(spendPlans1, Target.Abacus.Web.Apps.UserControls.SpendPlanSelector)
                .InitControl(Me.Page, Nothing, _clientID, Nothing, Nothing, True, False, True)
            End With

        End Sub

    End Class
End Namespace
