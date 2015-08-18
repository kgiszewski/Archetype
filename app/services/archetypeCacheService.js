angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource, notificationsService) {
    //private

    var isEntityLookupLoading = false;
    var entityCache = [];

    var isDatatypeLookupLoading = false;
    var datatypeCache = [];

    var notificationQueue = [];
    var notificationCache = [];

    var invalidationCache = [];

    function findItem(array, item) {
		return _.find(array, function(value){
			return value == item;
		});
    }

    return {
    	notifyEditor: function() {
    		console.log("queue-v");
    		console.log(notificationQueue);
    		console.log("sent-v");
    		console.log(notificationCache);

    		if(this.shouldBeNotified("minFieldsets") && !this.hasBeenNotified("minFieldsets")) {
    			notificationsService.error("Error", "Some of your properties do not contain enough fieldsets.");

    			this.removeNotification("minFieldsets");

    			notificationCache.push("minFieldsets");
    		}
    	},

    	clearInvalidations: function() {
    		invalidationCache = [];
    	},

    	addInvalidation: function(key) {
    		if(!this.hasBeenInvalidated(invalidationCache, key)) {
    			invalidationCache.push(key);
    		}
    	},

    	removeInvalidation: function(key) {
    		invalidationCache = _.reject(invalidationCache, function(value){
    			return value == key;
    		});
    	},

    	hasBeenInvalidated: function(key) {
    		return (typeof findItem(invalidationCache, key) != 'undefined');
    	},

    	clearNotifications: function() {
    		notificationCache = [];
    		
    	},

    	addNotification: function(key) {
    		if(!this.shouldBeNotified(key) && !this.hasBeenNotified(notificationCache, key)) {
    			notificationQueue.push(key);
    		}
    	},

    	removeNotification: function(key) {
    		notificationQueue = _.reject(notificationQueue, function(value){
    			return value == key;
    		});
    	},

    	shouldBeNotified: function(key) {
			return (typeof findItem(notificationQueue, key) != 'undefined');
    	},

    	hasBeenNotified: function(key) {
			return (typeof findItem(notificationCache, key) != 'undefined');
    	},

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