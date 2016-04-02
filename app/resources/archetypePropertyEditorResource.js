angular.module('umbraco.resources').factory('archetypePropertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetAll"), 'Failed to retrieve datatypes from tree service'
            );
        },
        getDataType: function (guid, useDeepDatatypeLookup, contentTypeAlias, propertyTypeAlias, archetypeAlias, nodeId) {
            if(useDeepDatatypeLookup) {
            	return umbRequestHelper.resourcePromise(
            		$http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid + "&contentTypeAlias=" + contentTypeAlias + "&propertyTypeAlias=" + propertyTypeAlias + "&archetypeAlias=" + archetypeAlias + "&nodeId=" + nodeId), 'Failed to retrieve datatype'
        		);
            }
            else {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid , { cache: true }), 'Failed to retrieve datatype'
                );
            }
        },
        getPropertyEditorMapping: function(alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetAllPropertyEditors", { cache: true }), 'Failed to retrieve datatype mappings'
            ).then(function (data) {
                var result = _.find(data, function(d) {
                    return d.alias === alias;
                });

                if (result != null) 
                    return result;

                return "";
            });
        },
        getDllVersion: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetDllVersion", { cache: true }), 'Failed to retrieve dll version'
            );
        }
    }
}); 
