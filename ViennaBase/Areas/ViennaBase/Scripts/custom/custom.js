/*** W2ui Add method **/
/*add recid attribute to record object if not exist */

if (jQuery.prototype.jquery == "3.4.1" && w2utils.version == "1.4.3") {
    var old2grid = $.fn.w2grid;

    $.fn.w2grid = function () {
        var grd = old2grid.apply(this, arguments);
        if (grd.records.length > 0) {
            //console.log("refresh call");
            grd.refresh();
        }
        return grd;
    }
};


; w2obj.grid.prototype.add = function (record) {
    if (!$.isArray(record)) record = [record];
    var added = 0;
    for (var o in record) {
        if (!record[o].recid) record[o].recid = added + 1;
        if (record[o].recid == null || typeof record[o].recid == 'undefined') {
            console.log('ERROR: Cannot add record without recid. (obj: ' + this.name + ')');
            continue;
        }
        this.records.push(record[o]);
        // event before
        var eventData = this.trigger({
            phase: 'before', type: 'rowAdd', recid: record[o].recid, index: added
        });

        added++;
    }
    this.buffered = this.records.length;
    var url = (typeof this.url != 'object' ? this.url : this.url.get);
    if (!url) {
        this.localSort();
        this.localSearch();
    }
    this.refresh(); // ??  should it be reload?
    return added;
};

