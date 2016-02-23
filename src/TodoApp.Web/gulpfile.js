/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp        = require('gulp'),
    source      = require('vinyl-source-stream'),
    browserify  = require('browserify'),
    relative    = require('relative'),
    tsify       = require('tsify'),
    gutil       = require('gulp-util'),
    exec        = require('child_process').exec
    runSequence = require('run-sequence');

gulp.task('service-generation', (cb) => {
    exec('dnvm use 1.0.0-rc1-update1 && dnx -p ../TodoApp.ServiceGeneration TodoApp.ServiceGeneration', (err, stdout, stderr) => {
        cb(err);
    });
});

gulp.task('scripts', () => {
    var bundler = browserify({})
        .add('wwwroot/main.tsx')
        .plugin(tsify);

    return bundler.bundle()
        .on('error', error => gutil.log(gutil.colors.red('Error:'), error.message))
        .pipe(source('bundle.js'))
        .pipe(gulp.dest('wwwroot'));
});

gulp.task('build', (cb) => {
    runSequence('service-generation', 'scripts', cb);
});

gulp.task('watch', () => {
    gulp.watch('../TodoApp.Service/**/*.cs', event => {
        gutil.log(gutil.colors.green(event.type + ':'), relative(event.path));
        runSequence('service-generation', 'scripts');
    });

    gulp.watch('wwwroot/**/*.ts*', event => {
        gutil.log(gutil.colors.green(event.type + ':'), relative(event.path));
        runSequence('scripts');
    });
});

gulp.task('default', ['scripts', 'watch'], () => { });
