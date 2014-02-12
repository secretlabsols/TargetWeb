
function Init() {
    parent.resizeIframe(document.body.scrollHeight, 'ifrNotes');
}

addEvent(window, "load", Init);
