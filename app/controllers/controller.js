angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, assetsService, angularHelper, notificationsService, $timeout) {

    //$scope.model.value = "";
    $scope.model.hideLabel = $scope.model.config.hideLabel == 1;

    //get a reference to the current form
    var form = angularHelper.getCurrentForm($scope);

    //set the config equal to our prevalue config
    $scope.model.config = $scope.model.config.archetypeConfig;

    //ini the model
    $scope.model.value = $scope.model.value || getDefaultModel($scope.model.config);
    init();

    //helper to get $eval the labelTemplate
    $scope.getFieldsetTitle = function(fieldsetConfigModel, fieldsetIndex) {
        var fieldset = $scope.model.value.fieldsets[fieldsetIndex];
        var fieldsetConfig = $scope.getConfigFieldsetByAlias(fieldset.alias);
        var template = fieldsetConfigModel.labelTemplate;

        if (template.length < 1)
            return fieldsetConfig.label;

        var rgx = /{{(.*?)}}*/g;
        var results;
        var parsedTemplate = template;

        while ((results = rgx.exec(template)) !== null) {
            var propertyAlias = results[1];
            var propertyValue = $scope.getPropertyValueByAlias(fieldset, propertyAlias);
            parsedTemplate = parsedTemplate.replace(results[0], propertyValue);
        }

        return parsedTemplate;
    };

    //sort config
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        update: function (ev, ui) {

        },
        stop: function (ev, ui) {

        }
    };

    //handles a fieldset add
    $scope.addRow = function (fieldsetAlias, $index) {
        if ($scope.canAdd())
        {
            if ($scope.model.config.fieldsets)
            {
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

            newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
            $scope.focusFieldset(newFieldset);
        }
    }

    $scope.removeRow = function ($index) {
        if ($scope.canRemove()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.model.value.fieldsets.splice($index, 1);
            }
        }
    }

    //helpers for determining if a user can do something
    $scope.canAdd = function ()
    {
        if ($scope.model.config.maxFieldsets)
        {
            return countVisible() < $scope.model.config.maxFieldsets;
        }

        return true;
    }

    //helper that returns if an item can be removed
    $scope.canRemove = function ()
    {
        return countVisible() > 1 
            || ($scope.model.config.maxFieldsets == 1 && $scope.model.config.fieldsets.length > 1)
            || $scope.model.config.startWithAddButton;
    }

    //helper that returns if an item can be sorted
    $scope.canSort = function ()
    {
        return countVisible() > 1;
    }

    //helpers for determining if the add button should be shown
    $scope.showAddButton = function () {
        return $scope.model.config.startWithAddButton
            && countVisible() === 0
            && $scope.model.config.fieldsets.length == 1;
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

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
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

    //custom js
    if ($scope.model.config.customJsPath) {
        assetsService.loadJs($scope.model.config.customJsPath);
    }

    //archetype css
    assetsService.loadCss("/App_Plugins/Archetype/css/archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }
});
