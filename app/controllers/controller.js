﻿angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, assetsService, angularHelper, notificationsService) {
 
    //$scope.model.value = "";

    //get a reference to the current form
    var form = angularHelper.getCurrentForm($scope);

    //set the config equal to our prevalue config
    $scope.model.config = $scope.model.config.archetypeConfig;
   
    //ini the model
    $scope.model.value = $scope.model.value || { fieldsets: [getEmptyRenderFieldset($scope.model.config.fieldsets[0])] };

    //ini the render model
    $scope.archetypeRenderModel = {};
    initArchetypeRenderModel();
     
    //helper to get $eval the labelTemplate
    $scope.getFieldsetTitle = function(fieldsetConfigModel, fieldsetIndex) {
        var fieldset = $scope.archetypeRenderModel.fieldsets[fieldsetIndex];
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
                    $scope.archetypeRenderModel.fieldsets.splice($index + 1, 0, newFieldset);
                }
                else
                {
                    $scope.archetypeRenderModel.fieldsets.push(newFieldset);
                }
            }
            newFieldset.collapse = true;
            $scope.focusFieldset(newFieldset);
        }
    }

    //rather than splice the archetypeRenderModel, we're hiding this and cleaning onFormSubmitting
    $scope.removeRow = function ($index) {
        if ($scope.canRemove()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.archetypeRenderModel.fieldsets[$index].remove = true;
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
        return countVisible() > 1;
    }

    //helper that returns if an item can be sorted
    $scope.canSort = function ()
    {
        return countVisible() > 1;
    }

    //helper, ini the render model from the server (model.value)
    function initArchetypeRenderModel() {
        $scope.archetypeRenderModel = $scope.model.value;

        _.each($scope.archetypeRenderModel.fieldsets, function (fieldset)
        {
            fieldset.remove = false;
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
    
    //helper for expanding/collapsing fieldsets
    $scope.focusFieldset = function(fieldset){
        
        var iniState;
        
        if(fieldset)
        {
            iniState = fieldset.collapse;
        }
    
        _.each($scope.archetypeRenderModel.fieldsets, function(fieldset){
            if($scope.archetypeRenderModel.fieldsets.length == 1 && fieldset.remove == false)
            {
                fieldset.collapse = false;
                return;
            }
        
            fieldset.collapse = true;
        });
        
        if(iniState)
        {
            fieldset.collapse = !iniState;
        }
    }
    
    //ini the fieldset expand/collapse
    $scope.focusFieldset();

    //developerMode helpers
    $scope.archetypeRenderModel.toString = stringify;

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }

    //watch for changes
    $scope.$watch('archetypeRenderModel', function (v) {
        if ($scope.model.config.developerMode) {
            console.log(v);
            if (typeof v === 'string') {
                $scope.archetypeRenderModel = JSON.parse(v);
                $scope.archetypeRenderModel.toString = stringify;
            }
        }
    });

    //helper to count what is visible
    function countVisible()
    {
        var count = 0;

        _.each($scope.archetypeRenderModel.fieldsets, function(fieldset){
            if (fieldset.remove == false) {
                count++;
            }
        });

        return count;
    }

    //helper to sync the model to the renderModel
    function syncModelToRenderModel()
    {
        $scope.model.value = { fieldsets: [] };

        _.each($scope.archetypeRenderModel.fieldsets, function(fieldset){
            $scope.model.value.fieldsets.push(cleanFieldset(fieldset));
        });
    }

    //helper to remove properties only used during editing that we don't want in the saved data
    //also removes properties that are no longer in the config
    function cleanFieldset(fieldset)
    {
        if (typeof fieldset != 'function' && !fieldset.remove){

            var fieldsetConfig = $scope.getConfigFieldsetByAlias(fieldset.alias);

            //clone and clean
            var tempFieldset = JSON.parse(JSON.stringify(fieldset));
            delete tempFieldset.remove;
            delete tempFieldset.isValid;
            delete tempFieldset.collapse;

            _.each(tempFieldset.properties, function(property, index){
                var propertyConfig = _.find(fieldsetConfig.properties, function(p){
                    return property.alias == p.alias;
                });

                //just prune the property
                if(propertyConfig){
                    delete property.isValid;
                }
                else 
                {
                    //need to remove the whole property
                    tempFieldset.properties.splice(index, 1);
                }

            });

            return tempFieldset;
        }
    }

    //helper to add an empty fieldset to the render model
    function getEmptyRenderFieldset (fieldsetModel)
    {
        return JSON.parse('{"alias": "' + fieldsetModel.alias + '", "remove": false, "isValid": true, "properties": []}');
    }

    //helper for validation
    function getValidation()
    {
        var validation = {}
        validation.isValid = true;
        validation.requiredAliases = [];
        validation.invalidProperties = [];

        //determine which fields are required
        _.each($scope.model.config.fieldsets, function(fieldset){
            _.each(fieldset.properties, function(property){
                if(property.required)
                {
                    validation.requiredAliases.push(property.alias);
                }
            });
        });

        //if nothing required; let's go
        if(validation.requiredAliases.length == 0)
        {
            return validation;
        }

        //otherwise we need to check the required aliases
        _.each($scope.archetypeRenderModel.fieldsets, function(fieldset){
            fieldset.isValid = true;

            _.each(fieldset.properties, function(property){
                property.isValid = true;

                //if a required field
                if(_.find(validation.requiredAliases, function(alias){ return alias == property.alias }))
                {                
                    //TODO: do a better validation test
                    if(property.value == ""){
                        fieldset.isValid = false;
                        property.isValid = false;
                        validation.isValid = false;

                        validation.invalidProperties.push(property);
                    }
                }
            });
        });

        if($scope.model.config.developerMode == '1')
        {
            console.log(validation);
        }

        return validation;
    }

    //helper to lookup validity when given a fieldsetIndex and property alias
    $scope.getPropertyValidity = function(fieldsetIndex, alias)
    {
        if($scope.archetypeRenderModel.fieldsets[fieldsetIndex])
        {
            var property = _.find($scope.archetypeRenderModel.fieldsets[fieldsetIndex].properties, function(property){
                return property.alias == alias;
            });
        }

        return (typeof property == 'undefined') ? true : property.isValid;
    }

    //sync things up on save
    $scope.$on("formSubmitting", function (ev, args) {

        //test for form; may have to do this differently for nested archetypes
        if(!form)
            return;
        
        var validation = getValidation();

        if(!validation.isValid)
        {
            notificationsService.warning("Cannot Save Document", "The document could not be saved because of missing required fields.")
            form.$setValidity("archetypeError", false);
        }
        else 
        {
            syncModelToRenderModel();
            form.$setValidity("archetypeError", true);
        }
    });

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
