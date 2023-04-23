// const core = require('core');
import core from "./core.js"
import { GameObject, Resources } from "UnityEngine"

function hello() {
    // const error = Error()
    // throw error
    var sprite=null;
    core.mono(()=>{
         sprite = core.loadResource("Textures/poseDefault")
      
    })
    sprite.name="test"
    core.log(sprite)
    core.setBackground(sprite)
    core.showName("name")
    core.log("hello world!")
}
// [


hello();