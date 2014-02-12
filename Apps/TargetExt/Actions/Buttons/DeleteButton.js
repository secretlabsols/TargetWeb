Ext.define('Actions.Buttons.DeleteButton', {
    extend: 'Actions.ActionButton',
    config: {
        autoRegisterEventsControls: [],
        deleteService: null,   // the service to use when deleting...AjaxPro class...Required
        deleteFunction: null,  // the function of the service to call...String...Required
        deleteID: 0,            // the id to delete...Integer...Required
        eventNameOnDeleteButtonIsDeleted: 'onDeleteButtonIsDeleted',
        eventNameOnDeleteButtonIsDeleting: 'onDeleteButtonIsDeleting'
    },
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config, hasID = !(cfg.deleteID <= 0);
        // setup button
        // disable control if no id
        if (!hasID) {
            me.disabled = true;
        }
        me.iconCls = 'BlockedImage';
        me.handler = function() {
            me.confirmDeleteRecord();
        };
        me.text = 'Delete';
        me.tooltip = 'Delete Record';
        // listen to enable evts
        me.addListener('enable', function(cmp) {
            if (!hasID) {
                me.setDisabled(true);
            }
        }, me);
        // init the parent
        me.callParent(arguments);
    },
    // request confirmation of deletion
    confirmDeleteRecord: function() {
        var me = this;
        Ext.Msg.confirm('Delete Record', 'Are you sure you wish to delete this record?.', me.confirmDeleteRecordCallback, me);
    },
    // request confirmation of deletion callback
    confirmDeleteRecordCallback: function(btnResult) {
        var me = this;
        // check that user has confirmed delete
        if (btnResult == 'yes') {
            me.deleteRecord();
        }
    },
    // delete the record using the function passed to the control
    deleteRecord: function() {
        var me = this, cfg = me.config, id = cfg.deleteID;
        // check we have an id of a record to delete
        if (id <= 0) {
            Ext.Msg.alert('deleteID Required', 'Please specify the parameter deleteID.');
            return false;
        }
        // check the delete svc is specified
        if (!cfg.deleteService) {
            Ext.Msg.alert('deleteService Required', 'Please specify the parameter deleteService.');
            return false;
        }
        // check the delete function is specified
        if (!cfg.deleteFunction) {
            Ext.Msg.alert('deleteFunction Required', 'Please specify the parameter deleteFunction.');
            return false;
        }
        // delete the record
        me.raiseOnDeleteButtonIsDeleting();
        // call the configured function of the service
        cfg.deleteService[cfg.deleteFunction].call(cfg.deleteService, id, me.deleteRecordCallBack, me);
    },
    // delete record call back
    deleteRecordCallBack: function(response) {
        var me = response.context, cfg = me.config, success = true;
        // check response and display errors if any
        if (!CheckAjaxResponse(response, cfg.deleteService.url, true)) {
            success = false;
        } else {
            cfg.deleteID = 0;
            me.setDisabled(true);
        }
        // advise observers of success
        me.raiseOnDeleteButtonIsDeleted({
            success: success
        });
    },
    // advise observers we have deleted (or not) a record
    raiseOnDeleteButtonIsDeleted: function(args) {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnDeleteButtonIsDeleted, cfg.autoRegisterEventsControls, args);        
    },
    // advise observers we are deleting a record
    raiseOnDeleteButtonIsDeleting: function() {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnDeleteButtonIsDeleting, cfg.autoRegisterEventsControls);  
    }
});
