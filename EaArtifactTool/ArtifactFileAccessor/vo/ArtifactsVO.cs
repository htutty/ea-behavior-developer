using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class ArtifactsVO
    {
        public string targetProject { get; set; }
        public string lastUpdated { get; set; }
        public string targetModel { get; set; }
        public List<ArtifactVO> artifactList { get; set; }

        public ArtifactsVO()
        {

        }

        public List<ArtifactVO> getArtifactsExcludeImplModel()
        {
            List<ArtifactVO> outlist = new List<ArtifactVO>();
            foreach( ArtifactVO atf in this.artifactList)
            {
                // 実装モデル配下のクラスは出力対象から除く
                if (!atf.pathName.Contains(".実装モデル."))
                {
                    outlist.Add(atf);
                }
            }

            return outlist;
        }


    }
}
