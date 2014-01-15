module.exports = function(grunt) {
  grunt.registerTask('umbracoPackage', 'Create Umbraco Package', function() {
  	grunt.config.requires('umbracoPackage.options.packageName');
  	grunt.config.requires('umbracoPackage.options.packageVersion');
  	grunt.config.requires('umbracoPackage.options.packageLicenseName');
  	grunt.config.requires('umbracoPackage.options.packageLicenseUrl');
  	grunt.config.requires('umbracoPackage.options.packageUrl');
  	grunt.config.requires('umbracoPackage.options.authorName');
  	grunt.config.requires('umbracoPackage.options.authorUrl');
  	grunt.config.requires('umbracoPackage.options.manifest');
  	grunt.config.requires('umbracoPackage.options.readme');
  	grunt.config.requires('umbracoPackage.options.outputDir');
  	grunt.config.requires('umbracoPackage.options.sourceDir');

	var fs = require('fs');
	var Guid = require('guid');
	var path = require('path');
	var rimraf = require('rimraf');
	var AdmZip = require('adm-zip');

    var options = this.options({
    	minimumUmbracoVersion: '',
    	files: [],
    	cwd: '/'
    });

	var packageFileName = options.packageName + "_" + options.packageVersion + ".zip"

	// Gather files
	var filesToPackage = [];
	getFilesRecursive(options.sourceDir);
	filesToPackage = filesToPackage.map(function(f) {
		return { guid: Guid.create(), dir: f.dir.replace(options.sourceDir, ''), name: f.name };	
	});

	// Load / transform Manifest
	options.files = filesToPackage;
	options.readmeContents = grunt.file.read(options.readme);
	var manifest = grunt.file.read(options.manifest);
	manifest = grunt.template.process(manifest, {data: options});
	grunt.file.write(path.join(options.sourceDir, "package.xml"), manifest); // TODO: Probably shouldn't use sourceDir - what if under source control

	// Zip
	var zip = new AdmZip();
	filesToPackage.forEach(function(f) {
		zip.addLocalFile(path.join(options.sourceDir, f.dir, f.name))
	})
	zip.addLocalFile(path.join(options.sourceDir, "package.xml"));
	zip.writeZip(path.join(options.outputDir, packageFileName))
	
	function getFilesRecursive(dir) {
    	var files = fs.readdirSync(dir);
    	for (var i in files) {
	        if (!files.hasOwnProperty(i)) continue;

	        var name = dir+'/'+files[i];
        	if (fs.statSync(name).isDirectory()) {
	            getFilesRecursive(name);
        	} else {
        		filesToPackage.push({ dir: dir, name: files[i]});
        	}
    	}
	}

  });
};