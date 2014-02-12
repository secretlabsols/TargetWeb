// enum for Debtor Invoice parameters
Selectors.DebtorInvoiceSelectorParameters = {
	ID: '@intSelectedID',
	DateFrom: '@pdteCreatedDateFrom',
	DateTo: '@pdteCreatedDateTo',
	ServiceUserID: '@pintClientID',
	InvoiceStatus: '@InvStatus',
	InvoiceExclusion: '@pbExclude',
	InvoiceBatching: '@pintBatchSelect',
	IncludeZeroValue: '@pbZeroValue',
	Standard: '@pbStandard',
	Manual: '@pbManual',
	Residential: '@pbResidential',
	Domiciliary: '@pbDomiciliary',
	SDS: '@pbSDS',
	ClientInv: '@pbClient',
	ThirdPartyInv: '@pbThirdParty',
	PropertyInv: '@pbProperty',
	OLAInv: '@pbOLA',
	PenCollectInv: '@pbPenCollect',
	HomeCollectInv: '@pbHomeCollect',
	ListFilterDebtor: '@strListFilterDebtor',
	ListFilterDebtorRef: '@strListFilterDebtorRef',
	ListFilterInvoiceNo: '@strListFilterInvoiceNo'
};

// enum for invoice statuses
Selectors.DebtorInvoiceSelectorStatuses = {
	Actual: '1',
	Provisional: '2',
	Retracted: '3',
	Retract: '4'
};

// enum for invoice batch statuses
Selectors.DebtorInvoiceSelectorBatchStatuses = {
	Batched: '1',
	NotBatched: '2'
};

// enum for invoice excluded statuses
Selectors.DebtorInvoiceSelectorExcludedStatuses = {
	NotExcluded: '1',
	Excluded: '2'
};

