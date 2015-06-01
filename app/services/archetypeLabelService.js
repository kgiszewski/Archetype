angular.module('umbraco.services').factory('archetypeLabelService', function (archetypePropertyEditorResource) {
    //private

    var isEntityLookupLoading = false;
    var entityCache = [];
    var isDatatypeLookupLoading = false;
    var datatypeCache = [];

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

    function getNativeLabel(datatype, value, scope, archetypeLabelService) {
    	switch (datatype.selectedEditor) {
    		case "Imulus.UrlPicker":
    			return archetypeLabelService.urlPicker(value, scope, {});
    		case "Umbraco.TinyMCEv3":
    			return archetypeLabelService.rte(value, scope, {});

    		default:
    			return null;
    	}
    }

	return {
		getFieldsetTitle: function($scope, fieldsetConfigModel, fieldsetIndex) {

            //console.log($scope.model.config);

            if(!fieldsetConfigModel)
                return "";

            var fieldset = $scope.model.value.fieldsets[fieldsetIndex];
            var fieldsetConfig = $scope.getConfigFieldsetByAlias(fieldset.alias);
            var template = fieldsetConfigModel.labelTemplate;

            if (template.length < 1)
                return fieldsetConfig.label;

            var rgx = /{{([^)].*)}}/g;
            var results;
            var parsedTemplate = template;

            while ((results = rgx.exec(template)) !== null) {

                // split the template in case it consists of multiple property aliases and/or functions
                var templates = results[0].replace("{{", '').replace("}}", '').split("|");
                var templateLabelValue = "";

                for(var i = 0; i < templates.length; i++) {
                    // stop looking for a template label value if a previous template part already yielded a value
                    if(templateLabelValue != "") {
                        break;
                    }
                    
                    var template = templates[i];
                    
                    //test for function
                    var beginParamsIndexOf = template.indexOf("(");
                    var endParamsIndexOf = template.indexOf(")");

                    //if passed a function
                    if(beginParamsIndexOf != -1 && endParamsIndexOf != -1)
                    {
                        var functionName = template.substring(0, beginParamsIndexOf);
                        var propertyAlias = template.substring(beginParamsIndexOf + 1, endParamsIndexOf).split(',')[0];

                        var args = {};

                        var beginArgsIndexOf = template.indexOf(',');

                        if(beginArgsIndexOf != -1) {

                            var argsString = template.substring(beginArgsIndexOf + 1, endParamsIndexOf).trim();

                            var normalizedJsonString = argsString.replace(/(\w+)\s*:/g, '"$1":');

                            args = JSON.parse(normalizedJsonString);
                        }

                        templateLabelValue = executeFunctionByName(functionName, window, $scope.getPropertyValueByAlias(fieldset, propertyAlias), $scope, args);
                    }
                    //normal {{foo}} syntax
                    else {
                        propertyAlias = template;
                        var rawValue = $scope.getPropertyValueByAlias(fieldset, propertyAlias);

                        templateLabelValue = rawValue;

                        var isObject = angular.isObject(templateLabelValue);

                        //try to match a built-in template
                        if(isObject || templateLabelValue.indexOf("<p>" != -1)) {
                            //determine the type of editor
                            var propertyConfig = _.find(fieldsetConfigModel.properties, function(property){
                                return property.alias == propertyAlias;
                            });

                            if(propertyConfig) {
                            	var datatype = this.getDatatypeByGuid(propertyConfig.dataTypeGuid);
                            	
                            	if(datatype) {
                            		//console.log(datatype);

	                            	//try to get built-in label
	                            	var label = getNativeLabel(datatype, templateLabelValue, $scope, this);

	                            	if(label) {
                            			templateLabelValue = label;
                            		}
                            		else {
                            			templateLabelValue = templateLabelValue;
                            		}
                            	}
                            }
                            else {
                            	return templateLabelValue;
                            }
                        }
                    }                
                }
                parsedTemplate = parsedTemplate.replace(results[0], templateLabelValue);
            }

            return parsedTemplate;
        },
        getDatatypeByGuid: function(guid) {
        	var cachedDatatype = _.find(datatypeCache, function (dt){
	            return dt.dataTypeGuid == guid;
	        });

	        if(cachedDatatype) {
	            return cachedDatatype;
	        }

	        //go get it from server
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
	    },
	    urlPicker: function(value, scope, args) {

            if(!args.propertyName) {
                args = {propertyName: "name"}
            }

            var entity;

            switch (value.type) {
                case "content":
                    if(value.typeData.contentId) {
                        entity = this.getEntityById(scope, value.typeData.contentId, "Document");
                    }
                    break;

                case "media":
                    if(value.typeData.mediaId) {
                        entity = this.getEntityById(scope, value.typeData.mediaId, "Media");
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
        rte: function (value, scope, args) {

            if(!args.contentLength) {
                args = {contentLength: 50}
            }

            return $(value).text().substring(0, args.contentLength);
        }
	}
});