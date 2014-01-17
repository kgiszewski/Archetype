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
    
###Developer Mode###

By turning this on, you will get a textarea visible with the data model (not prevalue model).  You can change the contents of the textarea for real-time editing.  Additionally when in developer mode, the console will reflect the scope object of each property.

##View Compatibility##
The basis of compatibility is based on the following:

- The model of the view must use `$scope.model.value` and the config must use `$scope.model.config`.

All core properties have not been tested yet :)

##Developer Notes##
If you decide to extend this project locally, you'll want to set the `<compilation>` `debug` property in your `web.config` to `true`:

    <compilation defaultLanguage="c#" debug="true" batch="false" targetFramework="4.5">

This is in order to circumvent the minification and caching of the JavaScript files.
