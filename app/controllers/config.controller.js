angular.module("umbraco").controller("Imulus.ArchetypeConfigController", function ($scope, $http, assetsService) {
    
    //$scope.model.value = "";
    console.log($scope.model.value);

    var defaultFieldsetConfigModel = eval("(" + newFieldsetModel + ")");
    var newPropertyModel = '{alias: "", remove: false, label: "", helpText: "", view: "", value: "", config: {}}';
    var newFieldsetModel = '{alias: "", remove: false, collapse: false, tooltip: "", icon: "", label: "", headerText: "", footerText: "", properties:[' + newPropertyModel + ']}';
    
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
    
    $scope.focusFieldset = function(fieldset){
        var iniState;
        
        if(fieldset)
        {
            iniState = fieldset.collapse;
        }
    
        for(var i in $scope.archetypeConfigRenderModel.fieldsets)
        {
            if($scope.archetypeConfigRenderModel.fieldsets[i].label)
            {
                $scope.archetypeConfigRenderModel.fieldsets[i].collapse = true;
            }
        }
        
        if(iniState && $scope.archetypeConfigRenderModel.fieldsets[i].label)
        {
            fieldset.collapse = !iniState;
        }
    }
    
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
    
    //helper to count what is visible
    function countVisibleFieldset()
    {
        var count = 0;

        for (var i in $scope.archetypeConfigRenderModel.fieldsets) {
            if ($scope.archetypeConfigRenderModel.fieldsets[i].remove == false) {
                count++;
            }
        }

        return count;
    }
    
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
        $scope.archetypeConfigRenderModel.fieldsets.splice($index + 1, 0, eval("(" + newFieldsetModel + ")"));
        $scope.focusFieldset();
        $event.stopPropagation();
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
        fieldset.properties.splice($index + 1, 0, eval("(" + newPropertyModel + ")"));
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
        
        for(var i in $scope.archetypeConfigRenderModel.fieldsets)
        {
            $scope.archetypeConfigRenderModel.fieldsets[i].remove = false;
            $scope.archetypeConfigRenderModel.fieldsets[i].collapse = true;
            
            for(var j in $scope.archetypeConfigRenderModel.fieldsets[i].properties)
            {
                $scope.archetypeConfigRenderModel.fieldsets[i].properties[j].remove = false;
            }
        }
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
        
        for (var i in $scope.archetypeConfigRenderModel.fieldsets) {
            //check fieldsets
            if (!$scope.archetypeConfigRenderModel.fieldsets[i].remove) {
                fieldsets.push($scope.archetypeConfigRenderModel.fieldsets[i]);
                
                //check properties
                var properties = [];
                for (var j in $scope.archetypeConfigRenderModel.fieldsets[i].properties)
                {
                    if (!$scope.archetypeConfigRenderModel.fieldsets[i].properties[j].remove) {
                        properties.push($scope.archetypeConfigRenderModel.fieldsets[i].properties[j]);
                    }
                }
                $scope.archetypeConfigRenderModel.fieldsets[i].properties = properties;
            }
        }
        
        $scope.model.value.fieldsets = fieldsets;
    }
    
    //archetype css
    assetsService.loadCss("/App_Plugins/Imulus.Archetype/css/archetype.css");
});
