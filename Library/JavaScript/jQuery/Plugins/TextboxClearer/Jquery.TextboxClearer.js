(function($) {

    var $classHard = 'textboxClearerHard';
    var $classSoft = 'textboxClearerSoft';
    var optionsKey = 'textboxClearerOptions';

    $.fn.textboxClearer = function(method, options) {

        return this.each(function() {

            var $this = $(this);

            if ($this.is('input:text')) {
                if (methods[method]) {
                    return methods[method].apply($this, [options]);
                } else if (typeof method === 'object' || !method) {
                    return methods.init.apply($this, [method]);
                } else {
                    alert('Method ' + method + ' does not exist on jQuery.textboxClearer');
                }
            } else {
                alert('jQuery.textboxClearer can only used on textboxes.');
            }
        });

    };

    function getOptions(ctrl) {
        return ctrl.data(optionsKey);
    }

    function setLocation(ctrl, link) {
        link.css('left', (ctrl.innerWidth() - 16).toString() + 'px');
    }

    function setOptions(ctrl, options) {
        ctrl.data(optionsKey, options);
    }

    function shouldShowLink(ctrl, link) {
        if (ctrl.val() != '' && ctrl.is(':disabled') == false) {
            return true;
        }
        return false;
    }

    var methods = {
        init: function(options) {
            var $ctrl = $(this);
            var $ctrlOptions = getOptions($ctrl);
            if (!$ctrlOptions) {
                var $linkContainer = $('<div class=\'textboxClearerContainer\'>');
                var $link = $('<a href=\'clear\' title=\'Clear?\' class=\'textboxClearer\'>test</a>');
                var $tmpOptions = {
                    link: $link
                };
                if (options) {
                    $.extend($tmpOptions, options);
                }
                $ctrl.wrap($linkContainer);
                $link.insertAfter($ctrl);
                setLocation($ctrl, $link);
                $link.show();
                $link.click(function() {
                    $ctrl.val('');
                    FireEvent($ctrl[0], 'change');
                    FireEvent($ctrl[0], 'keyup');
                    return false;
                });
                $link.mouseover(function() {
                    setLocation($ctrl, $link);
                    if (shouldShowLink($ctrl, $link)) {
                        $link.addClass($classHard);
                    }
                });
                $link.mouseout(function(e) {
                    $link.removeClass($classHard);
                });
                $ctrl.mouseover(function() {
                    setLocation($ctrl, $link);
                    if (shouldShowLink($ctrl, $link)) {
                        $link.addClass($classSoft);
                    }
                });
                $ctrl.mouseout(function() {
                    $link.removeClass($classSoft);
                });
                setOptions($ctrl, $tmpOptions);
            }
        }
    };

})(jQuery); 

