Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Partial Class FinancialYrQtrSelector
    Inherits System.Web.UI.UserControl

#Region " Properties "

    Public Property FinancialYear() As String
        Get
            Return cboFinancialYear.DropDownList.SelectedItem.Text
        End Get
        Set(ByVal value As String)
'            cboFinancialYear.DropDownList.SelectedItem.Text = value
            WebUtils.SetDropdownListValue(cboFinancialYear.DropDownList, value)
        End Set

    End Property

    Public Property Quarter() As String
        Get
            Return cboQuarter.DropDownList.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            'cboQuarter.DropDownList.SelectedItem.Text = value
            WebUtils.SetDropdownListValue(cboQuarter.DropDownList, value)
        End Set
    End Property

    Public Property Caption() As String
        Get
            Return litCaption.Text
        End Get
        Set(ByVal value As String)
            'cboQuarter.DropDownList.SelectedItem.Text = value
            litCaption.Text = value
        End Set
    End Property

#End Region
    Public Sub InitControl(ByVal thePage As BasePage)

            Const SP_FINANCIAL_YEARS_FETCH As String = "pr_FetchSPFinancialYearList"

            Dim msg As ErrorMessage
            Dim dt As DataTable

            Me.Caption = "Financial Year/Quarter Selector"

            ' grab the list of titles
            Try
                dt = SqlHelper.ExecuteDataset(thePage.DbConnection, CommandType.StoredProcedure, SP_FINANCIAL_YEARS_FETCH).Tables(0)

                With cboFinancialYear.DropDownList
                    .DataSource = dt
                    .DataTextField = "FinancialYear"
                    .DataBind()
                    '.Items.Insert(0, New ListItem("", ""))
                End With

                With cboQuarter.DropDownList
                    '.Items.Insert(0, New ListItem("", ""))
                    .Items.Insert(0, New ListItem("1", "1"))
                    .Items.Insert(1, New ListItem("2", "2"))
                    .Items.Insert(2, New ListItem("3", "3"))
                    .Items.Insert(3, New ListItem("4", "4"))
                End With

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_FINANCIAL_YEARS_FETCH, "FinancialYrQtrSelector.InitControl()")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            End Try

        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            lblFinancialYear.AssociatedControlID = cboFinancialYear.ID
            lblQuarter.AssociatedControlID = cboQuarter.ID

        End Sub

End Class
