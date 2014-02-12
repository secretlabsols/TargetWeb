
var MPCONTENT_PREFIX = "ctl00_MPContent_";

/**********************************************************************************/
/* BROWSER DETECTION */
/**********************************************************************************/
/*
Script Name: Full Featured Javascript Browser/OS detection
Authors: Harald Hope, Tapio Markula, Websites: http://techpatterns.com/
http://www.nic.fi/~tapio1/Teaching/index1.php3
Script Source URI: http://techpatterns.com/downloads/javascript_browser_detection.php
Version 4.2.2
Copyright (C) 08 July 2005

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

Lesser GPL license text:
http://www.gnu.org/licenses/lgpl.txt

Coding conventions:
http://cvs.sourceforge.net/viewcvs.py/phpbb/phpBB2/docs/codingstandards.htm?rev=1.3
*/
//initialization, browser, os detection
var d, utilsDom, nu='', brow='', ie, ie4, ie5, ie5x, ie6, ie7, ie8, ie9, ie10;
var ns4, moz, moz_rv_sub, release_date='', moz_brow, moz_brow_nu='', moz_brow_nu_sub='', rv_full=''; 
var mac, win, old, lin, ie5mac, ie5xwin, konq, saf, op, op4, op5, op6, op7;

d=document;
n=navigator;
nav=n.appVersion;
nan=n.appName;
nua=n.userAgent;
old=(nav.substring(0,1)<4);
mac=(nav.indexOf('Mac')!=-1);
win=( ( (nav.indexOf('Win')!=-1) || (nav.indexOf('NT')!=-1) ) && !mac)?true:false;
lin=(nua.indexOf('Linux')!=-1);
// begin primary dom/ns4 test
// this is the most important test on the page
if ( !document.layers )
{
	utilsDom = ( d.getElementById ) ? d.getElementById : false;
}
else { 
	utilsDom = false; 
	ns4 = true;// only netscape 4 supports document layers
}
// end main dom/ns4 test

op=(nua.indexOf('Opera')!=-1);
saf=(nua.indexOf('Safari')!=-1);
konq=(!saf && (nua.indexOf('Konqueror')!=-1) ) ? true : false;
moz=( (!saf && !konq ) && ( nua.indexOf('Gecko')!=-1 ) ) ? true : false;
ie=((nua.indexOf('MSIE')!=-1)&&!op);
if (op)
{
	str_pos=nua.indexOf('Opera');
	nu=nua.substr((str_pos+6),4);
	brow = 'Opera';
}
else if (saf)
{
	str_pos=nua.indexOf('Safari');
	nu=nua.substr((str_pos+7),5);
	brow = 'Safari';
}
else if (konq)
{
	str_pos=nua.indexOf('Konqueror');
	nu=nua.substr((str_pos+10),3);
	brow = 'Konqueror';
}
// this part is complicated a bit, don't mess with it unless you understand regular expressions
// note, for most comparisons that are practical, compare the 3 digit rv nubmer, that is the output
// placed into 'nu'.
else if (moz)
{
	// regular expression pattern that will be used to extract main version/rv numbers
	pattern = /[(); \n]/;
	// moz type array, add to this if you need to
	moz_types = new Array( 'Firebird', 'Phoenix', 'Firefox', 'Galeon', 'K-Meleon', 'Camino', 'Epiphany', 
		'Netscape6', 'Netscape', 'MultiZilla', 'Gecko Debian', 'rv' );
	rv_pos = nua.indexOf( 'rv' );// find 'rv' position in nua string
	rv_full = nua.substr( rv_pos + 3, 6 );// cut out maximum size it can be, eg: 1.8a2, 1.0.0 etc
	// search for occurance of any of characters in pattern, if found get position of that character
	rv_slice = ( rv_full.search( pattern ) != -1 ) ? rv_full.search( pattern ) : '';
	//check to make sure there was a result, if not do  nothing
	// otherwise slice out the part that you want if there is a slice position
	( rv_slice ) ? rv_full = rv_full.substr( 0, rv_slice ) : '';
	// this is the working id number, 3 digits, you'd use this for 
	// number comparison, like if nu >= 1.3 do something
	nu = rv_full.substr( 0, 3 );
	for (i=0; i < moz_types.length; i++)
	{
		if ( nua.indexOf( moz_types[i]) !=-1 )
		{
			moz_brow = moz_types[i];
			break;
		}
	}
	if ( moz_brow )// if it was found in the array
	{
		str_pos=nua.indexOf(moz_brow);// extract string position
		moz_brow_nu = nua.substr( (str_pos + moz_brow.length + 1 ) ,3);// slice out working number, 3 digit
		// if you got it, use it, else use nu
		moz_brow_nu = ( isNaN( moz_brow_nu ) ) ? moz_brow_nu = nu: moz_brow_nu;
		moz_brow_nu_sub = nua.substr( (str_pos + moz_brow.length + 1 ), 8);
		// this makes sure that it's only the id number
		sub_nu_slice = ( moz_brow_nu_sub.search( pattern ) != -1 ) ? moz_brow_nu_sub.search( pattern ) : '';
		//check to make sure there was a result, if not do  nothing
		( sub_nu_slice ) ? moz_brow_nu_sub = moz_brow_nu_sub.substr( 0, sub_nu_slice ) : '';
	}
	if ( moz_brow == 'Netscape6' )
	{
		moz_brow = 'Netscape';
	}
	else if ( moz_brow == 'rv' || moz_brow == '' )// default value if no other gecko name fit
	{
		moz_brow = 'Mozilla';
	} 
	if ( !moz_brow_nu )// use rv number if nothing else is available
	{
		moz_brow_nu = nu;
		moz_brow_nu_sub = nu;
	}
	if (n.productSub)
	{
		release_date = n.productSub;
	}
}
else if (ie)
{
	str_pos=nua.indexOf('MSIE');
	nu=nua.substr((str_pos+5),3);
	brow = 'Microsoft Internet Explorer';
}
// default to navigator app name
else 
{
	brow = nan;
}
op5=(op&&(nu.substring(0,1)==5));
op6=(op&&(nu.substring(0,1)==6));
op7=(op&&(nu.substring(0,1)==7));
op8=(op&&(nu.substring(0,1)==8));
op9=(op&&(nu.substring(0,1)==9));
ie4=(ie&&!utilsDom);
ie5=(ie&&(nu.substring(0,1)==5));
ie6=(ie&&(nu.substring(0,1)==6));
ie7=(ie&&(nu.substring(0,1)==7));
ie8=(ie&&(nu.substring(0,1)==8));
ie9=(ie&&(nu.substring(0,1)==9));
ie10=(ie&&(nu==10));

