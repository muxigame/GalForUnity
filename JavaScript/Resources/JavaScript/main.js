// const core = require('core');
import core from "./core.js"
import { GameObject, Resources } from "UnityEngine"

function hello() {
    // const error = Error()
    // throw error
    const sprite = core.loadResource("Textures/poseDefault")
    sprite.name="test"
    core.log(sprite)
    core.showName("name")
    core.setBackground(sprite)
    core.log("hello world!")
    // core.log(error.stack)
    // core.log(error.name)
    // core.log(error.message)
}
// [








hello();