angular.module('umbraco').factory('propertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/UmbracoTrees/DataTypeTree/GetNodes?id=-1&application=developer&tree=&isDialog=false"), 'Failed to retrieve datatypes from tree service'
            ).then(function (data) {
                return data.map(function(d) {
                    return { "id": d.id, "name": d.name }
                });
            });
        }
    };
});