<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ImageGallery.aspx.vb" Inherits="Target.Web.Apps.FileStore.Admin.ImageGallery" EnableViewState="True" MasterPageFile="~/Popup.master" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <script type="text/javascript">
        //<![CDATA[
	        var imageSelected = false;
	        var parentWindow = GetParentWindow();
	        function OpenFolder(folderID) {
		        document.location.href = "ImageGallery.aspx?folderID=" + folderID;
	        }
	        function PreviewImage(fileID, width, height, alt) {
		        imageSelected = true;
		        GetElement("img_preview").src = "../FileStoreGetFile.axd?id=" + fileID;
		        GetElement("img_width").value = width;
		        GetElement("img_height").value = height;
    			
		        GetElement("img_dim_percentage").checked = false;
		        GetElement("img_percentage").value = "";
		        GetElement("img_dim_custom").checked = false;
		        GetElement("img_dim_original").checked = true;
    						
		        GetElement("img_align").selectedIndex = 0;
		        GetElement("img_border").value = "0";
		        GetElement("img_vspace").value = "";
		        GetElement("img_hspace").value = "";
		        GetElement("img_alt").value = alt;
		        GetElement("img_title").value = alt;
	        }
	        function ResizePreviewImage(width, height) {
		        var img = GetElement("imgPreviewImage");
	        }
	        function FTB_DimensionChange(sender) {
		        switch (sender.id) {
			        default:
			        case "img_dim_original": 
				        GetElement('img_width_custom').value = '';
				        GetElement('img_height_custom').value = '';
				        GetElement('img_percentage').value = '';
    					
				        GetElement('img_width_custom').disabled = true;
				        GetElement('img_height_custom').disabled = true;
				        GetElement('img_percentage').disabled = true;
    					
				        FTB_ResetImage();
				        break;
			        case "img_dim_custom":
				        GetElement('img_width_custom').value = GetElement('img_width').value;
				        GetElement('img_height_custom').value = GetElement('img_height').value;
				        GetElement('img_percentage').value = '';
    					
				        GetElement('img_width_custom').disabled = false;
				        GetElement('img_height_custom').disabled = false;
				        GetElement('img_percentage').disabled = true;			
				        break;	
			        case "img_dim_percentage":
				        GetElement('img_width_custom').value = '';
				        GetElement('img_height_custom').value = '';
				        GetElement('img_percentage').value = '100';
    					
				        GetElement('img_width_custom').disabled = true;
				        GetElement('img_height_custom').disabled = true;
				        GetElement('img_percentage').disabled = false;	
				        FTB_SetImageByPercentage();		
				        break;	
			        }
		        };
		        function FTB_SetImageByPercentage() {
			        previewImage = GetElement('img_preview');
			        width = GetElement('img_width').value;
			        height = GetElement('img_height').value;
			        percentage = GetElement('img_percentage').value;

			        previewImage.width = width * percentage / 100 + "px";
			        previewImage.height = height * percentage / 100 + "px";	
		        };
		        function FTB_ResetImage() {
			        previewImage = GetElement('img_preview');
			        width = GetElement('img_width').value;
			        height = GetElement('img_height').value;

			        previewImage.width = width + "px";
			        previewImage.height = height + "px";
		        };
		        function FTB_UpdatePreview(sender) {
			        if(!imageSelected) return;
    				
			        previewImage = GetElement('img_preview');
			        width = GetElement('img_width').value;
			        height = GetElement('img_height').value;
			        customWidth = GetElement('img_width_custom').value;
			        customHeight = GetElement('img_height_custom').value;
			        lockRatio = GetElement('img_lockRatio').checked;
    				
			        if (sender.id == 'img_percentage') {
				        FTB_SetImageByPercentage();
			        } else {
    				
				        if (lockRatio) {
					        if (sender.id == 'img_width_custom') {			
						        previewImage.width = customWidth + "px";
						        previewImage.height = height * ( customWidth / width) + "px";
						        GetElement('img_height_custom').value = height * ( customWidth / width);
					        } else if (sender.id == 'img_height_custom') {
						        previewImage.width = width * ( customHeight / height) + "px";		
						        previewImage.height = customHeight + "px";
    							
						        GetElement('img_width_custom').value = width * ( customHeight / height);
					        }
				        } else {
					        previewImage.width = customWidth + "px";
					        previewImage.height = customHeight + "px";
				        }
    					
			        }
		        };
		        function FTB_InsertImage() {
    			
			        image = GetElement('img_preview');
			        src = GetElement('img_preview').src;
			        if (src == '' || src == null) return;
    			
			        alt = GetElement('img_alt').value;
			        title = GetElement('img_title').value;
			        width = image.width;
			        height = image.height;
			        align = GetElement('img_align').options[GetElement('img_align').selectedIndex].value;
    			
			        hspace = GetElement('img_hspace').value;
			        vspace = GetElement('img_vspace').value;
			        border = GetElement('img_border').value;
    			
			        html = '<img src="' + src + 
				        ( (width != '') ? '&x=' + width : '' ) + 
				        ( (height != '') ? '&y=' + height : '' ) + 
				        '"' + 
				        ( (alt != '') ? ' alt="' + alt + '"' : '' ) + 
				        ( (title != '') ? ' title="' + title + '"' : '' ) + 
				        ( (width != '') ? ' width="' + width + '"' : '' ) + 
				        ( (height != '') ? ' height="' + height + '"' : '' ) + 
				        ( (align != '') ? ' align="' + align + '"' : '' ) + 
				        ( (hspace != '') ? ' hspace="' + hspace + '"' : '' ) + 
				        ( (vspace != '') ? ' vspace="' + vspace + '"' : '' ) + 
				        ( (border != '') ? ' border="' + border + '"' : '' ) + 
				        ' />';

			        parentWindow.FileStoreImageGallery_InsertImage(html);
			        window.parent.close();
		        };
		        function NewFile() {
			        OpenDialog("ModalDialogWrapper.axd?EditFile.aspx?folderID=" + currentFolderID, 38.75, 24.80, window);
		        }
		        function EditFileComplete(fileID) {
			        document.location.href = document.location.href;
		        }
		        addEvent(window, "unload", DialogUnload);
	        //]]>
	        </script>

	        <fieldset class="gallery">
		        <legend>Image Gallery</legend>
		        <div id="divGallery" class="gallery">
			        <asp:Literal id="litImageList" runat="server" />
		        </div>
	        </fieldset>
        	
	        <fieldset class="gadget">
		        <legend>Preview</legend>
		        <div class="preview">
			        <img src="../../../Images/clear.gif" alt="Clear" id="img_preview" />
		        </div>
	        </fieldset>
        	
	        <fieldset class="gadget">
		        <legend>Dimensions</legend>
		        <input type="radio" id="img_dim_original" name="img_dim" onclick="FTB_DimensionChange(this);" checked="checked">Original Size</input>
		        <input type="text" id="img_width" class="numberGadget" onchange="FTB_UpdatePreview(this);" disabled="disabled" />x<input type="text" id="img_height" class="numberGadget" onchange="FTB_UpdatePreview(this);" disabled="disabled" />
		        <br />
		        <input type="radio" id="img_dim_custom" name="img_dim" onclick="FTB_DimensionChange(this);">Custom Size</input>
		        <input type="text" id="img_width_custom" class="numberGadget" onkeyup="FTB_UpdatePreview(this);" disabled="disabled" />x<input type="text" id="img_height_custom" style="width:3em;" onkeyup="FTB_UpdatePreview(this);" disabled="disabled" />
		        <br />
		        <input type="checkbox" id="img_lockRatio" checked="checked">Lock image ratio</input>
		        <br />
		        <input type="radio" id="img_dim_percentage" name="img_dim" onclick="FTB_DimensionChange(this);">Percentage</input>
		        <input type="text" id="img_percentage" class="numberGadget" onkeyup="FTB_UpdatePreview(this);" disabled="disabled" />									
	        </fieldset>
        		
	        <fieldset class="gadget">
		        <legend>Properties</legend>
		        Align <select id="img_align">
			        <option value=''>NotSet</option>
			        <option value='top'>Top</option>
			        <option value='bottom'>Bottom</option>
			        <option value='left'>Left</option>
			        <option value='right'>Right</option>
			        <option value='center'>Center</option>
			        <option value='absmiddle'>AbsMiddle</option>
		        </select>
		        <br />
		        Border <input type="text" id="img_border" class="numberGadget" />
		        <br />
		        VSpace <input type="text" id="img_vspace" class="numberGadget" />
		        <br />
		        HSpace <input type="text" id="img_hspace" class="numberGadget" />
		        <br />
		        Alt <input type="text" id="img_alt" class="textGadget" />
		        <br />
		        Title <input type="text" id="img_title" class="textGadget" />
	        </fieldset>
	        <br />
        	
	        <input type="button" value="   Insert   " onclick="FTB_InsertImage();" />
	        <input onclick="GetParentWindow().HideModalDIV();window.parent.close();" type="button" value="  Cancel  " />
	        <br /> 
	        <!--<input onclick="NewFile();" id="btnUpload" type="button" value="  Upload  " />-->
	        <br /><br /><br /><br />
    </asp:Content>