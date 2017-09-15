//create a namespace (optional)
var ArchetypeSampleLabelHelpers = {};

//create a function
//you will add it to your label template field as `{{ArchetypeSampleLabelHelpers.testPromise(someArchetypePropertyAlias)}}`
ArchetypeSampleLabelHelpers.testPromise = function(value) {   
    //you can inject services
    return function ($timeout, archetypeCacheService) {        
        //best to return a promise
        //NOTE: $timeout returns a promise
        return $timeout(function () {
            return "As Promised: " + value;
        }, 1000);
    }
}

ArchetypeSampleLabelHelpers.testEntityPromise = function(value) {          
    return function ($q, entityResource) {    
        var deferred = $q.defer();
    
        entityResource.getById(1054, 'document').then(function(entity) {
            console.log("Hello from testEntityPromise");
            console.log(entity);
            deferred.resolve(entity.name);
        });
    
        return deferred.promise;
    }
}

ArchetypeSampleLabelHelpers.testEntityPromise2 = function(value) {          
    return function ($q, archetypeCacheService) {    
        var deferred = $q.defer();
    
        archetypeCacheService.getEntityById(1054, 'document').then(function(entity) {
            console.log("Hello from testEntityPromise2");
            console.log(entity);
            deferred.resolve(entity.name);
        });
    
        return deferred.promise;
    }
}