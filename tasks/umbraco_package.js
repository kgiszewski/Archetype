module.exports = function(grunt) {
  grunt.registerTask('umbracoPackage', 'Create Umbraco Package', function() {
  	grunt.config.requires('umbracoPackage.options.name');
  	grunt.config.requires('umbracoPackage.options.version');
  	grunt.config.requires('umbracoPackage.options.license');
  	grunt.config.requires('umbracoPackage.options.licenseUrl');
  	grunt.config.requires('umbracoPackage.options.url');
  	grunt.config.requires('umbracoPackage.options.author');
  	grunt.config.requires('umbracoPackage.options.authorUrl');
  	grunt.config.requires('umbracoPackage.options.manifest');
  	grunt.config.requires('umbracoPackage.options.readme');
  	grunt.config.requires('umbracoPackage.options.outputDir');
  	grunt.config.requires('umbracoPackage.options.sourceDir');

	var Guid = require('guid');
	var path = require('path');
	var rimraf = require('rimraf');
	var AdmZip = require('adm-zip');
	var fs = require('fs-extra');

    var options = this.options({
    	minimumUmbracoVersion: '',
    	files: [],
    	cwd: '/'
    });

	var packageFileName = options.name + "_" + options.version + ".zip"

	// Gather files
	var filesToPackage = [];
	getFilesRecursive(options.sourceDir);
	filesToPackage = filesToPackage.map(function(f) {
		return { guid: Guid.create(), dir: f.dir.replace(options.sourceDir, ''), name: f.name, ext: f.name.split('.').pop() };	
	});

	// Create temp folder for package zip source
	var guidFolder = Guid.create().toString();
	var newDirName = path.join(options.sourceDir, "..\\" + guidFolder);
	fs.mkdirSync(newDirName);
	newDirName = path.join(newDirName, guidFolder);
	fs.mkdirSync(newDirName);

	// Copy flatten structure, with files renamed as <guid>.<ext>
	filesToPackage.forEach(function(f) {
		var newFileName = f.name == "package.xml" ? f.name : f.guid.toString() + '.' + f.ext;
		fs.copySync(path.join(options.sourceDir, f.dir, f.name), path.join(newDirName, newFileName));
	});

	// Load / transform XML Manifest
	options.files = filesToPackage;
	options.readmeContents = grunt.file.read(options.readme);
	var manifest = grunt.file.read(options.manifest);
	manifest = grunt.template.process(manifest, {data: options});
	grunt.file.write(path.join(options.sourceDir, "..\\", guidFolder, guidFolder, "package.xml"), manifest); // TODO: Probably shouldn't use sourceDir - what if under source control

	// Zip
	var zip = new AdmZip();
	zip.addLocalFolder(path.join(options.sourceDir, "..\\", guidFolder));
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