$.fn.qtip.defaults = $.extend(true, {}, $.fn.qtip.defaults, {
    content: { height : 400 },
    position: {
        my: 'bottom left',
        at: 'top center',
        viewport: $(window)
    },
    show: {
        solo: true,     
        delay: 150
    },
    hide: {
        fixed: true,
        delay: 400
    },
    style: {
        classes: 'ui-tooltip-shadow',
        widget: true
    },
    overwrite: false
});

function getToolTipNameValueContent(options) {
    var content = $('<div class=\'tooltipContent\' />');
    var defaultOptions = {
        items: null,
        maxHeight: ''
    };
    if (options) {
        $.extend(defaultOptions, options);
    }
    var items = defaultOptions.items;
    if (items) {
        var contentTable = $('<table class=\'tooltipTable\' cellspacing=\'0\' cellpadding=\'2\' style=\'width: 100%;\' />').appendTo(content);
        $.each(items, function(itemIdx, itemVal) {
            var contentTableRow = $('<tr />').appendTo(contentTable);
            var item = { name: '', value: '' };
            if (itemVal) {
                $.extend(item, itemVal);
            }
            contentTableRow.append($('<td class=\'tooltipName\'>' + ((item.name) ? item.name : '&nbsp;') + '</td>'));
            contentTableRow.append($('<td class=\'tooltipValue\'>' + ((item.value) ? item.value : '&nbsp;') + '</td>'));
            if (itemIdx === (items.length - 1)) {
                contentTableRow.find('td').css('border-bottom', '');
            }
        });
    } else {
        content.html('No items to display...');
    }
    if (defaultOptions.maxHeight) {
        content.css('max-height', defaultOptions.maxHeight);
    }
    return content;
}







