

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalForUnity.Graph.Nodes;
using UnityEngine;

namespace GalForUnity.Core.Block
{
    [Serializable]
    public class PlotBlock : IGalBlock
    {
        public string name;
        public string word;
        private RuntimeNode _runtimeNode;
        [SerializeReference] public List<ConfigAddition> configAdditions = new List<ConfigAddition>();

        RuntimeNode IGalBlock.RuntimeNode
        {
            get => _runtimeNode;
            set => _runtimeNode = value;
        }

        public async Task Process(GalCore galCore)
        {
           
            galCore.SetName(name);
            galCore.SetSay(word);
            
            foreach (var configAddition in configAdditions)
            {
                configAddition.Process(galCore.GetRole(name), galCore);
            }

            do{
                if(!Application.isPlaying) return;
                await Task.Yield();
            } while (!_runtimeNode.GalGraph.GraphProvider.Next.Invoke());
        }
    }

    [Serializable]
    public abstract class ConfigAddition
    {
        public abstract void Process(IRoleIO roleIO, ICoreIO coreIO);
    }

    [Serializable]
    public struct PoseLocation
    {
        public string roleName;
        public string poseName;
        public string anchorName;
        public string faceName;
    }
    [Serializable]
    public class AdditionPose : ConfigAddition
    {
        public string poseName;
        public string anchorName;
        public string faceName;

        public override void Process(IRoleIO roleIO, ICoreIO coreIO)
        {
            roleIO.SetPose(poseName, anchorName, faceName);
        }
    }

    [Serializable]
    public class AdditionPosition : ConfigAddition
    {
        public Unit xUnit;
        public Unit yUnit;
        public Vector2 position;

        public override void Process(IRoleIO roleIO, ICoreIO coreIO)
        {
            roleIO.SetPosition(xUnit, yUnit, position);
        }
    }

    [Serializable]
    public class AdditionAudio : ConfigAddition
    {
        public AudioClip audioClip;

        public override void Process(IRoleIO roleIO, ICoreIO coreIO)
        {
            coreIO.SetAudio(audioClip);
        }
    }

    [Serializable]
    public class AdditionRoleVoice : ConfigAddition
    {
        public AudioClip audioClip;

        public override void Process(IRoleIO roleIO, ICoreIO coreIO)
        {
            roleIO.SetVoice(audioClip);
        }
    }
}
