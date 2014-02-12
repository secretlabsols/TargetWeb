function ShowComment(commentText) {
    var d = new Apps.Dom.ProformaInvoice.Comment.Dialog("Comment", "Comment:", commentText);
    d.SetCallback(ShowQuery_Callback)
    d.SetType(1);
    d.Show();
}
function ShowQuery_Callback(evt, args) {
    var d = args[0];
    d.Hide();
}
