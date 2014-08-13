var ArchetypeLabels = (function() {

    var isEntityLookupLoading = false;
    var entityNameLookupCache = [];

    function getEntityById(scope, id, type) {

        for (var i in entityNameLookupCache) {
            if (entityNameLookupCache[i].id == id) {
                return entityNameLookupCache[i].value;
            }
        }

        if (!isEntityLookupLoading) {
            isEntityLookupLoading = true;

            scope.resources.entityResource.getById(id, type).then(function(entity) {
                entityNameLookupCache.push({id: id, value: entity.name});

                isEntityLookupLoading = false;
                return entity.name;
            });
        }

        return "";
    }

    return {
        GetFirstDocumentEntityName: function ($scope, value) {
            if (value) {
                var id = value.split(",")[0];

                if (id) {
                    return getEntityById($scope, id, "Document");
                }
            }

            return "";
        }
    }
})();