// enum for Generic Creditor Payment parameters
Selectors.GenericCreditorPaymentSelectorParameters = {
    ColumnFilterContractNumber: '@slfContractNumber',
    ColumnFilterCreditorName: '@slfCreditorName',
    ColumnFilterCreditorReference: '@slfCreditorRef',
    ColumnFilterPaymentNumber: '@slfPaymentNumber',
    ColumnFilterServiceUserName: '@slfServiceUser',
    DateFrom: '@dtPaymentDateFrom',
    DatePeriodType: '@intDatePeriodType',
    DateTo: '@dtPaymentDateTo',
    Excluded: '@blnIncludeExcludedFromCreditors',
    GenericContractID: '@intGenericContractID',
    GenericCreditorID: '@intGenericCreditorID',
	IncludeNonResidential: '@blnIncludeNonResidential',
	IncludeDirectPayment: '@blnIncludeDirectPayment',
    NumberAuthorised: '@intNumberOfAuthorised',
    NumberPaid: '@intNumberOfPaid',
    NumberSuspended: '@intNumberOfSuspended',
    NumberUnpaid: '@intNumberOfUnpaid',
	StatusDateFrom: '@dtPaymentStatusDateFrom',
	StatusDatePeriodType: '@intStatusDatePeriodType',
	StatusDateTo: '@dtPaymentStatusDateTo',
	StatusIncludeAuthorised: '@blnIncludeAuthorised',
	StatusIncludePaid: '@blnIncludePaid',
	StatusIncludeSuspended: '@blnIncludeSuspended',
	StatusIncludeSuspendedManually: '@blnIncludeManuallySuspended',
	StatusIncludeUnpaid: '@blnIncludeUnpaid',
	SuspensionReasons: '@additionalFilter',    
    WebSecurityUserID: '@webSecurityUserID'
};

// enum for payment types
Selectors.GenericCreditorPaymentSelectorTypes = {
    NonResidential: 2,
    DirectPayment: 3
};

