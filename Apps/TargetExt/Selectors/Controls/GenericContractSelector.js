// enum for generic contract types
Selectors.GenericContractSelectorTypes = {
    Residential: 1,
    NonResidential: 2,
    DirectPayment: 3
};

// enum for generic contract parameters
Selectors.GenericContractSelectorParameters = {
    FrameWorkTypes: '@xmlFrameworkTypes',
    ListFilterContractGroupDescription: '@strListFilterContractGroupDescription',
    ListFilterNumber: '@strListFilterNumber',
    ListFilterServiceGroupDescription: '@strListFilterServiceGroupDescription',
    ListFilterTitle: '@strListFilterTitle',
    ListFilterAdministrativeSector: '@strListFilterAdministrativeSector',
    ProviderID: '@intProviderID',
    Types: '@xmlTypes',
    DateFrom: '@dteDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dteDateTo',
    ServiceGroupID: '@intServiceGroupID',
    ContractTypeStr: '@strContractType',
    ContractGroupID: '@intContractGroupID',
    EndReasonID: '@intContractEndReasonID',
    CreditorID: '@intCreditorID',
    AdministrativeSectorID: '@intAdministrativeSectorID'
};

Selectors.GenericContractSelector = function(initSettings) {

    var me = this;
    var selectorType = 2;

    // set the type to contract always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to contract always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    me.SetProviderID = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.ProviderID, Selectors.Helpers.ToInt(value));
    }

    me.SetFrameWorkTypes = function(types) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.FrameWorkTypes, types);
    }

    me.SetTypes = function(types) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.Types, types);
    }

    // set the service group
    me.SetServiceGroupID = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.ServiceGroupID, value);
    }
    // set the contract type
    me.SetContractTypeStr = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.ContractTypeStr, value);
    }

    // set the contract group id
    me.SetContractGroupID = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.ContractGroupID, value);
    }

    // set the end reason id
    me.SetEndReasonID = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.EndReasonID, value);
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.DateTo, value);
    }

    me.SetCreditorID = function (value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.CreditorID, Selectors.Helpers.ToInt(value));
    }

    // set the contract group id
    me.SetAdministrativeSectorID = function (value) {
        me.AddParameter(Selectors.GenericContractSelectorParameters.AdministrativeSectorID, value);
    }

    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.DateFrom);
    }

    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.DateTo);
    }

    me.GetDatePeriodType = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.DatePeriodType));
    }

    me.GetFrameWorkTypes = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.FrameWorkTypes);
    }

    me.GetListFilterContractGroupDescription = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ListFilterContractGroupDescription);
    }

    me.GetListFilterNumber = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ListFilterNumber);
    }

    me.GetListFilterServiceGroupDescription = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ListFilterServiceGroupDescription);
    }

    me.GetListFilterAdministrativeSector = function () {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ListFilterAdministrativeSector);
    }

    me.GetListFilterTitle = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ListFilterTitle);
    }

    me.GetProviderID = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ProviderID);
    }

    me.GetTypes = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.Types);
    }

    me.GetTypesAsXml = function() {
        return Selectors.Helpers.ToXml(me.GetParameterValue(Selectors.GenericContractSelectorParameters.Types), 'types', 'type');
    }

    me.GetServiceGroupID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.ServiceGroupID));
    }

    me.GetServiceGroupID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.ServiceGroupID));
    }

    me.GetContractTypeStr = function() {
        return me.GetParameterValue(Selectors.GenericContractSelectorParameters.ContractTypeStr);
    }

    me.GetContractGroupID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.ContractGroupID));
    }

    me.GetEndReasonID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.EndReasonID));
    }

    me.GetAdministrativeSectorID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericContractSelectorParameters.AdministrativeSectorID));
    }

}

Selectors.GenericContractSelector.prototype = new Selectors.SelectorControl();
