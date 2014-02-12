Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Security
Imports Constants = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Extranet.Apps.UserControls
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps

Namespace Apps.Dom.ReferenceData

    Partial Public Class DurationClaimedRounding
        Inherits Target.Web.Apps.BasePage

#Region " Fields "
        Private Const _WebCmdAddNewKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.AddNew"
        Private Const _WebCmdEditKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.Edit"
        Private Const _WebCmdDeleteKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.Delete"
#End Region

#Region " Events "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ReferenceData.DurationClaimRounding"), "Duration Claimed Rounding")
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser

            CType(durationClaimedRounding,  _
            DurationClaimedRoundingSelector).InitControl(Me.Page, _
                                                        user.ID, _
                                                        SecurityBL.IsCouncilUser(Me.DbConnection, Me.Settings, user.ExternalUserID), _
                                                        UserHasAddNewCommand)

        End Sub

#End Region

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can add new records.
        ''' </summary>
        ''' <value><c>true</c> if user can add new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasAddNewCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdAddNewKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can delete new records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasDeleteCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdDeleteKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can edit records.
        ''' </summary>
        ''' <value><c>true</c> if user can edit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdEditKey))
            End Get
        End Property

#End Region

    End Class
End Namespace