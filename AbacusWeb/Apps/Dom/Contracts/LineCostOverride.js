var modeEdit = 3, tblOverrides, mode;

$(function() {
    tblOverrides = $('table[id$="_gvOverrides"]');
    mode = parseInt($('input[id$="_hidMode"]').val());
    tblOverrides.attr('disabled', false);
    tblOverrides.find('tr:first > th').attr('disabled', false);
}); 
