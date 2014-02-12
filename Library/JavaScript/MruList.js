
function MruList_TouchItem(mruListKey, itemKey) {
    var mruSvc = new Target.Web.Apps.Mru.WebSvc.Mru_class();
    mruSvc.MruListTouchItem(mruListKey, itemKey);
}

function MruList_ItemSelected(mruListKey, itemKey, ref, name) {
    var mruSvc = new Target.Web.Apps.Mru.WebSvc.Mru_class();
    var itemValue = ref + ": " + name;
    mruSvc.MruListItemSelected(mruListKey, itemKey, itemValue);
}