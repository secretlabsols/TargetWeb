
Selectors.ServiceUserSelectorParameters = {
    NameSearchTypeID: '@strNameSearchTypeID',
    NameSearchCriteria: '@strNameSearchCriteria',
    ReferenceSearchTypeID: '@strReferenceSearchTypeID',
    ReferenceSearchCriteria: '@strReferenceSearchCriteria',
    NHSNumberSearchTypeID: '@strNHSNumberSearchTypeID',
    NHSNumberSearchCriteria: '@strNHSNumberSearchCriteria',
    AltRefSearchTypeID: '@strAltRefSearchTypeID',
    AltRefSearchCriteria: '@strAltRefSearchCriteria',
    CreditorRefSearchTypeID: '@strCreditorRefSearchTypeID',
    CreditorRefSearchCriteria: '@strCreditorRefSearchCriteria',
    DebtorRefSearchTypeID: '@strDebtorRefSearchTypeID',
    DebtorRefSearchCriteria: '@strDebtorRefSearchCriteria',
    BirthDateSearchTypeID: '@intBirthDateSearchTypeID',
    BirthDateTo: '@BirthDateTo',
    BirthDateFrom: '@BirthDateFrom',
    DeathDateSearchTypeID: '@intDeathDateSearchTypeID',
    DeathDateTo: '@DeathDateTo',
    DeathDateFrom: '@DeathDateFrom',
    AddressSearchTypeID: '@strAddressSearchTypeID',
    AddressSearchCriteria: '@strAddressSearchCriteria',
    PostcodeSearchTypeID: '@strPostcodeSearchTypeID',
    PostcodeSearchCriteria: '@strPostcodeSearchCriteria',
    Types: '@xmlTypes',
    ShowResidential: '@blnResidential',
    ShowNonResidential: '@blnNonResidential',
    ProviderID: '@intProviderId',
    ContractID: '@intContractId'
};


Selectors.ServiceUserSelector = function(initSettings) {

    var me = this;
    var selectorType = 1;

    // set the type to service user always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType // service user
        }
    }, initSettings);

    // set the type to service user always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    me.GetProviderID = function () {
        return me.GetParameterValue(Selectors.ServiceUserSelectorParameters.ProviderID);
    }

    me.SetProviderID = function (value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ProviderID, value);
    }

    me.SetContractID = function (value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ContractID, value);
    }

    // show only residential clients
    me.SetShowResidential = function (value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ShowResidential, value);
    }
    // show only non ersidential clients
    me.SetShowNonResidential = function (value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ShowNonResidential, value);
    }
    
    me.SetTypes = function(types) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.Types, types);
    }
    // set name search type
    me.SetNameSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.NameSearchTypeID, value);
    }
    me.SetNameSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.NameSearchCriteria, value);
    }
    me.SetReferenceSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ReferenceSearchTypeID, value);
    }
    me.SetReferenceSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.ReferenceSearchCriteria, value);
    }
    me.SetNHSNumberSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.NHSNumberSearchTypeID, value);
    }
    me.SetNHSNumberSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.NHSNumberSearchCriteria, value);
    }
    me.SetAltRefSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.AltRefSearchTypeID, value);
    }
    me.SetAltRefSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.AltRefSearchCriteria, value);
    }
    me.SetCreditorRefSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.CreditorRefSearchTypeID, value);
    }
    me.SetCreditorRefSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.CreditorRefSearchCriteria, value);
    }
    me.SetDebtorRefSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.DebtorRefSearchTypeID, value);
    }
    me.SetDebtorRefSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.DebtorRefSearchCriteria, value);
    }
    me.SetBirthDateSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.BirthDateSearchTypeID, Selectors.Helpers.ToInt(value));
    }
    me.SetBirthDateTo = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.BirthDateTo, value);
    }
    me.SetBirthDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.BirthDateFrom, value);
    }       
    me.SetDeathDateSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.DeathDateSearchTypeID, Selectors.Helpers.ToInt(value));
    }
    me.SetDeathDateTo = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.DeathDateTo, value);
    }
    me.SetDeathDateFrom = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.DeathDateFrom, value);
    }       
    me.SetAddressSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.AddressSearchTypeID, value);
    }
    me.SetAddressSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.AddressSearchCriteria, value);
    }
    me.SetPostcodeSearchTypeID = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.PostcodeSearchTypeID, value);
    }
    me.SetPostcodeSearchCriteria = function(value) {
        me.AddParameter(Selectors.ServiceUserSelectorParameters.PostcodeSearchCriteria, value);
    }
}

Selectors.ServiceUserSelector.prototype = new Selectors.SelectorControl();