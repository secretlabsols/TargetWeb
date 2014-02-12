
// ui dialog extensions

(function($) {
    var _init = $.ui.dialog.prototype._init;
    var disabledClass = 'ui-state-disabled';
    var keyDisplayLoading = 'displayLoading';
    var classButtonPaneLeft = 'ui-dialog-buttonpaneLeft';

    //Custom Dialog Init
    $.ui.dialog.prototype._init = function() {
        var self = this;
        _init.apply(this, arguments);
        //custom init functionality, variables and event binding goes in here
    };
    $.extend($.ui.dialog.prototype, {
        addLeftAlignedButtonPaneControl: function(arguments) {
            var dlg = $($(this)[0].uiDialog);
            var buttonPane = dlg.find('.ui-dialog-buttonpane');
            var buttonPaneLeft = dlg.find('.' + classButtonPaneLeft); 
            if (buttonPaneLeft.length == 0) {
                buttonPaneLeft = $('<div style=\'margin: .5em 0 0 0; padding: .3em 1em .5em .4em;\' />').addClass(classButtonPaneLeft).appendTo(buttonPane);
            }
            buttonPaneLeft.append(arguments.control);
        },
        disableButton: function(arguments) {
            var button = $($(this)[0].uiDialog).find(":button:contains('" + arguments.Text + "')");
            button.attr('disabled', arguments.Disabled);
            if (arguments.Disabled) {
                button.addClass(disabledClass);
            } else {
                button.removeClass(disabledClass);
            }
        },
        disableAllButtons: function(arguments) {
            var buttons = $($(this)[0].uiDialog).find('button');
            buttons.attr('disabled', arguments.Disabled);
            if (arguments.Disabled) {
                buttons.addClass(disabledClass)
            } else {
                buttons.removeClass(disabledClass)
            }
        },
        displayLoading: function(arguments) {
            this.disableAllButtons({ Disabled: arguments.Display });
            var self = $($(this)[0].element);
            var dlgDivLoading = self.data(keyDisplayLoading);
            if (!dlgDivLoading) {
                dlgDivLoading = $('<div class=\'DialogLoading\'><label>Loading.....</label></div>');
                dlgDivLoading.appendTo(self);
            }
            dlgDivLoading.height(self.outerHeight());
            dlgDivLoading.width(self.outerWidth());
            var dlgDivLoadingLabel = dlgDivLoading.find('label');
            if (arguments.Text && arguments.Text != '') {
                dlgDivLoadingLabel.text(arguments.Text);
            } else {
                dlgDivLoadingLabel.text('Loading.....');
            }
            if (arguments.Display === true) {
                dlgDivLoading.show();
            } else {
                dlgDivLoading.hide();
            }
            self.data(keyDisplayLoading, dlgDivLoading);
        }
    });
})(jQuery);

//datePicker Functions/Extensions

function datePickerRestrictDays(date, availableDaysOfWeek) {
    if (availableDaysOfWeek.length > 0) {
        var reqDay = date.getDay();
        for (i = 0; i < availableDaysOfWeek.length; i++) {
            if ($.inArray(reqDay, availableDaysOfWeek) != -1) {
                return [true];
            }
        }
        return [false];
    } else {
        return [true];
    }
}