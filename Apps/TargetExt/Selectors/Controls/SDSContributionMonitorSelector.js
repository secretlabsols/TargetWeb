// enum for SDS Contribution parameters
Selectors.SDSContributionSelectorParameters = {
    ColumnFilterServiceUserName: '@strListFilterServiceUserName',
    ColumnFilterServiceUserReference: '@strListFilterServiceUserReference',
    ColumnFilterNHSNumber: '@strListFilterNHSNumber',
    ServiceUsersAreBeingCollected: '@serviceUsersAreBeingCollected',
    ServiceUsersRequiringContributionNotificationLetter: '@serviceUsersRequiringContributionNotificationLetter',
    ServiceUsersHavingOneOrMoreIncompleteContributionLevel: '@serviceUsersHavingOneOrMoreIncompleteContributionLevel',
    ServiceUsersHavingOneOrMissingOrProvAssessment: '@serviceUsersHavingOneOrMissingOrProvAssessment',
    ServiceUsersHavingNilValueCostAssessment: '@serviceUsersHavingNilValueCostAssessment'
    
};

Selectors.SDSContributionMonitorSelector = function(initSettings) {

    var me = this;
    var selectorType = 14;

    // set the type to SDS Contribution always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to SDS Contribution always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // get the Service Users Are Being Collected parameter
    me.GetServiceUsersAreBeingCollected = function() {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ServiceUsersAreBeingCollected);
    }

    // get the Service Users Requiring Contribution Notification Letter parameter
    me.GetServiceUsersRequiringContributionNotificationLetter = function(value) {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ServiceUsersRequiringContributionNotificationLetter);
    }

    // get the Service Users Having One Or More Incomplete ContributionLevel parameter
    me.GetServiceUsersHavingOneOrMoreIncompleteContributionLevel = function() {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingOneOrMoreIncompleteContributionLevel);
    }

    // get the Service Users Having One Or Missing Or ProvAssessment parameter
    me.GetServiceUsersHavingOneOrMissingOrProvAssessment = function() {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingOneOrMissingOrProvAssessment);
    }

    // get the Service Users Having Nil Value Cost Assessment parameter
    me.GetServiceUsersHavingNilValueCostAssessment = function() {
         return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingNilValueCostAssessment);
    }

     // get the service user name column filter
     me.GetColumnFilterServiceUserName = function() {
         return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ColumnFilterServiceUserName);
     }

     // get the service user ref column filter
     me.GetColumnFilterServiceUserReference = function() {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ColumnFilterServiceUserReference);
     }

     // get the nhs number column filter
     me.GetColumnFilterNHSNumber = function() {
        return me.GetParameterValue(Selectors.SDSContributionSelectorParameters.ColumnFilterNHSNumber);
     }
     
    // set the Service Users Are Being Collected
    me.SetServiceUsersAreBeingCollected = function(value) {
        me.AddParameter(Selectors.SDSContributionSelectorParameters.ServiceUsersAreBeingCollected, value);
    }

    // set the Service Users Requiring Contribution Notification Letter
    me.SetServiceUsersRequiringContributionNotificationLetter = function(value) {
        me.AddParameter(Selectors.SDSContributionSelectorParameters.ServiceUsersRequiringContributionNotificationLetter, value);
    }

    // set the Service Users Having One Or More Incomplete Contribution Level
    me.SetServiceUsersHavingOneOrMoreIncompleteContributionLevel = function(value) {
        me.AddParameter(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingOneOrMoreIncompleteContributionLevel, value);
    }

    // set the Service Users Having One Or Missing Or ProvAssessment
    me.SetServiceUsersHavingOneOrMissingOrProvAssessment = function(value) {
    me.AddParameter(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingOneOrMissingOrProvAssessment, value);
    }

    // set the Service Users Having Nil Value Cost Assessment
    me.SetServiceUsersHavingNilValueCostAssessment = function(value) {
        me.AddParameter(Selectors.SDSContributionSelectorParameters.ServiceUsersHavingNilValueCostAssessment, value);
    }

}

Selectors.SDSContributionMonitorSelector.prototype = new Selectors.SelectorControl();

