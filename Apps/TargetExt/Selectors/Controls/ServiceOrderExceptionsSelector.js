// enum for Service Order Exceptions parameters
Selectors.ServiceOrderExceptionsSelectorParameters = {
    FilterSUserRef: '@strListFilterSUserRef',
    FilterSUName: '@strListFilterSUName',
    FilterSUNHSNumber: '@strListFilterSUNHSNumber',
    FilterSORef: '@strListFilterSORef',
    FilterProviderName: '@strListFilterProviderName',
    DateSearchTypeID: '@intDateSearchTypeID',
    DateTo: '@DateTo',
    DateFrom: '@DateFrom',
    WebSecurityUserID: '@webSecurityUserID',
    XmlProcessStatuses: '@xmlProcessStatuses',
    XmlProcessStatusBy: '@xmlProcessStatusBy',
    XmlProcessSubStatuses: '@xmlProcessSubStatuses',
    XmlExceptionType: '@xmlExceptionType',
    XmlWithholdUpdate: '@xmlWithholdUpdate',
    ProviderID: '@intEstablishmentID'
};

Selectors.ServiceOrderExceptionsSelector = function (initSettings) {

    var me = this;
    var selectorType = 27;

    // set the type to ServiceOrderExceptions always
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
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUserRef, value);
    }

    me.SetFilterSUName = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUName, value);
    }

    me.SetFilterSUNHSNumber = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUNHSNumber, value);
    }

    me.SetFilterSORef = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSORef, value);
    }

    me.SetFilterProviderName = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.FilterProviderName, value);
    }

    me.SetDatePeriodType = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.DateSearchTypeID, Selectors.Helpers.ToInt(value));
    }

    me.SetDateTo = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.DateTo, value);
    }

    me.SetDateFrom = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.DateFrom, value);
    }

    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

    me.SetXmlProcessStatuses = function (types) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessStatuses, types);
    }

    me.SetXmlProcessStatusBy = function (types) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessStatusBy, types);
    }

    me.SetXmlProcessSubStatuses = function (types) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessSubStatuses, types);
    }

    me.SetXmlExceptionType = function (types) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.XmlExceptionType, types);
    }

    me.SetXmlWithholdUpdate = function (types) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.XmlWithholdUpdate, types);
    }

    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.ServiceOrderExceptionsSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }


    // get methods
    me.GetFilterSUserRef = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUserRef);
    }

    me.GetFilterSUName = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUName);
    }

    me.GetFilterSUNHSNumber = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSUNHSNumber);
    }

    me.GetFilterSORef = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.FilterSORef);
    }

    me.GetFilterProviderName = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.FilterProviderName);
    }

    me.GetDateSearchTypeID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.DatePeriodType));
    }

    me.GetDateFrom = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.DateFrom);
    }

    me.GetDateTo = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.DateTo);
    }

    me.GetWebSecurityUserID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.WebSecurityUserID));
    }

    me.GetXmlProcessStatuses = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessStatuses);
    }

    me.GetXmlProcessStatusBy = function (value) {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessStatusBy);
    }

    me.GetXmlProcessSubStatuses = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.XmlProcessSubStatuses);
    }

    me.GetXmlExceptionType = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.XmlExceptionType);
    }

    me.GetXmlWithholdUpdate = function () {
        return me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.XmlWithholdUpdate);
    }

    me.GetProviderID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.ServiceOrderExceptionsSelectorParameters.ProviderID));
    }

    me.GetWebServiceParameters = function () {
        var params = {
            FilterSUserRef: me.GetFilterSUserRef(),
            FilterSUName: me.GetFilterSUName(),
            FilterSUNHSNumber: me.GetFilterSUNHSNumber(),
            FilterSORef: me.GetFilterSORef(),
            FilterProviderName: me.GetFilterProviderName(),
            DateTo: me.GetDateTo(),
            DateFrom: me.GetDateFrom(),
            WebSecurityUserID: me.GetWebSecurityUserID(),
            XmlProcessStatuses: me.GetXmlProcessStatuses(),
            XmlProcessStatusBy: me.GetXmlProcessStatusBy(),
            XmlProcessSubStatuses: me.GetXmlProcessSubStatuses(),
            XmlExceptionType: me.GetXmlExceptionType(),
            XmlWithholdUpdate: me.GetXmlWithholdUpdate(),
            ProviderID: me.GetProviderID()
        };

        return params
    }
}

Selectors.ServiceOrderExceptionsSelector.prototype = new Selectors.SelectorControl();