; w2obj.grid.prototype.editField = function (recid, column, value, event) {

    var obj = this;
    var index = obj.get(recid, true);
    var rec = obj.records[index];
    var col = obj.columns[column];
    var edit = col ? col.editable : null;
    if (!rec || !col || !edit || rec.editable === false) return;
    if (['enum', 'file'].indexOf(edit.type) != -1) {
        console.log('ERROR: input types "enum" and "file" are not supported in inline editing.');
        return;
    }
    // event before
    var eventData = obj.trigger({
        phase: 'before', type: 'editField', target: obj.name, recid: recid, column: column, value: value,
        index: index, originalEvent: event
    });

    var tr = $('#grid_' + obj.name + '_rec_' + w2utils.escapeId(recid));
    var el = tr.find('[col=' + column + '] > div');
    var cl = tr.find('[col=' + column + ']');

    if (eventData.isCancelled === true) {
        //cl.css( 'background-color','whitesmoke');
        return true;
    }
    // cl.css('background-color', '');
    value = eventData.value;
    // default behaviour
    this.selectNone();
    this.select({ recid: recid, column: column });
    this.last.edit_col = column;
    if (['checkbox', 'check'].indexOf(edit.type) != -1) return true;
    // create input element

    if (typeof edit.inTag == 'undefined') edit.inTag = '';
    if (typeof edit.outTag == 'undefined') edit.outTag = '';
    if (typeof edit.style == 'undefined') edit.style = '';
    if (typeof edit.items == 'undefined') edit.items = [];
    var val = (rec.changes && typeof rec.changes[col.field] != 'undefined' ? w2utils.stripTags(rec.changes[col.field]) : w2utils.stripTags(rec[col.field]));
    if (val == null || typeof val == 'undefined') val = '';
    if (typeof value != 'undefined' && value != null) val = value;
    var addStyle = (typeof col.style != 'undefined' ? col.style + ';' : '');
    if (typeof col.render == 'string' && ['number', 'int', 'float', 'money', 'percent'].indexOf(col.render.split(':')[0]) != -1) {
        addStyle += 'text-align: right;';
    }
    // mormalize items
    if (edit.items.length > 0 && !$.isPlainObject(edit.items[0])) {
        edit.items = w2obj.field.prototype.normMenu(edit.items);
    }
    if (edit.type == 'select') {
        var html = '';
        for (var i in edit.items) {
            html += '<option value="' + edit.items[i].id + '" ' + (edit.items[i].id == val ? 'selected' : '') + '>' + edit.items[i].text + '</option>';
        }
        el.addClass('w2ui-editable')
            .html('<select id="grid_' + obj.name + '_edit_' + recid + '_' + column + '" column="' + column + '" ' +
                '    style="width: 100%; ' + addStyle + edit.style + '" field="' + col.field + '" recid="' + recid + '" ' +
                '    ' + edit.inTag +
                '>' + html + '</select>' + edit.outTag);
        el.find('select').focus()
            .on('change', function (event) {
                delete obj.last.move;
            })
            .on('blur', function (event) {
                obj.editChange.call(obj, this, index, column, event);
            });
    } else if (edit.type == 'custom' && edit.ctrl) { // change - custom Column
        var ctrl = edit.ctrl.getControl();
        var btn = null;

        var dt = edit.ctrl.getDisplayType();
        el.html('');

        el.addClass('w2ui-editable')
           .append(ctrl);
        ctrl.css({ 'width': el.width()+50});

        edit.ctrl.gridPos = { index: index, col: column, dialog: false, recid: recid };

        if (edit.ctrl.getBtnCount() > 0 && (!VIS.DisplayType.IsLookup(dt) || VIS.DisplayType.Search == dt)) {

            // ctrl.css({ 'width': '80%' });
            ctrl.attr('readonly', dt != VIS.DisplayType.Search).css('background-color', 'white');
            if (VIS.DisplayType.Location == dt)
                btn = edit.ctrl.getBtn(1);
            else
                btn = edit.ctrl.getBtn(0);
            edit.ctrl.gridPos.dialog = true;

        }

        if (!edit.ctrl.editingGrid) {
            edit.ctrl.editingGrid = this;
            ctrl.css({ 'margin': '0' });

           ctrl.on('change', function (event) {
                delete obj.last.move;
            })
             .on('blur', function (event) {
                 b = true;
                 var val = edit.ctrl.getValue();
                 // setTimeout(function () {
                 ctrl.detach();
                 obj.editChange.call(obj, this, edit.ctrl.gridPos.index, edit.ctrl.gridPos.col, event, val);
                 edit.ctrl.oldValue = 'oldValue';
                 //}, 10);
             });
            if (btn != null) {
                ctrl.on('click', function (event) {
                    btn.click();
                });

                ctrl.on('keyup', function (event) {
                    if (dt != VIS.DisplayType.Search) {
                        btn.click();
                        return;
                    }

                    if (event.keyCode == 13)
                        btn.click();
                });


            }

            setTimeout(function () {
                el.find('input, select, textarea, button')
                    .on('click', function (event) {
                        event.stopPropagation();
                    })
                    .on('keydown', function (event) {
                        var cancel = false;
                        recid = edit.ctrl.gridPos.recid;
                        rec = obj.get(recid);
                        switch (event.keyCode) {
                            case 9:  // tab
                                cancel = true;
                                recid = edit.ctrl.gridPos.recid;
                                var next_rec = recid;
                                var next_col = event.shiftKey ? obj.prevCell(column, true) : obj.nextCell(column, true);
                                // next or prev row
                                //if (next_col == null) {
                                //    var tmp = event.shiftKey ? obj.prevRow(index) : obj.nextRow(index);
                                //    if (tmp != null && tmp != index) {
                                //        next_rec = obj.records[tmp].recid;
                                //        // find first editable row
                                //        for (var c in obj.columns) {
                                //            var tmp = obj.columns[c].editable;
                                //            if (typeof tmp != 'undefined' && ['checkbox', 'check'].indexOf(tmp.type) == -1) {
                                //                next_col = parseInt(c);
                                //                if (!event.shiftKey) break;
                                //            }
                                //        }
                                //    }
                                //}
                                if (next_rec === false) next_rec = recid;
                                if (next_col == null) next_col = column;
                                // init new or same record
                                this.blur();
                                setTimeout(function () {
                                    if (obj.selectType != 'row') {
                                        obj.selectNone();
                                        obj.select({ recid: next_rec, column: next_col });
                                    } else {
                                        while (obj.editField(next_rec, next_col, null, event) && next_col != null) {
                                            next_col = event.shiftKey ? obj.prevCell(next_col, true) : obj.nextCell(next_col, true);
                                        }
                                    }
                                }, 1);
                                //event.stopPropagation();
                                break;

                            case 13: // enter
                                this.blur();
                                var next = null; //event.shiftKey ? obj.prevRow(index) : obj.nextRow(index);
                                if (next != null && next != index) {
                                    setTimeout(function () {
                                        if (obj.selectType != 'row') {
                                            obj.selectNone();
                                            obj.select({ recid: obj.records[next].recid, column: column });
                                        } else {
                                            obj.editField(obj.records[next].recid, column, null, event);
                                        }
                                    }, 100);
                                }
                                break;

                            case 38: // up arrow
                                if (!event.shiftKey) break;
                                cancel = true;
                                var next = obj.prevRow(index);
                                if (next != index) {
                                    this.blur();
                                    setTimeout(function () {
                                        if (obj.selectType != 'row') {
                                            obj.selectNone();
                                            obj.select({ recid: obj.records[next].recid, column: column });
                                        } else {
                                            obj.editField(obj.records[next].recid, column, null, event);
                                        }
                                    }, 1);
                                }
                                break;

                            case 40: // down arrow
                                if (!event.shiftKey) break;
                                cancel = true;
                                var next = null;// obj.nextRow(index);
                                if (next != null && next != index) {
                                    this.blur();
                                    setTimeout(function () {
                                        if (obj.selectType != 'row') {
                                            obj.selectNone();
                                            obj.select({ recid: obj.records[next].recid, column: column });
                                        } else {
                                            obj.editField(obj.records[next].recid, column, null, event);
                                        }
                                    }, 1);
                                }
                                break;

                            case 27: // escape
                                var old = obj.parseField(rec, col.field);
                                if (rec.changes && typeof rec.changes[col.field] != 'undefined') old = rec.changes[col.field];
                                this.value = typeof old != 'undefined' ? old : '';
                                this.blur();
                                setTimeout(function () { obj.select({ recid: recid, column: column }) }, 1);
                                break;
                        }
                        if (cancel) if (event.preventDefault) event.preventDefault();
                    });
                // focus and select
                var tmp = el.find('input').focus();
                if (value != null) {
                    // set cursor to the end
                    tmp[0].setSelectionRange(tmp.val().length, tmp.val().length);
                } else {
                    tmp.select();
                }

            }, 1);
        }
        ctrl.focus();

        edit.ctrl.setValue(val == '' ? null : val);
        ctrl.select();

        obj.trigger($.extend(eventData, { phase: 'after' }));
        return;
    }

    else {
        el.addClass('w2ui-editable')
            .html('<input id="grid_' + obj.name + '_edit_' + recid + '_' + column + '" ' +
                '    type="text" style="font-family: inherit; font-size: inherit; outline: none; ' + addStyle + edit.style + '" field="' + col.field + '" recid="' + recid + '" ' +
                '    column="' + column + '" ' + edit.inTag +
                '>' + edit.outTag);
        if (value == null) el.find('input').val(val != 'object' ? val : '');
        // init w2field
        var input = el.find('input').get(0);
        $(input).w2field(edit.type, $.extend(edit, { selected: val }))
        // add blur listener
        setTimeout(function () {
            var tmp = input;
            if (edit.type == 'list') {
                tmp = $($(input).data('w2field').helpers.focus).find('input');
                if (typeof val != 'object' && val != '') tmp.val(val).css({ opacity: 1 }).prev().css({ opacity: 1 });
            }
            $(tmp).on('blur', function (event) {
                obj.editChange.call(obj, input, index, column, event);
            });
        }, 10);
        if (value != null) $(input).val(val != 'object' ? val : '');
    }
    setTimeout(function () {
        el.find('input, select')
            .on('click', function (event) {
                event.stopPropagation();
            })
            .on('keydown', function (event) {
                var cancel = false;
                switch (event.keyCode) {
                    case 9:  // tab
                        cancel = true;
                        var next_rec = recid;
                        var next_col = event.shiftKey ? obj.prevCell(column, true) : obj.nextCell(column, true);
                        // next or prev row
                        if (next_col == null) {
                            var tmp = event.shiftKey ? obj.prevRow(index) : obj.nextRow(index);
                            if (tmp != null && tmp != index) {
                                next_rec = obj.records[tmp].recid;
                                // find first editable row
                                for (var c in obj.columns) {
                                    var tmp = obj.columns[c].editable;
                                    if (typeof tmp != 'undefined' && ['checkbox', 'check'].indexOf(tmp.type) == -1) {
                                        next_col = parseInt(c);
                                        if (!event.shiftKey) break;
                                    }
                                }
                            }

                        }
                        if (next_rec === false) next_rec = recid;
                        if (next_col == null) next_col = column;
                        // init new or same record
                        this.blur();
                        setTimeout(function () {
                            if (obj.selectType != 'row') {
                                obj.selectNone();
                                obj.select({ recid: next_rec, column: next_col });
                            } else {
                                obj.editField(next_rec, next_col, null, event);
                            }
                        }, 1);
                        break;

                    case 13: // enter
                        this.blur();
                        var next = event.shiftKey ? obj.prevRow(index) : obj.nextRow(index);
                        if (next != null && next != index) {
                            setTimeout(function () {
                                if (obj.selectType != 'row') {
                                    obj.selectNone();
                                    obj.select({ recid: obj.records[next].recid, column: column });
                                } else {
                                    obj.editField(obj.records[next].recid, column, null, event);
                                }
                            }, 100);
                        }
                        break;

                    case 38: // up arrow
                        if (!event.shiftKey) break;
                        cancel = true;
                        var next = obj.prevRow(index);
                        if (next != index) {
                            this.blur();
                            setTimeout(function () {
                                if (obj.selectType != 'row') {
                                    obj.selectNone();
                                    obj.select({ recid: obj.records[next].recid, column: column });
                                } else {
                                    obj.editField(obj.records[next].recid, column, null, event);
                                }
                            }, 1);
                        }
                        break;

                    case 40: // down arrow
                        if (!event.shiftKey) break;
                        cancel = true;
                        var next = obj.nextRow(index);
                        if (next != null && next != index) {
                            this.blur();
                            setTimeout(function () {
                                if (obj.selectType != 'row') {
                                    obj.selectNone();
                                    obj.select({ recid: obj.records[next].recid, column: column });
                                } else {
                                    obj.editField(obj.records[next].recid, column, null, event);
                                }
                            }, 1);
                        }
                        break;

                    case 27: // escape
                        var old = obj.parseField(rec, col.field);
                        if (rec.changes && typeof rec.changes[col.field] != 'undefined') old = rec.changes[col.field];
                        this.value = typeof old != 'undefined' ? old : '';
                        this.blur();
                        setTimeout(function () { obj.select({ recid: recid, column: column }) }, 1);
                        break;
                }
                if (cancel) if (event.preventDefault) event.preventDefault();
            });
        // focus and select
        var tmp = el.find('input').focus();
        if (value != null) {
            // set cursor to the end
            tmp[0].setSelectionRange(tmp.val().length, tmp.val().length);
        } else {
            tmp.select();
        }

    }, 1);
    // event after
    obj.trigger($.extend(eventData, { phase: 'after' }));
};

