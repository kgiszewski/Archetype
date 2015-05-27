var ArchetypeLabels = (function() {

    //private vars
    var isEntityLookupLoading = false;
    var entityCache = [];

    //private functions
    function getEntityById(scope, id, type) {

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

    //public functions
    return {
        GetEntityProperty: function (value, scope, args) {

           if(!args.entityType) {
                args = {entityType: "Document", propertyName: "name"}
            }

            if (value) {
                //if handed a csv list, take the first only
                var id = value.split(",")[0];

                if (id) {
                    var entity = getEntityById(scope, id, args.entityType);

                    if(entity) {
                        return entity[args.propertyName];
                    }
                }
            }

            return "";
        },
        UrlPicker: function(value, scope, args) {

            if(!args.propertyName) {
                args = {propertyName: "name"}
            }

            var entity;

            switch (value.type) {
                case "content":
                    if(value.typeData.contentId) {
                        entity = getEntityById(scope, value.typeData.contentId, "Document");
                    }
                    break;

                case "media":
                    if(value.typeData.mediaId) {
                        entity = getEntityById(scope, value.typeData.mediaId, "Media");
                    }
                    break;

                case "url":
                    return value.typeData.url;
                    
                default:
                    break;

            }

            if(entity) {
                return entity[args.propertyName];
            }

            return "";
        },
        Rte: function (value, scope, args) {

            if(!args.contentLength) {
                args = {contentLength: 50}
            }

            return $(value).text().substring(0, args.contentLength);
        }
    }
})();