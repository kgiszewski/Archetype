Archetype
=========

## Install Dependencies ##
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

## Build ##
    grunt


## Deploy ##
    grunt deploy --target=C:\\path\\to\\umbraco\\site
    grunt watch:dev --target=C:\\path\to\\umbraco\\site

Add `--touch` to either command to automatically touch the web.config on a deploy

## Installation ##

To use this package right from this repo you will need to manually download and copy the files into the appropriate spot:

###/App_Plugins###

Your /App_Plugins should contain this structure:

    /App_Plugins
    - package.manifest
    - /css
    -- archetype.css
    - /js
    -- archetype.js (our Grunt script concats the controller.js and all directives into one file, you may have to do so manually)
    - /views
    -- archetype.html

###/bin###
- Drop the Imulus.Archetype.dll in the /bin of your Umbraco install (this dll only handles template helpers, PVC's at present)
- You will have to build this yourself or download it here.

## Prevalue Configs ##

Most of the fields are self evident, the following fields will be discussed further:

###Empty Fieldset Models###
This option is required and defines the fieldsets and their properties.  

If you get a YSOD regarding truncating of data, it's b/c Umbraco has a arbitrarily low limit on the number of characters (2500) allowed as prevalues in the DB.
See: http://issues.umbraco.org/issue/U4-2120

An example follows below:
    
    ( //note that this needs to start with an open parentheses and notice the closing one at the end of this.  This avoids issues with only seeing [object].
        [ //this array holds the array of fieldset objects
            { //fieldset object
                alias: "FS1", //fieldset alias (unique)
                tooltip: "This is for fieldset 1", //used as a helper text
                remove: false, //this may go away, but is used for internal fieldset removal
                icon: "/umbraco/Images/aboutNew.png",  //what icon do you want to use?
                label: "Fieldset 1",  //what should we call this?
                headerText: "Please fill out the boxes.", //would you like any text to be prepended to the fieldset properties?
                footerText: "Thanks!", //would you like any text to be appended to the fieldset properties?
                properties:[ //an array of property objects
                    { 
                        alias: "firstName", //uniquely name the property (unique to the fieldset)
                        label: "First Name", //a label to use
                        helpText: "(Required)",  //a note that will appear under the label
                        view: "/umbraco/views/propertyeditors/textbox/textbox.html", //a path to a view
                        value: "", //set to none or set to a default; can be json (if the view understands it)
                        config: { //pass configs to the view;  configs are based on the view used
                            
                        } 
                    },
                    { //another property
                        alias: "lastName",
                        label: "Last Name",
                        helpText: "(Required)",
                        view: "/umbraco/views/propertyeditors/textbox/textbox.html", 
                        value: "", 
                        config: {
                            
                        } 
                    },
                    { 
                        alias: "age",
                        label: "Age",
                        helpText: "(Optional)",
                        view: "/umbraco/views/propertyeditors/textbox/textbox.html", 
                        value: "", 
                        config: {
                            
                        } 
                    },
                    { 
                        alias: "blah",
                        label: "Blah",
                        helpText: "",
                        view: "/umbraco/views/propertyeditors/contentpicker/contentpicker.html", 
                        value: "", 
                        config: {
                        
                        } 
                    }    
                ]
            },
            { //another fieldset
                alias: "FS2",
                tooltip: "This is for fieldset 2",
                remove: false, 
                icon: "/umbraco/Images/about.png",
                label: "Fieldset 2",
                headerText: "Please fill out the boxes.",
                footerText: "Thanks!",
                properties:[
                    { 
                        alias: "foo",
                        label: "Foo",
                        helpText: "(Required)",
                        view: "/umbraco/views/propertyeditors/textbox/textbox.html", 
                        value: "", 
                        config: {
                            
                        } 
                    },
                    { 
                        alias: "bar",
                        label: "Bar",
                        helpText: "",
                        view: "/umbraco/views/propertyeditors/textbox/textbox.html", 
                        value: "", 
                        config: {
                        
                        } 
                    }    
                ]
            }
        ]
    )//close it
    
###Default Model###
Here is the spot to define a starting value.  This model uses only the essential data that must be saved.  This example shows multiple fieldsets with multiple properties:
    
    ( //again use a parenthesis to avoid [object]
        {
          "fieldsets": [
            {
              "alias": "FS2",
              "remove": false,
              "properties": [
                {
                  "alias": "bar",
                  "value": "Field"
                },
                {
                  "alias": "foo",
                  "value": "Another"
                }
              ]
            },
            {
              "alias": "FS2",
              "remove": false,
              "properties": [
                {
                  "alias": "foo",
                  "value": "The Foo Field"
                },
                {
                  "alias": "bar",
                  "value": "The Bar Field"
                }
              ]
            },
            {
              "alias": "FS1",
              "remove": false,
              "properties": [
                {
                  "alias": "age",
                  "value": "Old"
                },
                {
                  "alias": "lastName",
                  "value": "Giszewski"
                },
                {
                  "alias": "firstName",
                  "value": "Kevin"
                },
                {
                  "alias": "blah",
                  "value": ""
                }
              ]
            }
          ]
        }
    )
    
###Developer Mode###

By turning this on, you will get a textarea visible with the data model (not prevalue model).  You can change the contents of the textarea for real-time editing.  Additionally when in developer mode, the console will reflect the scope object of each property.

###Sortable Options###
Just in case you need to pass something into the jQueryUI sort options, you can override the default behavior by passing in a new JS snippet here.

##View Compatibility##
The basis of compatibility is based on the following:

- The model of the view must use `$scope.model.value` and the config must use `$scope.model.config`.

All core properties have not been tested yet :)