; w2obj.grid.prototype.editChange = function (el, index, column, event, val2) {
    // all other fields
    var summary = index < 0;
    index = index < 0 ? -index - 1 : index;
    var records = summary ? this.summary : this.records;
    var rec = records[index];
    var tr = $('#grid_' + this.name + '_rec_' + w2utils.escapeId(rec.recid));
    var col = this.columns[column];
    var new_val = typeof val2 != 'undefined' ? val2 : el.value;
    var old_val = this.parseField(rec, col.field);
    var tmp = $(el).data('w2field');
    if (tmp) {
        new_val = tmp.clean(new_val);
        if (tmp.type == 'list' && new_val != '') new_val = $(el).data('selected');
    }
    if (el.type == 'checkbox') {
        if (rec.editable === false) el.checked = !el.checked;
        new_val = el.checked;
    }
    // change/restore event
    var eventData = {
        phase: 'before', type: 'change', target: this.name, input_id: el.id, recid: rec.recid, index: index, column: column,
        value_new: new_val, value_previous: (rec.changes && rec.changes.hasOwnProperty(col.field) ? rec.changes[col.field] : old_val), value_original: old_val
    };
    while (true) {
        new_val = eventData.value_new;
        if (((typeof new_val != 'object' || new_val == null) && String(old_val) != String(new_val)) ||
            (typeof new_val == 'object' && new_val != null && (typeof old_val != 'object' || new_val.id != old_val.id))) { //Changed
            // change event
            eventData = this.trigger($.extend(eventData, { type: 'change', phase: 'before' }));
            if (eventData.isCancelled !== true) {
                if (new_val !== eventData.value_new) {
                    // re-evaluate the type of change to be made
                    continue;
                }
                // default action
                rec.changes = rec.changes || {};
                rec.changes[col.field] = eventData.value_new;
                // event after
                this.trigger($.extend(eventData, { phase: 'after' }));
            }
        } else {
            // restore event
            eventData = this.trigger($.extend(eventData, { type: 'restore', phase: 'before' }));
            if (eventData.isCancelled !== true) {
                if (new_val !== eventData.value_new) {
                    // re-evaluate the type of change to be made
                    continue;
                }
                // default action
                if (rec.changes) delete rec.changes[col.field];
                if ($.isEmptyObject(rec.changes)) delete rec.changes;
                // event after
                this.trigger($.extend(eventData, { phase: 'after' }));
            }
        }
        break;
    }
    // refresh cell
    var cell = this.getCellHTML(index, column, summary);
    if (!summary) {
        if (rec.changes && typeof rec.changes[col.field] != 'undefined') {
            $(tr).find('[col=' + column + ']').addClass('w2ui-changed').html(cell);
        } else {
            $(tr).find('[col=' + column + ']').removeClass('w2ui-changed').html(cell);
        }
    }
};

; w2obj.grid.prototype.clearRowChanges = function (recid) {
    var changes = this.getChanges();
    for (var c in changes) {
        var record = this.get(changes[c].recid);
        if (record.recid == recid)
            delete record.changes;
    }
    this.refreshRow(recid);
};

; w2obj.grid.prototype.localSort = function (silent) {
    var url = (typeof this.url != 'object' ? this.url : this.url.get);
    if (url) {
        console.log('ERROR: grid.localSort can only be used on local data source, grid.url should be empty.');
        return;
    }

    //if (!this.defaultSort) {
    //    return;
    //}

    if ($.isEmptyObject(this.sortData)) return;
    var time = (new Date()).getTime();
    var obj = this;
    // process date fields
    obj.prepareData();
    // process sortData
    for (var s in this.sortData) {
        var column = this.getColumn(this.sortData[s].field);
        if (!column) return;
        if (typeof column.render == 'string') {
            if (['date', 'age'].indexOf(column.render.split(':')[0]) != -1) {
                this.sortData[s]['field_'] = column.field + '_';
            }
            if (['time'].indexOf(column.render.split(':')[0]) != -1) {
                this.sortData[s]['field_'] = column.field + '_';
            }
        }
    }
    // process sort

    this.records.sort(function (a, b) {
        var ret = 0;
        for (var s in obj.sortData) {
            var fld = obj.sortData[s].field;

            var column = obj.columns.find(function (col) {
                return col.columnName.toLowerCase() == fld.toLowerCase()
            });

            if (obj.sortData[s].field_)
                fld = obj.sortData[s].field_;
            var aa = a[fld];
            var bb = b[fld];

            if (column.lookup) { //Custom AD look up 
                if (aa) {
                    aa = column.lookup.get(aa).Name;
                }
                if (bb) {
                    bb = column.lookup.get(bb).Name;
                }
            }

            if (!aa) {
                aa = " ";
            }
            if (!bb) {
                bb = " ";
            }



            if (String(fld).indexOf('.') != -1) {
                aa = obj.parseField(a, fld);
                bb = obj.parseField(b, fld);
            }

            if (typeof aa == 'string') aa = $.trim(aa.toLowerCase());
            if (typeof bb == 'string') bb = $.trim(bb.toLowerCase());
            if (aa > bb) ret = (obj.sortData[s].direction == 'asc' ? 1 : -1);
            if (aa < bb) ret = (obj.sortData[s].direction == 'asc' ? -1 : 1);
            if (typeof aa != 'object' && typeof bb == 'object') ret = -1;
            if (typeof bb != 'object' && typeof aa == 'object') ret = 1;
            if (aa == null && bb != null) ret = 1;    // all nuls and undefined on bottom
            if (aa != null && bb == null) ret = -1;
            if (ret != 0) break;
        }
        return ret;
    });
    time = (new Date()).getTime() - time;
    if (silent !== true) setTimeout(function () { obj.status(w2utils.lang('Sorting took') + ' ' + time / 1000 + ' ' + w2utils.lang('sec')); }, 10);
    return time;
};





