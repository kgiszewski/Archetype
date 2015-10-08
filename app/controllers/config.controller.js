angular.module("umbraco").controller("Imulus.ArchetypeConfigController", function ($scope, $http, assetsService, dialogService, archetypePropertyEditorResource) {

    //$scope.model.value = "";
    //console.log($scope.model.value);

    //define empty items
    var newPropertyModel = '{"alias": "", "remove": false, "collapse": false, "label": "", "helpText": "", "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae", "value": ""}';
    var newFieldsetModel = '{"alias": "", "remove": false, "collapse": false, "labelTemplate": "", "icon": "", "label": "", "properties": [' + newPropertyModel + '], "group": null}';
    var defaultFieldsetConfigModel = JSON.parse('{"showAdvancedOptions": false, "startWithAddButton": false, "hideFieldsetToolbar": false, "enableMultipleFieldsets": false, "hideFieldsetControls": false, "hidePropertyLabel": false, "maxFieldsets": null, "enableCollapsing": true, "enableCloning": false, "enableDisabling": true, "enableDeepDatatypeRequests": false, "fieldsets": [' + newFieldsetModel + '], "fieldsetGroups": []}');

    //ini the model
    $scope.model.value = $scope.model.value || defaultFieldsetConfigModel;

    $scope.dllVersion = "";

    archetypePropertyEditorResource.getDllVersion().then(function(data){
        $scope.dllVersion = data.dllVersion;
    });

    //ini the render model
    initConfigRenderModel();

    //get the available datatypes
    archetypePropertyEditorResource.getAllDataTypes().then(function(data) {
        $scope.availableDataTypes = data;
    });

    //iconPicker
    $scope.selectIcon = function(fieldset){
        var dialog = dialogService.iconPicker({
            callback: function(data){
                fieldset.icon = data;
            }
        });
    }

    //config for the sorting
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        update: function (ev, ui) {

        },
        stop: function (ev, ui) {

        }
    };

    //function that determines how to manage expanding/collapsing fieldsets
    $scope.focusFieldset = function(fieldset){
        var iniState;

        if(fieldset)
        {
            iniState = fieldset.collapse;
        }

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            if($scope.archetypeConfigRenderModel.fieldsets.length == 1 && fieldset.remove == false)
            {
                fieldset.collapse = false;
                return;
            }

            if(fieldset.label)
            {
                fieldset.collapse = true;
            }
            else
            {
                fieldset.collapse = false;
            }
        });

        if(iniState)
        {
            fieldset.collapse = !iniState;
        }
    }

    //ini the fieldsets
    $scope.focusFieldset();

    //function that determines how to manage expanding/collapsing properties
    $scope.focusProperty = function(properties, property){
        var iniState;

        if(property)
        {
            iniState = property.collapse;
        }

        _.each(properties, function(property){
            if(property.label)
            {
                property.collapse = true;
            }
            else
            {
                property.collapse = false;
            }
        });

        if(iniState)
        {
            property.collapse = !iniState;
        }
    }

    //ini the properties
    _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
        $scope.focusProperty(fieldset.properties);
    });

    //setup JSON.stringify helpers
    $scope.archetypeConfigRenderModel.toString = stringify;

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }

    //watch for changes
    $scope.$watch('archetypeConfigRenderModel', function (v) {
        //console.log(v);
        if (typeof v === 'string') {
            $scope.archetypeConfigRenderModel = JSON.parse(v);
            $scope.archetypeConfigRenderModel.toString = stringify;
        }
    });

    $scope.autoPopulateAlias = function(s) {
        var modelType = s.hasOwnProperty('fieldset') ? 'fieldset' : 'property';
        var modelProperty = s[modelType];

        if (!modelProperty.aliasIsDirty) {
            modelProperty.alias = modelProperty.label.toUmbracoAlias();
        }
    }

    $scope.markAliasDirty = function(s) {
        var modelType = s.hasOwnProperty('fieldset') ? 'fieldset' : 'property';
        var modelProperty = s[modelType];

        if (!modelProperty.aliasIsDirty) {
            modelProperty.aliasIsDirty = true;;
        }
    }

    //helper that returns if an item can be removed
    $scope.canRemoveFieldset = function ()
    {
        return countVisibleFieldset() > 1;
    }

    //helper that returns if an item can be sorted
    $scope.canSortFieldset = function ()
    {
        return countVisibleFieldset() > 1;
    }

    //helper that returns if an item can be removed
    $scope.canRemoveProperty = function (fieldset)
    {
        return countVisibleProperty(fieldset) > 1;
    }

    //helper that returns if an item can be sorted
    $scope.canSortProperty = function (fieldset)
    {
        return countVisibleProperty(fieldset) > 1;
    }

    $scope.getDataTypeNameByGuid = function (guid) {
        if ($scope.availableDataTypes == null) // Might not be initialized yet?
            return "";
        
        var dataType = _.find($scope.availableDataTypes, function(d) {
            return d.guid == guid;
        });

        return dataType == null ? "" : dataType.name;
    }

    //helper to count what is visible
    function countVisibleFieldset()
    {
        var count = 0;

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            if (fieldset.remove == false) {
                count++;
            }
        });

        return count;
    }

    //determines how many properties are visible
    function countVisibleProperty(fieldset)
    {
        var count = 0;

        for (var i in fieldset.properties) {
            if (fieldset.properties[i].remove == false) {
                count++;
            }
        }

        return count;
    }

    //handles a fieldset add
    $scope.addFieldsetRow = function ($index, $event) {
        $scope.archetypeConfigRenderModel.fieldsets.splice($index + 1, 0, JSON.parse(newFieldsetModel));
        $scope.focusFieldset();
    }

    //rather than splice the archetypeConfigRenderModel, we're hiding this and cleaning onFormSubmitting
    $scope.removeFieldsetRow = function ($index) {
        if ($scope.canRemoveFieldset()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.archetypeConfigRenderModel.fieldsets[$index].remove = true;
            }
        }
    }

    //handles a property add
    $scope.addPropertyRow = function (fieldset, $index) {
        fieldset.properties.splice($index + 1, 0, JSON.parse(newPropertyModel));
    }

    //rather than splice the archetypeConfigRenderModel, we're hiding this and cleaning onFormSubmitting
    $scope.removePropertyRow = function (fieldset, $index) {
        if ($scope.canRemoveProperty(fieldset)) {
            if (confirm('Are you sure you want to remove this?')) {
                fieldset.properties[$index].remove = true;
            }
        }
    }

    //helper to ini the render model
    function initConfigRenderModel()
    {
        $scope.archetypeConfigRenderModel = $scope.model.value;
        if (!$scope.archetypeConfigRenderModel.fieldsetGroups) {
            $scope.archetypeConfigRenderModel.fieldsetGroups = [];
        }

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset) {

            if (fieldset.group) {
                // tie the fieldset group back up to the actual group object, not the clone that's been persisted
                fieldset.group = _.find($scope.archetypeConfigRenderModel.fieldsetGroups, function(fieldsetGroup) {
                    return fieldsetGroup.name == fieldset.group.name;
                })
            }

            fieldset.remove = false;
            if (fieldset.alias.length > 0)
                fieldset.aliasIsDirty = true;

            if(fieldset.label)
            {
                fieldset.collapse = true;
            }

            _.each(fieldset.properties, function(fieldset){
                fieldset.remove = false;
                if (fieldset.alias.length > 0)
                    fieldset.aliasIsDirty = true;
            });
        });
    }

    //sync things up on save
    $scope.$on("formSubmitting", function (ev, args) {
        syncModelToRenderModel();
    });

    //helper to sync the model to the renderModel
    function syncModelToRenderModel()
    {
        $scope.model.value = $scope.archetypeConfigRenderModel;
        var fieldsets = [];

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            //check fieldsets
            if (!fieldset.remove) {
                fieldsets.push(fieldset);

                var properties = [];

                _.each(fieldset.properties, function(property){
                   if (!property.remove) {
                        properties.push(property);
                    }
                });

                fieldset.properties = properties;
            }
        });

        $scope.model.value.fieldsets = fieldsets;
    }

    $scope.showOptions = function ($event, template) {
        $event.preventDefault();

        dialogService.closeAll();

        dialogService.open({
            template: template,
            show: true,
            callback: function(data) {
                $scope.archetypeConfigRenderModel = data;
            },
            dialogData: $scope.archetypeConfigRenderModel
        });
    }

    //archetype css
    assetsService.loadCss("../App_Plugins/Archetype/css/archetype.css");
});
