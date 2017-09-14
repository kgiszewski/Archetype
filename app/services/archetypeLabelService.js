angular.module('umbraco.services').factory('archetypeLabelService', function (archetypeCacheService, $q, $injector) {
    //private

    /**
     * This will repeatedly wait for all promises in an array of promises to resolve, and it allows
     * for promises to be added to the array (i.e., if you add more promises, those added promises
     * will also be resolved too).
     * @param promises The array of promises.
     * @returns {*} The promise that will resolve once all promises in the array resolve.
     */
    function repeatedlyWaitForPromises(promises) {

        // Remember the original number of promises being resolved.
        var originalLength = promises.length;
        
        return $q.all(promises).then(function () {

            // If there are new promises, resolve those too.
            if (promises.length > originalLength) {
                promises = promises.slice(originalLength);
                return repeatedlyWaitForPromises(promises);
            }
        });
    }

    /**
     * Processes a value to be used in a fieldset label. Since it might be a promise or a function, the
     * value is repeatedly processed until it becomes a string.
     * @param labelValue The value to be used in the fieldset label.
     * @param promises The collection of promises to add any promises to.
     * @param match The current matched substring object in a label template.
     */
    function processLabelValue(labelValue, promises, match) {

        // Normalize null/undefined values to an empty string.
        if(!labelValue) {
            labelValue = "";
        }

        // Check the type of value (may be a string, promise, function, or other).
        if (isString(labelValue)) {

            // handle collapsing dollar signs in labels (#387)
            if (labelValue.indexOf("$$") >= 0) {
                labelValue = labelValue.replace(/\$\$/g, "$$$$$$$$");
            }

            // Set a new value now that it has been processed.
            match.value = labelValue;
        } else if (isPromise(labelValue)) {

            // Remember the promise so we can wait for it to be completed before constructing the
            // fieldset label.
            promises.push(labelValue);
            labelValue.then(function (value) {
                // The value will probably be a string, but recursively process it in case it's
                // something else.
                processLabelValue(value, promises, match);
            });
        } else if (_.isFunction(labelValue)) {

            // Allow for the function to accept injected parameters, and invoke it.
            labelValue = $injector.invoke(labelValue);

            // Recursively check result (may be a string, promise, or another function (another
            // function would be pretty strange, though I see no reason to disallow it).
            processLabelValue(labelValue, promises, match);

        } else {

            // Some other data type (e.g., number, date, object).
            match.value = labelValue;
        }
    }

    /**
     * Checks if the specified value is of type string.
     * @param value The value to check the type of.
     * @returns {boolean} True, if the value is a string; otherwise, false.
     */
    function isString(value) {
        if (value === null) {
            return false;
        } else if (typeof value === 'string') {
            return true;
        } else if (value instanceof String) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * Checks if the specified value is a JavaScript promise.
     * @param value The value that may be a promise.
     * @returns {*} True, if the value appears to be a promise; otherwise, false.
     */
    function isPromise(value) {
        return value && _.isFunction(value.then);
    }

    /**
     * Splits a string value into a collection based on a regular expression.
     * @param rgx The regular expression to use to find matches in the string value.
     * @param value The string value to split.
     * @returns {Array} A collection of objects, each representing a portion of the supplied
     *      string. Each object will contain the substring value, as well as a property
     *      indicating whether or not that substring was matched the regular expression.
     */
    function splitByRegex(rgx, value) {
        // Validate input.
        if (!rgx || !value) {
            return [];
        }

        // Variables.
        var substring,
            splitParts = [],
            nextIndex = 0,
            index;

        // Reset regex so we get all the matches.
        rgx.lastIndex = 0;

        // Loop through each match until there are no more matches.
        var match = rgx.exec(value);
        while (match) {

            // Extract match index.
            index = match.index;

            // Is there text between the prior match and this one?
            if (nextIndex < index) {
                substring = value.substring(nextIndex, index);
                splitParts.push({
                    isMatch: false,
                    value: substring
                });
            }

            // Remember the end of this match for the next loop iteration.
            nextIndex = rgx.lastIndex;

            // Store info about this match.
            substring = value.substring(index, nextIndex);
            splitParts.push({
                isMatch: true,
                value: substring
            });

            // Get next match.
            match = rgx.exec(value);
        }

        // The text after the last match.
        if (nextIndex < value.length) {
            substring = value.substring(nextIndex);
            splitParts.push({
                isMatch: false,
                value: substring
            });
        }

        // Reset regex in case somebody else wants to use it.
        rgx.lastIndex = 0;

        // Return information about the matches.
        return splitParts;
    }

    function executeFunctionByName(functionName, context) {
        var args = Array.prototype.slice.call(arguments).splice(2);

        var namespaces = functionName.split(".");
        var func = namespaces.pop();

        for(var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }

        if(context && context[func]) {
            return context[func].apply(this, args);
        }

        return "";
    }

    function getNativeLabel(datatype, value, scope) {
        switch (datatype.selectedEditor) {
                case "Imulus.UrlPicker":
                    return imulusUrlPicker(value, scope, {});
                case "Umbraco.TinyMCEv3":
                    return coreTinyMce(value, scope, {});
                case "Umbraco.MultiNodeTreePicker":
                    return coreMntp(value, scope, datatype);
                case "Umbraco.MultiNodeTreePicker2":
                    return coreMntpV2(value, scope, datatype);
                case "Umbraco.MultipleMediaPicker":
                case "Umbraco.MediaPicker":
                    return coreMediaPicker(value, scope, datatype);
                case "Umbraco.MediaPicker2":
                    return coreMediaPickerV2(value, scope, datatype);
                case "Umbraco.DropDown":
                    return coreDropdown(value, scope, datatype);
                case "RJP.MultiUrlPicker":
                    return rjpMultiUrlPicker(value, scope, {});
                case "Umbraco.ContentPickerAlias":
                    return coreContentPicker(value, scope, datatype);
                case "Umbraco.ContentPicker2":
                    return coreContentPickerV2(value, scope, datatype);
                default:
                    return null;
        }
    }

    function coreDropdown(value, scope, args) {

        if(!value)
            return "";

        var prevalue = args.preValues[0].value[value];

        if(prevalue) {
            return prevalue.value;
        }

        return "";
    }

    function coreMntp(value, scope, args) {
        var ids = value.split(',');
        var type = "Document";

        switch(args.preValues[0].value.type) {
            case 'content':
                type = 'Document';
                break;
            case 'media':
                type = 'media';
                break;
            case 'member':
                type = 'member';
                break;
            default:
                break;
        }

        var entityArray = [];

        _.each(ids, function(id){
            if(id) {

                var entity = archetypeCacheService.getEntityById(scope, id, type);
                
                if(entity) {
                    entityArray.push(entity.name);
                }
            }
        });

        return entityArray.join(', ');
    }

    function coreMntpV2(value, scope, args) {
        var ids = value.split(',');
        if (ids.length == 0) {
          return "";
        }
        var type = "document";

        switch(args.preValues[0].value.type) {
            case 'content':
                type = 'document';
                break;
            case 'media':
                type = 'media';
                break;
            case 'member':
                type = 'member';
                break;

            default:
                break;
        }

        var entity;

        _.each(ids, function (id) {            
            if(id && !entity) {
              entity = archetypeCacheService.getEntityByUmbracoId(scope, id, type);
            }
        });

        return (entity != null ? entity.name : "") + (ids.length > 1 ? ", ..." : "");
    }

    function coreMediaPicker(value, scope, args) {
        if(value) {
             var entity = archetypeCacheService.getEntityById(scope, value, "media");     
             
            if(entity) {
                return entity.name; 
            }
        }

        return "";
    }

    function coreMediaPickerV2(value, scope, args) {
        if(value) {
            var entity = archetypeCacheService.getEntityByUmbracoId(scope, value, "media");
             
            if(entity) {
                return entity.name; 
            }
        }

        return "";
    }

    function coreContentPicker(value, scope, args) {
      if (value) {
        var entity = archetypeCacheService.getEntityById(scope, value, "document");

        if (entity) {
          return entity.name;
        }
      }

      return "";
    }

    function coreContentPickerV2(value, scope, args) {
      if (value) {
        var entity = archetypeCacheService.getEntityByUmbracoId(scope, value, "document");

        if (entity) {
          return entity.name;
        }
      }

      return "";
    }

    function imulusUrlPicker(value, scope, args) {

        if(!args.propertyName) {
            args = {propertyName: "name"}
        }

        var entity;

        if(value.length) {
            value = value[0];
        }
        
        switch (value.type) {
            case "content":
                if(value.typeData.contentId) {
                    entity = archetypeCacheService.getEntityById(scope, value.typeData.contentId, "Document");
                }
                break;

            case "media":
                if(value.typeData.mediaId) {
                    entity = archetypeCacheService.getEntityById(scope, value.typeData.mediaId, "Media");
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
    }

    function coreTinyMce(value, scope, args) {

        if(!args.contentLength) {
            args = {contentLength: 50}
        }

        var suffix = "";
        var strippedText = $("<div/>").html(value).text();

        if(strippedText.length > args.contentLength) {
            suffix = "…";
        }

        return strippedText.substring(0, args.contentLength) + suffix;
    }

    function rjpMultiUrlPicker(values, scope, args) {
        var names = [];

        _.each(values, function (value) {
            if (value.name) {
                names.push(value.name);
            }
        });

        return names.join(", ");
    }
    
    return {
        getFieldsetTitle: function(scope, fieldsetConfigModel, fieldsetIndex) {

            if(!fieldsetConfigModel)
                return $q.when("");

            var fieldset = scope.model.value.fieldsets[fieldsetIndex];
            var fieldsetConfig = scope.getConfigFieldsetByAlias(fieldset.alias);
            var template = fieldsetConfigModel.labelTemplate;
            var promises = [];

            if (template.length < 1)
                return $q.when(fieldsetConfig.label);

            var rgx = /{{.*?}}/g;
            var matches = splitByRegex(rgx, template);

            _.each(matches, function (match) {

                // Skip over substrings that didn't match the regex (they do not require a transformation).
                if (!match.isMatch) {
                    return;
                }

                // split the template in case it consists of multiple property aliases and/or functions
                var templates = match.value.replace("{{", '').replace("}}", '').split("|");
                var templateLabelValue = "";

                for(var i = 0; i < templates.length; i++) {
                    // stop looking for a template label value if a previous template part already yielded a value
                    if(templateLabelValue !== "") {
                        break;
                    }
                    
                    var template = templates[i];
                    
                    //test for function
                    var beginParamsIndexOf = template.indexOf("(");
                    var endParamsIndexOf = template.indexOf(")");

                    //if passed a function
                    if(beginParamsIndexOf !== -1 && endParamsIndexOf !== -1)
                    {
                        var functionName = template.substring(0, beginParamsIndexOf);
                        var propertyAlias = template.substring(beginParamsIndexOf + 1, endParamsIndexOf).split(',')[0];

                        var args = {};

                        var beginArgsIndexOf = template.indexOf(',');

                        if(beginArgsIndexOf !== -1) {

                            var argsString = template.substring(beginArgsIndexOf + 1, endParamsIndexOf).trim();

                            var normalizedJsonString = argsString.replace(/(\w+)\s*:/g, '"$1":');

                            args = JSON.parse(normalizedJsonString);
                        }
                       
                        templateLabelValue = executeFunctionByName(functionName, window, scope.getPropertyValueByAlias(fieldset, propertyAlias), scope, args);
                        
                        //if empty, promise to try again
                        if(!templateLabelValue) {
                            templateLabelValue = $timeout(function() {
                                return executeFunctionByName(functionName, window, scope.getPropertyValueByAlias(fieldset, propertyAlias), scope, args);
                            }, 1000);
                        }
                    }
                    //normal {{foo}} syntax
                    else {
                        propertyAlias = template;
                        
                        var rawValue = scope.getPropertyValueByAlias(fieldset, propertyAlias);

                        templateLabelValue = rawValue;

                        //determine the type of editor
                        var propertyConfig = _.find(fieldsetConfigModel.properties, function(property){
                            return property.alias === propertyAlias;
                        });

                        if(propertyConfig) {
                            var datatype = archetypeCacheService.getDatatypeByGuid(propertyConfig.dataTypeGuid);

                            if(datatype) {

                                //try to get built-in label
                                var label = getNativeLabel(datatype, rawValue, scope);
                                
                                if(label != "") {
                                    templateLabelValue = label;
                                }
                                
                                if(label == "") {
                                    templateLabelValue = $timeout(function() {
                                        return getNativeLabel(datatype, rawValue, scope);
                                    }, 1000);
                                }
                                
                                //if label is null, skip all that jazz
                            }
                        }
                    }
                }

                // Process the value (i.e., reduce any functions or promises down to strings).
                processLabelValue(templateLabelValue, promises, match);
            });

            // Wait for all of the promises to resolve before constructing the full fieldset label.
            return repeatedlyWaitForPromises(promises).then(function () {

                // Extract string values and combine them into a single string.
                var substrings = _.map(matches, function (value) {
                    return value.value;
                });
                
                var combinedSubstrings = substrings.join('');

                // Return the title.
                return combinedSubstrings;
            });
        }
    }
});