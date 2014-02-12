﻿var btnAddComment_ClientID, cboComment_ClientID;
var suspensions_btnAddComment, lookupSvc, suspensions_cboComment; 
var optSuspend, optUnsuspend, optAddComment;
var statusIsSuspended, statusIsPaid;
var chkChecked_ClientID, chkChecked;

function Init() {
    suspensions_btnAddComment = GetElement(btnAddComment_ClientID);
    suspensions_cboComment = GetElement(cboComment_ClientID + "_cboDropDownList");
    optSuspend = GetElement("optSuspend");
    optUnsuspend = GetElement("optUnsuspend");
    optAddComment = GetElement("optAddComment");
    suspensions_btnAddComment.disabled = true;
    lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    if (statusIsSuspended == "true")
        optAddComment.checked = true;
    if (statusIsSuspended == "false" && statusIsPaid == "false")
        optSuspend.checked = true;
    chkChecked = GetElement(chkChecked_ClientID);
    optType_Click();
}

function cboComment_Change() {
    if(suspensions_cboComment.value.length > 0) {
        // A suspension Comment has been entered
        suspensions_btnAddComment.disabled = false;
    } else {
        // A suspension comment has been removed
        suspensions_btnAddComment.disabled = true;
    }
}


function optType_Click() {

    if (optSuspend.checked == true | optAddComment.checked == true) {
        FetchSuspensionCommentList(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonType.Suspend, Target.Abacus.Library.DomProviderInvoiceSuspensionReasonAutoType.None)
    } else if(optUnsuspend.checked == true) {
        FetchSuspensionCommentList(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonType.Unsuspend, Target.Abacus.Library.DomProviderInvoiceSuspensionReasonAutoType.None)
    }
}


function FetchSuspensionCommentList(type, autoType) {
    
    DisplayLoading(true);
    
	lookupSvc.FetchDomProviderInvoiceSuspensionCommentList(type, autoType, FetchSuspensionCommentList_Callback);
}

function FetchSuspensionCommentList_Callback(response) {
    var comments, opt;
    if(CheckAjaxResponse(response, lookupSvc.url)) {
        comments = response.value.List;
    
		// clear
	    suspensions_cboComment.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    suspensions_cboComment.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = "";
		
		for(index=0; index<comments.length; index++) {
		    opt = document.createElement("OPTION");
		    suspensions_cboComment.options.add(opt);
		    SetInnerText(opt, comments[index].Text);
		    opt.value = comments[index].Value;
		}
		
	    
	}
	DisplayLoading(false);

}

function chkCheckAll_Click() 
{
    var table = document.getElementById('tblInvoices');
    var inputs = table.getElementsByTagName('input');
    var checkboxes = new Array();
    
    for (i = 0, inputlength=inputs.length; i < inputlength; i++)
    {
        if (!inputs[i].length)
        {
            if (inputs[i].type == 'checkbox')
                inputs[i].checked = chkChecked.checked;
        }     
        else
        {
            for(k = 0, inputsilength=inputs[i].length; k < inputsilength; k++)
            {
              if (inputs[i][k].type == 'checkbox')
                inputs[i].checked = chkChecked.checked;
            }
        }
    }
}

addEvent(window, "load", Init);