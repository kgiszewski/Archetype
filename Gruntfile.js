module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path')

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('pkg/meta.json'),
    dest: (grunt.option('target') || 'dist') + '/App_Plugins/Archetype',
    package_dir: 'pkg',
    package_temp_dir: '<%= package_dir %>/tmp/',
    csProj: 'app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj',


    watch: {
      options: {
        spawn: false
      },

      less: {
        files: ['app/less/*.less', 'lib/**/*.less'],
        tasks: ['less:dist'],
      },

      js: {
        files: ['app/**/*.js', 'lib/**/*.js'],
        tasks: ['concat', 'jshint'],
      },

      views: {
        files: ['/app/views/**/*'],
        tasks: ['copy:views']
      },

      dll: {
        files: ['app/Umbraco/**/*.dll'],
        tasks: ['copy:dll'],
      }
    },

    touch: {
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
      }
    },

    jshint: {
      options: {
        jshintrc: '.jshintrc'
      },
      src: {
        src: ['app/**/*.js', 'lib/**/*.js']
      }
    },

    less: {
      dist: {
        options: {
          paths: ["app/less", "lib/less", "vendor"],
        },
        files: {
          '<%= dest %>/css/archetype.css': 'app/less/archetype.less',
        }
      }
    },

    concat: {
      options: {
        stripBanners: false
      },
      dist: {
        src: [
          'app/controllers/controller.js',
          'app/controllers/config.controller.js',
          'app/directives/archetypeproperty.js',
          'app/directives/localize.js',
          'app/services/localization.js',
          'app/resources/propertyeditor.js'
        ],
        dest: '<%= dest %>/js/archetype.js'
      }
    },

    nugetpack: {
    	dist: {
    		src: '<%= package_temp_dir %>/nuget/package.nuspec',
    		dest: '<%= package_dir %>'
    	}
    },

    template: {
    	'nuspec': {
			'options': {
    			'data': { 
            name: '<%= pkgMeta.name %>',
    				version: '<%= pkgMeta.version %>',
            url: '<%= pkgMeta.url %>',
            license: '<%= pkgMeta.license %>',
            licenseUrl: '<%= pkgMeta.licenseUrl %>',
            author: '<%= pkgMeta.author %>',
            authorUrl: '<%= pkgMeta.authorUrl %>',

    				files: [{ path: '..\\..\\..\\<%= package_temp_dir %>\\nuget\\**', target: 'content\\App_Plugins\\Archetype'}]
	    		}
    		},
    		'files': { 
    			'<%= package_temp_dir %>/nuget/package.nuspec': ['<%= package_dir %>/nuget/package.nuspec']
    		}
    	}
    },

    clean: {
  		build: ['<%= dest %>']
    },

    copy: {
      views: {
        files: [
          {expand: true, cwd: 'app/views/', src: ['archetype.html', 'archetype.config.html'], dest: '<%= dest %>/views', flatten: true}
        ]
      },
      config: {
        files: [
          {expand: true, cwd: 'app/', src: ['package.manifest'], dest: '<%= dest %>', flatten: true},
          {expand: true, cwd: 'app/config/', src: ['propertyEditors.views.js'], dest: '<%= dest %>/config', flatten: true},
          {expand: true, cwd: 'app/langs/', src: ['**'], dest: '<%= dest %>/langs', flatten: true}
        ]
      },
      nuget: {
        files: [
          {expand: true, cwd: '<%= dest %>', src: ['**/*', '!bin', '!bin/*'], dest: '<%= package_temp_dir %>/nuget/content', flatten: false},
          {expand: true, cwd: '<%= dest %>/bin', src: ['*.dll'], dest: '<%= package_temp_dir %>/nuget/lib/net40', flatten: true}
        ]
      },
      umbraco: {
        files: [
          {expand: true, cwd: '<%= dest %>', src: ['**/*'], dest: 'pkg/tmp/umbraco', flatten: false}
        ]
      }
    },

    umbracoPackage: {
      options: {
        name: "<%= pkgMeta.name %>",
        version: '<%= pkgMeta.version %>',
        url: '<%= pkgMeta.url %>',
        license: '<%= pkgMeta.license %>',
        licenseUrl: '<%= pkgMeta.licenseUrl %>',
        author: '<%= pkgMeta.author %>',
        authorUrl: '<%= pkgMeta.authorUrl %>',
        manifest: 'pkg/umbraco/package.xml',
        readme: 'pkg/umbraco/readme.txt',
        sourceDir: 'pkg/tmp/umbraco',
        outputDir: 'pkg',
      }
    },

    clean: {
      build: ['<%= dest %>'],
      package_temp: ['pkg/tmp'],
      package_artifacts: ['pkg/*.zip', 'pkg/*.nupkg'],
    },

    assemblyinfo: {
        options: {
            files: ['<%= csProj %>'],
            filename: 'VersionInfo.cs',
            info: {
                version: '<%= (pkgMeta.version.indexOf("-") ? pkgMeta.version.substring(0, pkgMeta.version.indexOf("-")) : pkgMeta.version) %>', 
                fileVersion: '<%= pkgMeta.version %>'
            }
        }
    },

    msbuild: {
        dist: {
            src: ["<%= csProj %>"],
            options: {
                projectConfiguration: 'Debug',
                targets: ['Clean', 'Rebuild'],
                stdout: true,
                maxCpuCount: 4,
                buildParameters: {
                    WarningLevel: 2,
                    NoWarn: 1607,
                    OutputPath: path.resolve("<%= dest %>") + "/../../bin"
                },
                verbosity: 'quiet'
            }
        }
    }

  });

  grunt.registerTask('default', ['clean', 'less', 'concat', 'cs:build', 'copy:config', 'copy:views']);
  grunt.registerTask('cs:build', ['assemblyinfo', 'msbuild:dist']);
  grunt.registerTask('nuget', ['copy:nuget', 'template:nuspec', 'nugetpack', 'clean:package_temp']);
  grunt.registerTask('umbraco', ['copy:umbraco', 'umbracoPackage', 'clean:package_temp']);
};

