module.exports = function(grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('pkg/meta.json'),
    dest: 'dist',
    package_dir: 'pkg',
    package_temp_dir: '<%= package_dir %>/tmp/',


    watch: {
      less: {
        files: ['app/less/*.less', 'lib/**/*.less'],
        tasks: ['less:build'],
        options: {
          spawn: false
        }
      },

      js: {
        files: ['app/**/*.js', 'lib/**/*.js'],
        tasks: ['concat', 'jshint'],
        options: {
          spawn: false
        }
      },

      dev: {
        files: ['app/**', '!app/Umbraco/**', 'app/Umbraco/**/*.dll'],
        tasks: ['deploy'],
        options: {
          spawn: false
        }
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
      build: {
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
      application: {
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
    	'nuget_manifest': {
			'options': {
    			'data': { 
            name: '<%= pkgMeta.name %>',
    				version: '<%= pkgMeta.version %>',
            url: '<%= pkgMeta.url %>',
            license: '<%= pkgMeta.license %>',
            licenseUrl: '<%= pkgMeta.licenseUrl %>',
            author: '<%= pkgMeta.author %>',
            authorUrl: '<%= pkgMeta.authorUrl %>',

    				files: [{ path: '..\\..\\..\\<%= dest %>\\**', target: 'content\\App_Plugins\\Archetype'}]
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
      build: {
       files: [
        {expand: true, cwd: 'app/', src: ['package.manifest'], dest: '<%= dest %>', flatten: true},
        {expand: true, cwd: 'app/views/', src: ['archetype.html', 'archetype.config.html'], dest: '<%= dest %>/views', flatten: true},
        {expand: true, cwd: 'app/config/', src: ['propertyEditors.views.js'], dest: '<%= dest %>/js', flatten: true},
        {expand: true, cwd: 'app/langs/', src: ['**'], dest: '<%= dest %>/langs', flatten: true},
        {expand: true, cwd: 'app/Umbraco/Umbraco.Archetype/bin/Debug/', src: ['Archetype.dll', 'Archetype.pdb'], dest: '<%= dest %>/bin', flatten: true} 
        ]
      },
      deploy: {
        files: [
          {expand: true, cwd: '<%= dest %>/', src: ['**/*', "!bin", "!bin/**/*"], dest: '<%= grunt.option("target") %>\\App_Plugins\\Archetype', flatten: false},
          {expand: true, cwd: '<%= dest %>/', src: ['bin/**/*'], dest: '<%= grunt.option("target") %>', flatten: false},
        ]
      },
      nuget_prepare: {
        files: [
          {expand: true, cwd: '<%= dest %>/', src: ['**/*', '!bin', '!bin/*'], dest: '<%= package_temp_dir %>/nuget/content/', flatten: false},
          {expand: true, cwd: '<%= dest %>/', src: ['bin/**/*.dll'], dest: '<%= package_temp_dir %>/nuget/lib/net40/', flatten: true}
        ]
      },
      umbracopackage: {
        files: [
          {expand: true, cwd: '<%= dest %>/', src: ['**/*', '!bin', "!bin/*"], dest: 'pkg/tmp/umbraco/App_Plugins/Archetype', flatten: false},
          {expand: true, cwd: '<%= dest %>/', src: ['bin/**/*.dll'], dest: 'pkg/tmp/umbraco', flatten: false}
        ]
      }
    },

    touch: {
      options: {},
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
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

    msbuild: {
        dev: {
            src: ['app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj'],
            options: {
                projectConfiguration: 'Debug',
                targets: ['Clean', 'Rebuild'],
                stdout: true,
                maxCpuCount: 4,
                buildParameters: {
                    WarningLevel: 2
                },
                verbosity: 'quiet'
            }
        }
    }

  });

  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-contrib-less');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-contrib-copy');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-nuget');
  grunt.loadNpmTasks('grunt-template');
  grunt.loadNpmTasks('grunt-touch');
  grunt.loadNpmTasks('grunt-msbuild');
  grunt.loadTasks('tasks');


  grunt.registerTask('package', ['clean:package_artifacts', 'default', 'package:nuget', 'package:umbraco']);
  grunt.registerTask('package:nuget', ['copy:nuget_prepare', 'template:nuget_manifest', 'nugetpack', 'clean:package_temp']);
  grunt.registerTask('package:umbraco', ['copy:umbracopackage', 'umbracoPackage', 'clean:package_temp']);
  grunt.registerTask('touchwebconfigifenabled', function() { if (grunt.option("touch")) grunt.task.run("touch:webconfig") });
  grunt.registerTask('deploy', ['default', 'copy:deploy', 'touchwebconfigifenabled']);
  grunt.registerTask('css:build', ['less']);
  grunt.registerTask('js:build', ['concat']);
  grunt.registerTask('default', ['clean', 'css:build', 'js:build', 'copy:build', 'msbuild:dev']);
};

