function testPromise(value) {   

    //console.log(value);
    
    return function ($timeout) {
        return $timeout(function () {
            return "As Promised: " + value;
        }, 1000);
    }
}