// default to get number from navigator app version.
if(!nu) 
{
	nu = nav.substring(0,1);
}
/*ie5x tests only for functionavlity. dom or ie5x would be default settings. 
Opera will register true in this test if set to identify as IE 5*/
ie5x=(d.all&&utilsDom);
ie5mac=(mac&&ie5);
ie5xwin=(win&&ie5x);

/**********************************************************************************/
/* DATE FUNCTIONS */
/**********************************************************************************/	

Date.prototype.addDays = function(days) {
    this.setTime(this.getTime() + days*1000*60*60*24);
}

/**********************************************************************************/
/* STRING FUNCTIONS */
/**********************************************************************************/	

String.prototype.splitOnCapitals = function() {
	var result = "";
	for(charCount=0; charCount<=this.length; charCount++) {
		if(charCount !=0 && (this.charCodeAt(charCount) >= 65 && this.charCodeAt(charCount) <= 90)) {
			result += " ";
		}
		result += this.slice(charCount, charCount+1);
	}
	return result;
}
String.prototype.trim = function() {
	return this.replace(/^\s*|\s*$/g,"");
}
String.prototype.left = function(length) {
	if (length <= 0)
		return "";
	else if (length > this.length)
		return this;
	else
		return this.substring(0, length);
}
String.prototype.right = function(length) {
	if (length <= 0)
		return "";
	else if (length > this.length)
		return this;
	else
		return this.substring(this.length, this.length - length);
}
String.prototype.countString = function(str) {
	var count = 0;
	for(var i=0;i<this.length;i++) {
		if(str == this.substr(i, str.length))
			count++;
	}
	return count;
}
String.prototype.toDate = function() {
	// converts strings in the format dd/mm/yyyy to a date object
	var year, month, day;
	if(this.length <= 0) {
		return null;
	} else {
		day = parseInt(this.substr(0,2),10);
		month = parseInt(this.substr(3,2),10) - 1;
		year = parseInt(this.substr(6,4),10);
		return new Date(year, month, day);
	}
}
String.prototype.leftPad = function(padWith, minSize) {
	var newString = new String(this);
	while (newString.length < minSize) {
		newString = padWith + newString;
	}
	return newString;
}
String.prototype.formatCurrency = function(excludeSymbol) {
	var num = this.toString().replace(/\$|\,/g,'');
	if(isNaN(num)) num = "0";
	var sign = (num == (num = Math.abs(num)));
	num = Math.floor(num*100+0.50000000001);
	var pence = num%100;
	num = Math.floor(num/100).toString();
	if(pence<10) pence = "0" + pence;
	for (var i = 0; i < Math.floor((num.length-(1+i))/3); i++)
		num = num.substring(0,num.length-(4*i+3))+','+num.substring(num.length-(4*i+3));
	return (((sign)?'':'-') + (excludeSymbol ? '' : '&pound;') + num + '.' + pence);
}
/**********************************************************************************/
/* GENERAL FUNCTIONS */
/**********************************************************************************/
    function GetElement(id, suppressError) {
        var elem, result;
        elem = document.getElementById(id);
        if(elem) {
            result = elem;
        } else {
            result = document.getElementById(MPCONTENT_PREFIX + id);
        }
        if(!result && !suppressError) alert("Could not locate element with ID: " + id);
        return result;
    }
	function GetInnerText(obj) {
		if(typeof(obj) == "string") obj = GetElement(obj);
		if(ie)
			return obj.innerText;
		else //if(moz)
			return obj.textContent;
//		else
//			alert("GetInnerText() failed!");
	}
	function SetInnerText(obj, value) {
		if(typeof(obj) == "string") obj = GetElement(obj);
		if(ie)
			obj.innerText = value;
		else //if(moz)
			obj.textContent = value;
//		else
//			alert("SetInnerText() failed!");
	}
	function GetEvent(evt) {
		return window.event ? window.event : evt;
	}
	function GetSrcElementFromEvent(evt) {
		return window.event ? window.event.srcElement : evt.target;
	}
	function FindOffset(obj) {
	
		var shift = new Array(2);
		var curleft = 0;
		var curtop = 0;
		
		if (obj.offsetParent)
		{
			while (obj.offsetParent)
			{
				curleft += obj.offsetLeft
				curtop += obj.offsetTop
				obj = obj.offsetParent;
			}
		}
		else if (obj.x || obj.y) {
			curleft += obj.x;
			curtop += obj.y;
		}
		shift[0] = curleft;
		shift[1] = curtop;
		return shift;
	}
	function FilterKeyPress(evt, allowedKeys) {
		var key = window.event ? evt.keyCode : evt.which;
		for(i=0;i<allowedKeys.length;i++) {
			if(key == allowedKeys[i]) return true;
		}
		cancelEvent(evt, true);
	}
	function CatchKeyPress(evt, keyCode) {
		var key = window.event ? evt.keyCode : evt.which;
		return (key == keyCode);
	}
	function GenerateGuid()
	{
		var hex = new Array('0','1','2','3','4','5','6','7','8', '9','a','b','c','d','e','f');
		var outB = '{';		
		for (count = 0; count < 32; count++)
		{
			if ((count == 8) || (count == 12) || (count == 16) || (count == 20))
				outB += '-';				
			outB += hex[Math.floor(Math.random() * 16)];
		}		
		return outB.toUpperCase() + '}';
	}
	/* Takes a copy of the specified object.
		USAGE:
		var obj1 = new Object();
		var obj2 = new DeepCopy(obj1);
	*/
	function DeepCopy(curObj) {
		var name;
		for (name in curObj) {
			this[name] = curObj[name];
		}
	}
	if(!window.addNamespace) {
		window.addNamespace = function(ns) {
			var nsParts = ns.split(".");
			var root = window;

			for(var i=0; i<nsParts.length; i++) {
				if(typeof root[nsParts[i]] == "undefined")
					root[nsParts[i]] = {};
				root = root[nsParts[i]];
			}
		}
	}
	/**
	* Code below taken from - http://www.evolt.org/article/document_body_doctype_switching_and_more/17/30655/
	*
	* Modified 4/22/04 to work with Opera/Moz (by webmaster at subimage dot com)
	*
	* Gets the full width/height because it's different for most browsers.
	*/
	function GetViewportHeight(win) {
		if (win.innerHeight!=window.undefined) return win.innerHeight;
		if (win.document.compatMode=='CSS1Compat') return win.document.documentElement.clientHeight;
		if (win.document.body) return win.document.body.clientHeight; 
		return window.undefined; 
	}
	function GetViewportWidth(win) {
		if (win.innerWidth!=window.undefined) return win.innerWidth; 
		if (win.document.compatMode=='CSS1Compat') return win.document.documentElement.clientWidth; 
		if (win.document.body) return win.document.body.clientWidth; 
		return window.undefined; 
	}
	function GetScrollY(win) {
	    var doc;
	    if (!win) {
	        win = window;
	    }
	    doc = win.document;
		var scrollTop = doc.body.scrollTop;
		if (scrollTop == 0) {
			if (win.pageYOffset) 
				scrollTop = win.pageYOffset;
			else
			scrollTop = (doc.body.parentElement) ? doc.body.parentElement.scrollTop : 0;
		}
		return scrollTop;
	}
	function ConvertPxToEm(px) {
        var result = String(px);
        if(result.indexOf("em") > 0) return result.replace("em", "");
	    result = result.replace("px", "");
		return (result / 12.90).toFixed(2);
	}
	function ConvertEmToPx(em) {
	    var result = String(em);
        if(result.indexOf("px") > 0) return result.replace("px", "");
	    result = result.replace("em", "");
		return (result * 12.90).toFixed(0);
	}
		
	function IsDate(strDate) {
		var intDay;
		var intMonth;
		var intYear;
		var arrDate;

		if(strDate == undefined)
			return false;
		if(strDate == null)
			return false;

		// Check for delimiter and continue if found
		if(strDate.indexOf("/") != -1) {
			
			// Split date into seperate values
			arrDate = strDate.split("/");

			// Get each part
			intDay = arrDate[0];
			intMonth = arrDate[1];
			intYear = arrDate[2];	

			// Check day, month and year are the correct lengths
			if((intDay == undefined ) || (intMonth == undefined) || (intYear == undefined)) {
				return false;
			}
			if((intDay.toString().length != 2 ) || (intMonth.toString().length != 2) || (intYear.toString().length != 4)) {
				return false;
			}

			// check the date is valid
			var dteToday = new Date();
			intYear = ((!intYear) ? dteToday.getFullYear():intYear);
			intMonth = ((!intMonth) ? today.getMonth():intMonth-1);
			if (!intDay) return false;
			var test = new Date(intYear,intMonth,intDay);
			if ( (test.getFullYear() == intYear) &&
				(intMonth == test.getMonth()) &&
				(intDay == test.getDate()) )
				return true;
			else
				return false;
			
		} else {	
			return false
		}
	}
	
	function GetCurrentStyle(el){
     if (el.currentStyle) //IE
        return el.currentStyle
     else if (document.defaultView && document.defaultView.getComputedStyle) //Firefox
        return document.defaultView.getComputedStyle(el, "")
     else //try and get inline style
        return el.style
    }
    
    function cleanWhitespace(node) {
      var notWhitespace = /\S/;
      for (var x = 0; x < node.childNodes.length; x++) {
        var childNode = node.childNodes[x]
        if ((childNode.nodeType == 3)&&(!notWhitespace.test(childNode.nodeValue))) {
          // that is, if it's a whitespace text node
          node.removeChild(node.childNodes[x]);
          x--;
        }
        if (childNode.nodeType == 1) {
          // elements can have text child nodes of their own
          cleanWhitespace(childNode);
        }
      }
    }

    function FlashObject(id, flashedCssClass) {
        var obj = GetElement(id);
        var newClassName;
        if(obj) {
            if(obj.className.indexOf(flashedCssClass) > 0) {
                newClassName = obj.className.replace(flashedCssClass, "");
            } else {
                newClassName = obj.className.trim();
                newClassName += " " + flashedCssClass;
            }
            obj.className = newClassName;
        }
    }
    
    function SelectContent(obj) {
       DeSelectContent();
       if (document.selection) {
          var range = document.body.createTextRange();
          range.moveToElementText(obj);
          range.select();
       }
       else if (window.getSelection) {
          var range = document.createRange();
          range.selectNode(obj);
          window.getSelection().addRange(range);
       }
    }
    function DeSelectContent() {
        if (document.selection) {
            document.selection.empty();
        } else if (window.getSelection) {
            window.getSelection().removeAllRanges();
        }
    } 
    
    
