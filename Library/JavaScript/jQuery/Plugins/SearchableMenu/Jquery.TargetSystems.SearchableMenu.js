(function($) {

    $.fn.searchableMenu = function(items, options) {

        var keyRoot = 'TargetSystems.';
        var keyMenu = keyRoot + 'Menu';
        var menuBlurredHandler = function(event, menu) { menuBlurred(event, menu) };

        function getMenuConfig(src) {
            return src.data(keyMenu);
        }

        function menuBlurred(event) {
            var src = event.data.src;
            var menu = getMenuConfig(src).menu;
            if (event.target != src[0] && $(event.target).parents().is(menu) == false) {
                menu.hide();
            }
        }

        function menuItemClicked(src, menuItem) {
            var menu = getMenuConfig(src).menu;
            menu.hide();
            src.trigger('MenuItemClicked', menuItem);
        }

        function menuSearcherKeyUp(src) {
            var menuCfg = getMenuConfig(src);
            var menuItems = menuCfg.menuItems;
            var menuItemsNone = menuCfg.menuItemsNone;
            var filter = menuCfg.menuSearcher.val(), count = 0;
            menuItems.children().each(function() {
                if ($(this).text().search(new RegExp(filter, "i")) < 0) {
                    $(this).hide();
                } else {
                    $(this).show();
                    count++;
                }
            });
            if (count > 0) {
                menuItems.show();
                menuItemsNone.hide();
            } else {
                menuItems.hide();
                menuItemsNone.show();
            }
        }

        function sourceClicked(event, src) {
            var menuCfg = getMenuConfig(src);
            var menu = menuCfg.menu;
            var menuSearcher = menuCfg.menuSearcher;
            var x = 0;
            var y = 0;

            if (menuCfg.options.position) {
                switch (menuCfg.options.position) {
                    case 'BottomRight':
                        x = src.offset().left - (menu.outerWidth() - src.outerWidth());
                        y = src.offset().top + src.outerHeight();
                        break;
                    case 'BottomLeft':
                        x = src.offset().left;
                        y = src.offset().top + src.outerHeight();
                        break;
                    case 'TopRight':
                        x = src.offset().left - (menu.outerWidth() - src.outerWidth());
                        y = src.offset().top - menu.outerHeight();
                        break;
                    case 'TopLeft':
                        x = src.offset().left;
                        y = src.offset().top - menu.outerHeight();
                        break;
                }
            } else {
                y = event.pageY;
                x = event.pageX
            }

            menu.css('position', 'absolute')
                    .css('top', y + 'px')
                    .css('left', x + 'px')
                    .show();

            menuSearcher.focus()
            .select();
            if (menuCfg.isBlurBound != true) {
                $(document).bind('click', { src: src }, menuBlurredHandler);
            }
            menuCfg.isBlurBound = true;
        }

        return this.each(function() {

            var $this = $(this);
            var menuConfig = getMenuConfig($this);
            var $menuItem = null;
            var defaultSettings = {
                cssClass: '',
                position: null,
                showSearchBox: true,
                width: '300',
                shouldStopPropagation: true
            };

            if (options) {
                $.extend(defaultSettings, options);
            }

            if (!menuConfig) {

                menuConfig = {
                    source: $this,
                    menu: $(document.createElement('div')),
                    menuItems: $(document.createElement('ul')),
                    menuItemsContainer: $(document.createElement('div')),
                    menuItemsNone: $(document.createElement('div')),
                    menuSearcher: $(document.createElement('input')),
                    menuSearcherContainer: $(document.createElement('div')),
                    isBlurBound: false,
                    options: defaultSettings
                }

                menuConfig.menu.css('width', menuConfig.options.width)

                $('body').append(menuConfig.menu);
                menuConfig.menu.append(menuConfig.menuSearcherContainer);
                menuConfig.menuSearcherContainer.append(menuConfig.menuSearcher)
                .addClass('MenuSearcherContainer');

                if (!menuConfig.options.showSearchBox) {
                    menuConfig.menuSearcherContainer.hide();
                    menuConfig.menu.css('border-top-width', 0);
                }

                menuConfig.menu.append(menuConfig.menuItemsContainer)
                .addClass(menuConfig.options.cssClass)
                .hide();

                menuConfig.menuItemsContainer.addClass('MenuItems')
                .append(menuConfig.menuItems)
                .append(menuConfig.menuItemsNone);

                menuConfig.menuItemsNone.html('No matching items...')
                .addClass('MenuItemsNone')
                .hide();

                menuConfig.menuSearcher.keyup(function() { menuSearcherKeyUp($this) });
                menuConfig.menuSearcher.attr('title', 'Type here to filter items...')
                .addClass('MenuSearcher');

                $this.click(function(e) { sourceClicked(e, $this); if (menuConfig.options.shouldStopPropagation) { e.stopPropagation(); } });
                $this.data(keyMenu, menuConfig);

            } else {
                menuConfig.menuSearcher.val('');
            }

            menuConfig.menuItems.children().remove();
            menuConfig.menuItems.hide();
            menuConfig.menuItemsNone.hide()

            if (items) {
                $.each(items,
                            function(intIndex, objValue) {
                                $menuItem = $(document.createElement('li'));
                                $menuItem.html((objValue.description) ? objValue.description : '')
                                .addClass('MenuItem')
                                .addClass((objValue.cssClass) ? objValue.cssClass : '')
                                .attr('title', (objValue.tooltip) ? objValue.tooltip : '');
                                $menuItem.click(function() { menuItemClicked($this, objValue); });
                                menuConfig.menuItems.append($menuItem);
                            }
                        );
                menuConfig.menuItems.show();
            } else {
                menuConfig.menuItemsNone.show();
            }

        });

    };
})(jQuery);