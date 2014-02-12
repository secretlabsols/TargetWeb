<%@ Control Language="vb" AutoEventWireup="false" Codebehind="RecurrencePattern.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.RecurrencePattern" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" Width="35em" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabDaily" HeaderText="Daily">
            <ContentTemplate>
                <div style="float:left;">
                    <asp:radiobutton id="optDailyEveryXDay" Text="Every" groupname="grpDaily" TextAlign="right"
                        runat="server" checked="True" onclick="javascript:optDaily_Click();" />
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtDailyDays" Text="1" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label1" runat="server" Text="Label"> day(s)</asp:Label>
                </div>
                <div class="clearer"></div>
	            <br />
                <div style="float:left;">
                    <asp:radiobutton id="optDailyEveryWeekDay" Text="Every week day" groupname="grpDaily" TextAlign="right" 
                        runat="server" onclick="javascript:optDaily_Click();" />
                </div>
                <div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabWeekly" HeaderText="Weekly">
            <ContentTemplate>
                <div style="float:left;">
                    <asp:Label ID="Label3" runat="server" Text="Label">Every</asp:Label> 
                </div>
                <div style="float:left;padding-left:0.5em;">                    
                    <cc1:TextBoxEx ID="txtWeeklyNo" Text="1" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label2" runat="server" Text="Label"> week(s) on:</asp:Label>
                </div>
                <div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkWeeklyMonday" runat="server"></cc1:CheckBoxEx>
                <cc1:CheckBoxEx ID="chkWeeklyThursday" runat="server" ></cc1:CheckBoxEx>
                <cc1:CheckBoxEx ID="chkWeeklySaturday" runat="server" ></cc1:CheckBoxEx>
                <br />
                <div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkWeeklyTuesday" runat="server" ></cc1:CheckBoxEx>
                <cc1:CheckBoxEx ID="chkWeeklyFriday" runat="server" ></cc1:CheckBoxEx>
                <cc1:CheckBoxEx ID="chkWeeklySunday" runat="server" ></cc1:CheckBoxEx>
                <br />
                <div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkWeeklyWednesday" runat="server" ></cc1:CheckBoxEx>
	            <br />
	            <div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabMonthly" HeaderText="Monthly">
            <ContentTemplate>
                <div style="float:left;">
                    <asp:radiobutton id="optMonthlyDay" Text="Day" groupname="grpMonthly" TextAlign="right" 
                                runat="server" checked="True" onclick="javascript:optMonthly_Click();" />  
                </div>
                <div style="float:left;padding-left:0.5em;">  
                    <cc1:TextBoxEx ID="txtMonthlyDayNo" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label4" runat="server" Text="Label"> of every </asp:Label>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtMonthlyMonth" Width="2em" Text="1" runat="server" Enabled="false"></cc1:TextBoxEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label5" runat="server" Text="Label"> month(s) </asp:Label>
                </div>
                <div class="clearer"></div>
                <br />
                <div style="float:left;">
                    <asp:radiobutton id="optMonthlyPatternDays" Text="The" groupname="grpMonthly" TextAlign="right" 
                                runat="server" onclick="javascript:optMonthly_Click();" /> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:DropDownListEx id="cboMonthlyDayType" runat="server"></cc1:DropDownListEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
	                <cc1:DropDownListEx id="cboMonthlyTypeDesc" runat="server" ></cc1:DropDownListEx>
	            </div>
	            <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label6" runat="server" Text="Label"> of every </asp:Label> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtMonthlyMonth2" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label7" runat="server" Text="Label"> month(s) </asp:Label>  
                </div>
                <div class="clearer"></div>
                <div style="float:left;padding-left:9.75em;">
                    <asp:Label ID="Label8" runat="server" Text="Label">Plus/Minus</asp:Label> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtMonthlyPlusMinus" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label9" runat="server" Text="Label"> Day(s)</asp:Label>
                </div>
                <div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabYearly" HeaderText="Yearly">
            <ContentTemplate>
                <div style="float:left;">
                    <asp:radiobutton id="optYearlyEvery" Text="Every" groupname="grpYearly" TextAlign="right" 
                                runat="server" checked="True" onclick="javascript:optYearly_Click();" />
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:DropDownListEx id="cboYearlyEveryMonth" runat="server"></cc1:DropDownListEx>           
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtYearlyMonthNo" Text="1" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx>
                </div>
                <div class="clearer"></div>
                <br />
                <div style="float:left;">
                    <asp:radiobutton id="optYearlyPattern" Text="The" groupname="grpYearly" TextAlign="right" 
                                runat="server" onclick="javascript:optYearly_Click();" />
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:DropDownListEx id="cboYearlyDayType" runat="server"></cc1:DropDownListEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:DropDownListEx id="cboYearlyTypedesc" runat="server"></cc1:DropDownListEx>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label10" runat="server" Text="Label"> of </asp:Label>
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:DropDownListEx id="cboYearlyMonth" runat="server"></cc1:DropDownListEx>
                </div>
                <div class="clearer"></div>
                <div style="float:left;padding-left:9.75em;">
                    <asp:Label ID="Label11" runat="server" Text="Label">Plus/Minus</asp:Label> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <cc1:TextBoxEx ID="txtYearlyPluMinus" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx> 
                </div>
                <div style="float:left;padding-left:0.5em;">
                    <asp:Label ID="Label12" runat="server" Text="Label"> Day(s)</asp:Label>
                </div>
                <div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <br />
    <div style="float:left;">
        <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Start Date" LabelWidth="7em"
					        Required="true" RequiredValidatorErrMsg="Please enter a start date" Format="DateFormat"
					        ValidationGroup="Save"></cc1:TextBoxEx>
    </div>	
    <div style="float:left;padding-left:1.5em;">				
        <asp:radiobutton id="optEndOn" groupname="grpEnd" Text="End Date" TextAlign="right" runat="server" checked="True" onclick="javascript:optEnd_Click();" />
    </div>
    <div style="float:left;padding-left:0.5em;">   
        <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelWidth="7em" Format="DateFormat"></cc1:TextBoxEx>
    </div>
    <div class="clearer"></div>
    <br />
    <div style="float:left;">
        <cc1:TimePicker ID="tmeStartDate" runat="server" LabelWidth="7em" LabelText="Event Time " Enabled="False"  EnableViewState="False" ShowSeconds="False" ></cc1:TimePicker>
    </div>
    <div style="float:left;padding-left:1.75em;">
        <asp:radiobutton id="optEndAfter" groupname="grpEnd" Text="End after" TextAlign="right" runat="server" onclick="javascript:optEnd_Click();" />
    </div>
    <div style="float:left;padding-left:0.5em;">
        <cc1:TextBoxEx ID="txtOccurrences" Width="2em" runat="server" Enabled="false"></cc1:TextBoxEx>
    </div>
    <div style="float:left;padding-left:0.5em;">
        <asp:Label ID="Label14" runat="server" Text="Label"> occurrences</asp:Label>
    </div>
    <div class="clearer"></div>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
   
        <ContentTemplate>
            <div style="float:left;padding-left:0.5em;">
                <asp:Label ID="lblFriendly" runat="server" CssClass="warningText"></asp:Label>
                <br />
                <asp:Label ID="lblnextRunDate" runat="server" CssClass="warningText"></asp:Label>
            </div>
            <div style="float:left;">
                <input type="button" id="btntest" runat="server" validationgroup="Save" value="Refresh Friendly Text" style="margin-left:1em;float:left;width:12em;" /> 
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="clearer"></div>
    <br />
    <input type="hidden" id="hidSelectedTab" runat="server" />
    
