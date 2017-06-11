{{VERSION}}
angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, $filter, assetsService, angularHelper, notificationsService, $timeout, fileManager, entityResource, archetypeService, archetypeLabelService, archetypeCacheService, archetypePropertyEditorResource) {

    // Variables.
    var draggedParent;

    //$scope.model.value = "";
    $scope.model.hideLabel = $scope.model.config.hideLabel == 1;

    //get a reference to the current form
    $scope.form = $scope.form || angularHelper.getCurrentForm($scope);

    //set the config equal to our prevalue config
    $scope.model.config = $scope.model.config.archetypeConfig;

    //ini the model
    $scope.model.value = $scope.model.value || getDefaultModel($scope.model.config);

    // store the umbraco property alias to help generate unique IDs.  Hopefully there's a better way to get this in the future :)
    $scope.umbracoHostPropertyAlias = $scope.$parent.$parent.model.alias;
    
    $scope.isDebuggingEnabled = Umbraco.Sys.ServerVariables.isDebuggingEnabled;

    $scope.overlayMenu = {
        show: false,
        style: {}
    };

    init();

    //hold references to helper resources 
    $scope.resources = {
        entityResource: entityResource,
        archetypePropertyEditorResource: archetypePropertyEditorResource
    }

    //hold references to helper services 
    $scope.services = {
        archetypeService: archetypeService,
        archetypeLabelService: archetypeLabelService,
        archetypeCacheService: archetypeCacheService
    }

    //helper to get $eval the labelTemplate
    $scope.getFieldsetTitle = function (fieldsetConfigModel, fieldsetIndex) {
        return archetypeLabelService.getFieldsetTitle($scope, fieldsetConfigModel, fieldsetIndex);
    }

    //sort config
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        tolerance: "pointer",
        activate: function(ev, ui) {

            // Variables.
            var parentItem = ui.item.parent();
            var thisElement = parentItem[0];
            var targetElement = ev.target;
            var targetItem = angular.element(targetElement);

            // The source of the drag can always be dropped back onto.
            if (targetElement === thisElement) {
                return;
            }

            // Variables.
            var sourceScope = ui.item.scope();
            var targetScope = targetItem.scope();
            var valid = canMove(sourceScope, targetScope);

            // If the sortable can't be moved to the target Archetype, disable
            // the target Archetype's sortable temporarily.
            if (!valid) {
                var targetSortable = getSortableWidgetInstance(targetItem);
                targetSortable.disable();
                parentItem.sortable("refresh");
                archetypeService.rememberDisabledSortable(targetSortable);
            }

        },
        start: function(ev, ui) {
            archetypeService.storeEditors(ui.item.parent());
            $scope.$apply(function() {
                draggedParent = ui.item.parent();
                draggedParent.scope().doingSort = true;
            });
        },
        update: function (ev, ui) {

            // Variables.
            var targetScope = ui.item.sortable.droptarget.scope();
            var sourceScope = ui.item.scope();
            var sameScope = sourceScope === targetScope;
            var sourceIndex = ui.item.sortable.index;

            // Special constraints for when moving between Archetypes.
            // If sourceScope is populated, we are in the first of the two updates (when
            // moving between lists, ui-sortable calls the update function twice).
            if (sourceScope && !sameScope) {

                // Variables.
                var valid = canMove(sourceScope, targetScope);

                // If update isn't allowed, cancel the drag operation.
                if (!valid) {
                    ui.item.sortable.cancel();
                    return;
                }

                // Clear the validations for this item.
                clearValidations(ui.item.parent());
                clearValidations(ui.item.sortable.droptarget);

                // Reset "isValid" on the properties and fieldsets.
                var fieldsetGroups = [
                    $scope.model.value.fieldsets,
                    targetScope.model.value.fieldsets
                ];
                _.each(fieldsetGroups, function(fieldsets) {
                    recurseProperties(function(property, fieldset) {
                        property.isValid = true;
                        fieldset.isValid = true;
                    }, fieldsets);
                });

                // Move the activated fieldset to the target Archetype.
                var movedFieldset = $scope.model.value.fieldsets[sourceIndex];
                var loadedIndex = $scope.loadedFieldsets.indexOf(movedFieldset);
                if (loadedIndex >= 0) {
                    $scope.loadedFieldsets.splice(loadedIndex, 1);
                    if (targetScope.loadedFieldsets.indexOf(movedFieldset) < 0) {
                        targetScope.loadedFieldsets.push(movedFieldset);
                    }
                }

            }

            // Set scope dirty.
            $scope.setDirty();

        },
        stop: function (ev, ui) {

            // Done sorting.
            draggedParent.scope().doingSort = false;

            // Enable disabled sortables.
            archetypeService.enableSortables();

            // Restore rich text editors.
            var parent = null;
            var target = ui.item.sortable.droptarget;
            if (target) {
                parent = target.parent();
            }
            archetypeService.restoreEditors(parent);

        }
    };

    // Clears the Angular validations in an Archetype.
    function clearValidations(el) {
        var combined = $(".archetypeSortable", el).add(el);
        combined.each(function(index, item) {
            var $item = angular.element(item);
            var cont = $item.controller("ngModel");
            var err = cont.$error;
            var keys = Object.keys(err);
            for (var i = 0; i < keys.length; i++) {
                cont.$setValidity(keys[i], true);
            }
        });
    }

    // Enable cross-archetype dragging?
    if ($scope.model.config.enableCrossDragging) {
        $scope.sortableOptions.connectWith = ".archetypeSortable:not(.invalid)";
    }

    // Checks if the specified model's properties match all of the properties in any of
    // the specified fieldsets, while also checking if the fieldset aliases match.
    // This is an indicator of the compatibility of a fieldset model with a collection
    // of fieldset configurations.
    function modelMatchesAnyFieldset(model, fieldsets) {

        // Loop through fieldsets to find a match.
        for (var i = 0; i < fieldsets.length; i++) {
            var fieldset = fieldsets[i];

            // Confirm that this configured fielset contains exactly
            // the same properties as those that were supplied.
            var valid =
                // Does the alias match?
                model.alias === fieldset.alias &&
                // Does this fieldset have all the properties?
                arePropertiesSubset(model.properties, fieldset.properties) &&
                // Does the property array have all the fieldset properties?
                arePropertiesSubset(fieldset.properties, model.properties);

            // Match found?
            if (valid) {
                return true;
            }

        }

        // No match found.
        return false;

    }

    // Checks if the specified array of fieldsets contains the specified fieldset.
    function arrayContainsFieldset(fieldset, fieldsets) {
        for (var i = 0; i < fieldsets.length; i++) {
            if (fieldsets[i] === fieldset) {
                return true;
            }
        }
        return false;
    }

    // Confirms that an array of properties is a subset of another array of properties.
    function arePropertiesSubset(subset, superset) {

        // Loop through the subset of properties.
        for (var j = 0; j < subset.length; j++) {
            var subProperty = subset[j];
            var matchedProp = false;

            // Loop through the superset to find a matching property from the subset.
            for (var k = 0; k < superset.length; k++) {
                var superProperty = superset[k];
                if (superProperty.alias === subProperty.alias &&
                    superProperty.dataTypeGuid === subProperty.dataTypeGuid) {
                    matchedProp = true;
                    break;
                }
            }

            // If no matching property could be found, the array is not a subset.
            if (!matchedProp) {
                return false;
            }

        }

        // The array is a subset.
        return true;

    }

    //handles a fieldset add
    $scope.openFieldsetPicker = function ($index, event) {
        if ($scope.canAdd() == false) {
            return;
        }

        var allFieldsets = [];
        _.each($scope.model.config.fieldsets, function (fieldset) {
            var icon = fieldset.icon;
            allFieldsets.push({
                alias: fieldset.alias,
                label: fieldset.label,
                icon: (fieldset.icon || "icon-document-dashed-line"), // default icon if none is chosen
                group: fieldset.group ? fieldset.group.name : null,
                previewImage: fieldset.previewImage
            });
        });
        // sanity check
        if (allFieldsets == 0) {
            return;
        }
        if (allFieldsets.length == 1) {
            // only one fieldset type - no need to display the picker
            $scope.addRow(allFieldsets[0].alias, $index);
            return;
        }

        $scope.overlayMenu.fieldsetGroups = [];
        if ($scope.model.config.fieldsetGroups && $scope.model.config.fieldsetGroups.length > 0) {
            _.each($scope.model.config.fieldsetGroups, function (fieldsetGroup) {
                $scope.overlayMenu.fieldsetGroups.push({ name: fieldsetGroup.name, fieldsets: $filter("filter")(allFieldsets, { group: fieldsetGroup.name }, true) });
            })
        }
        else {
            $scope.overlayMenu.fieldsetGroups.push({ name: "", fieldsets: allFieldsets });
        }
        $scope.overlayMenu.index = $index;
        $scope.overlayMenu.activeFieldsetGroup = $scope.overlayMenu.fieldsetGroups[0];

        // calculate overlay position
        // - yeah... it's jQuery (ungh!) but that's how the Grid does it.
        var offset = $(event.target).offset();
        var scrollTop = $(event.target).closest(".umb-panel-body").scrollTop();
        if (offset.top < 400) {
            $scope.overlayMenu.style.top = 300 + scrollTop;
        }
        else {
            $scope.overlayMenu.style.top = offset.top - 150 + scrollTop;
        }
        $scope.overlayMenu.show = true;
    };

    $scope.closeFieldsetPicker = function () {
        $scope.overlayMenu.show = false;
        $scope.overlayMenu.fieldsetPreview = null;
    };
    
    $scope.pickFieldset = function (fieldsetAlias, $index) {
        $scope.closeFieldsetPicker();
        $scope.addRow(fieldsetAlias, $index);
    };    

    $scope.openFieldsetPreview = function (fieldset) {
        $scope.overlayMenu.fieldsetPreview = fieldset;
    }

    $scope.closeFieldsetPreview = function () {
        $scope.overlayMenu.fieldsetPreview = null;
    }

    $scope.addRow = function (fieldsetAlias, $index) {
        if ($scope.canAdd()) {
            if ($scope.model.config.fieldsets) {
                var newFieldset = getEmptyRenderFieldset($scope.getConfigFieldsetByAlias(fieldsetAlias));

                if (typeof $index != 'undefined')
                {
                    $scope.model.value.fieldsets.splice($index + 1, 0, newFieldset);
                }
                else
                {
                    $scope.model.value.fieldsets.push(newFieldset);
                }
            }

            addCustomPropertiesToFieldset(newFieldset);

            $scope.setDirty();

            $scope.$broadcast("archetypeAddFieldset", {index: $index, visible: countVisible()});

            newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;

            // If the fieldset is not collapsed, it should be instantly loaded.
            if (!newFieldset.collapse) {
                $scope.loadedFieldsets.push(newFieldset);
            }
            
            $scope.focusFieldset(newFieldset);
            handleMandatoryValidation();
        }
    }

    $scope.removeRow = function ($index) {
        if ($scope.canRemove()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.setDirty();
                $scope.model.value.fieldsets.splice($index, 1);
                $scope.$broadcast("archetypeRemoveFieldset", {index: $index});
                handleMandatoryValidation();
            }
        }
    }

    $scope.cloneRow = function ($index) {
        if ($scope.canClone() && typeof $index != 'undefined') {
            var newFieldset = angular.copy($scope.model.value.fieldsets[$index]);

            if(newFieldset) {

                // Regenerate the temporary ID on each nested fieldset.
                // This is done because no two fieldsets should have the same
                // temporary ID.
                recurseProperties(function(property) {
                    archetypeService.ensureTemporaryId(property, true);
                }, [newFieldset]);

                $scope.model.value.fieldsets.splice($index + 1, 0, newFieldset);

                $scope.setDirty();

                newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;

                // If the fieldset is not collapsed, it should be instantly loaded.
                if (!newFieldset.collapse) {
                    $scope.loadedFieldsets.push(newFieldset);
                }

                $scope.focusFieldset(newFieldset);
            }
        }
    }

    $scope.enableDisable = function (fieldset) {
        fieldset.disabled = !fieldset.disabled;
        // explicitly set the form as dirty when manipulating the enabled/disabled state of a fieldset
        $scope.setDirty();
    }

    //helpers for determining if a user can do something
    $scope.canAdd = function () {
        if ($scope.model.config.maxFieldsets)
        {
            var visibleCount = countVisible();
            var maxFieldsets = $scope.model.config.maxFieldsets;
            return visibleCount < maxFieldsets;
        }

        return true;
    };

    //helper that returns if an item can be removed
    $scope.canRemove = function () {
        return countVisible() > 1 
            || ($scope.model.config.maxFieldsets == 1 && $scope.model.config.fieldsets.length > 1)
            || $scope.model.config.startWithAddButton;
    };

    $scope.canClone = function () {

        if (!$scope.model.config.enableCloning) {
            return false;
        }

        if ($scope.model.config.maxFieldsets)
        {
            return countVisible() < $scope.model.config.maxFieldsets;
        }

        return true;
    }

    //helper that returns if an item can be sorted
    $scope.canSort = function ()
    {
        // Sorting can occur if there are multiple fieldsets, or if there is only one
        // fieldset that can be removed (in which case it can be sorted into an entirely
        // different Archetype).
        return countVisible() > 1 || $scope.canRemove();
    };

    //helper that returns if an item is the last and it's being sorted.
    $scope.sortingLastItem = function() {
        return $scope.doingSort && $scope.model.value.fieldsets.length <= 1;
    };

    //helper that returns if an item can be disabled
    $scope.canDisable = function () {
        return $scope.model.config.enableDisabling;
    }

    //helpers for determining if the add button should be shown
    $scope.showAddButton = function () {
        return $scope.model.config.startWithAddButton
            && countVisible() === 0;
            ///&& $scope.model.config.fieldsets.length == 1;
    }

    //helper that returns if an item can use publishing
    $scope.canPublish = function () {
        return $scope.model.config.enablePublishing;
    }

    $scope.canUseMemberGroups = function() {
        return $scope.model.config.enableMemberGroups;
    }

    //helper that returns if the "misc fieldset configuration" section should be visible
    $scope.canConfigure = function () {
        // currently the "misc fieldset configuration" section contains the publishing and the member groups setup
        return $scope.canPublish() || $scope.canUseMemberGroups();
    }

    $scope.showDisableIcon = function (fieldset) {
        if ($scope.canDisable() == false) {
            return false;
        }
        // disabled state takes precedence over publishing
        if (fieldset.disabled) {
            return true;
        }
        return $scope.isDisabledByPublishing(fieldset) == false;
    }

    $scope.showPublishingIcon = function (fieldset) {
        if ($scope.canPublish() == false) {
            return false;
        }
        if ($scope.canDisable()) {
            // disabled state takes precedence over publishing
            if (fieldset.disabled) {
                return false;
            }
            return $scope.isDisabledByPublishing(fieldset);
        }
        return true;
    }

    $scope.isDisabledByPublishing = function(fieldset) {
        if ($scope.canPublish() === false) {
            return false;
        }
        // NOTE: all comparison is done in local datetime
        //       - that's fine because the selected local datetimes will be converted to UTC datetimes when submitted
        if (fieldset.expireDateModel && fieldset.expireDateModel.value && (moment() > moment(fieldset.expireDateModel.value))) {
            // an expired release affects the fieldset
            return true;
        }
        if (fieldset.releaseDateModel && fieldset.releaseDateModel.value && (moment(fieldset.releaseDateModel.value) > moment())) {
            // a pending release affects the fieldset
            return true;
        }
        return false;
    }

    $scope.isDisabled = function(fieldset) {
        if (fieldset.disabled) {
            return true;
        }
        return $scope.isDisabledByPublishing(fieldset);
    }

    //helper, ini the render model from the server (model.value)
    function init() {
        $scope.model.value = removeNulls($scope.model.value);
        addDefaultProperties($scope.model.value.fieldsets);
        handleMandatoryValidation();
    }

    function addDefaultProperties(fieldsets)
    {
        _.each(fieldsets, function (fieldset)
        {
            fieldset.collapse = false;
            fieldset.isValid = true;
        });
    }

    function addCustomProperties(fieldsets) {
        // make sure we have loaded moment.js before using it
        assetsService.loadJs("lib/moment/moment-with-locales.js").then(function() {
            _.each(fieldsets, function(fieldset) {
                addCustomPropertiesToFieldset(fieldset);
            });
        });
    }

    function addCustomPropertiesToFieldset(fieldset) {
        // create models for publish configuration (utilizing the built-in datepicker data type)
        // NOTE: all datetimes must be converted from UTC to local
        fieldset.releaseDateModel = {
            alias: _.uniqueId("archetypeReleaseDate_"),
            view: "datepicker",
            value: fromUtc(fieldset.releaseDate)
        };
        fieldset.expireDateModel = {
            alias: _.uniqueId("archetypeExpireDate_"),
            view: "datepicker",
            value: fromUtc(fieldset.expireDate)
        };
        // create model for allowed member groups
        fieldset.allowedMemberGroupsModel = {
            alias: _.uniqueId("archetypeAllowedMemberGroups_"),
            view: "membergrouppicker",
            value: fieldset.allowedMemberGroups
        };
    }

    //helper to get the correct fieldset from config
    $scope.getConfigFieldsetByAlias = function(alias) {
        return _.find($scope.model.config.fieldsets, function(fieldset){
            return fieldset.alias == alias;
        });
    }

    //helper to get a property by alias from a fieldset
    $scope.getPropertyValueByAlias = function(fieldset, propertyAlias) {
        var property = _.find(fieldset.properties, function(p) {
            return p.alias == propertyAlias;
        });
        return (typeof property !== 'undefined') ? property.value : '';
    };

    $scope.isCollapsed = function(fieldset)
    {
        if(typeof fieldset.collapse === "undefined")
        {
            fieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
        }
        return fieldset.collapse;
    };

    // added to track loaded fieldsets 
    $scope.loadedFieldsets = [];
    $scope.isLoaded = function (fieldset) {
        return $scope.loadedFieldsets.indexOf(fieldset) >= 0;
    }

    //helper for expanding/collapsing fieldsets
    $scope.focusFieldset = function(fieldset){
        fixDisableSelection();

        if (!$scope.model.config.enableCollapsing) {
            return;
        }

        var iniState;

        if(fieldset)
        {
            iniState = fieldset.collapse;
        }

        _.each($scope.model.value.fieldsets, function(fieldset){
            fieldset.collapse = true;
        });

        if(!fieldset && $scope.model.value.fieldsets.length == 1)
        {
            $scope.model.value.fieldsets[0].collapse = false;
            $scope.loadedFieldsets.push($scope.model.value.fieldsets[0]);
            return;
        }

        if(iniState && fieldset)
        {
            fieldset.collapse = !iniState;
            $scope.loadedFieldsets.push(fieldset);
        }
    }

    //ini the fieldset expand/collapse
    $scope.focusFieldset();

    // Fieldsets which cannot be collapsed should start expanded.
    _.each($scope.model.value.fieldsets, function(fieldset) {
        fieldset.collapse = $scope.model.config.enableCollapsing;
    });
    $scope.loadedFieldsets = _.where($scope.model.value.fieldsets, { collapse: false });

    //developerMode helpers
    $scope.model.value.toString = stringify;

    // issue 114: register handler for file selection
    $scope.model.value.setFiles = setFiles;

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }

    // issue 114: handler for file selection
    function setFiles(files) {
        // get all currently selected files from file manager
        var currentFiles = fileManager.getFiles();
        
        // get the files already selected for this archetype (by alias)
        var archetypeFiles = [];
        _.each(currentFiles, function (item) {
            if (item.alias === $scope.model.alias) {
                archetypeFiles.push(item.file);
            }
        });

        // add the newly selected files
        _.each(files, function (file) {
            archetypeFiles.push(file);
        });

        // update the selected files for this archetype (by alias)
        fileManager.setFiles($scope.model.alias, archetypeFiles);
    }

    //watch for changes
    $scope.$watch('model.value', function (v) {
        if ($scope.model.config.developerMode) {
            console.log(v);
            if (typeof v === 'string') {
                $scope.model.value = JSON.parse(v);
                $scope.model.value.toString = stringify;
            }
        }

        // issue 114: re-register handler for files selection and reset the currently selected files on the file manager
        $scope.model.value.setFiles = setFiles;
        fileManager.setFiles($scope.model.alias, []);

        // reset submit watcher counter on save
        $scope.activeSubmitWatcher = 0;

        // init loaded fieldsets tracking
        _.each($scope.model.value.fieldsets, function (fieldset) {
            fieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
        });
        $scope.loadedFieldsets = _.where($scope.model.value.fieldsets, { collapse: false });

        // create properties needed for the backoffice to work (data that is not serialized to DB)
        addCustomProperties($scope.model.value.fieldsets);
    });

    //helper to count what is visible
    function countVisible()
    {
        return $scope.model.value.fieldsets.length;
    }

    // helper to get initial model if none was provided
    function getDefaultModel(config) {
        if (config.startWithAddButton)
            return { fieldsets: [] };

        return { fieldsets: [getEmptyRenderFieldset(config.fieldsets[0])] };
    }

    //helper to add an empty fieldset to the render model
    function getEmptyRenderFieldset (fieldsetModel) {
        return {alias: fieldsetModel.alias, collapse: false, isValid: true, properties: []};
    }

    //helper to ensure no nulls make it into the model
    function removeNulls(model){
        if(model.fieldsets){
            _.each(model.fieldsets, function(fieldset, index){
                if(!fieldset){
                    model.fieldsets.splice(index, 1);
                    removeNulls(model);
                }
            });

            return model;
        }
    }

    // Hack for U4-4281 / #61
    function fixDisableSelection() {
        $timeout(function() {
            $('.archetypeEditor .controls')
                .bind('mousedown.ui-disableSelection selectstart.ui-disableSelection', function(e) {
                    e.stopImmediatePropagation();
                });
        }, 1000);
    }

    //helper to lookup validity when given a fieldsetIndex and property alias
    $scope.getPropertyValidity = function(fieldsetIndex, alias)
    {
        if($scope.model.value.fieldsets[fieldsetIndex])
        {
            var property = _.find($scope.model.value.fieldsets[fieldsetIndex].properties, function(property){
                return property.alias == alias;
            });
        }

        return (typeof property == 'undefined') ? true : property.isValid;
    }

    //helper to lookup validity when given a fieldset
    $scope.getFieldsetValidity = function (fieldset) {

        // Variables.
        var valid = true;

        // Recursive validation of nested fieldsets.
        recurseFieldsets(function(item) {
            if (item.isValid === false) {
                valid = false;
            }
        }, [fieldset]);

        // Were all the nested fieldsets valid?
        return valid;

    };

    // helper to force the current form into the dirty state
    $scope.setDirty = function () {
        if($scope.form) {
            $scope.form.$setDirty();
        }
    }

    //custom js
    if ($scope.model.config.customJsPath) {
        assetsService.loadJs($scope.model.config.customJsPath);
    }

    //archetype css
    assetsService.loadCss("../App_Plugins/Archetype/css/archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }

    // submit watcher handling:
    // because some property editors use the "formSubmitting" event to set/clean up their model.value,
    // we need to monitor the "formSubmitting" event from a custom property and broadcast our own event
    // to forcefully update the appropriate model.value's
    $scope.activeSubmitWatcher = 0;
    $scope.submitWatcherOnLoad = function () {
        $scope.activeSubmitWatcher++;
        return $scope.activeSubmitWatcher;
    }
    $scope.submitWatcherOnSubmit = function (args) {
        $scope.$broadcast("archetypeFormSubmitting", args);
    }

    // we'll use our own "archetypeFormSubmitting" event to save custom properties, as at least some 
    // of the editors store their values back to the model on the core "formSubmitting" event
    $scope.$on("archetypeFormSubmitting", function (ev, args) {
        _.each($scope.model.value.fieldsets, function (fieldset) {
            // extract the publish configuration from the fieldsets (and convert local datetimes to UTC)
            fieldset.releaseDate = toUtc(fieldset.releaseDateModel.value);
            fieldset.expireDate = toUtc(fieldset.expireDateModel.value);
            // extract the allowed member groups 
            fieldset.allowedMemberGroups = fieldset.allowedMemberGroupsModel.value;
        });
    });

    // handle mandatory validation of the entire Archetype
    // - no fieldsets = not valid
    function handleMandatoryValidation() {
        var valid = true;
        if ($scope.model.validation && $scope.model.validation.mandatory) {
            valid = $scope.model.value.fieldsets && $scope.model.value.fieldsets.length > 0;
        }
        $scope.model.mandatoryValidation = valid ? "valid" : null;
    }

    function toUtc(date) {
        if (!date) {
            return null;
        }
        return moment(date, "YYYY-MM-DD HH:mm:ss").utc().toDate();
    }

    function fromUtc(date) {
        if (!date) {
            return null;
        }
        return moment(moment.utc(date).toDate()).format("YYYY-MM-DD HH:mm:ss")
    }

    // Serializes and deserializes an item to return a snapshot of that item (e.g., so it is not
    // changed before being inspected). Useful when troubleshooting.
    // Modified from: http://stackoverflow.com/a/11616993/2052963
    function jsonSnapshot(item) {
        var cache = [];
        var stringItem = JSON.stringify(item, function(key, value) {
            if (typeof value === "object" && value !== null) {
                if (cache.indexOf(value) !== -1) {
                    return "[Removed Circular Reference Item]";
                }
                cache.push(value);
            }
            return value;
        });
        return JSON.parse(stringItem);
    }

    // Recursively processes each Archetype fieldset.
    function recurseFieldsets(fn, fieldsets) {
        if (!fieldsets || !fieldsets.length) {
            return;
        }
        _.each(fieldsets, function(fieldset) {
            fn(fieldset);
            _.each(fieldset.properties, function (property) {
                if (property != null && property.value != null && property.propertyEditorAlias === "Imulus.Archetype") {
                    recurseFieldsets(fn, property.value.fieldsets);
                }
            });
        });
    }

    // Recursively processes each Archetype fieldset property.
    function recurseProperties(fn, fieldsets) {
        recurseFieldsets(function(fieldset) {
            _.each(fieldset.properties, function (property) {
                fn(property, fieldset);
            });
        }, fieldsets);
    }

    // Indicates whether or not the fieldset can be moved between the source and target scope.
    // The source scope is the scope of the fieldset being dragged.
    // The target scope is the scope of the Archetype to drop into.
    function canMove(sourceScope, targetScope) {
        var targetFieldsetConfigs = targetScope.model.config.fieldsets;
        var targetFieldsets = targetScope.model.value.fieldsets;
        var sourceFieldset = sourceScope.fieldset;
        var sameArchetype = arrayContainsFieldset(sourceFieldset, targetFieldsets);
        var model = sourceScope.fieldsetConfigModel;
        var canRemove = sourceScope.canRemove();
        var canAdd = targetScope.canAdd();
        var valid = sameArchetype || (canRemove && canAdd);
        valid = valid && modelMatchesAnyFieldset(model, targetFieldsetConfigs);
        return valid;
    }

    // This is an alternative to: element.sortable("instance")
    // Workaround for this issue: https://github.com/imulus/Archetype/pull/356#issuecomment-218527910
    // Snagged from this pull request: https://github.com/angular-ui/ui-sortable/pull/319
    // More technical details here: https://github.com/angular-ui/ui-sortable/issues/316
    function getSortableWidgetInstance(element) {
        // this is a fix to support jquery-ui prior to v1.11.x
        // otherwise we should be using `element.sortable('instance')`
        var data = element.data('ui-sortable');
        if (data && typeof data === 'object' && data.widgetFullName === 'ui-sortable') {
            return data;
        }
        return null;
    }

});
