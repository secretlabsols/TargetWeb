﻿var resultsPanel, psResultSettings;

$(function () {

    var resultsControl = 'Results.Controls.ServiceDeliveryFileResults';

    Ext.require(resultsControl)

    Ext.onReady(function () {

        Ext.suspendLayouts();

        var resultsPanel = Ext.create(resultsControl, {
            resultSettings: psResultSettings
        });

        Ext.resumeLayouts(true);

    });

});