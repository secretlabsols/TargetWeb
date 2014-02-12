// enum for Deceased Work Tray parameters
Selectors.DeceasedWorkTraySelectorParameters = {
    FilterSUserRef: '@strListFilterSUserRef',
    FilterSUName: '@strListFilterSUName',
    FilterSUNHSNumber: '@strListFilterSUNHSNumber',
    DDateTypeID: '@intDDateTypeID',
    DDateTo: '@DDateTo',
    DDateFrom: '@DDateFrom',
    DRDateTypeID: '@intDRDateTypeID',
    DRDateTo: '@DRDateTo',
    DRDateFrom: '@DRDateFrom',
    WebSecurityUserID: '@webSecurityUserID',
    XmlProcessStatuses: '@xmlProcessStatuses',
    XmlProcessStatusBy: '@xmlProcessStatusBy',
    XmlProcessSubStatuses: '@xmlProcessSubStatuses',
    XmlCareType: '@xmlCareType',
    BillRaised: '@IsBillRaised',
    AccountPaid :'@IsAccountPaid',
    Appointee: '@HasAppointee',
    Property: '@HasProperty',
    OpenSO: '@HasOpenServiceOrders',
    OpenCE: '@HasOpenCareEpisodes'
};

Selectors.DeceasedWorkTraySelector = function(initSettings) {

    var me = this;
    var selectorType = 28;

    // set the type to DeceasedWorkTray always
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
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.FilterSUserRef, value);
    }

    me.SetFilterSUName = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.FilterSUName, value);
    }

    me.SetFilterSUNHSNumber = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.FilterSUNHSNumber, value);
    }

    me.SetDDateType = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DDateTypeID, Selectors.Helpers.ToInt(value));
    }

    me.SetDDateFrom = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DDateFrom, value);
    }

    me.SetDDateTo = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DDateTo, value);
    }

    me.SetDRDateType = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DRDateTypeID, Selectors.Helpers.ToInt(value));
    }

    me.SetDRDateFrom = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DRDateFrom, value);
    }

    me.SetDRDateTo = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.DRDateTo, value);
    }

    me.SetCareType = function (types) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.XmlCareType, types);
    }

    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

    me.SetProcessStatuses = function (types) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessStatuses, types);
    }

    me.SetProcessStatusesBy = function (types) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessStatusBy, types);
    }

    me.SetProcessSubStatuses = function (types) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessSubStatuses, types);
    }

    me.SetBilledUpToDate = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.BillRaised, value);
    }

    me.SetAccountPaid = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.AccountPaid, value);
    }

    me.SetAppointee = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.Appointee, value);
    }

    me.SetPropertyWithCase = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.Property, value);
    }

    me.SetOpenServiceOrders = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.OpenSO, value);
    }

    me.SetOpenCareEpisodes = function (value) {
        me.AddParameter(Selectors.DeceasedWorkTraySelectorParameters.OpenCE, value);
    }

    // getters 


    me.GetFilterSUserRef = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.FilterSUserRef);
    }

    me.GetFilterSUName = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.FilterSUName);
    }

    me.GetFilterSUNHSNumber = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.FilterSUNHSNumber);
    }

    me.GetDDateType = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DDateTypeID);
    }

    me.GetDDateFrom = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DDateFrom);
    }

    me.GetDDateTo = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DDateTo);
    }

    me.GetDRDateType = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DRDateTypeID);
    }

    me.GetDRDateFrom = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DRDateFrom);
    }

    me.GetDRDateTo = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.DRDateTo);
    }

    me.GetCareType = function (types) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.XmlCareType);
    }

    me.GetWebSecurityUserID = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.WebSecurityUserID);
    }

    me.GetProcessStatuses = function (types) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessStatuses);
    }

    me.GetProcessStatusesBy = function (types) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessStatusBy);
    }

    me.GetProcessSubStatuses = function (types) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.XmlProcessSubStatuses);
    }

    me.GetBilledUpToDate = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.BillRaised);
    }

    me.GetAccountPaid = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.AccountPaid);
    }

    me.GetAppointee = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.Appointee);
    }

    me.GetPropertyWithCase = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.Property);
    }

    me.GetOpenServiceOrders = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.OpenSO);
    }

    me.GetOpenCareEpisodes = function (value) {
        return me.GetParameterValue(Selectors.DeceasedWorkTraySelectorParameters.OpenCE);
    }

    me.GetWebServiceParameters = function () {
        var params = {
            FilterSUserRef: me.GetFilterSUserRef(),
            FilterSUName: me.GetFilterSUName(),
            FilterSUNHSNumber: me.GetFilterSUNHSNumber(),
            DDateTo: me.GetDDateTo(),
            DDateFrom: me.GetDDateFrom(),
            DRDateTo: me.GetDRDateTo(),
            DRDateFrom: me.GetDRDateFrom(),
            XmlCareType : me.GetCareType,            
            WebSecurityUserID: me.GetWebSecurityUserID(),
            XmlProcessStatuses: me.GetProcessStatuses(),
            XmlProcessStatusBy: me.GetProcessStatusesBy(),
            XmlProcessSubStatuses: me.GetProcessSubStatuses(),
            BillRaised: me.GetBilledUpToDate(),
            AccountPaid: me.GetAccountPaid(),
            Appointee: me.GetAppointee(),
            PropertyWithCase: me.GetPropertyWithCase(),
            OpenServiceOrders: me.GetOpenServiceOrders(),
            OpenCareEpisodes: me.GetOpenCareEpisodes()
        };

        return params
    }

}

Selectors.DeceasedWorkTraySelector.prototype = new Selectors.SelectorControl();

