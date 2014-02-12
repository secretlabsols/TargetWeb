Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.SvcOrder

    ''' <summary>
    ''' Screen used to display Service Order Details on the funding screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Paul  17/02/2009  D11491 - Initial Version.
    ''' </history>
    Partial Public Class svcOrderHeader
        Inherits BasePage

        Private _svcOrderID As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), "Service Order Funding")
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim svcOrder As DomServiceOrder = New DomServiceOrder(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            Dim contract As DomContract = New DomContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            Dim provider As Establishment = New Establishment(Me.DbConnection)
            Dim svcUser As ClientDetail = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim style As New StringBuilder

            _svcOrderID = Utils.ToInt32(Request.QueryString("id"))

            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(style.ToString)

            msg = svcOrder.Fetch(_svcOrderID)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
            Else
                lblOrderRef.Text = svcOrder.OrderReference
                If svcOrder.DateTo = DataUtils.MAX_DATE Then
                    lblPeriod.Text = String.Format("{0} (open-ended)", svcOrder.DateFrom.ToString("dd MMM yyyy"))
                Else
                    lblPeriod.Text = String.Format("{0} to {1}", svcOrder.DateFrom.ToString("dd MMM yyyy"), svcOrder.DateTo.ToString("dd MMM yyyy"))
                End If
                'Contract
                msg = contract.Fetch(svcOrder.DomContractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                lblContract.Text = String.Format("{0} : {1}", contract.Number, contract.Title)
                'Provider
                msg = provider.Fetch(svcOrder.ProviderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                lblProvider.Text = String.Format("{0} : {1}", provider.AltReference, provider.Name)
                'Service User
                If svcOrder.ClientID > 0 Then
                    msg = svcUser.Fetch(svcOrder.ClientID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblSvcUser.Text = String.Format("{0} : {1}", svcUser.Reference, svcUser.Name)
                End If

            End If

        End Sub

    End Class

End Namespace