/**********************************************************************************/
/* EVENT FUNCTIONS */
/**********************************************************************************/	
function addEvent(o, evType, f, capture) {
	if(o.addEventListener) {
		o.addEventListener(evType, f, capture);
		return true;
	} else if (o.attachEvent) {
		var r = o.attachEvent("on" + evType, f);
		return r;
	} else {
		// alert("Handler could not be attached");
	}
} 
function removeEvent(o, evType, f, capture) {
	if(o.removeEventListener) {
		o.removeEventListener(evType, f, capture);
		return true;
	} else if (o.detachEvent) {
		var r = o.detachEvent("on" + evType, f);
		return r;
	} else {
		// alert("Handler could not be removed");
	}
}
// cancelEvent() function you can call within your event handlers to
// stop them performing the normal browser action or kill the event entirely.
// Pass an event object, and the second "c" parameter cancels event bubbling.
function cancelEvent(e, c) {
	if(e) {
		e.returnValue = false;
		if (e.preventDefault) e.preventDefault();
		if (c)
		{
			e.cancelBubble = true;
			if (e.stopPropagation) e.stopPropagation();
		}
	}
}

function fireEvent(o, evType) {
    if(o.fireEvent) {
        o.fireEvent("on" + evType);
    } else {
        var e = document.createEvent("Events");
        e.initEvent(evType, true, true);
        o.dispatchEvent(e);
    }
}
			
