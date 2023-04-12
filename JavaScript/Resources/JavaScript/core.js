import { CSCore } from 'CSCore'

var core = {
    showName: function showName(name) {
        CSCore.ShowName(name)
    },
    log: function log(message, context) {
        CSCore.Log(message, context)
    }
}

export default core
// module.exports.showName = showName
// module.exports.log = log