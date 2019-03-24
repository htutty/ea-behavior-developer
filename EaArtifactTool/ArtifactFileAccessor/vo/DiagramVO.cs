using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class DiagramVO
    {
        public int diagramId { get; set; }
        public int packageId { get; set; }
        public int parentId { get; set; }
        public string diagramType { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string author { get; set; }
        public int showDetails { get; set; }
        public string notes { get; set; }
        public string stereotype { get; set; }
        public bool attPub { get; set; }
        public bool attPri { get; set; }
        public bool attPro { get; set; }
        public string orientation { get; set; }
        public int cx { get; set; }
        public int cy { get; set; }
        public int scale { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
        public bool showForeign { get; set; }
        public bool showBorder { get; set; }
        public bool showPackageContents { get; set; }
        public string guid { get; set; }
        public int treePos { get; set; }
        public string swimlanes { get; set; }
        public string styleEx { get; set; }

        public List<DiagramLinkVO> diagramLinks;
        public List<DiagramObjectVO> diagramObjects;

        /// <summary>
        /// 変更有りフラグ : ' '=変更無し, C=追加(Create) U=変更(Update) D=削除(Delete)
        /// </summary>
        public char changed { get; set; }

        public DiagramVO()
        {
            this.changed = ' ';
            this.diagramLinks = new List<DiagramLinkVO>();
            this.diagramObjects = new List<DiagramObjectVO>();
        }

    }
}
