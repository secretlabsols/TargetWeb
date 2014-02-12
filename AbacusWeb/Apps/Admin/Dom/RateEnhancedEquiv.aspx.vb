'Imports System.ComponentModel
Imports System.Collections.Generic
'Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
'Imports Target.Library.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    Partial Public Class RateEnhancedEquiv
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"

        Private _stdBut As StdButtonsBase
        Private _frameworkID As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateFrameworks"), "Enhanced Rate Category Equivalents")
            Dim msg As ErrorMessage
            Dim inUse As Boolean = False

            _frameworkID = Utils.ToInt32(Request.QueryString("ID"))
            msg = frameworkInUse(_frameworkID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            With _stdBut
                .AllowNew = False
                .AllowEdit = (Not inUse And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateFrameworks.Edit")))
                .AllowDelete = False
                .AllowFind = False
                .AllowBack = True
                .EditableControls.Add(fsControls.Controls)
                '.AuditLogTableNames.Add("DomRateFramework")
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf FindClicked
            End With

            ' re-create the list of details (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                Dim rc As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
                msg = rc.Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                OutputDetailControls(rc)
            Next


        End Sub

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                Dim rc As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
                msg = rc.Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                rc.EnhancedRateCategoryID = CType(phDetails.FindControl(String.Format("cbo{0}", id)), DropDownListEx).GetPostBackValue()
                msg = rc.Save()
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Next

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim list As List(Of String)
            Dim standardRateCategories As DomRateCategoryCollection = Nothing

            ' refresh the list of existing details and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()

            msg = GetStandardRateCategories(_frameworkID, standardRateCategories)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each rc As DomRateCategory In standardRateCategories
                If Not list.Contains(rc.ID) Then
                    list.Add(rc.ID)
                    OutputDetailControls(rc)
                End If
            Next

            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputDetailControls "

        Private Sub OutputDetailControls(ByVal rateCat As DomRateCategory)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim enhanced As DropDownListEx

            row = New HtmlTableRow()
            phDetails.Controls.Add(row)

            ' standard
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerText = rateCat.Description

            ' enhanced
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            enhanced = New DropDownListEx()
            With enhanced
                .ID = String.Format("cbo{0}", rateCat.ID)
                .Required = True
                .RequiredValidatorErrMsg = "* Required"
                .ValidationGroup = "Save"
                .Width = New Unit(15, UnitType.Em)
                PopulateEnhancedDropdown(enhanced, _frameworkID)
                If rateCat.EnhancedRateCategoryID > 0 Then .DropDownList.SelectedValue = rateCat.EnhancedRateCategoryID
            End With
            cell.Controls.Add(enhanced)
            'End If

        End Sub

#End Region

#Region " frameworkInUse "

        ''' <summary>
        ''' Determines if the specified prefix is already in use by one or more framework.
        ''' </summary>
        ''' <param name="frameworkID">The ID of the framework to check.</param>
        ''' <param name="inUse">determines if the framework is in use on an order</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function frameworkInUse(ByVal frameworkID As Integer, _
                                            ByRef inUse As Boolean) As ErrorMessage

            Const SP_NAME As String = "spxDomRateFramework_InUseByDomServiceOrder"

            Dim msg As ErrorMessage = Nothing
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = frameworkID
                spParams(1).Direction = ParameterDirection.InputOutput

                SqlHelper.ExecuteNonQuery(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                inUse = Convert.ToBoolean(spParams(1).Value)

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region


#Region " GetStandardRateCategories "

        ''' <summary>
        ''' Determines if the specified prefix is already in use by one or more framework.
        ''' </summary>
        ''' <param name="frameworkID">The ID of the framework to check.</param>
        ''' <param name="standardRateColl">A collection of Standard Rate categories for the framework</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetStandardRateCategories(ByVal frameworkID As Integer, _
                                            ByRef standardRateColl As DomRateCategoryCollection) As ErrorMessage

            Const SP_NAME As String = "spxDomRateCategory_GetStandardRatesForFramework"

            Dim msg As ErrorMessage = Nothing
            Dim spParams As SqlParameter()
            Dim catCollection As DomRateCategoryCollection = New DomRateCategoryCollection
            Dim standardCategories As DataTable

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = frameworkID

                standardCategories = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)

                For Each category As DataRow In standardCategories.Rows
                    Dim rateCat As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
                    msg = rateCat.Fetch(category("ID"))
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    catCollection.Add(rateCat)
                Next

                standardRateColl = catCollection

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " PopulateEnhancedDropdown "

        Protected Function PopulateEnhancedDropdown(ByVal combo As DropDownListEx, ByVal frameworkID As Integer) As ErrorMessage

            Const SP_NAME As String = "spxDomRateCategory_GetEnhancedRatesForFramework"

            Dim msg As ErrorMessage = Nothing
            Dim spParams As SqlParameter()
            Dim standardCategories As DataTable

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = frameworkID

                standardCategories = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)

                combo.DropDownList.Items.Clear()
                For Each category As DataRow In standardCategories.Rows
                    combo.DropDownList.Items.Add(New ListItem(category("Description"), category("ID")))
                Next
                combo.DropDownList.Items.Insert(0, String.Empty)

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phDetails.Controls.Clear()
        End Sub

#End Region

#Region " GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of String))
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
        End Sub

#End Region

    End Class

End Namespace