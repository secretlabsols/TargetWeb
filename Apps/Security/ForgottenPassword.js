
var captchaHelp, captchaHelpLink;

function Init() {
    captchaHelp = GetElement("captchaHelp", true);
    captchaHelpLink = GetElement("captchaHelpLink", true);
}

function ShowCaptchaHelp() {
    captchaHelp.style.display = "block";
    captchaHelpLink.style.display = "none";
}

addEvent(window, "load", Init);