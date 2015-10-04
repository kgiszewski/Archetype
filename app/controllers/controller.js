angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, $filter, assetsService, angularHelper, notificationsService, $timeout, fileManager, entityResource, archetypeService, archetypeLabelService, archetypeCacheService, archetypePropertyEditorResource) {

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

    var draggedRteSettings;
    var rteClass = ".mce-tinymce";

    //sort config
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        start: function(ev, ui) {
            draggedRteSettings = {};
            ui.item.parent().find(rteClass).each(function () {
                // remove all RTEs in the dragged row and save their settings
                var $element = $(this);
                var wrapperId = $element.attr('id');
                var $textarea = $element.siblings('textarea');
                var textareaId = $textarea.attr('id');

                draggedRteSettings[textareaId] = _.findWhere(tinyMCE.editors, { id: textareaId }).settings;
                tinyMCE.execCommand('mceRemoveEditor', false, wrapperId);
            });
        },
        update: function (ev, ui) {
            $scope.setDirty();
        },
        stop: function (ev, ui) {
            ui.item.parent().find(rteClass).each(function () {
                var $element = $(this);
                var wrapperId = $element.attr('id');
                var $textarea = $element.siblings('textarea');
                var textareaId = $textarea.attr('id');

                draggedRteSettings[textareaId] = draggedRteSettings[textareaId] || _.findWhere(tinyMCE.editors, { id: textareaId }).settings;
                tinyMCE.execCommand('mceRemoveEditor', false, wrapperId);
                tinyMCE.init(draggedRteSettings[textareaId]);

                $element.siblings(rteClass).each(function() {
                    $(this).remove();
                });
            });
        }
    };

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
                group: fieldset.group ? fieldset.group.name : null
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
                $scope.overlayMenu.fieldsetGroups.push({ name: fieldsetGroup.name, fieldsets: $filter("filter")(allFieldsets, { group: fieldsetGroup.name }) });
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
    };
    
    $scope.pickFieldset = function (fieldsetAlias, $index) {
        $scope.closeFieldsetPicker();
        $scope.addRow(fieldsetAlias, $index);
    };    
    
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

            $scope.setDirty();

            $scope.$broadcast("archetypeAddFieldset", {index: $index, visible: countVisible()});

            newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
            
            $scope.focusFieldset(newFieldset);
        }
    }

    $scope.removeRow = function ($index) {
        if ($scope.canRemove()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.setDirty();
                $scope.model.value.fieldsets.splice($index, 1);
                $scope.$broadcast("archetypeRemoveFieldset", {index: $index});
            }
        }
    }

    $scope.cloneRow = function ($index) {
        if ($scope.canClone() && typeof $index != 'undefined') {
            var newFieldset = angular.copy($scope.model.value.fieldsets[$index]);

            if(newFieldset) {

                $scope.model.value.fieldsets.splice($index + 1, 0, newFieldset);

                $scope.setDirty();

                newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
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
            return countVisible() < $scope.model.config.maxFieldsets;
        }

        return true;
    }

    //helper that returns if an item can be removed
    $scope.canRemove = function () {
        return countVisible() > 1 
            || ($scope.model.config.maxFieldsets == 1 && $scope.model.config.fieldsets.length > 1)
            || $scope.model.config.startWithAddButton;
    }

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
        return countVisible() > 1;
    }

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

    //helper, ini the render model from the server (model.value)
    function init() {
        $scope.model.value = removeNulls($scope.model.value);
        addDefaultProperties($scope.model.value.fieldsets);
    }

    function addDefaultProperties(fieldsets)
    {
        _.each(fieldsets, function (fieldset)
        {
            fieldset.collapse = false;
            fieldset.isValid = true;
        });
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
            fieldset.collapse = true;
        }
        return fieldset.collapse;
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
            return;
        }

        if(iniState && fieldset)
        {
            fieldset.collapse = !iniState;
        }
    }

    //ini the fieldset expand/collapse
    $scope.focusFieldset();

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
        if (fieldset.isValid == false) {
            return false;
        }

        // recursive validation of nested fieldsets
        var nestedFieldsetsValid = true;
        _.each(fieldset.properties, function (property) {
            if (property != null && property.value != null && property.propertyEditorAlias == "Imulus.Archetype") {
                _.each(property.value.fieldsets, function (inner) {
                    if ($scope.getFieldsetValidity(inner) == false) {
                        nestedFieldsetsValid = false;
                    }
                });
            }
        });

        return nestedFieldsetsValid;
    }

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
    $scope.submitWatcherOnSubmit = function () {
        $scope.$broadcast("archetypeFormSubmitting");
    }
});
