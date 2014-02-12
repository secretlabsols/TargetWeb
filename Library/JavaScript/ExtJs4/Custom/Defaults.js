$(function () {

    var targetExtPath = SITE_VIRTUAL_ROOT + 'Apps/TargetExt';

    Ext.Loader.setConfig({ enabled: true, disableCaching: (window.location.toString().indexOf('http://localhost') == 0) });
    //** To Disable caching Comment out the above line and uncoment the line below
    //Ext.Loader.setConfig({ enabled: true, disableCaching: false });

    Ext.resetElement = Ext.getBody();

    Ext.Loader.setPath('Ext', SITE_VIRTUAL_ROOT + 'Library/JavaScript/ExtJs4/Source/src');
    Ext.Loader.setPath('Ext.ux', SITE_VIRTUAL_ROOT + 'Library/JavaScript/ExtJs4/Source/ux');
    Ext.Loader.setPath('Target.ux', SITE_VIRTUAL_ROOT + 'Library/JavaScript/ExtJs4/Custom/ux');
    Ext.Loader.setPath('Selectors', targetExtPath + '/Selectors');
    Ext.Loader.setPath('InPlaceSelectors', targetExtPath + '/InPlaceSelectors');
    Ext.Loader.setPath('Searchers', targetExtPath + '/Searchers');
    Ext.Loader.setPath('Actions', targetExtPath + '/Actions');
    Ext.Loader.setPath('Results', targetExtPath + '/Results');
    Ext.Loader.setPath('Reports', targetExtPath + '/Reports');
    Ext.Loader.setPath('TargetExt', targetExtPath);

    Ext.QuickTips.init();

    Ext.override(Ext.menu.Item, {
        setHref: function (href) {
            href = (href || '#');
            this.el.dom.children[0].href = href;
        }
    });

    Ext.override(Ext.form.field.Display, {
        setRawValue: function (value) {
            if (this.format) {
                var displayValue = value;
                switch (this.format) {
                    case 'date':
                        displayValue = Ext.util.Format.date(value, (this.formatString || 'd/m/Y'));
                        break;
                    case 'currency':
                        displayValue = Ext.util.Format.currency(value, '£', 2);
                        break;
                }
                this.callParent([displayValue]);
            } else {
                this.callParent(arguments);
            }
        }
    });

    Ext.override(Ext.form.ComboBox, {
        forceSelection: true,
        initComponent: function () {
            var me = this;
            if (me.groupingFunction) {
                me.listConfig = {
                    tpl: Ext.create('Ext.XTemplate',
							  '<ul><tpl for=".">',
							  '<tpl if="xindex == 1 || this.getGroupStr(parent[xindex - 2]) != this.getGroupStr(values)">',
							  '<li class="x-combo-list-group"><b>{[this.getGroupStr(values)]}</b></li>',
							  '</tpl>',
							  '<li role="option" class="x-boundlist-item" style="padding-left: 12px">{' + me.displayField + '}</li>',
							  '</tpl>' +
							  '</ul>',
							  {
							      getGroupStr: function (item) {
							          return me.groupingFunction(item);
							      }
							  }
					)
                }
            }
            me.callParent(arguments);
        },
        setReadOnly: function (isReadOnly) {
            var me = this;
            if (!isReadOnly) {
                me.removeCls('x-form-readonly');
            }
            me.callParent(arguments);
        }
    });

    Ext.override(Ext.toolbar.Paging, {
        onPagingBlur: function () {
            var me = this,
				pgData = me.getPageData(),
				curPage = pgData.currentPage,
				newPage = me.readPageFromInput(),
				newPageLtd;
            if (curPage != newPage && newPage != false) {
                newPageLtd = Math.min(Math.max(1, newPage), pgData.pageCount);
                if (me.fireEvent('beforechange', me, newPageLtd) !== false) {
                    me.store.loadPage(newPageLtd);
                }
            }
        }
    });

    Ext.override(Ext.data.Field, {
        constructor: function (config) {
            var me = this;
            if (typeof config == 'object' && config.type == 'date' && !config.convert) {
                config.convert = function (value, rcd) {
                    if (value) {
                        if (Ext.isString(value) && Date.isIsoDate(value)) {
                            return Date.fromIsoDate(value);
                        }
                    }
                    return this.type.convert(value);
                }
            }
            me.callParent(arguments);
        }
    });

    Ext.override(Ext.form.field.Date, {
        validateOnChange: true,
        checkChangeBuffer: 500,
        maxLength: 10,
        enforceMaxLength: true,
        maskRe: /[0-9\/]/,
        editable: true
    });


});