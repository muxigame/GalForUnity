// const core = require('core');
import core from "./core.js"

function hello() {
    // const error = Error()
    // throw error
    core.showName("name")
    core.log("hello world")
    // core.log(error.stack)
    // core.log(error.name)
    // core.log(error.message)
}
// [

hello();