
var FileUploader_Form;
var FileUploader_ParentControl;
var FileUploader_MaxFiles;
var FileUploader_UploadID;
var FileUploader_UploadBoxes;
var FileUploader_SelectedFiles;

function FileUploader_Init(form, parentControl, maxFiles) {
	FileUploader_Form = form;
	FileUploader_ParentControl = parentControl;
	FileUploader_MaxFiles = maxFiles;
	FileUploader_UploadID = GenerateGuid();
	FileUploader_UploadBoxes = [];

	FileUploader_Form.action = AddQSParam(RemoveQSParam(FileUploader_Form.action, "uploadId"), "uploadId", FileUploader_UploadID);
	FileUploader_AddUploadBox();
	FileUploader_SelectedFiles = 0;
}
	
function FileUploader_GetIndex(array, item) {
	for (var i = 0; i < array.length; i++)
	{
		if (array[i] == item)
			return i;
	}
}
			
function FileUploader_UploadBox_Changed(e)
{
	var evt = GetEvent(e);
	var target = GetSrcElementFromEvent(evt);
	if (target.value.length == 0 && FileUploader_UploadBoxes.length > 0)
	{
		var index = FileUploader_GetIndex(FileUploader_UploadBoxes, target);

		if (index < FileUploader_UploadBoxes.length - 1)
		{
			FileUploader_UploadBoxes.splice(index, 1);
			target.parentNode.removeChild(target);
			FileUploader_SelectedFiles--;
		}
	}
	else
	{
		FileUploader_SelectedFiles++;
		if (FileUploader_UploadBoxes[FileUploader_UploadBoxes.length - 1].value.length > 0)
			FileUploader_AddUploadBox();	
	}
	if (typeof (FileUploader_UploadBox_Changed_Custom) == "function") {
	    FileUploader_UploadBox_Changed_Custom(target);
	}
}
	
function FileUploader_AddUploadBox() {

	if(FileUploader_UploadBoxes.length == FileUploader_MaxFiles) return false;

	var newRow = document.createElement("div");
	var newBox = document.createElement("input");

	newBox.type = "file";
	newBox.name = "uploadBox" + FileUploader_UploadBoxes.length;
	newBox.className = "uploadBox";
		
	addEvent(newBox, "change", FileUploader_UploadBox_Changed);
	addEvent(newBox, "keyup", FileUploader_UploadBox_Changed);

	newRow.appendChild(newBox);
	FileUploader_ParentControl.appendChild(newRow);
	FileUploader_UploadBoxes.push(newBox);
}
