
function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}