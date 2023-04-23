import { CSCore } from 'CSCore'

var core = {
    showName: function showName(name) {
        CSCore.ShowName(name)
    },
    mono: function mono(func) {
        CSCore.Mono(func)
    },
    log: function log(message, context) {
        CSCore.Log(message, context)
    },
    setBackground: function setBackground(sprite) {
        CSCore.SetBackground(sprite)
    },
    loadResource: function LoadResource(path) {
        return  CSCore.LoadResource(path)
    }
}

export default core
// module.exports.showName = showName
// module.exports.log = log