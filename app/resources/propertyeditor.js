angular.module('umbraco').factory('propertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getViews: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("/App_Plugins/Archetype/js/config.views.js"), 'Failed to retreive config for views.'
            );
        }
    };
});