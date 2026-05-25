(function () {
    "use strict";

    var TRANSLATABLE_ATTRIBUTES = [
        "placeholder",
        "title",
        "aria-label",
        "data-confirm",
        "data-confirm-title",
        "data-placeholder"
    ];

    function onReady(fn) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", fn);
            return;
        }

        fn();
    }

    function getDictionary() {
        return window.appUi && window.appUi.text ? window.appUi.text : {};
    }

    function getSortedKeys(dictionary) {
        return Object.keys(dictionary)
            .filter(function (key) {
                return typeof key === "string" &&
                    key.trim() &&
                    typeof dictionary[key] === "string" &&
                    dictionary[key] !== key;
            })
            .sort(function (left, right) { return right.length - left.length; });
    }

    function translateText(text, dictionary, keys) {
        if (typeof text !== "string" || !text) {
            return text;
        }

        var trimmed = text.trim();
        var exact = dictionary[trimmed];
        if (exact && exact !== trimmed) {
            var start = text.indexOf(trimmed);
            if (start >= 0) {
                return text.substring(0, start) + exact + text.substring(start + trimmed.length);
            }
        }

        var translated = text;
        keys.forEach(function (key) {
            if (translated.indexOf(key) !== -1) {
                translated = translated.split(key).join(dictionary[key]);
            }
        });

        return translated;
    }

    function translateAttributes(element, dictionary, keys) {
        if (!(element instanceof Element)) {
            return;
        }

        TRANSLATABLE_ATTRIBUTES.forEach(function (attributeName) {
            if (!element.hasAttribute(attributeName)) {
                return;
            }

            var value = element.getAttribute(attributeName);
            var translated = translateText(value, dictionary, keys);
            if (translated !== value) {
                element.setAttribute(attributeName, translated);
            }
        });

        if (element instanceof HTMLInputElement) {
            var type = (element.getAttribute("type") || "").toLowerCase();
            if ((type === "button" || type === "submit" || type === "reset") && element.value) {
                var translatedValue = translateText(element.value, dictionary, keys);
                if (translatedValue !== element.value) {
                    element.value = translatedValue;
                }
            }
        }
    }

    function translateNode(root, dictionary, keys) {
        if (!root || !dictionary || !keys.length) {
            return;
        }

        if (root.nodeType === Node.TEXT_NODE) {
            var parentTag = root.parentElement ? root.parentElement.tagName : "";
            if (parentTag === "SCRIPT" || parentTag === "STYLE" || parentTag === "NOSCRIPT") {
                return;
            }

            var translatedText = translateText(root.textContent, dictionary, keys);
            if (translatedText !== root.textContent) {
                root.textContent = translatedText;
            }
            return;
        }

        if (!(root instanceof Element)) {
            return;
        }

        translateAttributes(root, dictionary, keys);

        Array.prototype.forEach.call(root.childNodes, function (child) {
            translateNode(child, dictionary, keys);
        });
    }

    function initLocalizationBridge() {
        var dictionary = getDictionary();
        var keys = getSortedKeys(dictionary);
        if (!keys.length || !document.body) {
            return;
        }

        window.appUi = window.appUi || {};
        window.appUi.translateText = function (value) {
            return translateText(value, dictionary, keys);
        };
        window.appUi.translateNode = function (root) {
            translateNode(root, dictionary, keys);
        };

        document.title = translateText(document.title, dictionary, keys);
        translateNode(document.body, dictionary, keys);

        var observerBusy = false;
        var observer = new MutationObserver(function (mutations) {
            if (observerBusy) {
                return;
            }

            observerBusy = true;
            try {
                mutations.forEach(function (mutation) {
                    if (mutation.type === "characterData") {
                        translateNode(mutation.target, dictionary, keys);
                        return;
                    }

                    if (mutation.type === "attributes" && mutation.target) {
                        translateAttributes(mutation.target, dictionary, keys);
                        return;
                    }

                    Array.prototype.forEach.call(mutation.addedNodes || [], function (node) {
                        translateNode(node, dictionary, keys);
                    });
                });
            } finally {
                observerBusy = false;
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true,
            characterData: true,
            attributes: true,
            attributeFilter: TRANSLATABLE_ATTRIBUTES
        });

        if (typeof window.alert === "function") {
            var originalAlert = window.alert.bind(window);
            window.alert = function (message) {
                return originalAlert(translateText(String(message), dictionary, keys));
            };
        }

        if (typeof window.confirm === "function") {
            var originalConfirm = window.confirm.bind(window);
            window.confirm = function (message) {
                return originalConfirm(translateText(String(message), dictionary, keys));
            };
        }
    }

    function getScope(root) {
        return root instanceof Element || root instanceof Document ? root : document;
    }

    function initSearchableSelects(root) {
        root = getScope(root);
        if (!window.jQuery || !window.jQuery.fn || !window.jQuery.fn.select2) {
            return;
        }

        var $ = window.jQuery;
        root.querySelectorAll("select.js-search-select").forEach(function (select) {
            if (select.dataset.select2Initialized === "true") {
                return;
            }

            if (select.hasAttribute("data-ajax-url")) {
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

    function initDatePickers(root) {
        root = getScope(root);
        if (!window.jQuery || !window.jQuery.fn || !window.jQuery.fn.datepicker) {
            return;
        }

        var $ = window.jQuery;
        var htmlLang = (document.documentElement.getAttribute("lang") || "").toLowerCase();
        var defaultLanguage = htmlLang.indexOf("fa") === 0 ? "fa" : "en";
        var defaultRtl = (document.documentElement.getAttribute("dir") || "").toLowerCase() === "rtl";

        function parseDateValue(value) {
            if (typeof value !== "string") {
                return null;
            }

            var match = value.trim().match(/^(\d{4})[\/-](\d{2})[\/-](\d{2})(?:[T\s](\d{2}):(\d{2})(?::(\d{2}))?)?$/);
            if (!match) {
                return null;
            }

            return new Date(
                Number(match[1]),
                Number(match[2]) - 1,
                Number(match[3]),
                Number(match[4] || 0),
                Number(match[5] || 0),
                Number(match[6] || 0)
            );
        }

        function pad(value) {
            return String(value).padStart(2, "0");
        }

        function formatDateTimeValue(date, timeSource) {
            var time = timeSource instanceof Date && !Number.isNaN(timeSource.getTime())
                ? timeSource
                : parseDateValue(String(timeSource || ""));
            var hours = time ? time.getHours() : 0;
            var minutes = time ? time.getMinutes() : 0;
            var year = date.getFullYear();
            var month = pad(date.getMonth() + 1);
            var day = pad(date.getDate());

            return year + "-" + month + "-" + day + "T" + pad(hours) + ":" + pad(minutes);
        }

        function initSingleDatePicker(input) {
            if (!(input instanceof HTMLInputElement)) {
                return;
            }

            var dateFormat = input.getAttribute("data-date-format") || "yyyy/mm/dd";
            var language = (input.getAttribute("data-date-language") || defaultLanguage || "en").toLowerCase();
            language = language.indexOf("fa") === 0 ? "fa" : "en";
            var options = {
                autoclose: true,
                rtl: defaultRtl,
                language: language,
                format: dateFormat,
                templates: {
                    leftArrow: '<i class="simple-icon-arrow-left"></i>',
                    rightArrow: '<i class="simple-icon-arrow-right"></i>'
                }
            };
            var isInitialized = input.dataset.datePickerInitialized === "true";
            var targetSelector = input.getAttribute("data-date-picker-target") || "";
            var target = targetSelector ? (root.querySelector(targetSelector) || document.querySelector(targetSelector)) : null;

            if (!isInitialized) {
                $(input).datepicker(options);
                input.dataset.datePickerInitialized = "true";
            }

            if (!target || !(target instanceof HTMLInputElement)) {
                return;
            }

            if (input.dataset.datePickerSyncBound !== "true") {
                var updateTarget = function (date) {
                    if (!date) {
                        target.value = "";
                        return;
                    }

                    var previousValue = parseDateValue(target.value);
                    target.value = formatDateTimeValue(date, previousValue || new Date());
                };

                $(input).on("changeDate", function (event) {
                    updateTarget(event.date || $(input).datepicker("getDate"));
                });

                $(input).on("change blur", function () {
                    var currentDate = $(input).datepicker("getDate") || parseDateValue(input.value);
                    updateTarget(currentDate);
                });

                input.dataset.datePickerSyncBound = "true";
            }

            var initialValue = target.value || input.value;
            var initialDate = parseDateValue(initialValue);
            if (initialDate) {
                $(input).datepicker("setDate", initialDate);
            }
        }

        root.querySelectorAll('input[type="datetime-local"]').forEach(function (input) {
            if (!(input instanceof HTMLInputElement) || input.dataset.datePickerUpgraded === "true") {
                return;
            }

            var targetId = input.id || "";
            if (!targetId) {
                targetId = "date-picker-" + Math.random().toString(36).slice(2, 10);
                input.id = targetId;
            }

            var display = document.createElement("input");
            display.type = "text";
            display.className = "form-control datepicker";
            display.setAttribute("data-date-picker-target", "#" + targetId);
            display.setAttribute("data-date-format", input.getAttribute("data-date-format") || "yyyy/mm/dd");
            display.setAttribute("autocomplete", "off");
            display.value = (() => {
                var parsed = parseDateValue(input.value || "");
                if (!parsed) {
                    return "";
                }

                return [
                    parsed.getFullYear(),
                    pad(parsed.getMonth() + 1),
                    pad(parsed.getDate())
                ].join("/");
            })();

            input.type = "hidden";
            input.dataset.datePickerUpgraded = "true";
            input.parentNode.insertBefore(display, input.nextSibling);
            initSingleDatePicker(display);
        });

        root.querySelectorAll("input.datepicker").forEach(initSingleDatePicker);
    }

    function initAutoSubmit(root) {
        root = getScope(root);

        root.querySelectorAll("[data-auto-submit]").forEach(function (element) {
            if (element.dataset.autoSubmitBound === "true") {
                return;
            }

            element.dataset.autoSubmitBound = "true";
            element.addEventListener("change", function () {
                var form = element instanceof HTMLFormElement ? element : element.form;
                if (!form) {
                    return;
                }

                if (typeof form.requestSubmit === "function") {
                    form.requestSubmit();
                    return;
                }

                form.submit();
            });
        });
    }

    function initBulkActions(root) {
        root = getScope(root);
        root.querySelectorAll("form[data-bulk-form]").forEach(function (form) {
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
                    window.alert((window.appUi && window.appUi.text && window.appUi.text["bulk.selectAtLeastOne"]) || "حداقل یک ردیف را برای عملیات گروهی انتخاب کنید.");
                    return;
                }

                if (actionField && !actionField.value) {
                    event.preventDefault();
                    window.alert((window.appUi && window.appUi.text && window.appUi.text["bulk.actionNotSpecified"]) || "نوع عملیات گروهی مشخص نشده است.");
                }
            });

            updateState();
        });
    }

    function initInventoryManagementPage(root) {
        root = getScope(root);

        var tabKey = 'inventory-warehouses-active-tab';
        var tabs = root.querySelectorAll('#warehouse-management-tabs .nav-link');
        if (tabs.length > 0) {
            tabs.forEach(function (tab) {
                if (tab.dataset.imBound === 'true') {
                    return;
                }

                tab.dataset.imBound = 'true';
                tab.addEventListener('shown.bs.tab', function () {
                    var href = tab.getAttribute('href');
                    if (href) {
                        localStorage.setItem(tabKey, href);
                    }
                });
            });

            var activeTab = localStorage.getItem(tabKey);
            if (activeTab && window.jQuery) {
                var target = root.querySelector('#warehouse-management-tabs .nav-link[href="' + activeTab + '"]');
                if (target) {
                    window.jQuery(target).tab('show');
                }
            }
        }

        root.querySelectorAll('[data-section-tabs]').forEach(function (tabList) {
            if (tabList.dataset.imBound === 'true') {
                return;
            }

            tabList.dataset.imBound = 'true';
            var links = tabList.querySelectorAll('[data-section-target]');
            links.forEach(function (link) {
                link.addEventListener('click', function () {
                    var targetSelector = link.getAttribute('data-section-target');
                    var target = targetSelector ? root.querySelector(targetSelector) : null;
                    if (!target) {
                        return;
                    }

                    links.forEach(function (item) { item.classList.remove('active'); });
                    link.classList.add('active');

                    var prefix = targetSelector.indexOf('#location-') === 0 ? 'location-' : 'warehouse-';
                    root.querySelectorAll('.inventory-section-pane[id^="' + prefix + '"]').forEach(function (pane) {
                        pane.classList.add('d-none');
                    });
                    target.classList.remove('d-none');
                });
            });
        });
    }

    onReady(function () {
        window.appUi = window.appUi || {};
        window.appUi.initSearchableSelects = initSearchableSelects;
        window.appUi.initDatePickers = initDatePickers;
        window.appUi.initAutoSubmit = initAutoSubmit;
        window.appUi.initBulkActions = initBulkActions;
        window.appUi.initInventoryManagementPage = initInventoryManagementPage;

        initLocalizationBridge();
        initSearchableSelects();
        initDatePickers();
        initAutoSubmit();
        initBulkActions();
        initInventoryManagementPage();
    });
})();
