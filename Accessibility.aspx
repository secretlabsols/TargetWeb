<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Accessibility.aspx.vb" Inherits="Target.Web.Accessibility"%>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">This is the accessibility statement for this web site.</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <h3>Browser Compatibility</h3>
	    <p>
		    This site has been tested on and is explicitly compatible with the following browsers.  
		    The site may function on other browsers/platforms but its operation cannot be guaranteed.
	    </p>
	    <asp:Literal id="litSupportedBrowsers" runat="server"></asp:Literal>
	    <p>
	    Your browser has been detected as being <strong><asp:Label id="lblYourBrowser" runat="server" /></strong>.
	    </p>
	    <h3>Access keys</h3>
	    <p>
		    Most browsers support jumping to specific links by typing keys defined on the web site.  
		    All pages on this site define the following access keys:
	    </p>
	    <ul>
		    <li>Access key 0 - Accessibility statement</li>
		    <li>Access key 1 - Home page</li>
		    <li>Access key 2 - Skip to the main content</li>
		    <li>Access key 3 - Display help for the current page</li>
	    </ul>
	    <p>To use these access keys, do the following:</p>
	    <ul>
		    <li>Microsoft Internet Explorer - Hold down the "ALT" key and press the required access key (to focus), then press "ENTER" (to navigate).</li>
		    <li>FireFox - Hold down the "ALT" key and press the required access key.</li>
	    </ul>
	    <h3>Text Size</h3>
	    You can change the size of the text displayed on this web site in one of two ways:
	    <ul>
		    <li>By using the <strong>Text Size</strong> control displayed in the top-right of each page</li>
		    <li>By changing the standard text size in your browser:
			    <ul>
				    <li>Internet Explorer - goto the <strong>View</strong> menu, then <strong>Text Size</strong> and choose the size you wish</li>
				    <li>FireFox - goto the <strong>View</strong> menu, then <strong>Text Size</strong> and choose to increase/decrease the size as you wish</li>
			    </ul>
		    </li>
	    </ul>
	    <h3>Standards Compliance</h3>
	    <ul>
		    <li>
			    Wherever possible, all non-administration pages on this site are WCAG 1.0 A approved, complying with 
			    <a href="http://www.w3.org/TR/WAI-WEBCONTENT/full-checklist.html">all priority 1 guidelines</a>
			    of the <a href="http://www.w3.org/TR/WCAG10/">W3C Web Content Accessibility Guidelines 1.0</a>. 
			    This is a judgement call; many guidelines are intentionally vague and cannot be tested automatically. 
			    The software suppliers will review all the guidelines and ensure that all possible pages are in compliance. 
		    </li>
		    <li>
			    Wherever possible <a href="#nonXHTML1.1"><span class="redText">*</span></a>, pages on this site are constructed using valid 
			    <a href="http://www.w3.org/TR/2001/REC-xhtml11-20010531/">Extensible HyperText Markup Language (XHTML) 1.1</a>. 
			    This is not a judgement call; <a href="http://validator.w3.org/">the W3C Markup Validator</a> can determine 
			    with 100% accuracy whether a page is valid XHTML.
		    </li>
		    <li>
			    The unedited, standard <a href="http://www.w3.org/Style/CSS/">Cascading Style Sheets (CSS)</a> as developed 
			    by the software suppliers used by this site are valid CSS. This is not a judgement call; 
			    <a href="http://jigsaw.w3.org/css-validator/">the W3C CSS Validator</a> can determine with 100% accuracy whether the 
			    CSS is valid.
		    </li>
	    </ul>
	    <p style="text-align:center;">
		    <a href="http://www.w3.org/WAI/"><img src="images/wai-a.gif" alt="Level A Conformance W3C WAI Web Content Accessibility Guidelines 1.0" /></a>&nbsp;
		    <a href="http://validator.w3.org/check?uri=referer"><img src="images/valid-xhtml11.gif" alt="Valid XHTML 1.1" /></a>&nbsp;
		    <a href="http://jigsaw.w3.org/css-validator/"><img src="images/valid-css.gif" alt="Valid CSS" /></a>&nbsp;
	    </p>
	    <h3>Visual design</h3>
	    <ul>
		    <li>This site uses Cascading Style Sheets (CSS) for visual layout.</li>
		    <li>This site uses only relative font sizes, compatible with the user-specified "text size" option in visual browsers.</li>
		    <li>If your browser does not support stylesheets or they are disabled, the content of each page is still readable.</li>
	    </ul>
	    <p class="footnote">
		    <a id="nonXHTML1.1"><span class="redText">*</span></a> Currently, non-conformance with the XHTML 1.1 standard on this 
		    site is limited to the following instances: pages produced using the CMS HTML editor; the CMS HTML editor itself; 
		    the online help documentation.
	    </p>
    </asp:Content>