/* support jquery Object also */

$.fn.w2overlay = function (html, options) {
    var obj = this;
    var name = '';
    var defaults = {
        name: null,      // it not null, then allows multiple concurent overlays
        html: '',        // html text to display
        align: 'none',    // can be none, left, right, both
        left: 0,         // offset left
        top: 0,         // offset top
        tipLeft: 30,        // tip offset left
        width: 0,         // fixed width
        height: 0,         // fixed height
        maxWidth: null,      // max width if any
        maxHeight: null,      // max height if any
        style: '',        // additional style for main div
        'class': '',        // additional class name for main div
        onShow: null,      // event on show
        onHide: null,      // event on hide
        openAbove: false,     // show abover control
        tmp: {}
    };
    if (arguments.length == 1) {
        if (typeof html == 'object' && !html.length) {
            options = html;
        } else {
            options = { html: html };
        }
    }
    if (arguments.length == 2) options.html = html;
    if (!$.isPlainObject(options)) options = {};
    options = $.extend({}, defaults, options);
    if (options.name) name = '-' + options.name;
    // if empty then hide
    var tmp_hide;
    if (this.length === 0 || options.html === '' || options.html == null) {
        if ($('#w2ui-overlay' + name).length > 0) {
            tmp_hide = $('#w2ui-overlay' + name)[0].hide;
            if (typeof tmp_hide === 'function') tmp_hide();
        } else {
            $('#w2ui-overlay' + name).remove();
        }
        return $(this);
    }
    if ($('#w2ui-overlay' + name).length > 0) {
        tmp_hide = $('#w2ui-overlay' + name)[0].hide;
        $(document).off('click', tmp_hide);
        if (typeof tmp_hide === 'function') tmp_hide();
    }
    $('body').append('<div id="w2ui-overlay-main' + name + '" style="position:absolute;z-index:1001;height:100%;width:100%;display:block;top:0;left:0">' +
        '<div id="w2ui-overlay' + name + '" style="display: none"' +
        '        class="w2ui-reset w2ui-overlay ' + ($(this).parents('.w2ui-popup, .w2ui-overlay-popup').length > 0 ? 'w2ui-overlay-popup' : '') + '">' +
        '    <style></style>' +
        '    <div style="' + options.style + '" class="' + options['class'] + '"></div>' +
        '</div></div>'
    );
    // init
    var divMain = $('#w2ui-overlay-main' + name);
    var div1 = $('#w2ui-overlay' + name);
    var div2 = div1.find(' > div');

    var isjObj = false;
    if (typeof (html) == "string")
        div2.html(html);
    else {
        div2.append(html);
        isjObj = true;
    }
    //div2.html(options.html);
    // pick bg color of first div
    var bc = div2.css('background-color');
    if (bc != null && bc !== 'rgba(0, 0, 0, 0)' && bc !== 'transparent') div1.css('background-color', bc);

    div1.data('element', obj.length > 0 ? obj[0] : null)
        .data('options', options)
        .data('position', $(obj).offset().left + 'x' + $(obj).offset().top)
        .fadeIn('fast').on('mousedown', function (event) {
            $('#w2ui-overlay' + name).data('keepOpen', true);
            if (['INPUT', 'TEXTAREA', 'SELECT'].indexOf(event.target.tagName) === -1) event.preventDefault();
        });
    div1[0].hide = hide;
    div1[0].resize = resize;


    // need time to display
    resize();
    setTimeout(function () {
        resize();
        //$(document).off('click', hide).on('click', hide);
        divMain.on('click', hide);
        if (isjObj) html.on('click', hide);
        if (typeof options.onShow === 'function') options.onShow();
    }, 10);

    monitor();
    return $(this);

    // monitor position
    function monitor() {
        var tmp = $('#w2ui-overlay' + name);
        if (tmp.data('element') !== obj[0]) return; // it if it different overlay
        if (tmp.length === 0) return;
        var pos = $(obj).offset().left + 'x' + $(obj).offset().top;
        if (tmp.data('position') !== pos) {
            hide();
        } else {
            setTimeout(monitor, 250);
        }
    }

    // click anywhere else hides the drop down
    function hide() {
        var div1 = $('#w2ui-overlay' + name);
        if (div1.data('keepOpen') === true) {
            div1.removeData('keepOpen');
            return;
        }
        var result;
        if (typeof options.onHide === 'function') result = options.onHide();
        if (result === false) return;
        div1.remove();
        //$(document).off('click', hide);
        divMain.off('click', hide);
        divMain.remove();
        if (isjObj) html.off('click', hide);
        clearInterval(div1.data('timer'));
    }

    function resize() {
        var div1 = $('#w2ui-overlay' + name);
        var div2 = div1.find(' > div');
        // if goes over the screen, limit height and width
        if (div1.length > 0) {
            div2.height('auto').width('auto');
            // width/height
            var overflowX = false;
            var overflowY = false;
            var h = div2.height();
            var w = div2.width();
            if (options.width && options.width < w) w = options.width;
            if (w < 30) w = 30;
            // if content of specific height
            if (options.tmp.contentHeight) {
                h = options.tmp.contentHeight;
                div2.height(h);
                setTimeout(function () {
                    if (div2.height() > div2.find('div.menu > table').height()) {
                        div2.find('div.menu').css('overflow-y', 'hidden');
                    }
                }, 1);
                setTimeout(function () { div2.find('div.menu').css('overflow-y', 'auto'); }, 10);
            }
            if (options.tmp.contentWidth) {
                w = options.tmp.contentWidth;
                div2.width(w);
                setTimeout(function () {
                    if (div2.width() > div2.find('div.menu > table').width()) {
                        div2.find('div.menu').css('overflow-x', 'hidden');
                    }
                }, 1);
                setTimeout(function () { div2.find('div.menu').css('overflow-y', 'auto'); }, 10);
            }
            // alignment
            switch (options.align) {
                case 'both':
                    options.left = 17;
                    if (options.width === 0) options.width = w2utils.getSize($(obj), 'width');
                    break;
                case 'left':
                    options.left = 17;
                    break;
                case 'right':
                    options.tipLeft = w - 45;
                    options.left = w2utils.getSize($(obj), 'width') - w + 10;
                    break;
            }
            // adjust position
            var tmp = (w - 17) / 2;
            var boxLeft = options.left;
            var boxWidth = options.width;
            var tipLeft = options.tipLeft;
            if (w === 30 && !boxWidth) boxWidth = 30; else boxWidth = (options.width ? options.width : 'auto');
            if (tmp < 25) {
                boxLeft = 25 - tmp;
                tipLeft = Math.floor(tmp);
            }
            // Y coord
            div1.css({
                top: (obj.offset().top + w2utils.getSize(obj, 'height') + options.top + 7) + 'px',
                left: ((obj.offset().left > 25 ? obj.offset().left : 25) + boxLeft) + 'px',
                'min-width': boxWidth,
                'min-height': (options.height ? options.height : 'auto')
            });
            // $(window).height() - has a problem in FF20
            var maxHeight = window.innerHeight + $(document).scrollTop() - div2.offset().top - 7;
            var maxWidth = window.innerWidth + $(document).scrollLeft() - div2.offset().left - 7;
            if ((maxHeight > -50 && maxHeight < 210) || options.openAbove === true) {
                // show on top
                maxHeight = div2.offset().top - $(document).scrollTop() - 7;
                if (options.maxHeight && maxHeight > options.maxHeight) maxHeight = options.maxHeight;
                if (h > maxHeight) {
                    overflowY = true;
                    div2.height(maxHeight).width(w).css({ 'overflow-y': 'auto' });
                    h = maxHeight;
                }
                div1.css('top', ($(obj).offset().top - h - 24 + options.top) + 'px');
                div1.find('>style').html(
                    '#w2ui-overlay' + name + ':before { display: none; margin-left: ' + parseInt(tipLeft) + 'px; }' +
                    '#w2ui-overlay' + name + ':after { display: block; margin-left: ' + parseInt(tipLeft) + 'px; }'
                );
            } else {
                // show under
                if (options.maxHeight && maxHeight > options.maxHeight) maxHeight = options.maxHeight;
                if (h > maxHeight) {
                    overflowY = true;
                    div2.height(maxHeight).width(w).css({ 'overflow-y': 'auto' });
                }
                div1.find('>style').html(
                    '#w2ui-overlay' + name + ':before { display: block; margin-left: ' + parseInt(tipLeft) + 'px; }' +
                    '#w2ui-overlay' + name + ':after { display: none; margin-left: ' + parseInt(tipLeft) + 'px; }'
                );
            }
            // check width
            w = div2.width();
            maxWidth = window.innerWidth + $(document).scrollLeft() - div2.offset().left - 7;
            if (options.maxWidth && maxWidth > options.maxWidth) maxWidth = options.maxWidth;
            if (w > maxWidth && options.align !== 'both') {
                options.align = 'right';
                setTimeout(function () { resize(); }, 1);
            }
            // check scroll bar
            if (overflowY && overflowX) div2.width(w + w2utils.scrollBarSize() + 2);
        }
    }
};

