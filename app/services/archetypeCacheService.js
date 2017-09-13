angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource) {
    //private

    var isEntityLookupLoading = false;
    var entityCache = [];

    var isDatatypeLookupLoading = false;
    var datatypeCache = [];
    
    var broadcastEntityCacheUpdated = function(scope, entity) {
        scope.$broadcast("archetypeEntityCacheUpdated", entity);   
    }

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
                    
                    broadcastEntityCacheUpdated(scope, entity);

                    return entity;
                });
            }

            return null;
        },

        getEntityByUmbracoId: function(scope, udi, type) {
            var cachedEntity = _.find(entityCache, function (e){
                return e.udi == udi;
            });

            if(cachedEntity) {
                return cachedEntity;
            }

            //go get it from server
            if (!isEntityLookupLoading) {
                isEntityLookupLoading = true;

                scope.resources.entityResource.getByIds([udi], type).then(function (entities) {
                    // prevent infinite lookups with a default entity
                    var entity = entities.length > 0 ? entities[0] : { udi: udi, name: "" };

                    entityCache.push(entity);

                    isEntityLookupLoading = false;

                    broadcastEntityCacheUpdated(scope, entity);

                    return entity;
                });
            }

            return null;
        }
    }
});