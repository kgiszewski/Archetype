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
    $scope.getFieldsetTitle = function(expression, fieldsetIndex)
    {
        if(!expression) return "";
        
        return expression.replace(/\$fieldsetIndex/g, fieldsetIndex);
    }

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
        
        for(var i in $scope.archetypeRenderModel.fieldsets)
        {
            $scope.archetypeRenderModel.fieldsets[i].remove = false;
            $scope.archetypeRenderModel.fieldsets[i].collapse = true;
            $scope.archetypeRenderModel.fieldsets[i].isValid = true;
        }
    }

    //helper to get the correct fieldset from config
    $scope.getConfigFieldsetByAlias = function(alias) {
        for (var i in $scope.model.config.fieldsets) {
            if ($scope.model.config.fieldsets[i].alias == alias) {
                return $scope.model.config.fieldsets[i];
            }
        }
    }
    
    //helper for collapsing
    $scope.focusFieldset = function(fieldset){
        
        var iniState;
        
        if(fieldset)
        {
            iniState = fieldset.collapse;
        }
    
        for(var i in $scope.archetypeRenderModel.fieldsets)
        {
            if($scope.archetypeRenderModel.fieldsets.length == 1 && $scope.archetypeRenderModel.fieldsets[i].remove == false)
            {
                $scope.archetypeRenderModel.fieldsets[i].collapse = false;
                return;
            }
        
            $scope.archetypeRenderModel.fieldsets[i].collapse = true;
        }
        
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

        for (var i in $scope.archetypeRenderModel.fieldsets) {
            if ($scope.archetypeRenderModel.fieldsets[i].remove == false) {
                count++;
            }
        }

        return count;
    }

    //helper to sync the model to the renderModel
    function syncModelToRenderModel()
    {
        $scope.model.value = { fieldsets: [] };

        for (var i in $scope.archetypeRenderModel.fieldsets) {
            var fieldset = $scope.archetypeRenderModel.fieldsets[i];

            if (typeof fieldset != 'function' && !fieldset.remove){
                console.log($scope.archetypeRenderModel.fieldsets[i]);
                //clone
                var tempFieldset = JSON.parse(JSON.stringify(fieldset));
                delete tempFieldset.remove;
                delete tempFieldset.isValid;
                $scope.model.value.fieldsets.push(tempFieldset);
            }
        }
    }

    //helper to add an empty fieldset
    function getEmptyRenderItem (fieldsetModel)
    {
        return eval("({ alias: '" + fieldsetModel.alias + "', remove: false, properties: []})");
    }

    $scope.hasValue = function(array, value) {
      var i;
      for (i=0; i<array.length; i++) { if (array[i] === value) return true; }
      return false;
    }

    //helper for validation
    function getValidation()
    {
        var validation = {}
        validation.isValid = true;
        validation.requiredAliases = [];
        validation.invalidProperties = [];

        //determine which fields are required
        for(var i in $scope.model.config.fieldsets)
        {
            for(var j in $scope.model.config.fieldsets[i].properties)
            {
                if($scope.model.config.fieldsets[i].properties[j].required)
                {
                    validation.requiredAliases.push($scope.model.config.fieldsets[i].properties[j].alias);
                }
            }
        }

        //if nothing required; let's go
        if(validation.requiredAliases.length == 0)
        {
            return validation;
        }

        //otherwise we need to check the required aliases
        for(var i in $scope.archetypeRenderModel.fieldsets)
        {
            var fieldset = $scope.archetypeRenderModel.fieldsets[i];
            fieldset.isValid = true;

            for(var j in fieldset.properties)
            {
                var property = $scope.archetypeRenderModel.fieldsets[i].properties[j];
                property.isValid = true;

                if($scope.hasValue(validation.requiredAliases, property.alias))
                {                
                    //TODO: do a better validation test
                    if(property.value == ""){
                        fieldset.isValid = false;
                        property.isValid = false;
                        validation.isValid = false;

                        validation.invalidProperties.push({ fieldset: i, property: j, alias: property.alias, value: property.value, isValid: property.isValid});
                    }
                }
            }
        }

        console.log(validation);
        return validation;
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