//$.widget("ui.dialog", {
//    version: "1.10.4",
//    options: {
//        appendTo: "body",
//        autoOpen: true,
//        buttons: [],
//        closeOnEscape: true,
//        closeText: "close",
//        dialogClass: "",
//        draggable: true,
//        hide: null,
//        height: "auto",
//        maxHeight: null,
//        maxWidth: null,
//        minHeight: 150,
//        minWidth: 150,
//        modal: false,
//        position: {
//            my: "center",
//            at: "center",
//            of: window,
//            collision: "fit",
//            // Ensure the titlebar is always visible
//            using: function (pos) {
//                var topOffset = $(this).css(pos).offset().top;
//                if (topOffset < 0) {
//                    $(this).css("top", pos.top - topOffset);
//                }
//            }
//        },
//        resizable: true,
//        show: null,
//        title: null,
//        width: 300,

//        // callbacks
//        beforeClose: null,
//        close: null,
//        drag: null,
//        dragStart: null,
//        dragStop: null,
//        focus: null,
//        open: null,
//        resize: null,
//        resizeStart: null,
//        resizeStop: null
//    },

//    _create: function () {
//        this.originalCss = {
//            display: this.element[0].style.display,
//            width: this.element[0].style.width,
//            minHeight: this.element[0].style.minHeight,
//            maxHeight: this.element[0].style.maxHeight,
//            height: this.element[0].style.height
//        };
//        this.originalPosition = {
//            parent: this.element.parent(),
//            index: this.element.parent().children().index(this.element)
//        };
//        this.originalTitle = this.element.attr("title");
//        this.options.title = this.options.title || this.originalTitle;

//        this._createWrapper();

//        this.element
//			.show()
//			.removeAttr("title")
//			.addClass("ui-dialog-content ui-widget-content")
//			.appendTo(this.uiDialog);

//        this._createTitlebar();
//        this._createButtonPane();

//        if (this.options.draggable && $.fn.draggable) {
//            this._makeDraggable();
//        }
//        if (this.options.resizable && $.fn.resizable) {
//            this._makeResizable();
//        }

//        this._isOpen = false;
//    },

//    _init: function () {
//        if (this.options.autoOpen) {
//            this.open();
//        }
//    },

//    _appendTo: function () {
//        var element = this.options.appendTo;
//        if (element && (element.jquery || element.nodeType)) {
//            return $(element);
//        }
//        return this.document.find(element || "body").eq(0);
//    },

//    _destroy: function () {
//        var next,
//			originalPosition = this.originalPosition;

//        this._destroyOverlay();

//        this.element
//			.removeUniqueId()
//			.removeClass("ui-dialog-content ui-widget-content")
//			.css(this.originalCss)
//			// Without detaching first, the following becomes really slow
//			.detach();

//        this.uiDialog.stop(true, true).remove();

//        if (this.originalTitle) {
//            this.element.attr("title", this.originalTitle);
//        }

//        next = originalPosition.parent.children().eq(originalPosition.index);
//        // Don't try to place the dialog next to itself (#8613)
//        if (next.length && next[0] !== this.element[0]) {
//            next.before(this.element);
//        } else {
//            originalPosition.parent.append(this.element);
//        }
//    },

//    widget: function () {
//        return this.uiDialog;
//    },

//    disable: $.noop,
//    enable: $.noop,

//    close: function (event) {
//        var activeElement,
//			that = this;

//        if (!this._isOpen || this._trigger("beforeClose", event) === false) {
//            return;
//        }

//        this._isOpen = false;
//        this._destroyOverlay();

//        if (!this.opener.filter(":focusable").focus().length) {

//            // support: IE9
//            // IE9 throws an "Unspecified error" accessing document.activeElement from an <iframe>
//            try {
//                activeElement = this.document[0].activeElement;

//                // Support: IE9, IE10
//                // If the <body> is blurred, IE will switch windows, see #4520
//                if (activeElement && activeElement.nodeName.toLowerCase() !== "body") {

//                    // Hiding a focused element doesn't trigger blur in WebKit
//                    // so in case we have nothing to focus on, explicitly blur the active element
//                    // https://bugs.webkit.org/show_bug.cgi?id=47182
//                    $(activeElement).blur();
//                }
//            } catch (error) { }
//        }

//        this._hide(this.uiDialog, this.options.hide, function () {
//            that._trigger("close", event);
//        });
//    },

//    isOpen: function () {
//        return this._isOpen;
//    },

//    moveToTop: function () {
//        this._moveToTop();
//    },

