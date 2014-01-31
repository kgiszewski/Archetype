angular.module('umbraco').factory('propertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/UmbracoTrees/DataTypeTree/GetNodes?id=-1&application=developer&tree=&isDialog=false", { cache: true }), 'Failed to retrieve datatypes from tree service'
            ).then(function (data) {
                return data.map(function(d) {
                    return { "id": d.id, "name": d.name }
                });
            });
        },
        getDataType: function(id) {
        	return umbRequestHelper.resourcePromise(
        		$http.get("/umbraco/backoffice/UmbracoApi/DataType/GetById?id=" + id, { cache: true }), 'Failed to retrieve datatype'
    		);
        },
        getPropertyEditorMapping: function(alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/App_plugins/Archetype/js/propertyEditors.views.js", { cache: true }), 'Failed to retrieve datatype mappings'
            ).then(function (data) {
                var result = _.find(data, function(d) {
                    return d.propertyEditorAlias === alias;
                });

                if (result != null) 
                    return result;

                return "";
            });
        }
    };
});