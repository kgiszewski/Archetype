angular.module('umbraco.resources').factory('archetypePropertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetAll"), 'Failed to retrieve datatypes from tree service'
            );
        },
        getDataType: function(guid) {
        	return umbRequestHelper.resourcePromise(
        		$http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid), 'Failed to retrieve datatype'
    		);
        },
        getPropertyEditorMapping: function(alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/App_plugins/Archetype/js/propertyEditors.views.js"), 'Failed to retrieve datatype mappings'
            ).then(function (data) {
                var result = _.find(data, function(d) {
                    return d.propertyEditorAlias === alias;
                });

                if (result != null) 
                    return result;

                return "";
            });
        }
    }
}); 