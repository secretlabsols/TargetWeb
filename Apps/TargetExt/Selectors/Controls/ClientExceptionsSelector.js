// enum for Client Exceptions parameters
Selectors.ClientExceptionsSelectorParameters = {
    FilterSUserRef: "@strListFilterSUserRef",
    FilterSUName: "@strListFilterSUName",
    FilterSUNHSNumber: "@strListFilterSUNHSNumber",
    FilterExceptionMessage: "@strListFilterExceptionMessage",
    DateSearchTypeID:  "@intDateSearchTypeID",
    DateTo: "@DateTo",
    DateFrom: "@DateFrom",
    WebSecurityUserID: "@webSecurityUserID",
    XmlProcessStatuses: "@xmlProcessStatuses",
    XmlProcessStatusBy: "@xmlProcessStatusBy",
    XmlProcessSubStatuses: "@xmlProcessSubStatuses",
    XmlWithholdAcceptance: "@xmlWithholdAcceptance",
    XmlDeceasedOrNot: "@xmlDeceasedOrNot",
    XmlOnAbacusOrNot: "@xmlOnAbacusOrNot",
	DIDateSearchTypeID: "@intDIDateSearchTypeID",
	DIDateFrom: "@DIDateFrom",	
	DIDateTo: "@DIDateTo",
    ClientID : "@intClientID"
};

Selectors.ClientExceptionsSelector = function(initSettings) {

    var me = this;
    var selectorType = 30;

    // set the type to ClientExceptions always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    me.SetFilterSUserRef = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.FilterSUserRef, value);
    }

    me.SetFilterSUName = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.FilterSUName, value);
    }

    me.SetFilterSUNHSNumber = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.FilterSUNHSNumber, value);
    }

    me.SetFilterExceptionMessage = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.FilterExceptionMessage, value);
    }

    me.SetDatePeriodType = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DateSearchTypeID, Selectors.Helpers.ToInt(value));
    }

    me.SetDateTo = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DateTo, value);
    }

    me.SetDateFrom = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DateFrom, value);
    }

    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

    me.SetXmlProcessStatuses = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlProcessStatuses, types);
    }

    me.SetXmlProcessStatusBy = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlProcessStatusBy, types);
    }

    me.SetXmlProcessSubStatuses = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlProcessSubStatuses, types);
    }

    me.SetXmlWithholdAcceptance = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlWithholdAcceptance, types);
    }

    me.SetXmlDeceasedOrNot = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlDeceasedOrNot, types);
    }

    me.SetXmlOnAbacusOrNot = function (types) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.XmlOnAbacusOrNot, types);
    }

    me.SetDIDatePeriodType = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DIDateSearchTypeID, Selectors.Helpers.ToInt(value));
    }

    me.SetDIDateTo = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DIDateTo, value);
    }

    me.SetDIDateFrom = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.DIDateFrom, value);
    }

    me.SetServiceUserID = function (value) {
        me.AddParameter(Selectors.ClientExceptionsSelectorParameters.ClientID, Selectors.Helpers.ToInt(value));
    }


//    // getters
    me.GetFilterSUserRef = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.FilterSUserRef);
    }

    me.GetFilterSUName = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.FilterSUName);
    }

    me.GetFilterSUNHSNumber = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.FilterSUNHSNumber);
    }

    me.GetFilterExceptionMessage = function (value) {
        return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.FilterExceptionMessage);
    }

    me.GSetDatePeriodType = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DateSearchTypeID);
    }

    me.GetDateTo = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DateTo);
    }

    me.GetDateFrom = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DateFrom);
    }

    me.GetWebSecurityUserID = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.WebSecurityUserID);
    }

    me.GetXmlProcessStatuses = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlProcessStatuses);
    }

    me.GetXmlProcessStatusBy = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlProcessStatusBy);
    }

    me.GetXmlProcessSubStatuses = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlProcessSubStatuses);
    }

    me.GetXmlWithholdAcceptance = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlWithholdAcceptance);
    }

    me.GetXmlDeceasedOrNot = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlDeceasedOrNot);
    }

    me.GetXmlOnAbacusOrNot = function (types) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.XmlOnAbacusOrNot);
    }

    me.GetDIDatePeriodType = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DIDateSearchTypeID);
    }

    me.GetDIDateTo = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DIDateTo);
    }

    me.GetDIDateFrom = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.DIDateFrom);
    }

    me.GetServiceUserID = function (value) {
         return me.GetParameterValue(Selectors.ClientExceptionsSelectorParameters.ClientID);
    }

     // get params to be passed about to web services etc
     me.GetWebServiceParameters = function () {
         var params = {
             FilterSUserRef: me.GetFilterSUserRef(),
             FilterSUName: me.GetFilterSUName(),
             FilterSUNHSNumber: me.GetFilterSUNHSNumber(),
             FilterExceptionMessage: me.GetFilterExceptionMessage(),
             DatePeriodType: me.GSetDatePeriodType(),
             DateTo: me.GetDateTo(),
             DateFrom: me.GetDateFrom(),
             WebSecurityUserID: me.GetWebSecurityUserID(),
             XmlProcessStatuses: me.GetXmlProcessStatuses(),
             XmlProcessStatusBy: me.GetXmlProcessStatusBy(),
             XmlProcessSubStatuses: me.GetXmlProcessSubStatuses(),
             XmlWithholdAcceptance: me.GetXmlWithholdAcceptance(),
             XmlDeceasedOrNot: me.GetXmlDeceasedOrNot(),
             XmlOnAbacusOrNot: me.GetXmlOnAbacusOrNot(),
             DIDateTo: me.GetDIDateTo(),
             DIDateFrom: me.GetDIDateFrom(),
             ServiceUserID: me.GetServiceUserID(),
             // added later to set new values
             newStatusId: null,
             newSubStatusId: null,
             newAssignedToId: null
         };
         return params
     }

}

Selectors.ClientExceptionsSelector.prototype = new Selectors.SelectorControl();