/**********************************************************************************/
/* TIMEOUT FUNCTIONS */
/**********************************************************************************/
	var timeoutStartMin, timeoutMin, timeoutSec;
	var timeoutStopped = false;
	var timeoutInterval = null;
	var countingDownTimeout = false, extendTimeoutDate, monitoringTimeout = false;

	function StartTimeout(minutes) {
	    if (GetElement("timeoutDesc", true) && GetElement("timeout", true)) {
	        timeoutStartMin = minutes;
	        if (!timeoutStopped) ResetTimeout(minutes);
	        GetElement("timeoutDesc").className = "timeoutHidden";
	        GetElement("timeout").className = "timeoutHidden";
	        timeoutMin = minutes;
	        timeoutSec = 01;
	        timeoutStopped = false;
	        if (timeoutInterval != null) window.clearInterval(timeoutInterval);
	        timeoutInterval = window.setInterval("Countdown()", 1000);
	    }
	}	
	function ExtendTimeout() {
	    var dateNow = new Date();
	    var delay = 30000;
	    var lastRequest = 0;
	    if (extendTimeoutDate) {
	        lastRequest = (dateNow - extendTimeoutDate);
	    }
	    if (lastRequest == 0 || lastRequest > delay || countingDownTimeout) {
	        if (countingDownTimeout) {
	            countingDownTimeout = false;
	            delay = 0;
	        }
	        setTimeout(function() {
	            $.getJSON(SITE_VIRTUAL_ROOT + 'Session/KeepSessionAliveHandler.axd', {}, function(data) {
	                ResetTimeout();
	            });
	        }, delay);
	        extendTimeoutDate = dateNow;
	    }
	}
	function MonitorTimeout() {
        if (!monitoringTimeout) {
            $(document).mouseup(function(evt) {
                ExtendTimeout();
            });
            $(document).keyup(function(evt) {
                ExtendTimeout();
            });
            monitoringTimeout = true;
        }
	}
	function ResetTimeout(minutes) {
		if(minutes == undefined) minutes = timeoutStartMin;
		timeoutMin = minutes;
		timeoutSec = 01;
	}
	function StopTimeout() {
		timeoutStopped = true;
		if(timeoutInterval != null) window.clearInterval(timeoutInterval);
		timeoutMin = 00;
		timeoutSec = 00;
	}
	function DisplayTimeout(min,sec) {
		var disp;
		if(min <=9) disp = " 0";   
		else disp = " ";
		disp += min + ":";  
		if(sec <= 9) disp += "0" + sec;       
		else disp += sec; 
		return(disp); 
	}
	function Countdown() { 
		if(!timeoutStopped)
		{
			timeoutSec--;        
			if (timeoutSec == -1) { 
				timeoutSec = 59;
				timeoutMin--; 
			}       
			if( (timeoutMin == 0) && (timeoutSec == 0) ) {
			    //Events here to occur when timer runs out.
			    countingDownTimeout = false;
				StopTimeout();
				return false;
			} 
		}
	}
	