//    _moveToTop: function (event, silent) {
//        var moved = !!this.uiDialog.nextAll(":visible").insertBefore(this.uiDialog).length;
//        if (moved && !silent) {
//            this._trigger("focus", event);
//        }
//        return moved;
//    },

//    open: function () {
//        var that = this;
//        if (this._isOpen) {
//            if (this._moveToTop()) {
//                this._focusTabbable();
//            }
//            return;
//        }

//        this._isOpen = true;
//        this.opener = $(this.document[0].activeElement);

//        this._size();
//        this._position();
//        this._createOverlay();
//        this._moveToTop(null, true);
//        this._show(this.uiDialog, this.options.show, function () {
//            that._focusTabbable();
//            that._trigger("focus");
//        });

//        this._trigger("open");
//    },

//    _focusTabbable: function () {
//        // Set focus to the first match:
//        // 1. First element inside the dialog matching [autofocus]
//        // 2. Tabbable element inside the content element
//        // 3. Tabbable element inside the buttonpane
//        // 4. The close button
//        // 5. The dialog itself
//        var hasFocus = this.element.find("[autofocus]");
//        if (!hasFocus.length) {
//            hasFocus = this.element.find(":tabbable");
//        }
//        if (!hasFocus.length) {
//            hasFocus = this.uiDialogButtonPane.find(":tabbable");
//        }
//        if (!hasFocus.length) {
//            hasFocus = this.uiDialogTitlebarClose.filter(":tabbable");
//        }
//        if (!hasFocus.length) {
//            hasFocus = this.uiDialog;
//        }
//        hasFocus.eq(0).focus();
//    },

//    _keepFocus: function (event) {
//        function checkFocus() {
//            var activeElement = this.document[0].activeElement,
//				isActive = this.uiDialog[0] === activeElement ||
//					$.contains(this.uiDialog[0], activeElement);
//            if (!isActive) {
//                this._focusTabbable();
//            }
//        }
//        event.preventDefault();
//        checkFocus.call(this);
//        // support: IE
//        // IE <= 8 doesn't prevent moving focus even with event.preventDefault()
//        // so we check again later
//        this._delay(checkFocus);
//    },

//    _createWrapper: function () {
//        this.uiDialog = $("<div>")
//			.addClass("ui-dialog ui-widget ui-widget-content ui-corner-all ui-front " +
//				this.options.dialogClass)
//			.hide()
//			.attr({
//			    // Setting tabIndex makes the div focusable
//			    tabIndex: -1,
//			    role: "dialog"
//			})
//			.appendTo(this._appendTo());

//        this._on(this.uiDialog, {
//            keydown: function (event) {
//                if (this.options.closeOnEscape && !event.isDefaultPrevented() && event.keyCode &&
//						event.keyCode === $.ui.keyCode.ESCAPE) {
//                    event.preventDefault();
//                    this.close(event);
//                    return;
//                }

//                // prevent tabbing out of dialogs
//                if (event.keyCode !== $.ui.keyCode.TAB) {
//                    return;
//                }
//                var tabbables = this.uiDialog.find(":tabbable"),
//					first = tabbables.filter(":first"),
//					last = tabbables.filter(":last");

//                if ((event.target === last[0] || event.target === this.uiDialog[0]) && !event.shiftKey) {
//                    first.focus(1);
//                    event.preventDefault();
//                } else if ((event.target === first[0] || event.target === this.uiDialog[0]) && event.shiftKey) {
//                    last.focus(1);
//                    event.preventDefault();
//                }
//            },
//            mousedown: function (event) {
//                if (this._moveToTop(event)) {
//                    this._focusTabbable();
//                }
//            }
//        });

//        // We assume that any existing aria-describedby attribute means
//        // that the dialog content is marked up properly
//        // otherwise we brute force the content as the description
//        if (!this.element.find("[aria-describedby]").length) {
//            this.uiDialog.attr({
//                "aria-describedby": this.element.uniqueId().attr("id")
//            });
//        }
//    },
//    _createTitlebar: function() {
//		var uiDialogTitle;

//		this.uiDialogTitlebar = $("<div>")
//			.addClass("ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix")
//			.prependTo( this.uiDialog );
//		this._on( this.uiDialogTitlebar, {
//			mousedown: function( event ) {
//				// Don't prevent click on close button (#8838)
//				// Focusing a dialog that is partially scrolled out of view
//				// causes the browser to scroll it into view, preventing the click event
//				if ( !$( event.target ).closest(".ui-dialog-titlebar-close") ) {
//					// Dialog isn't getting focus when dragging (#8063)
//					this.uiDialog.focus();
//				}
//			}
//		});

//		// support: IE
//		// Use type="button" to prevent enter keypresses in textboxes from closing the
//		// dialog in IE (#9312)
//		this.uiDialogTitlebarClose = $( "<button type='button'></button>" )
//			.button({
//				label: this.options.closeText,
//				icons: {
//					primary: "ui-icon-closethick"
//				},
//				text: false
//			})
//			.addClass("ui-dialog-titlebar-close")
//			.appendTo( this.uiDialogTitlebar );
//		this._on( this.uiDialogTitlebarClose, {
//			click: function( event ) {
//				event.preventDefault();
//				this.close( event );
//			}
//		});
//		this._on(this.uiDialogTitlebarClose, {
//		    touchstart: function (event) {
//		        event.preventDefault();
//		        this.close(event);
//		    }
//		});

//		uiDialogTitle = $("<span>")
//			.uniqueId()
//			.addClass("ui-dialog-title")
//			.prependTo( this.uiDialogTitlebar );
//		this._title( uiDialogTitle );

//		this.uiDialog.attr({
//			"aria-labelledby": uiDialogTitle.attr("id")
//		});
//    },
//    _title: function (title) {
//        if (!this.options.title) {
//            title.html("&#160;");
//        }
//        title.text(this.options.title);
//    },

//    _createButtonPane: function () {
//        this.uiDialogButtonPane = $("<div>")
//			.addClass("ui-dialog-buttonpane ui-widget-content ui-helper-clearfix");

//        this.uiButtonSet = $("<div>")
//			.addClass("ui-dialog-buttonset")
//			.appendTo(this.uiDialogButtonPane);

//        this._createButtons();
//    },

//    _createButtons: function () {
//        var that = this,
//			buttons = this.options.buttons;

//        // if we already have a button pane, remove it
//        this.uiDialogButtonPane.remove();
//        this.uiButtonSet.empty();

//        if ($.isEmptyObject(buttons) || ($.isArray(buttons) && !buttons.length)) {
//            this.uiDialog.removeClass("ui-dialog-buttons");
//            return;
//        }

