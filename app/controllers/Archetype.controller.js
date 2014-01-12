angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, assetsService) {

    //$scope.model.value = "";

    //set default value of the model
    //this works by checking to see if there is a model; then cascades to the default model then to an empty fieldset

    //validate the user configs
    $scope.model.config.defaultModel = getValidJson("$scope.model.config.defaultModel", $scope.model.config.defaultModel);
    $scope.model.config.fieldsetModels = getValidJson("$scope.model.config.fieldsetModels", $scope.model.config.fieldsetModels);

    $scope.model.value = $scope.model.value || ($scope.model.config.defaultModel || { fieldsets: [getEmptyRenderItem($scope.model.config.fieldsetModels[0])] });

    //ini
    $scope.archetypeRenderModel = {};
    initArchetypeRenderModel();

    /* add/remove/sort */

    //defines the options for the jquery sortable 
    //i used an ng-model="archetypeRenderModel" so the sort updates the right model
    //configuration overrides the default
    var configSortableOptions = getValidJson("$scope.model.config.sortableOptions", $scope.model.config.sortableOptions);

    $scope.sortableOptions = configSortableOptions || {
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
            if ($scope.model.config.fieldsetModels)
            {
                var newRenderItem = getEmptyRenderItem($scope.getConfigFieldsetByAlias(fieldsetAlias));

                if (typeof $index != 'undefined')
                {
                    $scope.archetypeRenderModel.fieldsets.splice($index + 1, 0, newRenderItem);
                }
                else
                {
                    $scope.archetypeRenderModel.fieldsets.push(newRenderItem);
                }
            }
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
        if ($scope.model.config.maxProperties)
        {
            return countVisible() < $scope.model.config.maxProperties;
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
    }

    //helper to get the correct fieldset from config
    $scope.getConfigFieldsetByAlias = function(alias) {
        for (var i in $scope.model.config.fieldsetModels) {
            if ($scope.model.config.fieldsetModels[i].alias == alias) {
                return $scope.model.config.fieldsetModels[i];
            }
        }
    }

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
            //console.log(v);
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
            if (!$scope.archetypeRenderModel.fieldsets[i].remove) {
                $scope.model.value.fieldsets.push($scope.archetypeRenderModel.fieldsets[i]);
            }
        }
    }

    //helper to add an empty fieldset
    function getEmptyRenderItem (fieldsetModel)
    {
        return eval("({ alias: '" + fieldsetModel.alias + "', remove: false, properties: []})");
    }

    //sync things up on save
    $scope.$on("formSubmitting", function (ev, args) {
        syncModelToRenderModel();
    });

    //custom js
    if ($scope.model.config.customJsPath) {
        assetsService.loadJs($scope.model.config.customJsPath);
    }

    //archetype css
    assetsService.loadCss("/App_Plugins/Imulus.Archetype/Archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }
});