/**********************************************************************************/
/* OPEN NEW WINDOW FUNCTIONS */
/**********************************************************************************/
	function OpenPopup(url, widthEm, heightEm, resizable) {
		var widthPx = ConvertEmToPx(widthEm);
		var heightPx = ConvertEmToPx(heightEm);
		var left = Math.round((screen.width - widthPx) / 2);
		var top = Math.round((screen.height - heightPx) / 2);
		if(resizable == undefined) resizable = 0;
	    var wind = 	window.open(url, "_blank", "toolbar=0,scrollbars=1,location=0,statusbar=0,menubar=0,resizable=" + resizable + ",width=" + widthPx + ",height=" + heightPx + ",left=" + left + ",top=" + top);
        setTimeout(function () { wind.focus(); }, 1);
	}
	function GetParentWindow() {
		if(ie) {
			if(window.dialogArguments) {
				return window.dialogArguments;
			} else {
				return window;
			}		
		} else {
			if(window.parent) {
				if(window.parent.opener) {
					return window.parent.opener;
				} else {
					return window.parent;
				}
			} else {
				return window;
			}
		}
	}
	function OpenDialog(url, widthEm, heightEm, args) {
        var widthPx = ConvertEmToPx(widthEm);
		var heightPx = ConvertEmToPx(heightEm);
		ShowModalDIV();
		if (ie && ie10) {
		    var sFeatures = "dialogHeight: " + heightPx + "px;";
		    window.showModalDialog(url, args, sFeatures);
        }
        else if(ie && !ie10){
            window.showModalDialog(url, args, "dialogHeight: " + heightPx + "px; dialogWidth: " + widthPx + "px; dialogTop: px; dialogLeft: px; edge: Raised; center: Yes; help: No; resizable: No; status: No;");			
		} else {
			var left = Math.round((screen.width - widthPx) / 2);
			var top = Math.round((screen.height - heightPx) / 2);
			window.open(url, "dialogName", "height=" + heightPx + ",width=" + widthPx + ",top=" + top + ",left=" + left + ",modal=yes,dialog=yes");
		}
	}
	function GetDocForModalDIV() {
	    var doc = window.top.document;
	    var div = doc.getElementById("divModal");
	    if (!div) {
	        doc = document;
	    }
	    return doc;
	}
	function ShowModalDIV() {
	    var doc = GetDocForModalDIV();
		var div = doc.getElementById("divModal");
		div.style.top = "0px";
		div.style.left = "0px";
		div.style.height = Math.max(doc.body.offsetHeight, GetViewportHeight(window.top)) + "px";
		div.style.width = Math.max(doc.body.offsetWidth, GetViewportWidth(window.top)) + "px";
		if (ie6) {
			var iframe = div.getElementsByTagName("IFRAME")[0];
			if(!iframe) {
				iframe = doc.createElement("IFRAME");
				iframe.style.width = "100%";
				iframe.style.height = "100%";
				iframe.src = SITE_VIRTUAL_ROOT + "Images/clear.gif";
				div.appendChild(iframe);
			}
		}
		div.style.background = "url(" + SITE_VIRTUAL_ROOT + "Images/maskBG.gif) repeat";
		div.style.display = "block";
		div.style.zIndex = 9999;
		return div;
	}
	function ShowProcessingModalDIV() {
	    var modalDiv, img, procDiv, doc;
	
	    modalDiv = ShowModalDIV();
	    doc = GetDocForModalDIV();
	    
	    procDiv = doc.createElement("DIV");
	    procDiv.className = "procModalDiv";

	    img = doc.createElement("IMG");
	    img.src = SITE_VIRTUAL_ROOT + "images/busy.gif";
	    img.align = "center";
	    procDiv.appendChild(img)
	    
	    procDiv.appendChild(doc.createTextNode("Processing, please wait..."));

	    modalDiv.appendChild(procDiv);
	    	    
	    // set position
	    var width = procDiv.offsetWidth;
		var height = procDiv.offsetHeight;
		var left = Math.round((GetViewportWidth(window.top) - width) / 2);
		var top = Math.round((GetViewportHeight(window.top) - height) / 2);
	    procDiv.style.top = top + "px";
	    procDiv.style.left = left + "px";
	}
	function HideModalDIV() {
	    var doc = GetDocForModalDIV();
		if(!doc) return;
		var div = doc.getElementById("divModal");
		if(!div) {
			if(doc.frames)
				doc = doc.frames[0].document;
			else
				doc = window.top.frames[0].document;			
			div = doc.getElementById("divModal");
		}
		if(!div) return;
		div.style.display = "none";
		div.innerHTML = "";
	}
	function DialogUnload() {
		GetParentWindow().HideModalDIV();
	}
	function OpenHelp(url) {
		var widthEm = 45;
		var heightEm = 50;
		var widthPx = ConvertEmToPx(widthEm);
		var heightPx = Math.min(ConvertEmToPx(heightEm), screen.height);
		var top = 0;
		var left = screen.width - widthPx - 12;
		window.open(url, "winHelp", "toolbar=0,scrollbars=1,location=0,statusbar=1,menubar=0,resizable=1,width=" + widthPx + ",height=" + heightPx + ",left=" + left + ",top=" + top);
	}
	function OpenFileUploadProgress(uploadID) {
	    var url;
	    url = SITE_VIRTUAL_ROOT + "Apps/FileStore/ProgressForm.aspx?uploadID=" + uploadID;
        ShowProcessingModalDIV();
        OpenPopup(url, 27, 17);
	}
	function OpenReport(url, divDownloadContainerID, hideCallback) {
	    var format = GetQSParam(url, "rc:Format"), hasExt = (typeof Selectors != "undefined");
	    if (format) {
	        if (!hasExt) {
	            ShowProcessingModalDIV();
	        }        
	        var div = GetElement(divDownloadContainerID);
	        var iframe = document.createElement("iframe");
            iframe.src = url;
            // for IE
            if (!hasExt) {
                if (iframe.onreadystatechange) {
                    iframe.onreadystatechange = HideModalDIV();
                }
                // for other browsers
                 HideModalDIV();
            } else {
                if (iframe.onreadystatechange) {
                    iframe.onreadystatechange = hideCallback();
                }
                // for other browsers
                hideCallback();
            }
            // clear any existing content
            div.innerHTML = "";
            // add iframe to DOM which triggers the download
            div.appendChild(iframe);
	    } else {
	        var reportUrl = SITE_VIRTUAL_ROOT + "Loading.aspx?";
	        reportUrl += "url=" + escape(url);
	        OpenPopup(reportUrl, 75, 50, 1);
	    }
	    // auto-hide popup menu afterwards by simulating a mouse click
	    fireEvent(document.body, "click");
	}
		
