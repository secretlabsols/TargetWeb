<%@ Page Language="vb" AutoEventWireup="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head runat="server">
   <title></title>
    <style type="text/css">
        body { font-family:"Verdana"; }
        #container { position:absolute; text-align:center; }
	</style>
   
  </head>
  <script type="text/javascript">
        function BeginPageLoad() {
                 // set position
                 var container = document.getElementById("container");
                 var width = container.offsetWidth;
                     var height = container.offsetHeight;
                     var left = Math.round((GetViewportWidth(window.top) - width) / 2);
                     var top = Math.round((GetViewportHeight(window.top) - height) / 2);
                 container.style.top = top + "px";
                 container.style.left = left + "px";
            // redirect
                     location.href = '<%= Request.QueryString("url")%>';
                     // restart animated gif
                     var img = document.getElementById("img");
                     img.src = img.src;
                     // hook auto close event
                     document.onreadystatechange = AutoClose;
        }
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
        function AutoClose()
        {
           <% If Not String.IsNullOrEmpty(Request.QueryString("close")) AndAlso Request.QueryString("close") = "1" Then %>
                if(document.readyState == "interactive")
                    // allow slight delay before closing to allow other actions to take place, e.g. displaying browser "Open/SaveAs" dialog
                         window.setTimeout("window.close()", 1000);
                 <% End if %>
        }
    </script>


  <body onload="BeginPageLoad()">
    <div id="container">
		<img id="img" src="images/busy.gif" alt="Busy Image" />
		<br /><br />
		Loading, please wait...
	</div>
  </body>
</html>
