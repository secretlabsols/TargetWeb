Imports System.Collections.Generic
Imports Target.Web.Apps
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls

Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    ''' Screen used to maintain a service user addresses.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO        28/04/2011  SDS issue #613 - correct issue with collapsible panel.
    '''     MikeVO        30/11/2010  SDS issue #316
    '''     MikeVO        05/10/2010  UI tidy up.
    '''     Mo Tahir      27/08/2010  D11814 - Service User Enquiry.
    ''' </history>
    Partial Public Class Addresses
        Inherits BasePage
        Const DEFAULT_IFRAME_HEIGHT As Integer = 330

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Me.UseJQuery = True
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID
            If Utils.ToInt32(Request.QueryString("clientID")) > 0 Then
                OutputAddressControls(Utils.ToInt32(Request.QueryString("clientID")))
            End If
        End Sub

#End Region

#Region " OutputAddressControls "

        Private Sub OutputAddressControls(ByVal clientId As Integer)

            Dim addressTypes As List(Of Lookup) = Nothing
            Dim msg As ErrorMessage
            Dim cPanel As CollapsiblePanel
            Dim addresses As ClientAddressCollection = Nothing
            Dim addressesArray As ClientAddress()
            Dim expanded As Boolean
            Dim currentAddress As ClientAddress
            Dim currentAddressTypeID As Integer

            msg = ServiceUserBL.GetServiceUserAddressTypes(Me.DbConnection, addressTypes)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = ServiceUserBL.GetServiceUserAddresses(Me.DbConnection, clientId, addresses)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            addressesArray = addresses.ToArray()

            For Each addType As Lookup In addressTypes
                cPanel = New CollapsiblePanel
                With cPanel
                    .ID = EncodeAddressDescForControlID(addType.Description, addType.ID)
                    phAddresses.Controls.Add(cPanel)
                    .EnsureChildControls()
                    .HeaderLinkText = addType.Description
                    .ExpandedJS = "parent.resizeIframe(document.body.scrollHeight, 'ifrAddresses');"
                    .CollapsedJS = "parent.resizeIframe(document.body.scrollHeight, 'ifrAddresses');"
                End With

                ' see if we have this address
                currentAddressTypeID = addType.ID
                currentAddress = (From add In addressesArray Where add.TypeID = currentAddressTypeID).FirstOrDefault()

                With cPanel.ContentPanel
                    If Not currentAddress Is Nothing Then
                        .Controls.Add(CreateTextBoxControls("Title & Initials", "Title", currentAddress.Title, currentAddress.ID))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Surname", "Surname", currentAddress.Surname, currentAddress.ID))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Address", "Address", currentAddress.Address, currentAddress.ID, 5))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Postcode", "PostCode", currentAddress.Postcode, currentAddress.ID))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Phone", "Phone", currentAddress.Phone, currentAddress.ID))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Relation", "Relation", currentAddress.Relation, currentAddress.ID))
                        .Controls.Add(CreateBr())
                        .Controls.Add(CreateTextBoxControls("Comments", "Comments", currentAddress.Comment, currentAddress.ID, 5))
                        .Controls.Add(CreateBr())
                        If Not expanded Then
                            cPanel.Expanded = Not expanded
                            expanded = True
                        End If
                    Else
                        cPanel.HeaderLinkTextCSS = "hlnkEmpty"
                    End If
                End With

            Next

        End Sub

#End Region

#Region " CreateTextBoxControls "

        Private Function CreateTextBoxControls(ByVal label As String, _
                                               ByVal controlPrefix As String, _
                                               ByVal value As String, _
                                               ByVal id As Integer, _
                                               Optional ByVal numberOfLines As Integer = 0) As TextBoxEx

            Dim txtBoxControl As TextBoxEx
            txtBoxControl = New TextBoxEx()

            With txtBoxControl
                .LabelText = label
                .LabelWidth = "8em"
                .ID = controlPrefix & "_" & id.ToString()
                .Text = value
                .TextBox.ReadOnly = True
                If numberOfLines > 0 Then
                    .TextBox.TextMode = TextBoxMode.MultiLine
                    .TextBox.Rows = numberOfLines
                    .TextBox.Columns = 40
                End If
            End With

            Return txtBoxControl

        End Function

#End Region

        Private Function CreateBr() As Literal

            Dim literal1 As Literal

            literal1 = New Literal()
            literal1.Text = "<br>"

            Return literal1

        End Function

#Region " EncodeAddressDescForControlID "

        Private Function EncodeAddressDescForControlID(ByVal addressDesc As String, ByVal addressID As Integer) As String
            Dim result As String
            result = addressDesc.Trim()
            result = result.Replace(" ", String.Empty)
            result = result.Replace("-", "_")
            result = result.Replace(".", "")
            result = result & "_" & addressID
            Return result
        End Function

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Const SCRIPT_STARTUP As String = "Startup"

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("addEvent(window, ""load"", function() {{ parent.resizeIframe({0},'{1}'); }});", DEFAULT_IFRAME_HEIGHT, "ifrAddresses"), True)
            End If
        End Sub

#End Region

    End Class
End Namespace
