using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElementEditor.model;

namespace ElementEditor.viewmodel
{
    class AttrMethSearchViewModel
    {
        public List<AttrMethItem> AttrMethItems = new List<AttrMethItem>();

        public AttrMethSearchViewModel()
        {
            AttrMethItem item1 = new AttrMethItem()
            {
                elemName = "払戻株主優待番号検索（クレジットカード／確認番号）",
                elemAlias = "RefundStockHolderNumberSearchPage",
                elemType = "Class",
                elemStereotype = "web model page",
                elemGuid = "{9050EE46-FF3C-4b30-B72C-87A2E24F5C8B}",
                attrMethFlg = "m",
                attrMethType = "String",
                attrMethGuid = "{21EA0C48-BC57-44cf-B396-A1397CC35EC8}",
                attrMethName = "ログイン実行処理",
                attrMethAlias = "executeLogin",
                methParameterDesc = "",
                artifactPath = "/次期ASWプロジェクト-本開発/論理モデル/レイヤ別ビュー/PL層設計モデル/プレゼンテーション/PLアプリケーション/Webモデル/照会系機能/払戻優待券検索"
            };

            AttrMethItem item2 = new AttrMethItem()
            {
                elemName = "払戻株主優待番号検索（クレジットカード／確認番号）",
                elemAlias = "RefundStockHolderNumberSearchPage",
                elemType = "Class",
                elemStereotype = "web model page",
                elemGuid = "{9050EE46-FF3C-4b30-B72C-87A2E24F5C8B}",
                attrMethFlg = "m",
                attrMethType = "void",
                attrMethGuid = "{C160C8BD-8907-49ea-8546-6EE572B0C510}",
                attrMethName = "初期処理",
                attrMethAlias = "init",
                methParameterDesc = "",
                artifactPath = "/次期ASWプロジェクト-本開発/論理モデル/レイヤ別ビュー/PL層設計モデル/プレゼンテーション/PLアプリケーション/Webモデル/照会系機能/払戻優待券検索"
            };

            AttrMethItems = new List<AttrMethItem>();
            AttrMethItems.Add(item1);
            AttrMethItems.Add(item2);

        }

    }



}
