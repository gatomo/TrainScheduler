// JavaScript source code

$(window).on("load", function () {
    //ドラッグ&ドロップ時の操作
    $(document).on('drop', '.drop-area', function (event) {
        $(this).find('.uploader')[0].files = event.originalEvent.dataTransfer.files
        $(this).find('.uploader').trigger('change')
    });

    //ドロップエリア以外のドロップ禁止
    $(document).on('dragenter dragover drop', function (event) {
        event.stopPropagation()
        event.preventDefault()
    });

    //ファイルがアップロードされたらプレビューエリアに表示
    $(document).on('change', '.drop-area .uploader', function (e) {
        let fileReader = new FileReader()
        fileReader.onload = (function () {
            var result = fileReader.result;
            window.timetable = $.parseXML(result);

            const line_id_template = '<input type="hidden" data-category="Line" data-attribute="LineID" value="{value}"/>';
            const line_name_template = '<input type="hidden" data-category="Line" data-attribute="Name" value="{value}"/>';
            const line_stopcount_template = '<input type="hidden" data-category="Line" data-attribute="TransportType" value="{value}"/>';
            const line_transporttype_template = '<input type="hidden" data-category="Line" data-attribute="StopCount" value="{value}"/>';
            const line_mode_template = '<input type="hidden" data-category="Line" data-attribute="Mode" value="{value}"/>';
            const line_enabled_template = '<input type="checkbox" id="enabled-{lineId}" name="enabled-{lineId}" data-category="Line" data-attribute="Enabled" {enabled}><label for="enabled-{lineId}"> Enabled </label>';
            const line_useDefaultTimetable_template = '<input type="checkbox" id="usedefaulttimetable-{lineId}" name="usedefaulttimetable-{lineId}" data-category="Line" data-attribute="UseDefaultTimeTable" {usedefaulttimetable}><label for="usedefaulttimetable-{lineId}"> UseDefaultTimetable </label>';
            const line_accordion_template = '<div class="accordion_one"><div class="accordion_header_one"> Line [{lineId}]: {lineName}<div class="i_box"><i class="one_i"></i></div></div><div class="accordion_inner_one"> {lineEnabled} {lineUseDefaultTimeTable} <div><p>{stops}</p></div></div></div>';

            const stop_index_template = '<input type="hidden" data-category="Stop" data-attribute="Index" value="{value}"/>';
            const stop_enabled_template = '<input type="checkbox" id="enabled-{lineId}-{stopId}-{nextId}" name="enabled-{lineId}-{stopId}-{nextId}" data-category="Stop" data-attribute="Enabled" {enabled}><label for="enabled-{lineId}-{stopId}-{nextId}"> Enabled </label>';
            const stop_useDefaultTimetable_template = '<input type="checkbox" id="usedefaulttimetable-{lineId}-{stopId}-{nextId}" name="usedefaulttimetable-{lineId}-{stopId}-{nextId}" data-category="Stop" data-attribute="UseDefaultTimeTable" {usedefaulttimetable}><label for="usedefaulttimetable-{lineId}-{stopId}-{nextId}"> UseDefaultTimetable </label>';
            const stop_id_template = '<input type="hidden" data-category="Stop" data-attribute="Id" value="{value}"/>';
            const stop_name_template = '<input type="hidden" data-category="Stop" data-attribute="Name" value="{value}"/>';
            const stop_nextid_template = '<input type="hidden" data-category="Stop" data-attribute="NextId" value="{value}"/>';
            const stop_nextname_template = '<input type="hidden" data-category="Stop" data-attribute="NextName" value="{value}"/>';
            const stop_mode_template = '<input type="hidden" data-category="Stop" data-attribute="Mode" value="{value}"/>';
            const stop_interval_template = ' <span> Interval</span ><input type="text" id="interval-{lineId}-{stopId}-{nextId}" name="interval-{lineId}-{stopId}-{nextId}" data-category="Stop" data-attribute="Interval" value="{interval}" />';
            const stop_end_template = ' <span> End</span ><input type="text" id="end-{lineId}-{stopId}-{nextId}" name="end-{lineId}-{stopId}-{nextId}" data-category="Stop" data-attribute="End" value="{end}" />';
            const stop_accordion_template = '<div class="accordion_two"><div class="accordion_header_two"> {stopName} to {nextName}<div class="i_box"><i class="one_i"></i></div></div><div class="accordion_inner_two"> {stopEnabled} {stopUseDefaultTimeTable} {stopMode} {stopInterval} {stopEnd}<div class="box_one"><table class="dept-table" id="departures-{lineId}-{stopId}-{nextId}"><thead><tr><th>Departures</th></tr></thead>{tables}</table><div class="box_one_column">{shiftText}<div class="box_buttons">{shiftButton}{CopyShiftButton}{CopyButton}{DeleteButton}</div></div></div></div></div>';
            const stop_shift_text_template = '<div><span>Shift</span ><input type="text" id="shift-{lineId}-{stopId}-{nextId}" name="shift-{lineId}-{stopId}-{nextId}" value="0" /><span> minute(s)</span></div>';
            const stop_shift_button_template = '<button id="shift-button-{lineId}-{stopId}-{nextId}" name="shift-button-{lineId}-{stopId}-{nextId}" data-target="{lineId}-{stopId}-{nextId}" data-action="shift">Shift</button>';
            const stop_copyshift_button_template = '<button id="copyshift-button-{lineId}-{stopId}-{nextId}" name="copyshift-button-{lineId}-{stopId}-{nextId}" data-target="{lineId}-{stopId}-{nextId}" data-action="copyshift">Copy and Shift</button>';
            const stop_copy_button_template = '<button id="copy-button-{lineId}-{stopId}-{nextId}" name="copy-button-{lineId}-{stopId}-{nextId}" data-target="{lineId}-{stopId}-{nextId}" data-action="copy" class="button_copy">Copy</button>';
            const stop_delete_button_template = '<button id="copy-button-{lineId}-{stopId}-{nextId}" name="copy-button-{lineId}-{stopId}-{nextId}" data-target="{lineId}-{stopId}-{nextId}" data-action="delete">Delete</button>';

            $(window.timetable).find('Line').each(function (index_line, e_line, array_line) {

                var innerHtml = '';
                $(this).find('Stop').each(function (index_stop, e_stop, array_stop) {
                    stopEnabled = JSON.parse($(this).attr('Enabled').toLowerCase()) ? "checked" : "";
                    stopUseDefaultTimeTable = JSON.parse($(this).attr('UseDefaultTimeTable').toLowerCase()) ? "checked" : "";
                    var stopMode = '';
                    if ($(this).attr('Mode') === "Indivisually") {
                        stopMode = '<select id="mode-{lineId}-{stopId}-{nextId}"><option value="Indivisually" selected>Indivisually</option><option value="IntervalTime">IntervalTime</option></select>';
                    } else {
                        stopMode = '<select id="mode-{lineId}-{stopId}-{nextId}"><option value="Indivisually">Indivisually</option><option value="IntervalTime" selected>IntervalTime</option></select>';
                    }

                    var tables = '';

                    $(this).find('Departure').each(function () {
                        tables += '<tr><td><input type="text" id="inteval-stop" name="inteval-stop" value="' + this.textContent + '" /><td><tr>';
                    });

                    stop_enabled = stop_enabled_template.replace('{enabled}', stopEnabled);
                    stop_usedefault = stop_useDefaultTimetable_template.replace('{usedefaulttimetable}', stopUseDefaultTimeTable);
                    stop_interval = stop_interval_template.replace('{interval}', $(this).attr('Interval'));
                    stop_end = stop_end_template.replace('{end}', $(this).attr('End'));

                    innerHtml += stop_accordion_template
                        .replace("{tables}", tables)
                        .replace("{shiftText}", stop_shift_text_template)
                        .replace("{shiftButton}", stop_shift_button_template)
                        .replace("{CopyShiftButton}", stop_copyshift_button_template)
                        .replace("{CopyButton}", stop_copy_button_template)
                        .replace("{DeleteButton}", stop_delete_button_template)
                        .replace("{stopEnabled}", stop_enabled)
                        .replace("{stopUseDefaultTimeTable}", stop_usedefault)
                        .replace("{stopMode}", stopMode)
                        .replace("{stopInterval}", stop_interval)
                        .replace("{stopEnd}", stop_end)
                        .replaceAll("{stopId}", $(this).attr('Id'))
                        .replaceAll("{nextId}", $(this).attr('NextId'))
                        .replaceAll("{stopName}", $(this).attr('Name'))
                        .replaceAll("{nextName}", $(this).attr('NextName'));
                    //innerHtml += stop_accordion_template.replace("{tables}", tables).replace("{shiftText}", stop_shift_text_template).replace("{shiftButton}", stop_shift_button_template).replace("{CopyShiftButton}", stop_copyshift_button_template).replace("{CopyButton}", stop_copy_button_template).replace("{DeleteButton}", stop_delete_button_template).replace("{stopEnabled}", stop_enabled).replace("{stopUseDefaultTimeTable}", stop_usedefault).replace("{stopMode}", stopMode).replace("{stopInterval}", stop_interval).replace("{stopEnd}", stop_end).replaceAll("{stopId}", $(this).attr('Id')).replaceAll("{nextId}", $(this).attr('NextId')).replaceAll("{stopName}", $(this).attr('Name')).replaceAll("{nextName}", $(this).attr('NextName'));
                });

                enabled = JSON.parse($(this).attr('Enabled').toLowerCase()) ? "checked" : "";
                line_enabled = line_enabled_template.replace("{enabled}", enabled);
                useDefaultTimeTable = JSON.parse($(this).attr('UseDefaultTimeTable').toLowerCase()) ? "checked" : "";
                line_useDefaultTimetable = line_useDefaultTimetable_template.replace("{usedefaulttimetable}", useDefaultTimeTable);

                html = line_accordion_template.replace("{stops}", innerHtml).replace("{lineEnabled}", line_enabled).replace("{lineUseDefaultTimeTable}", line_useDefaultTimetable).replaceAll("{lineId}", $(this).attr('LineID')).replace("{lineName}", $(this).attr('Name'));
                $('#preview').append(html);
            });

            //.accordion_oneの中の.accordion_headerがクリックされたら#preview > div:nth-child(1) > div.accordion_header
            $('.accordion_one .accordion_header_one').click(function () {
                //クリックされた.accordion_oneの中の.accordion_headerに隣接する.accordion_innerが開いたり閉じたりする。
                $(this).next('.accordion_inner_one').slideToggle();
                $(this).toggleClass("open");
            });
            //.accordion_oneの中の.accordion_headerがクリックされたら#preview > div:nth-child(1) > div.accordion_header
            $('.accordion_two .accordion_header_two').click(function () {
                //クリックされた.accordion_oneの中の.accordion_headerに隣接する.accordion_innerが開いたり閉じたりする。
                $(this).next('.accordion_inner_two').slideToggle();
                $(this).toggleClass("open");
            });
        });

        $(document).ready(function () {
            $(document).on('click', 'button', function (e) {
                actionType = e.currentTarget.dataset.action;
                if (actionType === 'save') {
                    // XMLデータの作成
                    //var xmlData = '<?xml version="1.0" encoding="utf-8"?><TimeTableRecord xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Lines></Lines></TimeTableRecord>';
                    //var lines = $(xmlDocument).find('Lines');
                    // XMLデータをHTMLに基づいて更新
                    $(window.timetable).find('Line').each(function (index_line, e_line, array_line) {
                        var line = $(this).attr('LineID');
                        $(this).attr('Enabled', $('#enabled-' + line)[0].checked);
                        $(this).attr('UseDefaultTimeTable', $('#usedefaulttimetable-' + line)[0].checked);

                        $(this).find('Stop').each(function (index_stop, e_stop, array_stop) {
                            var id = $(this).attr('Id');
                            var nextid = $(this).attr('NextId');
                            $(this).attr('Enabled', $('#enabled-' + line + '-' + id + '-' + nextid)[0].checked);
                            $(this).attr('UseDefaultTimeTable', $('#usedefaulttimetable-' + line + '-' + id + '-' + nextid)[0].checked);

                            // Modeはどうなるか要確認
                            $(this).attr('Mode', $('#mode-' + line + '-' + id + '-' + nextid).first().val());
                            $(this).attr('Interval', $('#interval-' + line + '-' + id + '-' + nextid).first().val());
                            $(this).attr('End', $('#end-' + line + '-' + id + '-' + nextid).first().val());

                            $(this).find('Departures').empty();

                            var item = $(this).find('Departures').first();
                            $('#departures-' + line + '-' + id + '-' + nextid + ' input').each(function () {
                                item.append('<Departure>'+$(this).val()+'</Departure>');
                            });

                        });
                    });

                    var xmlString = new XMLSerializer().serializeToString(window.timetable);

                    //// 整形
                    //var reg = /(>)(<)(\/*)/g;
                    //var wsexp = / *(.*) +\n/g;
                    //var contexp = /(<.+>)(.+\n)/g;
                    //xmlString = xmlString.replace(reg, '$1\n$2$3').replace(wsexp, '$1\n').replace(contexp, '$1\n$2');

                    // Blobオブジェクトを作成
                    var blob = new Blob([xmlString], { type: 'application/xml' });

                    // aタグを作成して、ダウンロード属性を追加
                    var a = $('<a></a>')
                        .attr('href', window.URL.createObjectURL(blob))
                        .attr('download', 'TimeTables.xml')
                        .appendTo($('body'));

                    // aタグをクリックしてダウンロード
                    a[0].click();
                } else {
                    selector = "#departures-" + e.currentTarget.dataset.target;
                    selectedRows = $(selector + " tbody tr.selected");
                    // 選択された行をコピーして追加
                    selectedRows.each(function () {
                        if (actionType === 'copy') {
                            const clone = $(this).clone();
                            $(selector + " tbody").append(clone);
                        } else if (actionType === 'shift') {
                            // まずは基準となる時刻を設定
                            date = new Date('2000-01-01T00:00:00');
                            textBox = $(this).find('input[type="text"]');
                            date.setHours(parseInt(textBox.val().slice(0, 2), 10)); // 時を設定
                            date.setMinutes(parseInt(textBox.val().slice(2), 10)); // 分を設定

                            // Shift計算
                            date.setMinutes(date.getMinutes() + parseInt($('#shift-' + e.currentTarget.dataset.target).val()));
                            textBox.val(('0' + date.getHours()).slice(-2) + ('0' + date.getMinutes()).slice(-2));

                        } else if (actionType === 'copyshift') {
                            clone = $(this).clone();
                            // まずは基準となる時刻を設定
                            date = new Date('2000-01-01T00:00:00');
                            textBox = clone.find('input[type="text"]');
                            date.setHours(parseInt(textBox.val().slice(0, 2), 10)); // 時を設定
                            date.setMinutes(parseInt(textBox.val().slice(2), 10)); // 分を設定

                            // Shift計算
                            date.setMinutes(date.getMinutes() + parseInt($('#shift-' + e.currentTarget.dataset.target).val()));
                            textBox.val(('0' + date.getHours()).slice(-2) + ('0' + date.getMinutes()).slice(-2));

                            $(selector + " tbody").append(clone);
                        } else if (actionType === 'delete') {
                            // Delete
                            $(this).remove();
                        }
                    });

                    // 選択項目を解除
                    $('tr.selected').removeClass('selected');
                }
            });

            $(document).on('click', 'tr', function (e) {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected'); // selectedクラスを削除
                } else {
                    $(this).addClass('selected'); // selectedクラスを削除
                }
            });
        });

        fileReader.readAsText(e.target.files[0]);
    });

})