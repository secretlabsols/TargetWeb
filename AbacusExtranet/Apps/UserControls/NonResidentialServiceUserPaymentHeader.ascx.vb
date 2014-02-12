Imports Target.Web.Apps
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.SDS
Imports System.Data.SqlClient

Namespace Apps.UserControls

    ''' <summary>
    ''' Control to display header information about non-residential service user payment.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' BCW 25/10/2013
    ''' </history>
    Partial Public Class NonResidentialServiceUserPaymentHeader
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, ByVal conn As SqlConnection, ByVal selectedClientID As Integer)

            Dim msg As ErrorMessage
            Dim cd As ClientDetail = Nothing
            msg = ServiceUserBL.GetServiceUser(conn, cd, selectedClientID)

            If msg.Success Then
                txtReference.Text = cd.Reference
                txtName.Text = cd.Name
                If Utils.IsDate(cd.BirthDate) Then txtDateOfBirth.Text = cd.BirthDate
                If Utils.IsDate(cd.DeathDate) Then txtDateOfDeath.Text = cd.DeathDate

                ' store MRU client
                Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                mruManager("SERVICE_USERS")(selectedClientID.ToString()) = String.Format("{0}: {1}", cd.Reference, cd.Name)
                mruManager.Save(HttpContext.Current)

            End If

        End Sub

    End Class
End Namespace