//        $.each(buttons, function (name, props) {
//            var click, buttonOptions;
//            props = $.isFunction(props) ?
//				{ click: props, text: name } :
//				props;
//            // Default to a non-submitting button
//            props = $.extend({ type: "button" }, props);
//            // Change the context for the click callback to be the main element
//            click = props.click;
//            props.click = function () {
//                click.apply(that.element[0], arguments);
//            };
//            buttonOptions = {
//                icons: props.icons,
//                text: props.showText
//            };
//            delete props.icons;
//            delete props.showText;
//            $("<button></button>", props)
//				.button(buttonOptions)
//				.appendTo(that.uiButtonSet);
//        });
//        this.uiDialog.addClass("ui-dialog-buttons");
//        this.uiDialogButtonPane.appendTo(this.uiDialog);
//    },

//    _makeDraggable: function () {
//        var that = this,
//			options = this.options;

//        function filteredUi(ui) {
//            return {
//                position: ui.position,
//                offset: ui.offset
//            };
//        }

//        this.uiDialog.draggable({
//            cancel: ".ui-dialog-content, .ui-dialog-titlebar-close",
//            handle: ".ui-dialog-titlebar",
//            containment: "document",
//            start: function (event, ui) {
//                $(this).addClass("ui-dialog-dragging");
//                that._blockFrames();
//                that._trigger("dragStart", event, filteredUi(ui));
//            },
//            drag: function (event, ui) {
//                that._trigger("drag", event, filteredUi(ui));
//            },
//            stop: function (event, ui) {
//                options.position = [
//					ui.position.left - that.document.scrollLeft(),
//					ui.position.top - that.document.scrollTop()
//                ];
//                $(this).removeClass("ui-dialog-dragging");
//                that._unblockFrames();
//                that._trigger("dragStop", event, filteredUi(ui));
//            }
//        });
//    },

//    _makeResizable: function () {
//        var that = this,
//			options = this.options,
//			handles = options.resizable,
//			// .ui-resizable has position: relative defined in the stylesheet
//			// but dialogs have to use absolute or fixed positioning
//			position = this.uiDialog.css("position"),
//			resizeHandles = typeof handles === "string" ?
//            handles :
//				"n,e,s,w,se,sw,ne,nw";

//        function filteredUi(ui) {
//            return {
//                originalPosition: ui.originalPosition,
//                originalSize: ui.originalSize,
//                position: ui.position,
//                size: ui.size
//            };
//        }

//        this.uiDialog.resizable({
//            cancel: ".ui-dialog-content",
//            containment: "document",
//            alsoResize: this.element,
//            maxWidth: options.maxWidth,
//            maxHeight: options.maxHeight,
//            minWidth: options.minWidth,
//            minHeight: this._minHeight(),
//            handles: resizeHandles,
//            start: function (event, ui) {
//                $(this).addClass("ui-dialog-resizing");
//                that._blockFrames();
//                that._trigger("resizeStart", event, filteredUi(ui));
//            },
//            resize: function (event, ui) {
//                that._trigger("resize", event, filteredUi(ui));
//            },
//            stop: function (event, ui) {
//                options.height = $(this).height();
//                options.width = $(this).width();
//                $(this).removeClass("ui-dialog-resizing");
//                that._unblockFrames();
//                that._trigger("resizeStop", event, filteredUi(ui));
//            }
//        })
//		.css("position", position);
//    },

//    _minHeight: function () {
//        var options = this.options;

//        return options.height === "auto" ?
//			options.minHeight :
//			Math.min(options.minHeight, options.height);
//    },

//    _position: function () {
//        // Need to show the dialog to get the actual offset in the position plugin
//        var isVisible = this.uiDialog.is(":visible");
//        if (!isVisible) {
//            this.uiDialog.show();
//        }
//        this.uiDialog.position(this.options.position);
//        if (!isVisible) {
//            this.uiDialog.hide();
//        }
//    },

//    _setOptions: function (options) {
//        var that = this,
//			resize = false,
//			resizableOptions = {};

//        $.each(options, function (key, value) {
//            that._setOption(key, value);

//            if (key in sizeRelatedOptions) {
//                resize = true;
//            }
//            if (key in resizableRelatedOptions) {
//                resizableOptions[key] = value;
//            }
//        });

//        if (resize) {
//            this._size();
//            this._position();
//        }
//        if (this.uiDialog.is(":data(ui-resizable)")) {
//            this.uiDialog.resizable("option", resizableOptions);
//        }
//    },

//    _setOption: function (key, value) {
//        var isDraggable, isResizable,
//			uiDialog = this.uiDialog;

//        if (key === "dialogClass") {
//            uiDialog
//				.removeClass(this.options.dialogClass)
//				.addClass(value);
//        }

//        if (key === "disabled") {
//            return;
//        }

//        this._super(key, value);

//        if (key === "appendTo") {
//            this.uiDialog.appendTo(this._appendTo());
//        }

//        if (key === "buttons") {
//            this._createButtons();
//        }

//        if (key === "closeText") {
//            this.uiDialogTitlebarClose.button({
//                // Ensure that we always pass a string
//                label: "" + value
//            });
//        }

//        if (key === "draggable") {
//            isDraggable = uiDialog.is(":data(ui-draggable)");
//            if (isDraggable && !value) {
//                uiDialog.draggable("destroy");
//            }

//            if (!isDraggable && value) {
//                this._makeDraggable();
//            }
//        }

//        if (key === "position") {
//            this._position();
//        }

//        if (key === "resizable") {
//            // currently resizable, becoming non-resizable
//            isResizable = uiDialog.is(":data(ui-resizable)");
//            if (isResizable && !value) {
//                uiDialog.resizable("destroy");
//            }

//            // currently resizable, changing handles
//            if (isResizable && typeof value === "string") {
//                uiDialog.resizable("option", "handles", value);
//            }

//            // currently non-resizable, becoming resizable
//            if (!isResizable && value !== false) {
//                this._makeResizable();
//            }
//        }

//        if (key === "title") {
//            this._title(this.uiDialogTitlebar.find(".ui-dialog-title"));
//        }
//    },

//    _size: function () {
//        // If the user has resized the dialog, the .ui-dialog and .ui-dialog-content
//        // divs will both have width and height set, so we need to reset them
//        var nonContentHeight, minContentHeight, maxContentHeight,
//			options = this.options;

//        // Reset content sizing
//        this.element.show().css({
//            width: "auto",
//            minHeight: 0,
//            maxHeight: "none",
//            height: 0
//        });

//        if (options.minWidth > options.width) {
//            options.width = options.minWidth;
//        }

//        // reset wrapper sizing
//        // determine the height of all the non-content elements
//        nonContentHeight = this.uiDialog.css({
//            height: "auto",
//            width: options.width
//        })
//			.outerHeight();
//        minContentHeight = Math.max(0, options.minHeight - nonContentHeight);
//        maxContentHeight = typeof options.maxHeight === "number" ?
//			Math.max(0, options.maxHeight - nonContentHeight) :
//			"none";

//        if (options.height === "auto") {
//            this.element.css({
//                minHeight: minContentHeight,
//                maxHeight: maxContentHeight,
//                height: "auto"
//            });
//        } else {
//            this.element.height(Math.max(0, options.height - nonContentHeight));
//        }

