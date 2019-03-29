﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElementEditor.model
{

    class AttrMethSearchModel
    {

    }


    public class AttrMethItem
    {

        /// <summary>要素名</summary>
        public string elemName { get; set; }

        /// <summary>要素別名</summary>
        public string elemAlias { get; set; }

        /// <summary>要素のタイプ</summary>
        public string elemType { get; set; }

        /// <summary>ステレオタイプ</summary>
        public string elemStereotype { get; set; }

        /// <summary>要素GUID</summary>
        public string elemGuid { get; set; }

        /// <summary>属性操作フラグ(a=属性,m=操作)</summary>
        public string attrMethFlg { get; set; }

        /// <summary>属性操作タイプ(属性なら属性型、操作なら戻り値型)</summary>
        public string attrMethType { get; set; }

        /// <summary>属性操作GUID</summary>
        public string attrMethGuid { get; set; }

        /// <summary>属性操作名</summary>
        public string attrMethName { get; set; }

        /// <summary>属性操作別名</summary>
        public string attrMethAlias { get; set; }

        /// <summary>操作のパラメータ詳細</summary>
        public string methParameterDesc { get; set; }

        /// <summary>成果物パッケージのパス（"/"区切り） </summary>
        public string artifactPath { get; set; }


    }
}
