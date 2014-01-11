angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, assetsService) {

    //$scope.model.value = "";

    //test config/model
    //$scope.model.config.emptyFieldSetModel = '{ remove: false, properties:[{ type: "textbox", options: { label: "Name" }, data: "" }, { type: "contentPicker", options: { label: "Pick some content" }, data: "" }]}';

    //set default value of the model
    //this works by checking to see if there is a model; then cascades to the default model then to an empty fieldset
    var validDefaultModel = getValidJson("$scope.model.config.defaultModel", $scope.model.config.defaultModel);
    var validEmptyFieldsetModel = getValidJson("$scope.model.config.emptyFieldsetModel", $scope.model.config.emptyFieldsetModel);

    $scope.model.value = $scope.model.value || (validDefaultModel || { fieldsets: [validEmptyFieldsetModel] });

    //ini
    $scope.archetypeRenderModel = {};
    initArchetypeRenderModel();

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
            console.log($scope.archetypeRenderModel);
        }
    };

    $scope.addRow = function ($index) {
        if (true)
        {
            var validJson = getValidJson("$scope.model.config.emptyFieldsetModel", $scope.model.config.emptyFieldsetModel);

            if (validJson)
            {
                $scope.archetypeRenderModel.fieldsets.splice($index + 1, 0, validJson);
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

    $scope.canRemove = function ()
    {   
        return countVisible() > 1;
    }

    $scope.canSort = function ()
    {
        return countVisible() > 1;
    }

    //helper, ini the render model from the server (model.value)
    function initArchetypeRenderModel() {
        $scope.archetypeRenderModel = $scope.model.value;
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
            if (!$scope.archetypeRenderModel.fieldsets[i].remove) {
                $scope.model.value.fieldsets.push($scope.archetypeRenderModel.fieldsets[i]);
            }
        }
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
    assetsService.loadCss("/App_Plugins/Archetype/css/archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }
});
