const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const sourcemaps = require('gulp-sourcemaps');
const autoprefixer = require('gulp-autoprefixer');
const cleanCSS = require('gulp-clean-css');

// Paths
const paths = {
    styles: {
        src: 'Assets/Styles/**/*.scss',
        dest: 'dist/css/'
    },
    scripts: {
        src: 'Assets/Scripts/**/*.js',
        dest: 'dist/js/'
    }
};

// Compile SCSS to CSS
function compileSass() {
    return gulp.src(paths.styles.src)
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer({
            cascade: false
        }))
        .pipe(cleanCSS())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(paths.styles.dest));
}

// Minify JavaScript
function minifyJs() {
    return gulp.src(paths.scripts.src)
        .pipe(sourcemaps.init())
        .pipe(concat('bundle.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(paths.scripts.dest));
}

// Watch files for changes
function watchFiles() {
    gulp.watch(paths.styles.src, compileSass);
    gulp.watch(paths.scripts.src, minifyJs);
}

// Build task
const build = gulp.series(compileSass, minifyJs);

// Watch task
const watch = gulp.series(build, watchFiles);

// Export tasks
exports.compileSass = compileSass;
exports.minifyJs = minifyJs;
exports.watch = watch;
exports.build = build;
exports.default = build;