angular.module("umbraco").controller("Imulus.ArchetypeConfigController", function ($scope, $http, assetsService) {
    
    //$scope.model.value = "";

    var defaultFieldsetConfigModel = "";
    
    $scope.model.value = $scope.model.value || defaultFieldsetConfigModel;
    
    initConfigRenderModel();
    
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        update: function (ev, ui) {

        },
        stop: function (ev, ui) {

        }
    };
    
    //helpers
    $scope.archetypeConfigRenderModel.toString = stringify;
    setConfigPropertyToString();
    
    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }
    
    function setConfigPropertyToString()
    {
        for(var i in $scope.archetypeConfigRenderModel.fieldsets)
        {
            for(var j in $scope.archetypeConfigRenderModel.fieldsets[i].properties)
            {
                $scope.archetypeConfigRenderModel.fieldsets[i].properties[j].config.toString = stringify;
            }
        }
    }
    
    //watch for changes
    $scope.$watch('archetypeConfigRenderModel', function (v) {
        //console.log(v);
        if (typeof v === 'string') {     
            $scope.archetypeConfigRenderModel = JSON.parse(v);
            $scope.archetypeConfigRenderModel.toString = stringify;
        }
        
        for(var i in $scope.archetypeConfigRenderModel.fieldsets)
        {
            for(var j in $scope.archetypeConfigRenderModel.fieldsets[i].properties)
            {
                if(typeof $scope.archetypeConfigRenderModel.fieldsets[i].properties[j].config === 'string')
                {
                    $scope.archetypeConfigRenderModel.fieldsets[i].properties[j].config = JSON.parse($scope.archetypeConfigRenderModel.fieldsets[i].properties[j].config);
                    $scope.archetypeConfigRenderModel.fieldsets[i].properties[j].config.toString = stringify;
                }
            }
        }
    }, true);
    
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
    
    //helper to count what is visible
    function countVisible()
    {
        var count = 0;

        for (var i in $scope.archetypeConfigRenderModel.fieldsets) {
            if ($scope.archetypeConfigRenderModel.fieldsets[i].remove == false) {
                count++;
            }
        }

        return count;
    }
    
    //helper to ini the render model
    function initConfigRenderModel()
    {
        $scope.archetypeConfigRenderModel = {};
        $scope.archetypeConfigRenderModel.fieldsets = [];
        $scope.archetypeConfigRenderModel.fieldsets = $scope.model.value;
        
        for(var i in $scope.archetypeConfigRenderModel.fieldsets)
        {
            $scope.archetypeConfigRenderModel.fieldsets[i].remove = false;
        }
    }
    
    //sync things up on save
    $scope.$on("formSubmitting", function (ev, args) {
        syncModelToRenderModel();
    });
    
    //helper to sync the model to the renderModel
    function syncModelToRenderModel()
    {
        $scope.model.value = [];

        for (var i in $scope.archetypeConfigRenderModel.fieldsets) {
            if (!$scope.archetypeConfigRenderModel.fieldsets[i].remove) {
                $scope.model.value.push($scope.archetypeConfigRenderModel.fieldsets[i]);
            }
        }
    }
});