/**********************************************************************************/
/* MANIPULATE QUERYSTRING FUNCTIONS */
/**********************************************************************************/

    function ParseQueryString(fullUrl) {
        var data = new Collection();
        var beforeQm = "";
        
        this.Read = function() 
        {
            var aPairs, aTmp;
	        var queryString = "";
            var aUrl;
            
            aUrl = fullUrl.split("?");
            if(aUrl.length > 0) {
		        beforeQm = aUrl[0].trim();
		        if(aUrl.length > 1) { queryString = aUrl[1]; }
	        }
            
	        if(queryString.substr(0, 1) == "?") queryString = queryString.substr(1);
	        aPairs = queryString.split("&");	
    		
	        for (var i=0 ; i<aPairs.length; i++)
	        {
		        aTmp = aPairs[i].split("=");
		        data.add(aTmp[0].toLowerCase(), aTmp[1]);
	        }
        }
    	
        this.GetValue = function( key )
        {
	        var result = null;
	        if(data.exists(key.toLowerCase())) {
	            result = data.item(key.toLowerCase());
            }
	        return result;
        }
        this.SetValue = function( key, value )
        {
	        if (value == null) {
	            if(data.exists(key.toLowerCase())) {
	                data.remove(key.toLowerCase());
	            }
	        } else {
	            if(data.exists(key.toLowerCase())) {
	                data.update(key.toLowerCase(), value);
	            } else {
	                data.add(key.toLowerCase(), value);
	            }
            }
        }
        this.ToString = function()
        {
	        var queryString = new String(""); 
    		var keys = data.getKeys();
    		for(i=0; i<keys.length; i++) {
	            if (queryString != "") {
	                if(queryString.right(1) != "&") {
		                queryString += "&"
                    }
                }
                queryString += keys[i] + "=" + data.item(keys[i]);
            }

	        if (queryString.length > 0) {
		        return beforeQm + "?" + queryString;
	        } else {
	            if(beforeQm.length > 0) {
	                return beforeQm;
	            } else {
	                return "?";
	            }
            }
        }
        this.Clear = function()
        {
	        data = new Collection();
        }
    }

	function AddQSParam(fullUrl, name, value) {
		var qs = new ParseQueryString(fullUrl);
        qs.Read();
        qs.SetValue(name, value);
        return qs.ToString();
	}
	function RemoveQSParam(fullUrl, name) {
		var qs = new ParseQueryString(fullUrl);
        qs.Read();
        qs.SetValue(name, null);
        return qs.ToString();
	}
	function GetQSParam(qString, name) {
	    var qs = new ParseQueryString(qString);
        qs.Read();
        return qs.GetValue(name);
	}

