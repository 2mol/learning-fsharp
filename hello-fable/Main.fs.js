import { printf, toConsole } from "./.fable/fable-library.3.2.9/String.js";

(function (argv) {
    toConsole(printf("%A"))(argv);
    return 0;
})(typeof process === 'object' ? process.argv.slice(2) : []);

