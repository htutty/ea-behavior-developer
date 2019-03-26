using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDFileReader.vo
{
    public class ArtifactsVO
    {
        public string targetProject { get; set; }
        public string lastUpdated { get; set; }
        public string targetModel { get; set; }
        public List<ArtifactVO> artifactList { get; set; }

    }
}
