angular.module('umbraco').factory('propertyEditorService', function($q, $http, umbRequestHelper){
    return { 
        getViews: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("/App_Plugins/Imulus.Archetype/js/views.js"), 'Failed to retreive data for views.'
            );
        }
    };
});