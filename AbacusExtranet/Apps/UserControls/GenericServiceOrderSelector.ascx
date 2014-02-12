<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GenericServiceOrderSelector.ascx.vb"
    Inherits="Target.Abacus.Extranet.Apps.UserControls.GenericServiceOrderSelector"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc10" TagName="DSOBasics" Src="~/Library/UserControls/DSOBasicDetails.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<div style="height: 10px;">
</div>
<legend id="fsControlsLegend" runat="server" />
<label id="lblFilterCriteria" class="errorText DialogTableFiltering">
</label>
<table border="0" id="tblDSOs" class="listTable tablescroll" cellspacing="0" cellpadding="2"
    summary="List of Service Orders" width="98%">
    <thead>
        <tr>
            <th filtertabletype="TextBox" style="width: 10%">
                S/U Ref
            </th>
            <th filtertabletype="TextBox" style="width: 57%">
                Service User
            </th>
            <th filtertabletype="TextBox" style="width: 10%">
                Order Ref
            </th>
            <th filtertabletype="TextBox" style="width: 10%">
                Date From
            </th>
            <th filtertabletype="TextBox" style="width: 10%">
                Date To
            </th>
            <th filtertabletype="Custom" style="width: 3%">
                &nbsp;
            </th>
        </tr>
    </thead>
    <tbody id="tblBody">
        <%--Table rows are added Dynamically--%>
    </tbody>