/**********************************************************************************/
/* COOKIE FUNCTIONS */
/**********************************************************************************/
// Cookie functions obtained from http://www.webreference.com/  "Crispy Javascript Cookies" as @06/12/2002
//
// name - name of the cookie
// value - value of the cookie
// [expires] - expiration date of the cookie (defaults to end of current session)
// [path] - path for which the cookie is valid (defaults to path of calling document)
// [domain] - domain for which the cookie is valid (defaults to domain of calling document)
// [secure] - Boolean value indicating if the cookie transmission requires a secure transmission
// * an argument defaults when it is assigned null as a placeholder
// * a null placeholder is not required for trailing omitted arguments
function setCookie(name, value, expires, path, domain, secure) {
  var curCookie = name + "=" + escape(value) +
      ((expires) ? "; expires=" + expires.toGMTString() : "") +
      ((path) ? "; path=" + path : "") +
      ((domain) ? "; domain=" + domain : "") +
      ((secure) ? "; secure" : "");
  document.cookie = curCookie;
}

// name - name of the desired cookie
// * return string containing value of specified cookie or null if cookie does not exist
function getCookie(name) {
  var dc = document.cookie;
  var prefix = name + "=";
  var begin = dc.indexOf("; " + prefix);
  if (begin == -1) {
    begin = dc.indexOf(prefix);
    if (begin != 0) return null;
  } else
    begin += 2;
  var end = document.cookie.indexOf(";", begin);
  if (end == -1)
    end = dc.length;
  return unescape(dc.substring(begin + prefix.length, end));
}

// name - name of the cookie
// [path] - path of the cookie (must be same as path used to create cookie)
// [domain] - domain of the cookie (must be same as domain used to create cookie)
// * path and domain default if assigned null or omitted if no explicit argument proceeds
function deleteCookie(name, path, domain) {
  if (getCookie(name)) {
    document.cookie = name + "=" + 
    ((path) ? "; path=" + path : "") +
    ((domain) ? "; domain=" + domain : "") +
    "; expires=Thu, 01-Jan-70 00:00:01 GMT";
  }
}

/**********************************************************************************/
/* IE6 HOVER FIX */
/**********************************************************************************/
function IEHoverPseudo() {
    // only needed for IE6
    if(document.getElementById("newNav")) {
        var nav = document.getElementById("newNav");
        var navItems = nav.children;
        // add over/out events for menu items
        for (var i=0; i<navItems.length; i++) {
	        navItems[i].onmouseover=function() { IEHoverPseudoToggle(this, true); }
            navItems[i].onmouseout=function() { IEHoverPseudoToggle(this, false); }
        }
    }
}

function IEHoverPseudoToggle(navItem, show) {
    // only needed for IE6
    var display = show ? "block" : "none";
    var subnav = navItem.getElementsByTagName("div")[0];
    
    navItem.style.backgroundPosition = show ? "bottom left" : "top left";
    
    if(subnav) {
        var iframe = subnav.nextSibling;
        subnav.style.display = display;
        iframe.style.display = display;
        if(show) {
            var height = subnav.offsetHeight;
            var width = subnav.offsetWidth;
            var left = subnav.currentStyle.left;
            var top = subnav.currentStyle.top;
            iframe.style.top = top;
            iframe.style.left = left;
            iframe.style.width = width;
            iframe.style.height = height;
        }
    }
}

/**********************************************************************************/
/* IE6 IMAGE FLICKER FIX */
/**********************************************************************************/
function IEImageFlicker() {
	// only needed for IE6
	try {
		document.execCommand("BackgroundImageCache", false, true);
	} catch(err) { }
}

/**********************************************************************************/
/* JAVASCRIPT COLLECTION OBJECT */
/**********************************************************************************/

function Collection() { 
 var collection = {}; 
 var order = []; 

 this.add = function(property, value) { 
  if (!this.exists(property)) { 
   collection[property] = value; 
   order.push(property); 
  } 
 } 
 this.remove = function(property) { 
  collection[property] = null; 
  var ii = order.length; 
  while (ii-- > 0) { 
   if (order[ii] == property) { 
    order[ii] = null; 
    break; 
   } 
  } 
 } 
 this.toString = function() { 
  var output = []; 
  for (var ii = 0; ii < order.length; ++ii) { 
   if (order[ii] != null) { 
    output.push(collection[order[ii]]); 
   } 
  } 
  return output; 
 } 
 this.getKeys = function() { 
  var keys = []; 
  for (var ii = 0; ii < order.length; ++ii) { 
   if (order[ii] != null) { 
    keys.push(order[ii]); 
   } 
  } 
  return keys; 
 } 
 this.update = function(property, value) { 
  if (value != null) { 
   collection[property] = value; 
  } 
  var ii = order.length; 
  while (ii-- > 0) { 
   if (order[ii] == property) { 
    order[ii] = null; 
    order.push(property); 
    break; 
   } 
  } 
 } 
 this.exists = function(property) { 
  return collection[property] != null; 
 }
 this.item = function(property) {
   if (this.exists(property))
     return collection[property];
   else
     return null;
 }
 this.sort = function() {
   // sort keys in ascending, ASCII order
   // to sort descedning, call sort() and then reverse()
   order.sort();
 }
 this.reverse = function() {
  // reverses the order of the elements
  order.reverse();
 }
} 

/**********************************************************************************/
/* TEXT SIZE */
/**********************************************************************************/

function TextSize(size) {
	document.location.href = AddQSParam(RemoveQSParam(document.location.href, "textSize"), "textSize", size);
}