Selectors.GenericCreditorPaymentSelector = function(initSettings) {

    var me = this;
    var selectorType = 18;

    // set the type to GenericCreditorPayment always
    initSettings = $.extend(true, {
        request: {
            Type: selectorType
        }
    }, initSettings);

    // set the type to service order always
    initSettings.request.Type = selectorType;

    // call the parent constructor
    me.constructor(initSettings);

    // gets any meta data for commitment indicators
    me.GetCommitmentIndicators = function() {
        var indicators = [
            {
                colour: '#C94421',
                tooltip: 'No Service Order',
                value: 16
            },
            {
                colour: '#D89684',
                tooltip: 'Cost Exceeded',
                value: 4
            },
            {
                colour: '#F1D413',
                tooltip: 'Different Service Delivered within Cost',
                value: 2
            },
            {
                colour: '#7AE45A',
                tooltip: 'Excess Tolerated',
                value: 8
            },
            {
                colour: '#338F17',
                tooltip: 'Within Commitment',
                value: 1
            }
        ];
        return indicators;
    }

    // render suspension reasons with icons rather than text
    me.RendererSuspensionReasonBitWise = function(val, md, rcd) {
        var cellHtml = '';
        if (val > 0) {
            var items = 0;
            var tpl = new Ext.XTemplate('<div ',
                                            'class=\'Selectors GenericCreditorPayment-SuspensionReasonBitWiseIndicator \'',
                                            'style=\'background-color: {colour};\' ',
                                            'title=\'{tooltip}\'>',
                                        '</div>');
            Ext.Array.each(me.GetCommitmentIndicators(), function(indVal, indIdx) {
                if ((indVal.value & val) == indVal.value) {
                    items++;
                    cellHtml += tpl.apply(indVal);
                }
            });
            cellHtml = '<div style=\'height: 14px; width: ' + (items * 20) + 'px;\'>' + cellHtml + '</div>';
        }
        return cellHtml;
    }

    // get the contract number column filter
    me.GetColumnFilterContractNumber = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.ColumnFilterContractNumber);
    }

    // get the creditor name column filter
    me.GetColumnFilterCreditorName = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.ColumnFilterCreditorName);
    }

    // get the creditor ref column filter
    me.GetColumnFilterCreditorReference = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.ColumnFilterCreditorReference);
    }

    // get the payment number column filter
    me.GetColumnFilterPaymentNumber = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.ColumnFilterPaymentNumber);
    }

    // get the service user name column filter
    me.GetColumnFilterServiceUserName = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.ColumnFilterServiceUserName);
    }

    // get the date from
    me.GetDateFrom = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.DateFrom);
    }

    // get the date to
    me.GetDateTo = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.DateTo);
    }

    // get the date period type
    me.GetDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.DatePeriodType));
    }

    // get the generic contract id
    me.GetGenericContractID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.GenericContractID));
    }

    // get the generic creditor id
    me.GetGenericCreditorID = function() {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.GenericCreditorID));
    }

    // get the excluded flag
    me.GetExcludeFromCreditors = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.Excluded);
    }

    // get if we include direct payments
    me.GetIncludeDirectPayments = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.IncludeDirectPayment);
    }

    // get if we include non res payments
    me.GetIncludeNonResidentialPayments = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.IncludeNonResidential);
    }

    // get number of authorised payments
    me.GetNumberAuthorised = function() {
        return me.GetAdditionalItemAsInt(Selectors.GenericCreditorPaymentSelectorParameters.NumberAuthorised);
    }

    // get number of paid payments
    me.GetNumberPaid = function() {
        return me.GetAdditionalItemAsInt(Selectors.GenericCreditorPaymentSelectorParameters.NumberPaid);
    }

    // get number of suspended payments
    me.GetNumberSuspended = function() {
        return me.GetAdditionalItemAsInt(Selectors.GenericCreditorPaymentSelectorParameters.NumberSuspended);
    }

    // get number of unpaid payments
    me.GetNumberUnpaid = function() {
        return me.GetAdditionalItemAsInt(Selectors.GenericCreditorPaymentSelectorParameters.NumberUnpaid);
    }

    // get additional item as an integer
    me.GetAdditionalItemAsInt = function(key) {
        var addItem = me.GetAdditionalItem(key);
        return (addItem ? Selectors.Helpers.ToInt(addItem.Value) : 0);
    }

    // get the status date from
    me.GetStatusDateFrom = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusDateFrom);
    }

    // get the status date to
    me.GetStatusDateTo = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusDateTo);
    }

    // get the status date period type
    me.GetStatusDatePeriodType = function(value) {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusDatePeriodType));
    }

    // get if we include payment of status authorised
    me.GetStatusIncludeAuthorised = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeAuthorised);
    }

    // get if we include payment of status manually suspended
    me.GetStatusIncludeManuallySuspended = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeSuspendedManually);
    }

    // get if we include payment of status paid
    me.GetStatusIncludePaid = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludePaid);
    }

    // get if we include payment of status suspended
    me.GetStatusIncludeSuspended = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeSuspended);
    }

    // get if we include payment of status unpaid
    me.GetStatusIncludeUnpaid = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeUnpaid);
    }

    // get the suspension reasons
    me.GetSuspensionReasons = function() {
        return me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.SuspensionReasons);
    }

    //get web security id
    me.GetWebSecurityUserID = function () {
        return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.GenericCreditorPaymentSelectorParameters.WebSecurityUserID));
    }

    // get creditor payment params to be passed about to web services etc
    me.GetWebServiceParameters = function() {
        var params = {
            DateFrom: me.GetDateFrom(),
            DatePeriodType: me.GetDatePeriodType(),
            DateTo: me.GetDateTo(),
            Excluded: me.GetExcludeFromCreditors(),
            GenericContractID: me.GetGenericContractID(),
            GenericCreditorID: me.GetGenericCreditorID(),
            StatusDateFrom: me.GetStatusDateFrom(),
            StatusDatePeriodType: me.GetStatusDatePeriodType(),
            StatusDateTo: me.GetStatusDateTo(),
            StatusIncludeAuthorised: me.GetStatusIncludeAuthorised(),
            StatusIncludePaid: me.GetStatusIncludePaid(),
            StatusIncludeSuspended: me.GetStatusIncludeSuspended(),
            StatusIncludeSuspendedManually: me.GetStatusIncludeManuallySuspended(),
            StatusIncludeUnpaid: me.GetStatusIncludeUnpaid(),
            SuspensionReasons: me.GetSuspensionReasons(),
            TypesIncludeDirectPayments: me.GetIncludeDirectPayments(),
            TypesIncludeNonResidentialPayments: me.GetIncludeNonResidentialPayments(),
            WebSecurityUserID: me.GetWebSecurityUserID()
        };
        return params
    }

    // get if has authorised payments
    me.HasAuthorisedPayments = function() {
        return (me.GetNumberAuthorised() > 0);
    }

    // get if has paid payments
    me.HasPaidPayments = function() {
        return (me.GetNumberPaid() > 0);
    }

    // get if has suspended payments
    me.HasSuspendedPayments = function() {
        return (me.GetNumberSuspended() > 0);
    }

    // get if has unpaid payments
    me.HasUnpaidPayments = function() {
        return (me.GetNumberUnpaid() > 0);
    }

    // set excluded
    me.SetExcluded = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.Excluded, value);
    }

    // set the date from
    me.SetDateFrom = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.DateFrom, value);
    }

    me.SetDatePeriodType = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.DatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the date to
    me.SetDateTo = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.DateTo, value);
    }

    // set the generic contract id
    me.SetGenericContractID = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.GenericContractID, Selectors.Helpers.ToInt(value));
    }

    // set the generic creditor id
    me.SetGenericCreditorID = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.GenericCreditorID, Selectors.Helpers.ToInt(value));
    }

    // set the status date from
    me.SetStatusDateFrom = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusDateFrom, value);
    }

    // set the status date period type
    me.SetStatusDatePeriodType = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusDatePeriodType, Selectors.Helpers.ToInt(value));
    }

    // set the status date to
    me.SetStatusDateTo = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusDateTo, value);
    }

    // set whether to include authorised payments
    me.SetStatusIncludeAuthorised = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeAuthorised, value);
    }

    // set whether to include paid payments
    me.SetStatusIncludePaid = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludePaid, value);
    }

    // set whether to include suspended payments
    me.SetStatusIncludeSuspended = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeSuspended, value);
    }

    // set whether to include suspended manually payments
    me.SetStatusIncludeSuspendedManually = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeSuspendedManually, value);
    }

    // set whether to include unpaid payments
    me.SetStatusIncludeUnpaid = function(value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.StatusIncludeUnpaid, value);
    }

    // set suspension reasons
    me.SetSuspensionReasons = function(val) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.SuspensionReasons, val);
    }

    // set the web security user id
    me.SetWebSecurityUserID = function (value) {
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.WebSecurityUserID, Selectors.Helpers.ToInt(value));
    }

    // set types of creditor payments
    me.SetSelectorTypes = function(val) {
        var includesNonRes, includeDp;
        val = (val || []);
        includesNonRes = Ext.Array.contains(val, Selectors.GenericCreditorPaymentSelectorTypes.NonResidential);
        includeDp = Ext.Array.contains(val, Selectors.GenericCreditorPaymentSelectorTypes.DirectPayment);
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.IncludeNonResidential, includesNonRes);
        me.AddParameter(Selectors.GenericCreditorPaymentSelectorParameters.IncludeDirectPayment, includeDp);
    }

}

Selectors.GenericCreditorPaymentSelector.prototype = new Selectors.SelectorControl();

