using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class RepositorySettingVO
    {
        public string name { get; set; }
        public string baseRepository { get; set; }
        public string localPath { get; set; }
        public string projectPath { get; set; }
        public string connectionString { get; set; }
        public ProjectSettingVO projectSettingVO { get; set; }
    }
}
