var ArchetypeLabels = (function() {

    var isLoading = false;
    var entityNameLookupCache = [];

    function getEntityById($scope, entityResource, id, type) {

        for (var i in entityNameLookupCache) {
            if (entityNameLookupCache[i].id == id) {
                return entityNameLookupCache[i].value;
            }
        }


        if (!isLoading) {
            isLoading = true;

            entityResource.getById(id, type).then(function(entity) {
                entityNameLookupCache.push({id: id, value: entity.name});

                isLoading = false;
                return entity.name;
            });
        }

        return "";
    }

    return {
        GetFirstDocumentEntityName: function ($scope, entityResource, value) {
            if (value) {
                var id = value.split(",")[0];

                if (id) {
                    return getEntityById($scope, entityResource, id, "Document");
                }
            }

            return "";
        }
    }
})();