// Sets the timeout for any web services made via AjaxPro

function setAjaxProTimeout(timeout) 
{
    try {
        AjaxPro.timeoutPeriod = timeout * 1000;
    }
    catch (e) {
        // AJAX Pro is not active for this page, even though ExtJS is in use
        // Not necessarily a problem, so sink the exception
    }
}