Ext.define("Searchers.Controls.NonResidentialServiceUserPaymentSearcher", {
    extend: "Searchers.SearcherControl",
    requires: [
        "Target.ux.form.field.ClearButton",
        "Target.ux.form.field.DateTimeRange",
        "InPlaceSelectors.Controls.GenericContractInPlaceSelector",
        "InPlaceSelectors.Controls.ProviderInPlaceSelector",
        "InPlaceSelectors.Controls.ServiceUserInPlaceSelector",
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = "NonResidentialServiceUserPaymentItem";
        Ext.define(cfg.modelName, {
            extend: "Ext.data.Model",
            fields: [
                { name: "Provider", mapping: "Provider" },
                { name: "GenericContract", mapping: "GenericContract" },
                { name: "Client", mapping: "Client" },
                { name: "InvoiceNumber", mapping: "InvoiceNumber", type: "string" },
                { name: "PeriodFrom", mapping: "PeriodFrom", type: "date" },
                { name: "PeriodTo", mapping: "PeriodTo", type: "date" },
                { name: "WeekendingFrom", mapping: "WeekendingFrom", type: "date" },
                { name: "WeekendingTo", mapping: "WeekendingTo", type: "date" },
                { name: "Unpaid", mapping: "Unpaid", type: "boolean" },
                { name: "Paid", mapping: "Paid", type: "boolean" },
                { name: "Authorised", mapping: "Authorised", type: "boolean" },
                { name: "Suspended", mapping: "Suspended", type: "boolean" },
                { name: "HideRetracted", mapping: "HideRetracted", type: "boolean" }
            ],
            idProperty: "ID"
        });
        // setup forms
        me.setupFrmGeneral(me.resultSettings);
        // add forms to panel
        me.items = [
            cfg.frmGeneral
        ];
        // load forms with data from model
        me.setSearch(me.resultSettings.SearcherSettings.Item);
        // call parent      
        this.callParent(arguments);
    },
    getSearch: function () 
    {
        var frmGeneral = this.config.frmGeneral.getForm();
        
        var searchRecord;
        var returnRecord;

        if (frmGeneral.isValid()) 
        {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                ProviderID: searchRecord.data.Provider.ID,
                GenericContractID: searchRecord.data.GenericContract.ID,
                ClientID: searchRecord.data.Client.ID,
                InvoiceNumber: searchRecord.data.InvoiceNumber,
                WeekendingFrom: searchRecord.data.WeekendingFrom,
                WeekendingTo: searchRecord.data.WeekendingTo,
                PeriodFrom: searchRecord.data.PeriodFrom,
                PeriodTo: searchRecord.data.PeriodTo,
                Unpaid: searchRecord.data.Unpaid,
                Paid: searchRecord.data.Paid,
                Authorised: searchRecord.data.Authorised,
                Suspended: searchRecord.data.Suspended,
                HideRetracted: searchRecord.data.HideRetracted
            });
        }
        
        return returnRecord;
    },
    resetSearch: function () 
    {    
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.Client = null;
        searchRecord.data.ClientID = 0;

        searchRecord.data.InvoiceNumber = null;
        searchRecord.data.PeriodFrom = null;
        searchRecord.data.PeriodTo = null;
        searchRecord.data.WeekendingFrom = null;
        searchRecord.data.WeekendingTo = null;
        searchRecord.data.Unpaid = true;
        searchRecord.data.Paid = true;
        searchRecord.data.Authorised = true;
        searchRecord.data.Suspended = true;
        searchRecord.data.HideRetracted = true;
        
        frmGeneral.loadRecord(searchRecord);

        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config, defaultLabelWidth = 100;
        if (!cfg.frmGeneral) {
            cfg.frmGeneralProviderSelector = Ext.create("InPlaceSelectors.Controls.ProviderInPlaceSelector", {
                fieldLabel: "Provider",
                labelWidth: 100,
                anchor: "100%",
                name: "Provider",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(true);
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(false);
            // create contract in place selector
            cfg.frmGeneralContractSelector = Ext.create("InPlaceSelectors.Controls.GenericContractInPlaceSelector", {
                fieldLabel: "Contract",
                labelWidth: 100,
                anchor: "100%",
                name: "GenericContract",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            // show only non res contracts
            cfg.frmGeneralContractSelector.getSelector().SetTypes([Selectors.GenericContractSelectorTypes.NonResidential]);
            cfg.frmGeneralProviderSelector.linkToInPlaceContractSelector(cfg.frmGeneralContractSelector, args.SearcherSettings.Item.Provider.ID);   // link contracts selector to provider selector
            // create service user selector
            cfg.frmGeneralServiceUserSelector = Ext.create("InPlaceSelectors.Controls.ServiceUserInPlaceSelector", {
                fieldLabel: "Service User",
                labelWidth: 100,
                anchor: "100%",
                name: "Client",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralServiceUserSelector.getSelector().SetShowNonResidential(true);
            // show only service users that are linked with this Provider
            cfg.frmGeneralProviderSelector.linkToInPlaceServiceUserSelector(cfg.frmGeneralServiceUserSelector, args.SearcherSettings.Item.Provider.ID);   // link provider selector to Service user selector
            cfg.frmGeneralContractSelector.linkToInPlaceServiceUserSelector(cfg.frmGeneralServiceUserSelector, args.SearcherSettings.Item.GenericContract.ID);   // link contracts selector to Service user selector

            cfg.frmInvoiceNumber = Ext.create("Ext.form.field.Text", {
                fieldLabel: "Invoice No",
                name: "InvoiceNumber",
                searchTextBoxConfig: {
                    allowBlank: true 
                }
            });
            cfg.frmInvoiceWeekendingDateRange = Ext.create("Target.ux.form.field.DateTimeRange", {
                showTime: false,
                fieldLabel: "Week Ending",
                dateRangeDateTimeFromControlConfig: {
                    name: "WeekendingFrom"
                },
                dateRangeDateTimeToControlConfig: {
                    name: "WeekendingTo"
                },
                rangeOptionStoreData: [
                    { value: 0, description: "Falling within the period" }
                ]
            });
            cfg.frmInvoiceStatus = Ext.create("Ext.form.CheckboxGroup", {
                columns: 2,
                vertical: true,
                items:
                [
                    {
                        boxLabel: "Unpaid",
                        name: "Unpaid",
                        inputValue: "1",
                        id: "chkUnpaid"
                    },
                    {
                        boxLabel: "Authorised",
                        name: "Authorised",
                        inputValue: "1",
                        id: "chkAuthorised"
                    },
                    {
                        boxLabel: "Paid",
                        name: "Paid",
                        inputValue: "1",
                        id: "chkPaid"
                    },
                    {
                        boxLabel: "Suspended",
                        name: "Suspended",
                        inputValue: "1",
                        id: "chkSuspended"
                    }
                ]
            });
            cfg.frmInvoiceStatusFieldSet = Ext.create("Ext.form.FieldSet", {
                title: "Status",
                collapsible: true,
                items: [ cfg.frmInvoiceStatus ]
            });
            cfg.frmInvoiceDateRange = Ext.create("Target.ux.form.field.DateTimeRange", {
                showTime: false,
                fieldLabel: "Invoice Date",
                dateRangeDateTimeFromControlConfig: {
                    name: "PeriodFrom"
                },
                dateRangeDateTimeToControlConfig: {
                    name: "PeriodTo"
                },
                rangeOptionStoreData: [
                    { value: 0, description: "Falling within the period" }
                ]
            });
            cfg.frmInvoiceDateRangeFieldSet = Ext.create("Ext.form.FieldSet", {
                title: "Period",
                collapsible: true,
                items: [cfg.frmInvoiceDateRange,
                cfg.frmInvoiceWeekendingDateRange]
            });
            cfg.frmInvoiceHideRetracted = Ext.create("Ext.form.CheckboxGroup", {
                columns: 1,
                vertical: true,
                items:
                [
                    {
                        boxLabel: "Hide retracted and retraction invoices",
                        style: "margin-left: 105px",
                        name: "HideRetracted",
                        inputValue: "1",
                        id: "chkHideRetracted"
                    }
                ]
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create("Ext.form.Panel", {
                title: "General",
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmInvoiceNumber,
                    cfg.frmInvoiceStatusFieldSet,
                    cfg.frmInvoiceDateRangeFieldSet,
                    cfg.frmInvoiceHideRetracted
                ],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener("change", function () {
                                var meagain = me;
                                setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);
                            }, me);
                        }
                    }
                }
            });
        }
    },
    setSearch: function (item) {
        var me = this;
        cfg = me.config;
        mdl = Ext.create(cfg.modelName, item);
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneral.resetValueFromWebService();
    }
});


