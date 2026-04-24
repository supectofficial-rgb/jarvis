(function () {
    "use strict";

    function onReady(fn) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", fn);
            return;
        }

        fn();
    }

    function initSearchableSelects() {
        if (!window.jQuery || !window.jQuery.fn || !window.jQuery.fn.select2) {
            return;
        }

        var $ = window.jQuery;
        document.querySelectorAll("select.js-search-select").forEach(function (select) {
            if (select.dataset.select2Initialized === "true") {
                return;
            }

            $(select).select2({
                theme: "bootstrap",
                dir: document.documentElement.getAttribute("dir") || "rtl",
                width: select.getAttribute("data-width") || "100%",
                placeholder: select.getAttribute("data-placeholder") || "",
                minimumResultsForSearch: select.hasAttribute("data-no-search") ? Infinity : 0
            });

            select.dataset.select2Initialized = "true";
        });
    }

    function initBulkActions() {
        document.querySelectorAll("form[data-bulk-form]").forEach(function (form) {
            var scopeSelector = form.getAttribute("data-bulk-scope") || "";
            var scope = scopeSelector ? document.querySelector(scopeSelector) : null;
            if (!scope) {
                return;
            }

            var rows = Array.prototype.slice.call(scope.querySelectorAll("input[type='checkbox'][data-bulk-row]"));
            var master = scope.querySelector("input[type='checkbox'][data-bulk-toggle]");
            var targetField = form.querySelector("input[data-bulk-target]");
            var actionField = form.querySelector("input[data-bulk-action-input]");
            var countElements = Array.prototype.slice.call(form.querySelectorAll("[data-bulk-count]"));
            var actionButtons = Array.prototype.slice.call(form.querySelectorAll("button[data-bulk-action]"));

            function selectedIds() {
                return rows
                    .filter(function (row) { return row.checked; })
                    .map(function (row) { return row.value || ""; })
                    .filter(Boolean);
            }

            function updateState() {
                var ids = selectedIds();
                if (targetField) {
                    targetField.value = ids.join(",");
                }

                countElements.forEach(function (element) {
                    element.textContent = ids.length.toString();
                });

                actionButtons.forEach(function (button) {
                    var allowed = (button.getAttribute("data-bulk-permission") || "allowed") === "allowed";
                    button.disabled = ids.length === 0 || !allowed;
                });

                if (master) {
                    master.checked = rows.length > 0 && ids.length === rows.length;
                    master.indeterminate = ids.length > 0 && ids.length < rows.length;
                }
            }

            if (master) {
                master.addEventListener("change", function () {
                    rows.forEach(function (row) {
                        row.checked = master.checked;
                    });
                    updateState();
                });
            }

            rows.forEach(function (row) {
                row.addEventListener("change", updateState);
            });

            actionButtons.forEach(function (button) {
                button.addEventListener("click", function () {
                    if (actionField) {
                        actionField.value = button.getAttribute("data-bulk-action") || "";
                    }
                });
            });

            form.addEventListener("submit", function (event) {
                if (selectedIds().length === 0) {
                    event.preventDefault();
                    window.alert("حداقل یک ردیف را برای عملیات گروهی انتخاب کنید.");
                    return;
                }

                if (actionField && !actionField.value) {
                    event.preventDefault();
                    window.alert("نوع عملیات گروهی مشخص نشده است.");
                }
            });

            updateState();
        });
    }

    onReady(function () {
        initSearchableSelects();
        initBulkActions();
    });
})();
