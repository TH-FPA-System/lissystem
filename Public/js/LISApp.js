moment.tz.setDefault('Asia/Bangkok');
moment.locale('en-nz');

$.validator.setDefaults({
    highlight: function (element) {
        let elm = $(element);
        elm.addClass("is-invalid").removeClass("is-valid");
        elm.parent().children('.input-group-text').addClass('is-invalid').removeClass('is-valid');
    },
    unhighlight: function (element) {
        let elm = $(element);
        elm.addClass("is-valid").removeClass("is-invalid");
        elm.parent().children('.input-group-text').addClass('is-valid').removeClass('is-invalid');
    },
    errorPlacement: function (error, element) {
        // Customize error message placement if needed
        if (element.hasClass('select2-hidden-accessible')) {
            error.insertAfter(element.next('span.select2'));
        } else if (element.attr("type") === "radio" || element.attr("type") === "checkbox") {
            error.insertAfter(element.closest('.form-check'));
        } else {
            error.insertAfter(element);
        }
    }
});

const SELECTOR_SIDEBAR_WRAPPER = '.sidebar-wrapper';
const Default = {
    scrollbarTheme: 'os-theme-dark',
    scrollbarAutoHide: 'leave',
    scrollbarClickScroll: true,
};

document.addEventListener('DOMContentLoaded', function () {

    const sidebarWrapper = document.querySelector(SELECTOR_SIDEBAR_WRAPPER);
    if (sidebarWrapper && OverlayScrollbarsGlobal?.OverlayScrollbars !== undefined) {
        OverlayScrollbarsGlobal.OverlayScrollbars(sidebarWrapper, {
            scrollbars: {
                theme: Default.scrollbarTheme,
                autoHide: Default.scrollbarAutoHide,
                clickScroll: Default.scrollbarClickScroll,
            },
        });
    }

    DismisLoading();

    $(document).on('ajaxStart', function (e) {
        ShowLoading();
    });

    $(document).on('ajaxStop', function (e) {
        DismisLoading();
    });

    $(document).on('ajaxError', function (event, jqxhr, settings, thrownError) {
        DismisLoading();
        let message = `Error requesting page ${settings.url}`;
        if (thrownError) {
            message += `<br/>${thrownError}`;
        }
        //ShowToastError(message, "AJAX Error", 10000);
        ShowToastError('Error requesting!', 'Request error');
    });

    window.addEventListener('error', (event) => {
        //console.error('An unhandled error occurred:', event.message);
        //console.error('File:', event.filename);
        //console.error('Line:', event.lineno);
        //console.error('Column:', event.colno);
        DismisLoading();
    });

    $('aside .sidebar-menu a.nav-link').each((i, elm) => {
        let currentUrl = window.location.pathname;
        let aElement = $(elm);
        let linkUrl = aElement.attr('href');
        let isMatched = linkUrl === currentUrl;
        if (isMatched) {
            aElement.addClass('active');
            let group = $(aElement.closest('ul.nav-treeview')).closest('li');
            if (group) {
                group.addClass('menu-open');
            }
        }
    });

    var currentTimeElm = document.getElementById('currentTime');
    if (currentTimeElm) {
        // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat/DateTimeFormat#date-time_component_options
        function updateTime() {
            var now = new Date();
            var options = { year: 'numeric', month: 'long', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' };
            //currentTimeElm.textContent = now.toLocaleDateString(undefined, options);
            currentTimeElm.textContent = moment(now).format('DD MMMM YYYY, LTS');
        }
        updateTime();
        setInterval(updateTime, 1000);
    }

});

var ShowLoading = function () {
    let pageLoad = document.getElementById('loading-spinner');
    if (pageLoad) {
        pageLoad.classList.add('loading');
    }
}

var DismisLoading = function () {
    let pageLoad = document.getElementById('loading-spinner');
    if (pageLoad) {
        pageLoad.classList.remove('loading');
    }
}

function IsNullOrEmpty(data) {
    //return (data === null || data === undefined || data === "" || (typeof data !== "string" && isNaN(data)));
    // Check for null or undefined
    if (data === undefined || data === null) {
        return true;
    }

    // Check for empty string or whitespace string
    if (typeof data === 'string' && data.trim() === '') {
        return true;
    }

    // Check for NaN (Not a Number)
    if (typeof data === 'number' && isNaN(data)) {
        return true;
    }

    // Check for an empty array
    if (Array.isArray(data) && data.length === 0) {
        return true;
    }

    // If none of the above conditions are met, the value is not considered empty
    return false;
}

/*
    JQuery Validation.
*/
jQuery.validator.setDefaults({
    errorClass: "invalid-feedback"
});

/* 
    Datatable configurations
    Languages: https://datatables.net/plug-ins/i18n/English.html, https://datatables.net/plug-ins/i18n/Thai.html
    Example:
        AJAX: https://datatables.net/reference/option/ajax
        DOM: https://datatables.net/reference/option/dom
        Buttons: https://datatables.net/reference/feature/buttons.buttons
        Row click: https://live.datatables.net/qemodapi/2037/edit
        Select All: https://live.datatables.net/mutavegi/6/edit
*/
var dataTableLayoutConfig = {
    top: {
        searchBuilder: {
            greyscale: true
        }
    },
    topEnd: {
        search: {
            placeholder: 'Search'
        }
    }
};

var dataTableExportButtons = [
    { extend: 'csv', className: 'btn btn-sm' },
    { extend: 'excel', className: 'btn btn-sm' },
]

var dataTableConfig = {
    /*responsive: {
        details: true,
        breakpoints: [
            { name: 'bigdesktop', width: Infinity },
            { name: 'meddesktop', width: 1480 },
            { name: 'smalldesktop', width: 1280 },
            { name: 'medium', width: 1188 },
            { name: 'tabletl', width: 1024 },
            { name: 'btwtabllandp', width: 848 },
            { name: 'tabletp', width: 768 },
            { name: 'mobilel', width: 480 },
            { name: 'mobilep', width: 320 }
        ]
    },*/
};

$.extend(true, DataTable.defaults, {
    lengthMenu: [10, 15, 25, 50, 100, 150, 250, -1],
    pageLength: 15,
    deferRender: false,
    orderCellsTop: true,
    fixedHeader: true,
    ordering: { handler: true },
    orderMulti: true,
    searchBuilder: true,
    search: {
        regex: true
    },
    layout: {
        top: {
            searchBuilder: {
                greyscale: true
            }
        },
        topEnd: {
            search: {
                placeholder: 'Search'
            }
        }
    },
    select: {
        selector: 'td:first-child input[type="checkbox"]'
    },
    responsive: true,
    language: {
        emptyTable: "No data available",
        info: '<i class="bi bi-info-lg me-1"></i>Showing on page _PAGE_ of _PAGES_ (Total _TOTAL_ records)',
        infoEmpty: "No records available (0 record)",
        infoFiltered: "(filtered from _MAX_ total entries)",
        lengthMenu: '<i class="bi bi-list-ol me-2"></i>_MENU_ records per page',
        zeroRecords: "Nothing found data",
        search: "Search data:",
        searchBuilder: {
            title: { '_': '<i class="bi bi-search me-2"></i>Filter option (%d)', 0: '<i class="bi bi-search me-2"></i>Filter option' },
            add: '<i class="bi bi-plus-circle me-2"></i>Add filter',
            button: { '_': "Filter Builder (%d)", 0: "Filter Builder" },
            data: "Column",
            clearAll: "Clear all filters",
        },
        thousands: ",",
        loadingRecords: "Loading data...",
        paginate: {
            first: '<i class="bi bi-chevron-bar-left me-1"></i>First',
            last: 'Last<i class="bi bi-chevron-bar-right ms-1"></i>',
            next: '<i class="bi bi-chevron-double-right"></i>',
            previous: '<i class="bi bi-chevron-double-left"></i>'
        },
        aria: {
            orderable: "Order by this column",
            orderableReverse: "Reverse order this column"
        }
    },
    dom: '<"top-bar pt-2"Q>' +
        '<"top-bar d-flex justify-content-between align-items-center py-3"<"d-flex"l<"ms-2"B>><"d-flex align-items-center"f>>' +
        'rt' +
        '<"bottom-bar d-flex justify-content-between pt-3"ip>',
    buttons: [
        { extend: 'excel', text: '<i class="bi bi-file-excel-fill me-1"></i>Excel', className: 'btn-success' },
        { extend: 'csv', text: '<i class="bi bi-filetype-csv me-1"></i>CSV', className: 'btn-light' }
    ]
});

$.fn.dataTable.Buttons.defaults.dom.button.className = 'btn btn-sm btn-secondary';

/* Plugin API method to determine is a column is sortable */
$.fn.dataTable.Api.register('column().searchable()', function () {
    var ctx = this.context[0];
    return ctx.aoColumns[this[0]].bSearchable;
});

$.fn.dataTable.ext.errMode = 'throw';

$.fn.dataTable.moment('dd-MM-yyyy HH:mm:ss');
//$.fn.dataTable.moment('DD/MM/YYYY');

// Create an array with the values of all the input boxes in a column
DataTable.ext.order['dom-text'] = function (settings, col) {
    return this.api()
        .column(col, { order: 'index' })
        .nodes()
        .map(function (td, i) {
            let el = td.querySelector('input');
            return el ? el.value : 0;
        });
};

// Create an array with the values of all the input boxes in a column, parsed as numbers
DataTable.ext.order['dom-text-numeric'] = function (settings, col) {
    return this.api()
        .column(col, { order: 'index' })
        .nodes()
        .map(function (td, i) {
            let el = td.querySelector('input');
            return el ? el.value * 1 : 0;
        });
};

// Create an array with the values of all the select options in a column
DataTable.ext.order['dom-select'] = function (settings, col) {
    return this.api()
        .column(col, { order: 'index' })
        .nodes()
        .map(function (td, i) {
            let el = td.querySelector('select');
            return el ? el.value : 0;
        });
};

// Create an array with the values of all the checkboxes in a column
DataTable.ext.order['dom-checkbox'] = function (settings, col) {
    return this.api()
        .column(col, { order: 'index' })
        .nodes()
        .map(function (td, i) {
            let el = td.querySelector('input');
            return el ? (el.checked ? 1 : 0) : 0;
        });
};

/*
* Toast plug-in
* https://kamranahmed.info/toast
* Options:
    text: "Don't forget to star the repository if you like it.", // Text that is to be shown in the toast
    heading: 'Note', // Optional heading to be shown on the toast
    icon: 'Information, Error, Warning, Success', // Type of toast icon
    showHideTransition: 'fade', // fade, slide or plain
    allowToastClose: true, // Boolean value true or false
    hideAfter: 3000, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
    stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
    position: 'bottom-left', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
    textAlign: 'left',  // Text alignment i.e. left, right or center
    loader: true,  // Whether to show loader or not. True by default
    loaderBg: '#9EC600',  // Background color of the toast loader
    beforeShow: function () {}, // will be triggered before the toast is shown
    afterShown: function () {}, // will be triggered after the toat has been shown
    beforeHide: function () {}, // will be triggered before the toast gets hidden
    afterHidden: function () {}  // will be triggered after the toast has been hidden
*/
var toastStackAllowred = 5;
var toastTransition = 'slide';
var toastLoader = false;
var toastDefaultHideAfterMS = 5000;
function ShowToastSuccess(message, headText = "Success", hideAfterMS = toastDefaultHideAfterMS) {
    $.toast({
        heading: headText,
        icon: 'success',
        position: 'top-right',
        text: message,
        showHideTransition: toastTransition,
        hideAfter: hideAfterMS,
        stack: toastStackAllowred,
        loader: toastLoader
    });
}
function ShowToastError(message, headText = "Error", hideAfterMS = toastDefaultHideAfterMS) {
    $.toast({
        heading: headText,
        icon: 'error',
        position: 'top-right',
        text: message,
        showHideTransition: toastTransition,
        hideAfter: hideAfterMS,
        stack: toastStackAllowred,
        loader: toastLoader
    });
}
function ShowToastWarning(message, headText = "Warning", hideAfterMS = toastDefaultHideAfterMS) {
    $.toast({
        heading: headText,
        icon: 'warning',
        position: 'top-right',
        text: message,
        showHideTransition: toastTransition,
        hideAfter: hideAfterMS,
        stack: toastStackAllowred,
        loader: toastLoader
    });
}
function ShowToastInfo(message, headText = "Information", hideAfterMS = toastDefaultHideAfterMS) {
    $.toast({
        heading: headText,
        icon: 'info',
        position: 'top-right',
        text: message,
        showHideTransition: toastTransition,
        hideAfter: hideAfterMS,
        stack: toastStackAllowred,
        loader: toastLoader
    });
}

$.fn.select2.defaults.set("theme", "bootstrap-5");


async function ConfirmDialog(title, msg, icon = "question", confirmText = "Yes", cancelText = "No") {
    const result = await Swal.fire({
        theme: 'dark',
        title: title,
        icon: icon,
        html: msg,
        showCancelButton: true,
        confirmButtonColor: "#d77125",
        cancelButtonColor: "#d33",
        confirmButtonText: confirmText,
        cancelButtonText: cancelText,
    });
    if (result.isConfirmed) {
        return true;
    } else {
        return false;
    }
}