Selectors.DebtorInvoiceSelector = function (initSettings) {

	var me = this;
	var selectorType = 10;

	// set the type to DebtorInvoice always
	initSettings = $.extend(true, {
		request: {
			Type: selectorType
		}
	}, initSettings);

	// set the type to service order always
	initSettings.request.Type = selectorType;

	// call the parent constructor
	me.constructor(initSettings);

	// get the date from
	me.GetDateFrom = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.DateFrom);
	}

	me.GetDebtorInvoiceID = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ID);
	}
	// get the date to
	me.GetDateTo = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.DateTo);
	}

	// get the svc user id
	me.GetServiceUserID = function (value) {
		return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ServiceUserID));
	}
	// get the Invoice Status
	me.GetInvoiceStatus = function (value) {
		return Selectors.Helpers.ToInt(me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.InvoiceStatus));
	}
	// get the InvoiceExclusion
	me.GetInvoiceExclusion = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.InvoiceExclusion);
	}
	// get the InvoiceBatching
	me.GetInvoiceBatching = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.InvoiceBatching);
	}
	// get the IncludeZeroValue
	me.GetIncludeZeroValue = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.IncludeZeroValue);
	}
	// get the Standard
	me.GetStandard = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.Standard);
	}
	// get the Manual
	me.GetManual = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.Manual);
	}
	// get the Residential
	me.GetResidential = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.Residential);
	}
	// get the Domiciliary
	me.GetDomiciliary = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.Domiciliary);
	}
	// get the SDS
	me.GetSDS = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.SDS);
	}
	// get the ClientInv
	me.GetClientInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ClientInv);
	}
	// get the ThirdPartyInv
	me.GetThirdPartyInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ThirdPartyInv);
	}
	// get the PropertyInv
	me.GetPropertyInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.PropertyInv);
	}
	// get the OLAInv
	me.GetOLAInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.OLAInv);
	}
	// get the PenCollectInv
	me.GetPenCollectInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.PenCollectInv);
	}
	// get the HomeCollectInv
	me.GetHomeCollectInv = function (value) {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.HomeCollectInv);
	}
	// get the List Filter Debtor
	me.GetListFilterDebtor = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ListFilterDebtor);
	}
	// get the List Filter Debtor Ref
	me.GetListFilterDebtorRef = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ListFilterDebtorRef);
	}
	// get the List Filter Invoice No
	me.GetListFilterInvoiceNo = function () {
		return me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.ListFilterInvoiceNo);
	}

	me.GetActual = function () {
		return me.GetHasStatus(Selectors.DebtorInvoiceSelectorStatuses.Actual);
	}

	me.GetProvisional = function () {
		return me.GetHasStatus(Selectors.DebtorInvoiceSelectorStatuses.Provisional);
	}

	me.GetRetracted = function () {
		return me.GetHasStatus(Selectors.DebtorInvoiceSelectorStatuses.Retracted);
	}

	me.GetViaRetract = function () {
		return me.GetHasStatus(Selectors.DebtorInvoiceSelectorStatuses.Retract);
	}

	me.GetHasStatus = function (status) {
		return Ext.Array.contains(me.GetParameterValue(Selectors.DebtorInvoiceSelectorParameters.InvoiceStatus), status);
	}

	me.GetBatchSel = function () {
		var invoicebatching = me.GetInvoiceBatching(), value = null;
		if (invoicebatching.length == 1 && !invoicebatching[0] == '') {
			value = parseInt(Ext.Array.contains(invoicebatching, Selectors.DebtorInvoiceSelectorBatchStatuses.Batched) ? Selectors.DebtorInvoiceSelectorBatchStatuses.Batched : Selectors.DebtorInvoiceSelectorBatchStatuses.NotBatched);
		}
		return value;
	}

	me.GetExclude = function () {
		var invoiceExclude = me.GetInvoiceExclusion(), value = null;
		if (invoiceExclude.length == 1 && !invoiceExclude[0] == '') {
			value = (Ext.Array.contains(invoiceExclude, Selectors.DebtorInvoiceSelectorExcludedStatuses.NotExcluded) ? false : true);
		}
		return value;
	}

	// set the date from
	me.SetDateFrom = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.DateFrom, value);
	}
	// set the date to
	me.SetDateTo = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.DateTo, value);
	}
	// set the svc user id
	me.SetServiceUserID = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.ServiceUserID, Selectors.Helpers.ToInt(value));
	}
	// set the Invoice Status
	me.SetInvoiceStatus = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.InvoiceStatus, value);
	}
	// set the InvoiceExclusion
	me.SetInvoiceExclusion = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.InvoiceExclusion, value);
	}
	// set the InvoiceBatching
	me.SetInvoiceBatching = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.InvoiceBatching, value);
	}
	// set the IncludeZeroValue
	me.SetIncludeZeroValue = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.IncludeZeroValue, value);
	}
	// set the Standard
	me.SetStandard = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.Standard, value);
	}
	// set the Manual
	me.SetManual = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.Manual, value);
	}
	// set the Residential
	me.SetResidential = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.Residential, value);
	}
	// set the Domiciliary
	me.SetDomiciliary = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.Domiciliary, value);
	}
	// set the SDS
	me.SetSDS = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.SDS, value);
	}
	// set the ClientInv
	me.SetClientInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.ClientInv, value);
	}
	// set the ThirdPartyInv
	me.SetThirdPartyInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.ThirdPartyInv, value);
	}
	// set the PropertyInv
	me.SetPropertyInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.PropertyInv, value);
	}
	// set the OLAInv
	me.SetOLAInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.OLAInv, value);
	}
	// set the PenCollectInv
	me.SetPenCollectInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.PenCollectInv, value);
	}
	// set the HomeCollectInv
	me.SetHomeCollectInv = function (value) {
		me.AddParameter(Selectors.DebtorInvoiceSelectorParameters.HomeCollectInv, value);
	}

}

Selectors.DebtorInvoiceSelector.prototype = new Selectors.SelectorControl();