//        if (this.uiDialog.is(":data(ui-resizable)")) {
//            this.uiDialog.resizable("option", "minHeight", this._minHeight());
//        }
//    },

//    _blockFrames: function () {
//        this.iframeBlocks = this.document.find("iframe").map(function () {
//            var iframe = $(this);

//            return $("<div>")
//				.css({
//				    position: "absolute",
//				    width: iframe.outerWidth(),
//				    height: iframe.outerHeight()
//				})
//				.appendTo(iframe.parent())
//				.offset(iframe.offset())[0];
//        });
//    },

//    _unblockFrames: function () {
//        if (this.iframeBlocks) {
//            this.iframeBlocks.remove();
//            delete this.iframeBlocks;
//        }
//    },

//    _allowInteraction: function (event) {
//        if ($(event.target).closest(".ui-dialog").length) {
//            return true;
//        }

//        // TODO: Remove hack when datepicker implements
//        // the .ui-front logic (#8989)
//        return !!$(event.target).closest(".ui-datepicker").length;
//    },

//    _createOverlay: function () {
//        if (!this.options.modal) {
//            return;
//        }

//        var that = this,
//			widgetFullName = this.widgetFullName;
//        if (!$.ui.dialog.overlayInstances) {
//            // Prevent use of anchors and inputs.
//            // We use a delay in case the overlay is created from an
//            // event that we're going to be cancelling. (#2804)
//            this._delay(function () {
//                // Handle .dialog().dialog("close") (#4065)
//                if ($.ui.dialog.overlayInstances) {
//                    this.document.bind("focusin.dialog", function (event) {
//                        if (!that._allowInteraction(event)) {
//                            event.preventDefault();
//                            $(".ui-dialog:visible:last .ui-dialog-content")
//								.data(widgetFullName)._focusTabbable();
//                        }
//                    });
//                }
//            });
//        }

//        this.overlay = $("<div>")
//			.addClass("ui-widget-overlay ui-front")
//			.appendTo(this._appendTo());
//        this._on(this.overlay, {
//            mousedown: "_keepFocus"
//        });
//        $.ui.dialog.overlayInstances++;
//    },

//    _destroyOverlay: function () {
//        if (!this.options.modal) {
//            return;
//        }

//        if (this.overlay) {
//            $.ui.dialog.overlayInstances--;

//            if (!$.ui.dialog.overlayInstances) {
//                this.document.unbind("focusin.dialog");
//            }
//            this.overlay.remove();
//            this.overlay = null;
//        }
//    }
//});

//$.fn.w2overlay = function (html, options) {
//    if (!$.isPlainObject(options)) options = {};
//    if (!$.isPlainObject(options.css)) options.css = {};
//    if (this.length == 0 || typeof html == 'undefined' || html == '') { hide(); return $(this); }
//    if ($('#w2ui-overlay').length > 0) {
//        // $(document).click();
//        hide();
//    }
//    $('body').append('<div id="w2ui-overlay-main" style="position:absolute;height:100%;width:100%;display:block;top:0;left:0"><div id="w2ui-overlay" class="w2ui-reset w2ui-overlay ' +
//                        ($(this).parents('.w2ui-popup').length > 0 ? 'w2ui-overlay-popup' : '') + '">' +
//                    '	<div></div>' +
//                    '</div></div>');
//    var evt = "click";
//    if ('ontouchstart' in document) {
//        evt = 'touchstart';
//    }

//    // init
//    var obj = this;
//    var div = $('#w2ui-overlay div');
//    var divMain = $('#w2ui-overlay-main');
//    //div.css(options.css).html(html); //original line;
//    /* added lines */
//    var isjObj = false;
//    if (typeof (html) == "string")
//        div.html(html);
//    else {
//        div.append(html);
//        isjObj = true;
//    }

//    if (typeof options['class'] != 'undefined') div.addClass(options['class']);
//    if (typeof options.top == 'undefined') options.top = 0;
//    if (typeof options.left == 'undefined') options.left = 0;
//    if (typeof options.width == 'undefined') options.width = 100;
//    if (typeof options.height == 'undefined') options.height = 0;
//    // pickup bg color of first div
//    var bc = div.css('background-color');
//    var div = $('#w2ui-overlay');
//    if (typeof bc != 'undefined' && bc != 'rgba(0, 0, 0, 0)' && bc != 'transparent') div.css('background-color', bc);


//    var left = ($(obj).offset().left + options.left);

//    var screenSize = $(document).width();
//    if (screenSize <= (parseInt(left) + parseInt(options.width))) {
//        left = left - parseInt(options.width);
//    }

//    var top = ($(obj).offset().top + w2utils.getSize($(obj), 'height') + 3 + options.top);
//    screenSize = $(document).height();
//    if (screenSize <= (parseInt(top) + parseInt(options.height))) {
//        top = top - parseInt(options.height);
//    }


//    div.css({
//        display: 'none',
//        left: left + 'px',
//        top: top + 'px',
//        'min-width': (options.width ? options.width : 'auto'),
//        'min-height': (options.height ? options.height : 'auto')
//    })
//        .fadeIn('fast')
//        .data('position', ($(obj).offset().left) + 'x' + ($(obj).offset().top + obj.offsetHeight))
//        .on('touchstart', function (event) {
//            if (event.stopPropagation) event.stopPropagation(); else event.cancelBubble = true;
//        });

//    // click anywhere else hides the drop down
//    function hide() {
//        var result;
//        if (typeof options.onHide == 'function') result = options.onHide();
//        if (result === false) return;
//        $('#w2ui-overlay-main').off(evt, hide);
//        $('#w2ui-overlay-main').remove();
//        $('#w2ui-overlay').remove();
//        // $(document).off('click', hide);
//        if (isjObj) html.off(evt, hide);
//    }

//    // need time to display
//    setTimeout(fixSize, 0);
//    return $(this);

//    function fixSize() {
//        // $(document).on('click', hide);
//        $('#w2ui-overlay-main').on(evt, hide);
//        if (isjObj) html.on(evt, hide);
//        // if goes over the screen, limit height and width
//        if ($('#w2ui-overlay > div').length > 0) {
//            var h = $('#w2ui-overlay > div').height();
//            var w = $('#w2ui-overlay> div').width();
//            // $(window).height() - has a problem in FF20
//            var max = window.innerHeight - $('#w2ui-overlay > div').offset().top - 7;
//            if (h > max) $('#w2ui-overlay> div').height(max).width(w + w2utils.scrollBarSize()).css({ 'overflow-y': 'auto' });
//            // check width
//            w = $('#w2ui-overlay> div').width();
//            max = window.innerWidth - $('#w2ui-overlay > div').offset().left - 7;
//            if (w > max) $('#w2ui-overlay> div').width(max).css({ 'overflow-x': 'auto' });
//            // onShow event
//            if (typeof options.onShow == 'function') options.onShow();
//        }
//    }
//}