</table>
<br />
<%--The markup below is used in the Dso Dialogue--%>
<div id="dso_dialog" style="display: none;">
    <uc10:DSOBasics id="DSOAdditionalDetails" runat="server">
    </uc10:DSOBasics>
    <div id="tabs">
        <ul>
            <li><a href="#tab-Order">Order</a></li>
            <li><a href="#tab-Details">Details</a></li>
            <li><a href="#tab-Suspensions">Suspensions</a></li>
            <li><a href="#tab-Costs">Costs</a></li>
            <li><a href="#tab-Documents">Documents</a></li>
        </ul>
        <div id="tab-Order" style="height: 140px;">
        </div>
        <div id="tab-Details">
            <div id="tab-Details-Content">
            </div>
            <div style="margin-top: 20px; margin-bottom: 2px;">
                <cc1:TextBoxEx ID="detailWeekEndingDate" runat="server" LabelText="Care Plan in effect on"
                    Format="DateFormatJquery" LabelWidth="13em" EnableViewState="true" Width="7em"
                    AllowClear="false">
                </cc1:TextBoxEx>
            </div>
        </div>
        <div id="tab-Suspensions">
            <table id="tblDSOSuspension" class="listTable " cellspacing="0" cellpadding="2" summary="Service Order Suspentions"
                width="99%">
                <thead>
                    <tr>
                        <th style="width: 15%">
                            Date From
                        </th>
                        <th style="width: 15%">
                            Date To
                        </th>
                        <th style="width: 70%">
                            Reason
                        </th>
                    </tr>
                </thead>
                <tbody id="tblDSOSuspensionBody">
                    <%--Table rows are added Dynamically--%>
                </tbody>
            </table>
        </div>
        <div id="tab-Costs">
            <table id="tblDSOCosts" class="listTable " cellspacing="0" cellpadding="2" summary="Service Order Costs"
                width="99%">
                <thead>
                    <tr>
                        <th style="width: 50%">
                            Rate Category
                        </th>
                        <th style="width: 10%">
                            Units
                        </th>
                        <th style="width: 20%">
                            Measured In
                        </th>
                        <th style="width: 10%">
                            Unit Cost
                        </th>
                        <th style="width: 10%">
                            Total Cost
                        </th>
                    </tr>
                </thead>
                <tbody id="tblDSOCostBody">
                    <%--Table rows are added Dynamically--%>
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="4" style="text-align: right; font-weight: bold; border-bottom: none;">
                            Overall Cost
                        </td>
                        <td style="text-align: left; font-weight: bold; border-bottom: none;">
                            <span id="lblOverallCostValue" style="width: 40%;"></span>
                        </td>
                    </tr>
                </tfoot>
            </table>
            <div style="float: left;">
                <%-- <span id="lblRatesInfo"></span>--%>
                <div style="padding-bottom: 10px;">
                    <cc1:TextBoxEx ID="costWeekEndingDate" runat="server" LabelText="Cost as at " Format="DateFormatJquery"
                        LabelWidth="7em" EnableViewState="true" Width="7em" AllowClear="false">
                    </cc1:TextBoxEx>
                </div>
            </div>
            <br />
        </div>
        <div id="tab-Documents" >
            <table id="tblDSODocuments" class="listTable " cellspacing="0" cellpadding="2" summary="Service Order Documents"
                width="99%">
                <thead>
                    <tr>
                        <th style="width: 33%">
                            Type
                        </th>
                        <th style="width: 33%">
                            Description
                        </th>
                        <th style="width: 33%">
                            Filename
                        </th>
                    </tr>
                </thead>
                <tbody id="tblDSODocumentBody">
                    <%--Table rows are added Dynamically--%>
                </tbody>
            </table>
            <div>
                <div id="Document_PagingLinks" style="float: left;">
                </div>
                <div style="float: right;">
                    <input type="button" id="btnAdd" style="width: 7em;" value="Add" onclick="GenericServiceOrderSelector_btnAdd_Click();"
                        runat="server" />
                </div>
                <div class="clearer">
                </div>
            </div>
        </div>
    </div>
    <script id="orderTemplate" type="text/html">  
	        <div style="float:left; width:50%;">                  
	            <div style="padding-bottom:0.5em;">
	            <label style="font-weight:bold;" for="lblProvider" style="width:10em;">Provider</label>    
	            <label id="lblProvider" class="content">${ProviderView}</label> 
	            </div>
	            <div style="padding-bottom:0.5em;textOverflow:ellipsis;overflow:hidden;">
	            <label style="font-weight:bold;" for="lblContract" style="width:10em;">Contract</label>    
	            <label id="lblContract" class="content">${ContractView}</label>
	            </div>
	            <div style="padding-bottom:0.5em;"> 
	            <label style="font-weight:bold;" for="lblOrderRef" style="width:10em;">Order Reference</label>    
	            <label id="lblOrderRef" class="content">${OrderRef}</label>  
	            </div>
	            <div style="padding-bottom:0.5em;">  
	            <label style="font-weight:bold;" for="lblDateFrom" style="width:10em;">Date From</label>    
	            <label id="lblDateFrom" class="content">${formatDate(DateFrom)}</label>  
	            </div>
	            <div style="padding-bottom:0.5em;">  
	            <label style="font-weight:bold;" for="lblDateTo" style="width:10em;">Date To</label>    
	            <label id="lblDateTo" class="content">${formatDate(DateTo)}</label> 
	            </div>
	            <div style="padding-bottom:0.5em;">  
	            <label style="font-weight:bold;" for="lblEndReason" style="width:10em;">End Reason</label>    
	            <label id="lblEndReason" class="content">${EndReason}</label> 
	            </div>
	            <div style="padding-bottom:0.5em;"> 
	            <label style="font-weight:bold;" for="lblTeam" style="width:10em;">Team</label>    
	            <label id="lblTeam" class="content" >${TeamView}</label> 
	            
	            </div> 
	        </div> 
	        <div style="float:left; width:50%;">
	            <div style="padding-bottom:0.5em;">
	            <label style="font-weight:bold;" for="lblServiceUser" style="width:9.8em;">Service User</label>    
	            <label id="lblServiceUser" class="content">${ServiceUserView}</label> 
	            </div>
	            <div style="padding-bottom:0.5em;">  
	                <label style="font-weight:bold;float:left;" for="lblHomeAddress" style="width:10em;">Home Address</label>    
	                <label id="lblHomeAddress" style="float:left;" class="content">{{html HomeAddress }}</label>
	            </div>
	        </div> 
	            
    </script>
    <script id="OrderDetailTemplate" type="text/html">
        <table id="tblDSODetail" class="listTable " cellspacing="0" cellpadding="2" summary="Service Order Details" width="99%">
                <thead>
                    <tr>
                       <th style="width: 15%">Line From</th>
                       <th style="width: 15%">Line To</th>
                       {{if ShowDayOfWeek}}
                            <th style="width: 14%">Day of Week</th>
                       {{/if}}
                       <th style="width: 20%">Rate Category</th>
                       <th style="width: 10%">Units</th>
                       <th style="width: 14%">Measured In</th>
                       {{if TimeBased}}
                            <th style="width: 10%">Visits</th>
                       {{/if}}
                       <th style="width: 10%">Frequency</th>
                      
                    </tr>
                </thead>
                <tbody id="tblDSODetailBody">
                    <%--Table rows are added Dynamically--%>
                </tbody>
            </table>
    </script>
    <script id="OrderDetailTemplateBody" type="text/html">
	       <tr>
                <td>${formatDate(DateFrom)}</td>
                <td>${formatDate(DateTo)}</td>
	            {{if ShowDayOfWeek}}
                    <td >${DayOfWeek}&nbsp;</td>
                {{/if}}
	            <td>${RateCategory}</td>
                <td>${UnitsView}</td>
                <td>${MeasuredIn}</td>
                {{if TimeBased}}
                    <td>${Visits}</td>
                {{/if}}
                <td>${Frequency}</td>
                
	        </tr>
    </script>
    <script id="OrderDetailSvcRegistersTemplate" type="text/html">
	        <table id="tblDSODetail" class="listTable " cellspacing="0" cellpadding="2" summary="Service Order Details" width="99%">
                <thead>
                    <tr>
                       <th style="width: 15%">Line From</th>
                       <th style="width: 15%">Line To</th>
                       <th style="width: 30%">Rate Category</th>
                       {{if ShowDayOfWeek}}
                            <th style="width: 14%">Days</th>
                       {{/if}}
                       <th style="width: 10%">Units</th>
                       <th style="width: 14%">Measured In</th>
                       <th style="width: 10%">Frequency</th>
                       
                    </tr>
                </thead>
                <tbody id="tblDSODetailBody">
                    <%--Table rows are added Dynamically--%>
                </tbody>
            </table>
    </script>
    <script id="OrderDetailSvcRegistersTemplateBody" type="text/html">
	        <tr> 
                <td>${formatDate(DateFrom)}</td>
                <td>${formatDate(DateTo)}</td> 
	            <td>${RateCategory}</td>
	            {{if ShowDayOfWeek}}
                    <td >${DayOfWeek} &nbsp;</td>
                {{/if}}
                <td>${Units}</td>
                <td>${MeasuredIn}</td>
                <td>${Frequency}</td>
               
	        </tr>
    </script>
    <script id="suspensionRowsTemplate" type="text/html">
	        <tr>
                <td>${formatDate(DateFrom)}</td>
                <td>${formatDate(DateTo)}</td>
                <td>${Reason}</td>
	        </tr>
    </script>
    <script id="costRowsTemplate" type="text/html">
	        <tr>
	            <td>${RateCategory}</td>
                <td>${Units}</td>
                <td>${MeasuredIn}</td>
                <td>${formatCurrency(UnitCost)}</td>
                <td>${formatCurrency(TotalCost)}</td>
	        </tr>
    </script>
    <script id="documentRowsTemplate" type="text/html">
	        <tr>
	            <td>${DocumentType}&nbsp;</td>
                <td>${Description}&nbsp;</td>
	            <td>
	                
	                <a title="Click to download '${Filename}'" href="/TargetWeb/AbacusExtranet/Apps/Documents/DocumentDownloadHandler.axd?id=${DocumentID}&saveas=1">
	                    <img width="16" height="16" style="cursor: pointer;" alt="Click to download '${Filename}'" src="/TargetWeb/Images/FileTypes/${IconFile}" complete="complete"/>
	                    ${Filename}
	                </a>
	            </td>
	        </tr>
    </script>
</div>
