angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, $interpolate, assetsService, angularHelper, notificationsService) {
 
    //$scope.model.value = "";
    //set default value of the model
    //this works by checking to see if there is a model; then cascades to the default model then to an empty fieldset

    var form = angularHelper.getCurrentForm($scope);

    $scope.model.config = $scope.model.config.archetypeConfig;
   
    $scope.model.value = $scope.model.value || { fieldsets: [getEmptyRenderItem($scope.model.config.fieldsets[0])] };

    //ini
    $scope.archetypeRenderModel = {};
    initArchetypeRenderModel();
    
    //helper to get $eval the labelExpression
    $scope.getFieldsetTitle = function(fieldsetConfigModel, fieldsetIndex) {
        var fieldset = $scope.archetypeRenderModel.fieldsets[fieldsetIndex];
        var template = fieldsetConfigModel.labelExpression;
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

    /* add/remove/sort */

    //defines the options for the jquery sortable 
    //i used an ng-model="archetypeRenderModel" so the sort updates the right model

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
                var newFieldset = getEmptyRenderItem($scope.getConfigFieldsetByAlias(fieldsetAlias));

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
    
    //helper for collapsing
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
    
    //ini
    $scope.focusFieldset();

    //helper returns valid JS or null
    function getValidJson(variable, json)
    {
        if(!json) return null;

        try {
            return eval("(" + json + ")");
        }
        catch (e) {
            console.log("There was an error while using 'eval' on " + variable);
            console.log(json);
            return null;
        }
    }

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
            if (typeof fieldset != 'function' && !fieldset.remove){

                //clone and clean
                var tempFieldset = JSON.parse(JSON.stringify(fieldset));
                delete tempFieldset.remove;
                delete tempFieldset.isValid;
                delete tempFieldset.collapse;

                _.each(tempFieldset.properties, function(property){
                    delete property.isValid;
                });

                $scope.model.value.fieldsets.push(tempFieldset);
            }
        });
    }

    //helper to add an empty fieldset
    function getEmptyRenderItem (fieldsetModel)
    {
        return eval("({ alias: '" + fieldsetModel.alias + "', remove: false, properties: []})");
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
            validation.requiredAliases = _.find(fieldset.properties, function(property){
                return property.required;
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

        return validation;
    }

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
    assetsService.loadCss("/App_Plugins/Imulus.Archetype/css/archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }
});