/**********************************************************************************/
/* CLIPBOARD */
/**********************************************************************************/

function CopyToClipboard(text, successText) {
	try {
		if(ie) {
			window.clipboardData.setData("Text", text);
		} else if(moz) { 
			// enable the required privilege
			netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
			   
			// make an interface to the clipboard
			var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
			if (!clip) {
				alert("ERROR: Could not copy data to the clipboard\n[could not create clipboard interface].");
				return;
			}
			   
			// make transferable
			var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
			if (!trans) {
				alert("ERROR: Could not copy data to the clipboard\n[could not create clipboard transfer].");
				return;
			}
			   
			// specify what type data we on want to obtain; text in this case
			trans.addDataFlavor('text/unicode');

			var str = new Object();
			var len = new Object();
			str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
			var copytext = text;
			str.data = copytext;
			trans.setTransferData("text/unicode", str , copytext.length * 2);
			var clipid = Components.interfaces.nsIClipboard;
			if (!clip) {
				alert("ERROR: Could not copy data to the clipboard\n[could not get clipboard reference].");
				return;
			}
			clip.setData(trans, null, clipid.kGlobalClipboard);
		}
		alert(successText);
	} catch(ex) {
		alert("ERROR: Could not copy data to the clipboard\n[" + ex + "].");
	} finally {
		return false;
	}
}

function CopyToClipboardHtml(obj, successText) {
	try {
	    var selectedHtml, result;
		SelectContent(obj);
		if (document.selection) {
		    selectedHtml = (document.selection.createRange()).htmlText;
		} else if (window.getSelection) {
		    var selection = window.getSelection();
		    if (selection.rangeCount) {
                var range = selection.getRangeAt(0);
                selectedHtml = range.cloneContents().innerHTML;
            }
            // enable the required privilege
			netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
		}
	    result = document.execCommand("Copy", false, null);
	    DeSelectContent();
		if(result) {
		    alert(successText);
        }
	} catch(ex) {
		alert("ERROR: Could not copy html to the clipboard\n[" + ex + "].");
	} finally {
	    DeSelectContent();
		return false;
	}
}

function externalLinks() { 
 if (!document.getElementsByTagName) return; 
 var anchors = document.getElementsByTagName("a"); 
 for (var i=0; i<anchors.length; i++) { 
   var anchor = anchors[i]; 
   if (anchor.getAttribute("href") && 
       anchor.getAttribute("rel") == "external") 
     anchor.target = "_blank"; 
 } 
} 

function PlaceTopNav() {
    var nav = GetElement("ctl00_menuContainer", true);
    if(nav) {
        // place top level items
        var width = nav.offsetWidth;
        var left = Math.round((GetViewportWidth(window.top) - width) / 2);
        nav.style.left = left + "px";
        // place each subnav menu
        var ulElem = nav.getElementsByTagName("ul")[0];
        var liElems = ulElem.childNodes;
        for(index=0; index<liElems.length; index++) {
            if(liElems[index].tagName.toLowerCase() == "li") {
                var subnav = liElems[index].getElementsByTagName("div")[0];
                if(subnav) {
                    var subnavWidth = ConvertEmToPx(GetCurrentStyle(subnav).width);
                    var subNavLeft = Math.round((GetViewportWidth(window.top) - subnavWidth) / 2);
                    // nav left-offset
                    subnav.style.left = (subNavLeft - left) + "px";
                }
            }
        }
    }
}

function FireEvent(element, event) {
    if (document.createEventObject) {
        // dispatch for IE
        var evt = document.createEventObject();
        return element.fireEvent('on' + event, evt)
    }
    else {
        // dispatch for firefox + others
        var evt = document.createEvent("HTMLEvents");
        evt.initEvent(event, true, true); // event type,bubbling,cancelable
        return !element.dispatchEvent(evt);
    }
}

// ADD ONLOAD EVENTS
addEvent(window, "load", PlaceTopNav);
addEvent(window, "resize", PlaceTopNav);
if(ie6) {
	addEvent(window, "load", IEHoverPseudo);
	addEvent(window, "load", IEImageFlicker);
}
addEvent(window, "load", externalLinks);

//===========================================================================
// Provides a Dictionary object for client-side java scripts
// Source: http://forums.asp.net/t/1324415.aspx/1
// Created 09/09/2011 Motahir D11766 - e-Invoicing Provider Invoice Tolerances
//=========================================================================== 

function Lookup(key) {
    return (this[key]);
}


function Delete() {
    for (c = 0; c < arguments.length; c++) {
        this[arguments[c]] = null;
    }
    // Adjust the keys (not terribly efficient) 
    var keys = new Array()
    for (var i = 0; i < this.Keys.length; i++) {
        if (this[this.Keys[i]] != null)
            keys[keys.length] = this.Keys[i];
    }
    this.Keys = keys;
}

function Add() {
    for (c = 0; c < arguments.length; c += 2) {
        // Add the property 
        this[arguments[c]] = arguments[c + 1];
        // And add it to the keys array 
        this.Keys[this.Keys.length] = arguments[c];
    }
}

function Dictionary() {
    this.Add = Add;
    this.Lookup = Lookup;
    this.Delete = Delete;
    this.Keys = new Array();
}

