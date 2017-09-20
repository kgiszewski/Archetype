angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource, $q, entityResource) {
    //private
    var entityCache = [];
    var datatypeCache = [];
    
    return {
        initialize: function() {
            return archetypePropertyEditorResource.getAllDataTypesForCache().then(function(data) {
                _.each(data, function(datatype) {
                    datatypeCache[datatype.dataTypeGuid] = datatype;
                });
            });
        },
        
        getDataTypeFromCache: function(guid) {
            return datatypeCache[guid];
        },
 
        getDatatypeByGuid: function(guid) {
            var cachedDatatype = this.getDataTypeFromCache(guid);
            
            if(cachedDatatype) {
                return cachedDatatype;
            }

            return null;
        },

        getEntityById: function(id, type) {
            var deferred = $q.defer();
            
            //console.log(entityCache);
                
            var cachedEntity = entityCache[id];

            if(cachedEntity) {
                //console.log("Found ID " + id);
                
                deferred.resolve(cachedEntity);
                
                return deferred.promise;
            }

            //go get it from server
            entityResource.getById(id, type).then(function(entity) {
                entityCache[id] = entity;
                
                //console.log("entity ID is now resolved into cache...");
                //console.log(entityCache);
                
                deferred.resolve(entity);
            });

            return deferred.promise;
        },

        //perhaps this should return a promise?
        getEntityByUmbracoId: function(udi, type) {
            var deferred = $q.defer();
            
            var cachedEntity = entityCache[udi];

            if(cachedEntity) {
                //console.log("Found UDI " + udi);
                
                deferred.resolve(cachedEntity);
                
                return deferred.promise;
            }

            //go get it from server
            entityResource.getByIds([udi], type).then(function (entities) {
                // prevent infinite lookups with a default entity
                var entity = entities.length > 0 ? entities[0] : { udi: udi, name: "" };

                entityCache[udi] = entity;
                
                //console.log("entity UDI is now resolved into cache...");
                //console.log(entityCache);

                deferred.resolve(entity);
            });

            return deferred.promise;
        }
    }
});