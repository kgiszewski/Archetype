angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource) {
    //private

    var isEntityLookupLoading = false;
    var entityCache = [];

    var isDatatypeLookupLoading = false;
    var datatypeCache = [];

    return {
    	getDataTypeFromCache: function(guid) {
        	return _.find(datatypeCache, function (dt){
	            return dt.dataTypeGuid == guid;
	        });
    	},

    	addDatatypeToCache: function(datatype, dataTypeGuid) {
            var cachedDatatype = this.getDataTypeFromCache(dataTypeGuid);

            if(!cachedDatatype) {
            	datatype.dataTypeGuid = dataTypeGuid;
            	datatypeCache.push(datatype);
            }
    	},
 
		getDatatypeByGuid: function(guid) {
			var cachedDatatype = this.getDataTypeFromCache(guid);

	        if(cachedDatatype) {
	            return cachedDatatype;
	        }

	        //go get it from server, but this should already be pre-populated from the directive, but I suppose I'll leave this in in case used ad-hoc
	        if (!isDatatypeLookupLoading) {
	            isDatatypeLookupLoading = true;

	            archetypePropertyEditorResource.getDataType(guid).then(function(datatype) {

	            	datatype.dataTypeGuid = guid;

	                datatypeCache.push(datatype);

	                isDatatypeLookupLoading = false;

	                return datatype;
	            });
	        }

	        return null;
        },

     	getEntityById: function(scope, id, type) {
	        var cachedEntity = _.find(entityCache, function (e){
	            return e.id == id;
	        });

	        if(cachedEntity) {
	            return cachedEntity;
	        }

	        //go get it from server
	        if (!isEntityLookupLoading) {
	            isEntityLookupLoading = true;

	            scope.resources.entityResource.getById(id, type).then(function(entity) {

	                entityCache.push(entity);

	                isEntityLookupLoading = false;

	                return entity;
	            });
	        }

	        return null;
	    }
    }
});