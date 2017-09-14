angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource) {
    //private
    var entityCache = [];
    var datatypeCache = [];
    
    return {
        initialize: function() {
            return archetypePropertyEditorResource.getAllDataTypesForCache().then(function(data) {
                datatypeCache = data;
            });
        },
        
        getDataTypeFromCache: function(guid) {
            return _.find(datatypeCache, function (dt){
                return dt.dataTypeGuid == guid;
            });
        },
 
        getDatatypeByGuid: function(guid) {
            var cachedDatatype = this.getDataTypeFromCache(guid);

            
            if(cachedDatatype) {
                return cachedDatatype;
            }

            return null;
        },

        //perhaps this should return a promise?
        getEntityById: function(scope, id, type) {
            var cachedEntity = _.find(entityCache, function (e){
                return e.id == id;
            });

            if(cachedEntity) {
                return cachedEntity;
            }

            //go get it from server
            scope.resources.entityResource.getById(id, type).then(function(entity) {
                entityCache.push(entity);
            });

            return null;
        },

        //perhaps this should return a promise?
        getEntityByUmbracoId: function(scope, udi, type) {
            var cachedEntity = _.find(entityCache, function (e){
                return e.udi == udi;
            });

            if(cachedEntity) {
                return cachedEntity;
            }

            //go get it from server
            scope.resources.entityResource.getByIds([udi], type).then(function (entities) {
              // prevent infinite lookups with a default entity
              var entity = entities.length > 0 ? entities[0] : { udi: udi, name: "" };

              entityCache.push(entity);

            });

            return null;
        }
    }
});